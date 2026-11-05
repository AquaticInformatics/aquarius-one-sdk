using System.Net;
using Moq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enterprise.Core;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK.Tests.Enterprise.Core;

public class CoreApiInsightsTests
{
	private static readonly byte[] Body = "{\"product\":\"RioMobile\",\"items\":[]}"u8.ToArray();
	private static readonly TimeSpan Margin = TimeSpan.FromMinutes(2);
	private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

	private readonly Mock<IOneApiHelper> _helper = new(MockBehavior.Strict);

	private (CoreApi Core, AuthenticationApi Auth) Create(RecordingHandler handler, EnumPlatformEnvironment environment = EnumPlatformEnvironment.AqiFeature,
		bool throwApiErrors = true)
	{
		var auth = new AuthenticationApi(PlatformEnvironmentHelper.GetPlatformEnvironment(environment), false, throwApiErrors);

		return (new CoreApi(_helper.Object, auth, false, throwApiErrors, handler), auth);
	}

	private static void SignIn(AuthenticationApi auth, string accessToken = "token-a", TimeSpan? lifetime = null) =>
		auth.SetToken(accessToken, DateTime.Now + (lifetime ?? TimeSpan.FromHours(1)), "refresh");

	[Fact]
	public async Task NoToken_GivesNotAuthenticated_AndSendsNothing()
	{
		var handler = new RecordingHandler();
		var (core, _) = Create(handler);

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
		result.StatusCode.Should().BeNull();
		handler.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task EmptyToken_GivesNotAuthenticated_AndSendsNothing()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		auth.Token = new Token { access_token = "", expires = DateTime.Now.AddHours(1) };

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
		handler.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task TokenInsideTheMargin_GivesNotAuthenticated_AndSendsNothing()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, lifetime: TimeSpan.FromSeconds(90));

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
		handler.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task TokenOutsideTheMargin_IsSent()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, lifetime: TimeSpan.FromMinutes(3));

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.Accepted);
		handler.Requests.Should().HaveCount(1);
	}

	[Fact]
	public async Task AuthorizationHeader_IsTheCurrentBearer_AndTheOnlyOne()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth);

		await core.PostInsightsAsync(Body, new Dictionary<string, string> { ["Authorization"] = "Bearer stolen", ["X-Test"] = "1" }, Margin, Timeout);

		var request = handler.Requests.Single();
		request.AuthorizationValues.Should().Equal("Bearer token-a");
		request.Headers.Should().ContainKey("X-Test").WhoseValue.Should().Equal("1");
	}

	[Fact]
	public async Task ClientDefaultHeaders_AreNotUsed()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, "token-a");

		// The shared clients hold token-a as a default header. Replacing the token on the AuthenticationApi without touching them
		// leaves the old value there, as Logout() does (finding F-3).
		auth.Token = new Token { access_token = "token-b", expires = DateTime.Now.AddHours(1) };

		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		handler.Requests.Single().AuthorizationValues.Should().Equal("Bearer token-b");
	}

	[Fact]
	public async Task AfterSetToken_TheNextCallSendsTheNewTokenOnly()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, "token-a");
		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		SignIn(auth, "token-b");
		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		handler.Requests.Select(r => r.AuthorizationValues.Single()).Should().Equal("Bearer token-a", "Bearer token-b");
	}

	[Fact]
	public async Task AfterLogout_NoRequestIsMade_AndTheOldTokenIsNeverSent()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, "token-a");
		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		auth.Logout();
		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
		handler.Requests.Should().HaveCount(1);
	}

	[Fact]
	public async Task AfterLogoutAndSetToken_TheNewTokenIsSent()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth, "token-a");
		auth.Logout();

		SignIn(auth, "token-b");
		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		handler.Requests.Single().AuthorizationValues.Should().Equal("Bearer token-b");
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task TokenExpired_IsNeverRaised_AndTheHelperIsNeverUsed(bool tokenIsValid)
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		auth.Token = new Token { access_token = "token-a", expires = DateTime.Now + (tokenIsValid ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(-5)) };
		auth.AutoRenewToken = true;
		var tokenExpiredRaised = false;
		auth.TokenExpired += (_, _) => tokenExpiredRaised = true;

		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		tokenExpiredRaised.Should().BeFalse();
		_helper.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task TheUri_IsTheEnvironmentBaseAddressPlusTheCorePath()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth);

		await core.PostInsightsAsync(Body, null!, Margin, Timeout);
		await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		var uris = handler.Requests.Select(r => r.Uri).ToList();
		uris.Should().OnlyContain(u => u.GetLeftPart(UriPartial.Path) == "https://api-feature-us.aquaticinformatics.net/enterprise/core/v1/Insights");
		var requestIds = uris.Select(u => Guid.Parse(u.Query.Replace("?requestId=", ""))).ToList();
		requestIds.Distinct().Should().HaveCount(2);
		handler.Requests.Should().OnlyContain(r => r.Method == HttpMethod.Post);
	}

	[Fact]
	public async Task ACoreApiBuiltWithThePublicConstructor_Throws_AndSendsNothing()
	{
		var core = new CoreApi(_helper.Object, false, true);

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, Timeout)).Should().ThrowAsync<InvalidOperationException>();
		_helper.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task AnInsecureBaseAddress_Throws_AndSendsNothing()
	{
		var handler = new RecordingHandler();
		var environment = new PlatformEnvironment
		{
			PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature,
			BaseUri = new Uri("http://api.example.com/")
		};
		var auth = new AuthenticationApi(environment, false, true);
		SignIn(auth);
		var core = new CoreApi(_helper.Object, auth, false, true, handler);

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, Timeout)).Should().ThrowAsync<InvalidOperationException>();
		handler.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task NoEnvironment_Throws_AndSendsNothing()
	{
		var handler = new RecordingHandler();
		var auth = new AuthenticationApi(null!, false, true);
		SignIn(auth);
		var core = new CoreApi(_helper.Object, auth, false, true, handler);

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, Timeout)).Should().ThrowAsync<InvalidOperationException>();
		handler.Requests.Should().BeEmpty();
	}

	[Fact]
	public async Task ARedirect_GivesRedirectRefused_AndTheTargetReceivesNothing()
	{
		var handler = new RecordingHandler(_ =>
		{
			var response = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect);
			response.Headers.Location = new Uri("https://elsewhere.example.com/collect");
			return response;
		});
		var (core, auth) = Create(handler);
		SignIn(auth);

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.RedirectRefused);
		result.StatusCode.Should().Be(HttpStatusCode.TemporaryRedirect);
		handler.Requests.Should().HaveCount(1);
	}

	[Fact]
	public void TheDefaultHandler_DoesNotFollowRedirects()
	{
		var handler = CoreApi.CreateInsightsHandler().Should().BeOfType<HttpClientHandler>().Subject;

		handler.AllowAutoRedirect.Should().BeFalse();
		handler.UseCookies.Should().BeFalse();
	}

	[Fact]
	public async Task TheTimeout_CancelsTheRequest_AndHttpClientTimeoutIsNotUsed()
	{
		var handler = new RecordingHandler(async (_, cancellation) =>
		{
			await Task.Delay(System.Threading.Timeout.Infinite, cancellation);
			return new HttpResponseMessage(HttpStatusCode.OK);
		});
		var (core, auth) = Create(handler);
		SignIn(auth);

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, TimeSpan.FromMilliseconds(100))).Should().ThrowAsync<TimeoutException>();
	}

	[Fact]
	public async Task TheHttpClientTimeout_DoesNotApply()
	{
		var handler = new RecordingHandler(async (_, cancellation) =>
		{
			await Task.Delay(200, cancellation);
			return new HttpResponseMessage(HttpStatusCode.OK);
		});
		var (core, auth) = Create(handler);
		auth.HttpClientTimeout = TimeSpan.FromMilliseconds(1);
		SignIn(auth);

		var result = await core.PostInsightsAsync(Body, null!, Margin, TimeSpan.FromSeconds(10));

		result.Outcome.Should().Be(InsightsPostOutcome.Accepted);
	}

	[Fact]
	public async Task ACancelledCall_ThrowsOperationCanceled_NotTimeout()
	{
		var handler = new RecordingHandler(async (_, cancellation) =>
		{
			await Task.Delay(System.Threading.Timeout.Infinite, cancellation);
			return new HttpResponseMessage(HttpStatusCode.OK);
		});
		var (core, auth) = Create(handler);
		SignIn(auth);
		using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, TimeSpan.FromSeconds(30), cancellation.Token))
			.Should().ThrowAsync<OperationCanceledException>();
	}

	[Fact]
	public async Task TheRequest_IsJson_WithNoAcceptHeader_AndTheBodyAsGiven()
	{
		var handler = new RecordingHandler();
		var (core, auth) = Create(handler);
		SignIn(auth);

		await core.PostInsightsAsync(Body, new Dictionary<string, string>
		{
			["Content-Type"] = "text/plain", ["Accept"] = "text/html", ["Host"] = "elsewhere.example.com", ["Bad Name"] = "x"
		}, Margin, Timeout);

		var request = handler.Requests.Single();
		request.ContentType.Should().Be("application/json");
		request.Headers.Should().NotContainKey("Accept");
		request.Headers.Should().NotContainKey("Host");
		request.Headers.Should().NotContainKey("Bad Name");
		request.Body.Should().Equal(Body);
	}

	[Theory]
	[InlineData(HttpStatusCode.OK, InsightsPostOutcome.Accepted)]
	[InlineData(HttpStatusCode.NoContent, InsightsPostOutcome.Accepted)]
	[InlineData(HttpStatusCode.BadRequest, InsightsPostOutcome.Rejected)]
	[InlineData(HttpStatusCode.Unauthorized, InsightsPostOutcome.Unauthorized)]
	[InlineData(HttpStatusCode.Forbidden, InsightsPostOutcome.Forbidden)]
	[InlineData(HttpStatusCode.NotFound, InsightsPostOutcome.Rejected)]
	[InlineData(HttpStatusCode.RequestEntityTooLarge, InsightsPostOutcome.Rejected)]
	[InlineData((HttpStatusCode)429, InsightsPostOutcome.Throttled)]
	[InlineData(HttpStatusCode.InternalServerError, InsightsPostOutcome.ServerError)]
	[InlineData(HttpStatusCode.ServiceUnavailable, InsightsPostOutcome.ServerError)]
	[InlineData(HttpStatusCode.MovedPermanently, InsightsPostOutcome.RedirectRefused)]
	public async Task EveryStatus_GivesItsOutcome_AndNeverThrowsRestApiException(HttpStatusCode status, InsightsPostOutcome expected)
	{
		// The response body is never read: this one throws if it is.
		var handler = new RecordingHandler(_ => new HttpResponseMessage(status) { Content = new UnreadableContent() });
		var (core, auth) = Create(handler, throwApiErrors: true);
		SignIn(auth);

		var result = await core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(expected);
		result.StatusCode.Should().Be(status);
	}

	[Fact]
	public async Task NoCoreApiEvent_IsRaised_OnSuccess_OnAnHttpFailure_OrOnATransportFailure()
	{
		var handler = new RecordingHandler(request => request.Headers.Contains("X-Fail")
			? throw new HttpRequestException("connection refused")
			: new HttpResponseMessage(request.Headers.Contains("X-Status") ? HttpStatusCode.InternalServerError : HttpStatusCode.OK));
		var (core, auth) = Create(handler);
		SignIn(auth);
		var events = new List<ClientApiLoggerEventArgs>();
		core.Event += (_, args) => events.Add(args);

		await core.PostInsightsAsync(Body, null!, Margin, Timeout);
		await core.PostInsightsAsync(Body, new Dictionary<string, string> { ["X-Status"] = "1" }, Margin, Timeout);
		await core.Invoking(c => c.PostInsightsAsync(Body, new Dictionary<string, string> { ["X-Fail"] = "1" }, Margin, Timeout))
			.Should().ThrowAsync<HttpRequestException>();

		handler.Requests.Should().HaveCount(3);
		events.Should().BeEmpty();
	}

	[Fact]
	public async Task ANullBody_Throws()
	{
		var (core, auth) = Create(new RecordingHandler());
		SignIn(auth);

		await core.Invoking(c => c.PostInsightsAsync(null!, null!, Margin, Timeout)).Should().ThrowAsync<ArgumentNullException>();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task ANonPositiveTimeout_Throws(int seconds)
	{
		var (core, auth) = Create(new RecordingHandler());
		SignIn(auth);

		await core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, TimeSpan.FromSeconds(seconds))).Should().ThrowAsync<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task OneApiCore_IsBuiltWithItsAuthentication()
	{
		var api = new OneApi(EnumPlatformEnvironment.AqiFeature);

		// No token: the answer comes before any request. A CoreApi built with the public constructor would throw instead.
		var result = await api.Core.PostInsightsAsync(Body, null!, Margin, Timeout);

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
	}

	[Fact]
	public void OneApiCore_IsTheSameInstance_WhileNothingChanges()
	{
		var api = new OneApi(EnumPlatformEnvironment.AqiFeature);

		api.Core.Should().BeSameAs(api.Core);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public void OneApiCore_IsRebuilt_WhenTheEnvironmentChanges(bool readAuthenticationFirst)
	{
		var api = new OneApi(EnumPlatformEnvironment.AqiFeature);
		var before = api.Core;
		var authenticationBefore = api.Authentication;

		api.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
		if (readAuthenticationFirst)
			api.Authentication.Should().NotBeSameAs(authenticationBefore);

		api.Core.Should().NotBeSameAs(before);
	}

	[Fact]
	public async Task OneApiCore_AfterAnEnvironmentChange_NeverSendsTheOldLogin()
	{
		// The .invalid hosts never resolve, so a request that should not happen fails the test instead of leaving the machine.
		var api = new OneApi { Environment = new PlatformEnvironment { PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature, BaseUri = new Uri("https://old.invalid/") } };
		api.Authentication.SetToken("token-old", DateTime.Now.AddHours(1), "refresh");
		var before = api.Core;

		api.Environment = new PlatformEnvironment { PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature, BaseUri = new Uri("https://new.invalid/") };

		// The new AuthenticationApi holds no login. A Core still on the old one would post token-old to the old host.
		var result = await api.Core.PostInsightsAsync(Body, null!, Margin, TimeSpan.FromMilliseconds(500));

		result.Outcome.Should().Be(InsightsPostOutcome.NotAuthenticated);
		api.Core.Should().NotBeSameAs(before);
	}

	[Fact]
	public async Task OneApiCore_AfterAnEnvironmentChange_ReadsTheNewLoginAndTheNewHost()
	{
		var api = new OneApi { Environment = new PlatformEnvironment { PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature, BaseUri = new Uri("https://old.invalid/") } };
		_ = api.Core;

		// An insecure host makes the post throw after the token check and before any request, so this needs no network.
		api.Environment = new PlatformEnvironment { PlatformEnvironmentEnum = EnumPlatformEnvironment.AqiFeature, BaseUri = new Uri("http://new.invalid/") };
		api.Authentication.SetToken("token-new", DateTime.Now.AddHours(1), "refresh");

		// A Core still on the old AuthenticationApi would answer NotAuthenticated here, because the old one holds no login.
		await api.Core.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, Timeout)).Should().ThrowAsync<InvalidOperationException>();
	}

	[Theory]
	[InlineData(EnumPlatformEnvironment.AqiFeature, "https://api-feature-us.aquaticinformatics.net/")]
	[InlineData(EnumPlatformEnvironment.Local, "http://localhost:9100/")]
	public void GetHttpClient_StillSetsTheBaseAddressGetBaseAddressReturns(EnumPlatformEnvironment environment, string expected)
	{
		var auth = new AuthenticationApi(PlatformEnvironmentHelper.GetPlatformEnvironment(environment), false);

		auth.GetBaseAddress("enterprise/core/v1/User").Should().Be(new Uri(expected));
		auth.GetBaseClient("enterprise/core/v1/User").BaseAddress.Should().Be(new Uri(expected));
		auth.GetLocalHttpJsonClient("enterprise/core/v1/User").BaseAddress.Should().Be(new Uri(expected));
	}

	[Fact]
	public async Task ExistingMethods_StillSendThroughTheHelper_WithTheirCurrentPaths()
	{
		_helper.Setup(h => h.BuildRequestAndSendAsync(HttpMethod.Get, It.Is<string>(uri => uri.StartsWith("enterprise/core/v1/User?requestId=")),
				It.IsAny<CancellationToken>(), null, false))
			.ReturnsAsync(new ApiResponse { StatusCode = 200, Content = new Content { Users = new Users() } });
		var (core, _) = Create(new RecordingHandler());

		var users = await core.GetUsersAsync();

		users.Should().BeEmpty();
		_helper.VerifyAll();
	}

	private sealed record RecordedRequest(HttpMethod Method, Uri Uri, string[] AuthorizationValues, string? ContentType, byte[]? Body,
		IReadOnlyDictionary<string, string[]> Headers);

	private sealed class RecordingHandler : HttpMessageHandler
	{
		private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _respond;

		public List<RecordedRequest> Requests { get; } = new();

		public RecordingHandler() : this(_ => new HttpResponseMessage(HttpStatusCode.OK))
		{
		}

		public RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : this((request, _) => Task.FromResult(respond(request)))
		{
		}

		public RecordingHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) => _respond = respond;

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// The SDK disposes the request after the call, so everything is copied here.
			var body = request.Content == null ? null : await request.Content.ReadAsByteArrayAsync(cancellationToken);
			var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray());
			var authorization = headers.TryGetValue("Authorization", out var values) ? values : Array.Empty<string>();

			Requests.Add(new RecordedRequest(request.Method, request.RequestUri!, authorization, request.Content?.Headers.ContentType?.ToString(), body, headers));

			return await _respond(request, cancellationToken);
		}
	}

	private sealed class UnreadableContent : HttpContent
	{
		protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => throw new InvalidOperationException("The body was read.");

		protected override bool TryComputeLength(out long length)
		{
			length = 0;
			return false;
		}
	}
}
