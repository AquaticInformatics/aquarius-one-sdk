using Moq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Operations.Spreadsheet;
using ONE.ClientSDK.Utilities;
using System.Web;

namespace ONE.ClientSDK.Tests.ApiTests;

public class SpreadsheetApiTests
{
	private readonly IOneApiHelper _apiHelper = Mock.Of<IOneApiHelper>();

	private SpreadsheetApi GetApi(bool throwApiErrors) => new(_apiHelper, false, throwApiErrors);

	[Fact]
	public async Task CellValidateAsync_ShouldReturnValidatedCell_WhenApiResponseIsSuccessful()
	{
		// Arrange
		var cell = new Cell { ColumnNumber = 4, CellDatas = { new CellData { StringValue = "5.235" } } };

		var apiResponse = new ApiResponse
		{
			Content = new Content
			{
				Cells = new Cells { Items = { { 4, cell } } }
			},
			StatusCode = 200
		};

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Post, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                cell, false)).ReturnsAsync(apiResponse);

		// Act
		var result = await GetApi(true).CellValidateAsync(Guid.NewGuid().ToString(), EnumWorksheet.WorksheetDaily, cell);

		// Assert
		result.Should().BeEquivalentTo(cell);
	}

	[Fact]
	public async Task CellValidateAsync_ShouldThrow_WhenApiResponseIsNotSuccessful_AndThrowApiErrorsIsTrue()
	{
		// Arrange
		var cell = new Cell { ColumnNumber = 4, CellDatas = { new CellData { StringValue = "5.235" } } };

		var apiResponse = new ApiResponse { StatusCode = 400 };

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Post, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                cell, false)).ReturnsAsync(apiResponse);

		// Act
		await Assert.ThrowsAsync<RestApiException>(() =>
			GetApi(true).CellValidateAsync(Guid.NewGuid().ToString(), EnumWorksheet.WorksheetDaily, cell));
	}

	[Fact]
	public async Task CellValidateAsync_ShouldReturnNull_WhenApiResponseIsNotSuccessful_AndThrowApiErrorsIsFalse()
	{
		// Arrange
		var cell = new Cell { ColumnNumber = 4, CellDatas = { new CellData { StringValue = "5.235" } } };

		var apiResponse = new ApiResponse { StatusCode = 400 };

        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Post, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                cell, false)).ReturnsAsync(apiResponse);

		// Act
		
		var result = await GetApi(false).CellValidateAsync(Guid.NewGuid().ToString(), EnumWorksheet.WorksheetDaily, cell);

		// Assert
		result.Should().BeNull();
	}

    [Fact]
    public async Task GetRowsLimitedCellDataAsync_NoColumnsOrView_ValidateRelativeUri()
    {
        // Arrange
        var apiResponse = new ApiResponse { Content = new Content { Rows = new Rows() }, StatusCode = 200 };

		var endpointString = string.Empty;
        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Get, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                null, false)).ReturnsAsync(apiResponse)
            .Callback((HttpMethod _, string uri, CancellationToken _, object _, bool _) => endpointString = uri);

        // Act
        var result = await GetApi(false).GetRowsAsync(Guid.NewGuid().ToString(),
            EnumWorksheet.WorksheetDaily, 1, 2, [], null, 5, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        var query = HttpUtility.ParseQueryString(endpointString.Split('?')[1]);
        query.Count.Should().Be(4);
        query["requestId"].Should().NotBeNull();
        query["startRow"].Should().Be("1");
		query["endRow"].Should().Be("2");
		query["maxCellDataIncluded"].Should().Be("5");
		query["viewId"].Should().BeNull();
		query["columns"].Should().BeNull();

        Mock.Get(_apiHelper).Verify(x => x.BuildRequestAndSendAsync(HttpMethod.Get, It.IsAny<string>(), CancellationToken.None, null, false), Times.Once);
    }

    [Fact]
    public async Task GetRowsLimitedCellDataAsync_WithColumnsAndView_ValidateRelativeUri()
    {
        // Arrange
        var apiResponse = new ApiResponse { Content = new Content { Rows = new Rows() }, StatusCode = 200 };

        var endpointString = string.Empty;
        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Get, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                null, false)).ReturnsAsync(apiResponse)
            .Callback((HttpMethod _, string uri, CancellationToken _, object _, bool _) => endpointString = uri);

        // Act
        var result = await GetApi(false).GetRowsAsync(Guid.NewGuid().ToString(),
            EnumWorksheet.WorksheetDaily, 1, 2, [1,2], Guid.NewGuid(), null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        var query = HttpUtility.ParseQueryString(endpointString.Split('?')[1]);
        query.Count.Should().Be(6);
        query["requestId"].Should().NotBeNull();
        query["startRow"].Should().Be("1");
        query["endRow"].Should().Be("2");
        query["maxCellDataIncluded"].Should().BeNull();
        query["viewId"].Should().NotBeNull();
        query["columns[0]"].Should().Be("1");
        query["columns[1]"].Should().Be("2");

        Mock.Get(_apiHelper).Verify(x => x.BuildRequestAndSendAsync(HttpMethod.Get, It.IsAny<string>(), CancellationToken.None, null, false), Times.Once);
    }

    [Fact]
    public async Task GetRowsAsync_WithChangedSince_ValidateRelativeUri()
    {
        var apiResponse = new ApiResponse { Content = new Content { Rows = new Rows() }, StatusCode = 200 };

        var endpointString = string.Empty;
        Mock.Get(_apiHelper)
            .Setup(x => x.BuildRequestAndSendAsync(HttpMethod.Get, It.IsAny<string>(), It.IsAny<CancellationToken>(),
                null, false)).ReturnsAsync(apiResponse)
            .Callback((HttpMethod _, string uri, CancellationToken _, object _, bool _) => endpointString = uri);

        var now = DateTime.UtcNow.ToString("O");
        var result = await GetApi(false).GetRowsAsync(Guid.NewGuid().ToString(),
            EnumWorksheet.WorksheetDaily, 1, 2, [],null, 10, now, CancellationToken.None);

        result.Should().NotBeNull();

        var query = HttpUtility.ParseQueryString(endpointString.Split('?')[1]);
        query.Count.Should().Be(5);
        query["requestId"].Should().NotBeNull();
        query["startRow"].Should().Be("1");
        query["endRow"].Should().Be("2");
        query["maxCellDataIncluded"].Should().Be("10");
        query["viewId"].Should().BeNull();
        query["columns"].Should().BeNull();
        query["changedSince"].Should().Be(now);

        Mock.Get(_apiHelper).Verify(x => x.BuildRequestAndSendAsync(HttpMethod.Get, endpointString, CancellationToken.None, null, false), Times.Once);
    }
}
