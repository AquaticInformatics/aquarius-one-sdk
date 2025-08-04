using Moq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enterprise.Twin;
using ONE.ClientSDK.Utilities;
using System.Web;

namespace ONE.ClientSDK.Tests.ApiTests;

public class DigitalTwinApiTests
{
    private readonly IOneApiHelper _apiHelper = Mock.Of<IOneApiHelper>();

    private DigitalTwinApi GetApi(bool throwApiErrors) => new(_apiHelper, false, throwApiErrors);

    private string[] DigitalTwinRefIds = ["588f884b-6338-40b0-b137-3a151f49b34e", "6e73865f-23e6-4cab-b8a9-c257dd45dd16"];

    private List<DigitalTwin> GetDigitalTwins() =>
    [
        new DigitalTwin
        {
            Name = "TwinOne",
            TwinReferenceId = DigitalTwinRefIds[0]
        },
        new DigitalTwin
        {
            Name = "TwinTwo",
            TwinReferenceId = DigitalTwinRefIds[1]
        }
    ];

    [Fact]
    public async Task UpdateManyAsync_SuccessfulResponse()
    {
        var digitalTwins = new DigitalTwins();
        digitalTwins.Items.AddRange(GetDigitalTwins());

        var apiResponse = new ApiResponse { StatusCode = 200, Content = new Content { DigitalTwins = digitalTwins } };

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Patch, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                It.IsAny<List<DigitalTwin>>(), false)).ReturnsAsync(apiResponse);

        var response = await GetApi(false).UpdateManyAsync(GetDigitalTwins());

        response.Should().NotBeNull();
        response.Count.Should().Be(2);
        response[0].TwinReferenceId.Should().Be(DigitalTwinRefIds[0]);
        response[1].TwinReferenceId.Should().Be(DigitalTwinRefIds[1]);
    }

    [Fact]
    public async Task UpdateManyAsync_UnsuccessfulResponse_ThrowApiErrorIsTrue_ThrowsException()
    {
        var apiResponse = new ApiResponse { StatusCode = 400 };

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Patch, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                It.IsAny<List<DigitalTwin>>(), false)).ReturnsAsync(apiResponse);

        await Assert.ThrowsAsync<RestApiException>(() => GetApi(true).UpdateManyAsync(GetDigitalTwins()));
    }

    [Fact]
    public async Task UpdateManyAsync_UnsuccessfulResponse_ThrowApiErrorIsFalse_ReturnsNull()
    {
        var apiResponse = new ApiResponse { StatusCode = 400 };

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Patch, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                It.IsAny<List<DigitalTwin>>(), false)).ReturnsAsync(apiResponse);

        var response = await GetApi(false).UpdateManyAsync(GetDigitalTwins());

        response.Should().BeNull();
    }

    [Fact]
    public async Task UpdateManyAsync_ValidateRelativeUri()
    {
        var apiResposne = new ApiResponse { StatusCode = 200, Content = new Content { DigitalTwins = new DigitalTwins() } };

        var endpointString = string.Empty;

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Patch, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                It.IsAny<List<DigitalTwin>>(), false)).ReturnsAsync(apiResposne)
            .Callback((HttpMethod _, string uri, CancellationToken _, object _, bool _) => endpointString = uri);

        var result = await GetApi(false).UpdateManyAsync(GetDigitalTwins());

        result.Should().NotBeNull();

        var splitEndpoint = endpointString.Split('?');
        
        splitEndpoint[0].Should().Be("enterprise/twin/v1/DigitalTwin/update");

        var query = HttpUtility.ParseQueryString(splitEndpoint[1]);
        query.Count.Should().Be(1);
        query["requestId"].Should().NotBeNull();

        Mock.Get(_apiHelper).Verify(x => x.BuildRequestAndSendAsync(HttpMethod.Patch, It.IsAny<string>(), It.IsAny<CancellationToken>(),
            It.IsAny<List<DigitalTwin>>(), false), Times.Once);
    }
}
