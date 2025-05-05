using Moq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Operations.Spreadsheet;
using ONE.ClientSDK.Utilities;

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
			.Setup(x => x.BuildRequestAndSendAsync<ApiResponse>(
				HttpMethod.Post,
				It.IsAny<string>(),
				It.IsAny<CancellationToken>(),
				cell))
			.ReturnsAsync(apiResponse);

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
			.Setup(x => x.BuildRequestAndSendAsync<ApiResponse>(
				HttpMethod.Post,
				It.IsAny<string>(),
				It.IsAny<CancellationToken>(),
				cell))
			.ReturnsAsync(apiResponse);

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
			.Setup(x => x.BuildRequestAndSendAsync<ApiResponse>(
				HttpMethod.Post,
				It.IsAny<string>(),
				It.IsAny<CancellationToken>(),
				cell))
			.ReturnsAsync(apiResponse);

		// Act
		
		var result = await GetApi(false).CellValidateAsync(Guid.NewGuid().ToString(), EnumWorksheet.WorksheetDaily, cell);

		// Assert
		result.Should().BeNull();
	}
}
