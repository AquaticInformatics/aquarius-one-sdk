using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Historian.Data
{
	public class DataApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;
		
		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
		
		public DataApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		public async Task<List<HistorianData>> GetDataAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate, CancellationToken cancellation = default)
		{
			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}?startTime={startDate:O}&endTime={endDate:O}&requestId={Guid.NewGuid()}";
			
			var apiResponse = await ExecuteRequest("GetDataAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.HistorianDatas?.Items.ToList();
		}

		public async Task<HistorianData> GetOneDataAsync(string telemetryTwinRefId, DateTime dateTime, CancellationToken cancellation = default)
		{
			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}/{dateTime:O}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetOneDataAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.HistorianDatas?.Items.FirstOrDefault();
		}

		public async Task<bool> SaveDataAsync(string telemetryTwinRefId, HistorianDatas historianDatas, CancellationToken cancellation = default)
		{
			if (historianDatas?.Items == null || historianDatas.Items.Count == 0)
				return true;
													  
			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}?requestId={Guid.NewGuid()}";
			
			var apiResponse = await ExecuteRequest("SaveDataAsync", HttpMethod.Post, endpoint, cancellation, historianDatas).ConfigureAwait(_continueOnCapturedContext);
			
			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> SaveBulkDataAsync(string telemetryTwinRefId, HistorianDatas historianDatas, CancellationToken cancellation = default)
		{
			if (historianDatas?.Items == null || historianDatas.Items.Count == 0)
				return true;

			var endpoint =$"/historian/data/v1/{telemetryTwinRefId}/bulkingest?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("SaveBulkDataAsync", HttpMethod.Post, endpoint, cancellation, historianDatas).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UpdateDataAsync(string telemetryTwinRefId, HistorianData historianData, CancellationToken cancellation = default)
		{
			if (historianData == null)
				return true;

			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}?requestId={Guid.NewGuid()}";
			
			var apiResponse = await ExecuteRequest("UpdateDataAsync", HttpMethod.Put, endpoint, cancellation, historianData).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> DeleteManyAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate, CancellationToken cancellation = default)
		{
			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}?startTime={startDate:O}&endTime={endDate:O}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteManyAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> FlushAsync(string telemetryTwinRefId, CancellationToken cancellation = default)
		{
			var endpoint = $"/historian/data/v1/{telemetryTwinRefId}/Flush?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("FlushAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync(httpMethod, endpoint, cancellation, content, true).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = " Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (!apiResponse.StatusCode.IsSuccessStatusCode())
				{
					message = " Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ApiResponse = apiResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(null,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "DataApi",
						Message = callingMethod + message
					});

				return apiResponse;
			}
			catch (Exception e)
			{
				Event(e,
					new ClientApiLoggerEventArgs
					{
						EventLevel = EnumOneLogLevel.OneLogLevelError,
						Module = "DataApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
