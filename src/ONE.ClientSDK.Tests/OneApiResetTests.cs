using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Tests.Communication;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK.Tests;

// What OneApi does with its helper and the API objects built on it when a setting changes.
public class OneApiResetTests
{
	private static readonly byte[] Body = "{\"product\":\"RioMobile\",\"items\":[]}"u8.ToArray();
	private static readonly TimeSpan Margin = TimeSpan.FromMinutes(2);
	private static readonly TimeSpan PostTimeout = TimeSpan.FromSeconds(10);

	private static OneApi SignedInApi()
	{
		var api = new OneApi(EnumPlatformEnvironment.AqiFeature);
		api.Authentication.SetToken("token-a", DateTime.Now.AddHours(1), "refresh");

		return api;
	}

	private static readonly Dictionary<string, Func<OneApi, object>> Apis = new()
	{
		["Core"] = api => api.Core, ["Configuration"] = api => api.Configuration, ["Logbook"] = api => api.Logbook,
		["Library"] = api => api.Library, ["Schedule"] = api => api.Schedule, ["Activity"] = api => api.Activity,
		["DigitalTwin"] = api => api.DigitalTwin, ["Notification"] = api => api.Notification, ["Report"] = api => api.Report,
		["Spreadsheet"] = api => api.Spreadsheet, ["Data"] = api => api.Data, ["Sample"] = api => api.Sample,
		["UserHelper"] = api => api.UserHelper
	};

	public static IEnumerable<object[]> ApiNames => Apis.Keys.Select(name => new object[] { name });

	// Each API is read on its own after the change. Reading Core first would rebuild the helper and clear most of the other APIs as a side
	// effect, hiding the ones the change itself left stale.
	private static void AssertRebuiltByChange(string name, Action<OneApi> change)
	{
		var api = SignedInApi();
		foreach (var read in Apis.Values)
			_ = read(api);
		var before = Apis[name](api);

		change(api);

		Apis[name](api).Should().NotBeSameAs(before, $"{name} was built on the helper the change replaced");
	}

	[Theory]
	[MemberData(nameof(ApiNames))]
	public void ChangingUseProtobufModels_RebuildsTheApi_WhicheverIsReadFirst(string name) =>
		AssertRebuiltByChange(name, api => api.UseProtobufModels = !api.UseProtobufModels);

	[Theory]
	[MemberData(nameof(ApiNames))]
	public void ChangingLogRestfulCalls_RebuildsTheApi_WhicheverIsReadFirst(string name) =>
		AssertRebuiltByChange(name, api => api.LogRestfulCalls = !api.LogRestfulCalls);

	[Theory]
	[MemberData(nameof(ApiNames))]
	public void ChangingThrowApiErrors_RebuildsTheApi_WhicheverIsReadFirst(string name) =>
		AssertRebuiltByChange(name, api => api.ThrowApiErrors = !api.ThrowApiErrors);

	[Theory]
	[MemberData(nameof(ApiNames))]
	public void ChangingContinueOnCapturedContext_RebuildsTheApi_WhicheverIsReadFirst(string name) =>
		AssertRebuiltByChange(name, api => api.ContinueOnCapturedContext = !api.ContinueOnCapturedContext);

	[Theory]
	[MemberData(nameof(ApiNames))]
	public void ChangingTheEnvironment_RebuildsTheApi_WhicheverIsReadFirst(string name) =>
		AssertRebuiltByChange(name, api => api.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage));

	[Fact]
	public void ChangingThrowApiErrors_RebuildsThePoEditorApi_AndOtherChangesKeepIt()
	{
		var api = SignedInApi();
		var poEditor = api.PoEditor;

		api.UseProtobufModels = !api.UseProtobufModels;
		api.LogRestfulCalls = !api.LogRestfulCalls;
		api.ContinueOnCapturedContext = !api.ContinueOnCapturedContext;
		api.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);
		api.PoEditor.Should().BeSameAs(poEditor, "it does not use the helper, the environment or those settings");

		api.ThrowApiErrors = !api.ThrowApiErrors;
		api.PoEditor.Should().NotBeSameAs(poEditor, "it is built with ThrowApiErrors");
	}

	[Fact]
	public async Task ACoreFromBeforeAChange_RefusesInsightsPosts_AndTheNewCoreIsADifferentInstance()
	{
		var api = SignedInApi();
		var oldCore = api.Core;

		api.UseProtobufModels = !api.UseProtobufModels;

		// The token and host are valid, so the post gets as far as the helper's own check, which fails before any request.
		await oldCore.Invoking(c => c.PostInsightsAsync(Body, null!, Margin, PostTimeout)).Should().ThrowAsync<ObjectDisposedException>();
		api.Core.Should().NotBeSameAs(oldCore);
	}

	[Fact]
	public void RepeatedSettingChanges_DoNotAccumulateTokenExpiredSubscribers()
	{
		var api = SignedInApi();
		var auth = api.Authentication;
		var baseline = TokenExpiredSubscribers.Of(auth);
		_ = api.Core;
		TokenExpiredSubscribers.Of(auth).Should().Be(baseline + 1, "the one helper listens for the token to expire");

		for (var i = 0; i < 10; i++)
		{
			api.UseProtobufModels = !api.UseProtobufModels;
			api.LogRestfulCalls = !api.LogRestfulCalls;
			_ = api.Core;
		}

		TokenExpiredSubscribers.Of(auth).Should().Be(baseline + 1, "every replaced helper was retired and stopped listening");
	}

	[Fact]
	public void AnEnvironmentChange_RetiresTheOldHelper_SoTheOldAuthenticationDropsIt()
	{
		var api = SignedInApi();
		var oldAuthentication = api.Authentication;
		var oldBaseline = TokenExpiredSubscribers.Of(oldAuthentication);
		_ = api.Core;
		TokenExpiredSubscribers.Of(oldAuthentication).Should().Be(oldBaseline + 1);

		api.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiStage);

		TokenExpiredSubscribers.Of(oldAuthentication).Should().Be(oldBaseline);
		var newAuthentication = api.Authentication;
		var newBaseline = TokenExpiredSubscribers.Of(newAuthentication);
		_ = api.Core;
		TokenExpiredSubscribers.Of(newAuthentication).Should().Be(newBaseline + 1);
	}
}
