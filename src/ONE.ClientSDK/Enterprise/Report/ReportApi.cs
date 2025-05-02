using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Enterprise.Report
{
	public class ReportApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public ReportApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		public async Task<List<ReportDefinition>> GetDefinitionsAsync(string operationId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/definitions?plantId={operationId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetDefinitionsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitions?.Items.ToList();
		}

		public async Task<ReportDefinition> GetDefinitionAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/definitions/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetDefinitionAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitions?.Items.FirstOrDefault();
		}

		public async Task<ReportDefinition> CreateDefinitionAsync(ReportDefinition reportDefinition, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/definitions?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateDefinitionAsync", HttpMethod.Post, endpoint, cancellation, reportDefinition).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitions?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteDefinitionAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/definitions/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteDefinitionAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<ReportDefinition> UpdateDefinitionAsync(ReportDefinition reportDefinition, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/definitions/{reportDefinition.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateDefinitionAsync", new HttpMethod("PATCH"), endpoint, cancellation, reportDefinition).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitions?.Items.FirstOrDefault();
		}

		public async Task<ReportDefinition> UploadDefinitionTemplateAsync(string id, string filePath, CancellationToken cancellation = default)
		{
			try
			{
				var endpoint = $"enterprise/report/v1/definitions/upload/{id}?requestId={Guid.NewGuid()}";

				if (string.IsNullOrWhiteSpace(filePath))
					throw new ArgumentNullException(nameof(filePath));

				if (!File.Exists(filePath))
					throw new FileNotFoundException($"File [{filePath}] not found.");

				var fileBytes = File.ReadAllBytes(filePath);
				var fileContent = new ByteArrayContent(fileBytes);
				fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
				var form = new MultipartFormDataContent();
				form.Add(fileContent, "file", Path.GetFileName(filePath));

				var request = _apiHelper.CreateRequest(HttpMethod.Post, endpoint);
				request.Content = form;

				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.SendAsync<ApiResponse>(request, cancellation).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = "UploadDefinitionTemplateAsync Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (apiResponse == null || !apiResponse.StatusCode.IsSuccessStatusCode())
				{
					message = "UploadDefinitionTemplateAsync Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ApiResponse = apiResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = apiResponse == null ? HttpStatusCode.InternalServerError : (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "ReportApi",
						Message = message
					});

				return apiResponse?.Content?.ReportDefinitions?.Items.FirstOrDefault();
			}
			catch (Exception e)
			{
				Event(e,
					new ClientApiLoggerEventArgs
					{
						EventLevel = EnumOneLogLevel.OneLogLevelError,
						Module = "ReportApi",
						Message = $"UploadDefinitionTemplateAsync Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}

		public async Task<ReportDefinitionRun> RenderReportAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/render/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("RenderReportAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitionRuns?.Items.FirstOrDefault();
		}

		public async Task<bool> DownloadReportAsync(string id, string filename, CancellationToken cancellation = default)
		{
			try
			{
				var endpoint = $"enterprise/report/v1/report/output/{id}/report?requestId={Guid.NewGuid()}";

				var watch = Stopwatch.StartNew();

				var httpResponse = await _apiHelper.BuildRequestAndSendAsync<HttpResponseMessage>(HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);
				
				watch.Stop();

				var message = "DownloadReportAsync Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (httpResponse.IsSuccessStatusCode)
				{
					using (var fs = new FileStream(filename, FileMode.CreateNew))
					{
						await httpResponse.Content.CopyToAsync(fs);
					}
				} 
				else
				{
					message = "DownloadReportAsync Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ResponseMessage = httpResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = httpResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "ReportApi",
						Message = message
					});

				return httpResponse.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "ReportApi", Message = $"DownloadReportAsync Failed - {e.Message}" });
				if (_throwApiErrors) 
					 throw;
				return false;
			}
		}

		public async Task<bool> DownloadTemplateAsync(string id, string filename, CancellationToken cancellation = default)
		{
			try
			{
				var endpoint = $"enterprise/report/v1/report/output/{id}/template?requestId={Guid.NewGuid()}";

				var watch = Stopwatch.StartNew();

				var httpResponse = await _apiHelper.BuildRequestAndSendAsync<HttpResponseMessage>(HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = "DownloadTemplateAsync Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (httpResponse.IsSuccessStatusCode)
				{
					using (var fs = new FileStream(filename, FileMode.CreateNew))
					{
						await httpResponse.Content.CopyToAsync(fs);
					}
				}
				else
				{
					message = "DownloadTemplateAsync Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ResponseMessage = httpResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = httpResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "ReportApi",
						Message = message
					});

				return httpResponse.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "ReportApi", Message = $"DownloadTemplateAsync Failed - {e.Message}" });
				if (_throwApiErrors)
					throw;
				return false;
			}
		}

		public async Task<List<ReportDefinitionTag>> GetReportTagsAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/tags?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetReportTagsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitionTags?.Items.ToList();
		}

		public async Task<ReportDefinitionTag> GetReportTagAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/tags/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetReportTagAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitionTags?.Items.FirstOrDefault();
		}

		public async Task<ReportDefinitionTag> CreateReportTagAsync(string reportDefinitionId, string tag, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/tags?requestId={Guid.NewGuid()}";

			var reportDefinitionTag = new ReportDefinitionTag
			{
				ReportDefinitionId = reportDefinitionId,
				Tag = tag
			};

			var apiResponse = await ExecuteRequest("CreateReportTagAsync", HttpMethod.Post, endpoint, cancellation, reportDefinitionTag).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitionTags?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteReportTagAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/tags/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteReportTagAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<ReportDefinitionTag> UpdateReportDefinitionTagAsync(ReportDefinitionTag reportDefinitionTag, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/report/v1/report/tags/{reportDefinitionTag.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateReportDefinitionTagAsync", new HttpMethod("PATCH"), endpoint, cancellation, reportDefinitionTag).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ReportDefinitionTags?.Items.FirstOrDefault();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync<ApiResponse>(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

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

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "ReportApi",
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
						Module = "ReportApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
