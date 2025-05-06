using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK.Tests;

public class OneApiTests
{
	[Fact]
	public void InstantiateOneApi_Success()
	{
		var api = new OneApi(EnumPlatformEnvironment.Local);

		api.Environment.Should().Be(PlatformEnvironmentHelper.Environments.FirstOrDefault(e => e.Name == "Local"));
		api.UseProtobufModels.Should().BeTrue();
		api.ThrowApiErrors.Should().BeFalse();
		api.ContinueOnCapturedContext.Should().BeFalse();
		api.LogRestfulCalls.Should().BeFalse();

		var auth = api.Authentication;

		auth.Should().NotBeNull();
		auth.IsAuthenticated.Should().BeFalse();
		
		var client = auth.GetBaseClient("/operations/spreadsheet");

		client.Should().NotBeNull();
		client.BaseAddress.Should().Be("http://localhost:9502");

		var activity = api.Activity;
		activity.Should().NotBeNull();

		var config = api.Configuration;
		config.Should().NotBeNull();

		var twin = api.DigitalTwin;
		twin.Should().NotBeNull();

		var data = api.Data;
		data.Should().NotBeNull();

		var spreadsheet = api.Spreadsheet;
		spreadsheet.Should().NotBeNull();

		var core = api.Core;
		core.Should().NotBeNull();

		var sample = api.Sample;
		sample.Should().NotBeNull();

		var schedule = api.Schedule;
		schedule.Should().NotBeNull();

		var report = api.Report;
		report.Should().NotBeNull();

		var notification = api.Notification;
		notification.Should().NotBeNull();

		var logbook = api.Logbook;
		logbook.Should().NotBeNull();

		var library = api.Library;
		library.Should().NotBeNull();

		var userHelper = api.UserHelper;
		userHelper.Should().NotBeNull();

		var cacheHelper = api.CacheHelper;
		cacheHelper.Should().NotBeNull();
	}
}