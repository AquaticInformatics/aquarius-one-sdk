using System.Net;
using System.Reflection;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enterprise.Core;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK.Tests.Communication;

// Retire() is what OneApi calls when it drops a helper. It must dispose the insights client, but never under a post that is still
// using it, and a retired helper must refuse further insights posts instead of rebuilding a client nobody will dispose.
public class OneApiHelperRetireTests
{
	private static readonly byte[] Body = "{\"product\":\"RioMobile\",\"items\":[]}"u8.ToArray();
	private static readonly TimeSpan Margin = TimeSpan.FromMinutes(2);
	private static readonly TimeSpan PostTimeout = TimeSpan.FromSeconds(10);

	// A guard so that a broken test fails instead of hanging the run.
	private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

	private static AuthenticationApi SignedInAuthentication()
	{
		var auth = new AuthenticationApi(PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature), false, true);
		auth.SetToken("token-a", DateTime.Now.AddHours(1), "refresh");

		return auth;
	}

	private static (OneApiHelper Helper, TrackingHandler Handler) Create(bool holdRequests = false)
	{
		var handler = new TrackingHandler(holdRequests);

		return (new OneApiHelper(SignedInAuthentication(), false, true, false, handler), handler);
	}

	private static Task<InsightsPostResult> Post(OneApiHelper helper, CancellationToken cancellation = default) =>
		helper.PostInsightsAsync(Body, null!, Margin, PostTimeout, cancellation);

	[Fact]
	public void Retire_BeforeAnyPost_DisposesTheHandler()
	{
		var (helper, handler) = Create();

		helper.Retire();

		handler.Disposed.Should().BeTrue();
	}

	[Fact]
	public async Task Retire_AfterAPost_DisposesTheClientAndItsHandler()
	{
		var (helper, handler) = Create();
		(await Post(helper)).Outcome.Should().Be(InsightsPostOutcome.Accepted);
		handler.Disposed.Should().BeFalse("the client is kept between posts");

		helper.Retire();

		handler.Disposed.Should().BeTrue();
	}

	[Fact]
	public async Task APostAfterRetire_ThrowsObjectDisposed_AndSendsNothing()
	{
		var (helper, handler) = Create();
		await Post(helper);
		helper.Retire();
		var requestsBefore = handler.Requests;

		await helper.Invoking(h => Post(h)).Should().ThrowAsync<ObjectDisposedException>();

		handler.Requests.Should().Be(requestsBefore);
	}

	[Fact]
	public async Task Retire_WhileAPostIsInFlight_WaitsForItToFinish()
	{
		var (helper, handler) = Create(holdRequests: true);
		var post = Post(helper);
		await handler.WaitForRequestAsync();

		helper.Retire();
		await Task.Delay(100);
		handler.Disposed.Should().BeFalse("a request is still using the client");

		handler.ReleaseRequests();
		var result = await post.WaitAsync(Patience);

		result.Outcome.Should().Be(InsightsPostOutcome.Accepted);
		handler.Disposed.Should().BeTrue("the last post to finish disposes a retired client");
	}

	[Fact]
	public async Task Retire_WhileTwoPostsAreInFlight_DisposesOnlyAfterTheLastOneFinishes()
	{
		var (helper, handler) = Create(holdRequests: true);
		var first = Post(helper);
		var second = Post(helper);
		await handler.WaitForRequestAsync();
		await handler.WaitForRequestAsync();

		helper.Retire();
		handler.ReleaseRequests();
		await Task.WhenAll(first, second).WaitAsync(Patience);

		handler.Disposed.Should().BeTrue();
	}

	[Fact]
	public async Task Retire_AfterACancelledPost_DisposesAtOnce()
	{
		// The post leaves through its finally, so the count of posts in flight is balanced even when it does not complete.
		var (helper, handler) = Create(holdRequests: true);
		using var cancellation = new CancellationTokenSource();
		var post = Post(helper, cancellation.Token);
		await handler.WaitForRequestAsync();
		cancellation.Cancel();
		await post.Invoking(p => p.WaitAsync(Patience)).Should().ThrowAsync<OperationCanceledException>();

		helper.Retire();

		handler.Disposed.Should().BeTrue();
	}

	[Fact]
	public async Task Retire_CalledTwice_IsHarmless()
	{
		var (helper, handler) = Create();
		await Post(helper);

		helper.Retire();

		helper.Invoking(h => h.Retire()).Should().NotThrow();
		handler.Disposed.Should().BeTrue();
	}

	[Fact]
	public void Retire_RemovesTheHelpersTokenExpiredSubscription()
	{
		var auth = SignedInAuthentication();
		var baseline = TokenExpiredSubscribers.Of(auth);
		var helper = new OneApiHelper(auth, false, true, false, new TrackingHandler());
		TokenExpiredSubscribers.Of(auth).Should().Be(baseline + 1, "the helper listens for the token to expire");

		helper.Retire();

		TokenExpiredSubscribers.Of(auth).Should().Be(baseline);
	}

	private sealed class TrackingHandler : HttpMessageHandler
	{
		private readonly bool _hold;
		private readonly SemaphoreSlim _entered = new(0);
		private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);
		private int _requests;

		public TrackingHandler(bool hold = false) => _hold = hold;

		public volatile bool Disposed;

		public int Requests => Volatile.Read(ref _requests);

		public async Task WaitForRequestAsync()
		{
			if (!await _entered.WaitAsync(Patience))
				throw new TimeoutException("No request reached the handler.");
		}

		public void ReleaseRequests() => _release.TrySetResult();

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Interlocked.Increment(ref _requests);
			_entered.Release();

			if (_hold)
				await _release.Task.WaitAsync(cancellationToken);

			return new HttpResponseMessage(HttpStatusCode.OK);
		}

		protected override void Dispose(bool disposing)
		{
			Disposed = true;
			base.Dispose(disposing);
		}
	}
}

// How many handlers listen to AuthenticationApi.TokenExpired. The event starts with an empty delegate, so tests compare against a
// baseline taken before the helper under test is built.
internal static class TokenExpiredSubscribers
{
	public static int Of(AuthenticationApi authentication)
	{
		var field = typeof(AuthenticationApi).GetField(nameof(AuthenticationApi.TokenExpired), BindingFlags.Instance | BindingFlags.NonPublic)
			?? throw new InvalidOperationException("TokenExpired is no longer a field-like event, so its subscribers cannot be counted.");

		return ((Delegate?)field.GetValue(authentication))?.GetInvocationList().Length ?? 0;
	}
}
