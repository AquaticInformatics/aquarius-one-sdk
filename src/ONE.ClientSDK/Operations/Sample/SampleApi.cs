using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ONE.ClientSDK.Common.Activity;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Operations.Sample
{
	public class SampleApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;
		private readonly ActivityApi _activityApi;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public SampleApi(IOneApiHelper apiHelper, ActivityApi activityApi, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_activityApi = activityApi;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		/// <summary>
		/// Retrieves activities and their children based on criteria.
		/// </summary>
		/// <param name="authTwinRefId">
		/// Optional: Filter results based on activities associated with a specific twin, defaults
		/// to tenantId in token if not provided.
		/// </param>
		/// <param name="activityTypeId">
		/// Optional: Defaults to all. Filters results to a specific ActivityType.
		/// </param>
		/// <param name="statusCode">
		/// Optional: Defaults to all. Filters results to a specific status code.
		/// </param>
		/// <param name="priorityCode">
		/// Optional: Defaults to all. Filters results to a specific priority code.
		/// </param>
		/// <param name="startDate">
		/// Optional: Defaults to null. Filters results to those that start at or later than the
		/// specified time. Supplying a StartDate without an EndDate is acceptable.
		/// </param>
		/// <param name="endDate">
		/// Optional: Defaults to null. Filters results to those that end at or before than the
		/// specified time. Supplying a EndDate without an StartDate is acceptable.
		/// </param>
		/// <param name="scheduleId">
		/// Optional: Defaults to null. Filter results to those that are for this particular schedule.
		/// </param>
		/// <param name="cancellation"></param>
		/// <returns>One or more Activities that meet the criteria.</returns>
		public async Task<List<Activity>> GetActivitiesAsync(string authTwinRefId, string activityTypeId = null,
			int? statusCode = null, int? priorityCode = null, DateTime? startDate = null, DateTime? endDate = null,
			string scheduleId = null, CancellationToken cancellation = default) =>
			await _activityApi.GetActivitiesAsync(authTwinRefId, includeActivityDescendants: null,
				includeAuthTwinDescendants: null, activityTypeId, statusCode, priorityCode, startDate, endDate,
				scheduleId, cancellation: cancellation);

		/// <summary>
		/// Retrieves an activity and its children.
		/// </summary>
		/// <param name="activityId">
		/// Identifier of the Activity being returned.
		/// </param>
		/// <param name="includeDescendants">
		/// Optional: Defaults to false. Determines if child activities are returned.
		/// </param>
		/// <param name="cancellation"></param>
		/// <returns>The specified activity and optionally, its child activities.</returns>
		public async Task<List<Activity>> GetActivityAsync(string activityId, bool includeDescendants = false, CancellationToken cancellation = default) =>
			await _activityApi.GetOneActivityAsync(activityId, includeDescendants, cancellation);

		/// <summary>
		/// Updates one or more activities.
		/// </summary>
		/// <param name="activities"> List of activities to update. </param>
		/// <param name="updatePropertyBag">If true, the activity propertyBags will be replaced.</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the activities were updated successfully.</returns>
		public async Task<bool> UpdateActivitiesAsync(List<Activity> activities, bool updatePropertyBag = false, CancellationToken cancellation = default)
		{
			var proto = new Activities();
			proto.Items.AddRange(activities);

			return await _activityApi.UpdateActivitiesAsync(proto, updatePropertyBag, cancellation);
		}

		/// <summary>
		/// Creates an analyte
		/// </summary>
		/// <param name="analyte">Analyte to be created</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the analyte was successfully created</returns>
		public async Task<bool> CreateAnalyteAsync(Analyte analyte, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/analyte?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateAnalyteAsync", HttpMethod.Post, endpoint, cancellation, analyte).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Creates a testGroup
		/// </summary>
		/// <param name="testGroup">TestGroup to be created</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the testgroup was successfully created</returns>
		public async Task<bool> CreateTestGroupAsync(TestAnalyteGroup testGroup, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/testgroup?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateTestGroupAsync", HttpMethod.Post, endpoint, cancellation, testGroup).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Updates an analyte
		/// </summary>
		/// <param name="analyte">Analyte to be updated</param>
		/// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are updated associated with the analyte</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the analyte was successfully updated</returns>
		public async Task<bool> UpdateAnalyteAsync(Analyte analyte, DateTime? effectiveDate = null, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/analyte/{analyte.Id}?requestId={Guid.NewGuid()}";

			if (effectiveDate.HasValue)
				endpoint += $"&effectiveDate={effectiveDate.Value:O}";

			var apiResponse = await ExecuteRequest("UpdateAnalyteAsync", HttpMethod.Put, endpoint, cancellation, analyte).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Deletes an existing analyte
		/// </summary>
		/// <param name="analyteId">ID of the analyte to be deleted</param>
		/// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are deleted associated with the analyte</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the analyte was successfully deleted</returns>
		public async Task<bool> DeleteAnalyteAsync(string analyteId, DateTime? effectiveDate = null, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/analyte/{analyteId}?requestId={Guid.NewGuid()}";

			if (effectiveDate.HasValue)
				endpoint += $"&effectiveDate={effectiveDate.Value:O}";
			
			var apiResponse = await ExecuteRequest("DeleteAnalyteAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Retrieve all analytes associated to a specific authTwinRefId
		/// </summary>
		/// <param name="authTwinRefId">Reference id of the digital twin </param>
		/// <param name="includeInactive">Optional boolean value to include inactive analytes</param>
		/// <param name="cancellation"></param>
		/// <returns>List of analytes</returns>
		public async Task<List<Analyte>> GetAnalytesAsync(string authTwinRefId, bool? includeInactive = null, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/analyte/analytes/{authTwinRefId}?requestId={Guid.NewGuid()}";

			if (includeInactive.HasValue)
				endpoint += $"&includeInActive={includeInactive}";

			var apiResponse = await ExecuteRequest("GetAnalytesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Analytes?.Items.ToList();
		}

		/// <summary>
		/// Retrieves an analyte based on the provided analyte id.
		/// </summary>
		/// <param name="analyteId">ID of the analyte to retrieve</param>
		/// <param name="cancellation"></param>
		/// <returns>Analyte object</returns>
		public async Task<Analyte> GetOneAnalyteAsync(string analyteId, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/analyte/{analyteId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetOneAnalyteAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Analytes?.Items.FirstOrDefault();
		}

		/// <summary>
		/// Updates an testGroup
		/// </summary>
		/// <param name="testGroup">TestGroup to be updated</param>
		/// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are updated associated with the testGroup</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the testgroup was successfully updated</returns>
		public async Task<bool> UpdateTestGroupAsync(TestAnalyteGroup testGroup, DateTime? effectiveDate = null, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/testgroup/{testGroup.Id}?requestId={Guid.NewGuid()}";

			if (effectiveDate.HasValue)
				endpoint += $"&effectiveDate={effectiveDate.Value:O}";

			var apiResponse = await ExecuteRequest("UpdateTestGroupAsync", HttpMethod.Put, endpoint, cancellation, testGroup).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Deletes an existing testGroup
		/// </summary>
		/// <param name="testGroupId">ID of the testGroup to be deleted</param>
		/// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are deleted associated with the testGroup</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the testGroup was successfully deleted</returns>
		public async Task<bool> DeleteTestGroupAsync(string testGroupId, DateTime? effectiveDate = null, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/testgroup/{testGroupId}?requestId={Guid.NewGuid()}";

			if (effectiveDate.HasValue)
				endpoint += $"&effectiveDate={effectiveDate.Value:O}";

			var apiResponse = await ExecuteRequest("DeleteTestGroupAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Retrieve all testGroups associated to a specific authTwinRefId
		/// </summary>
		/// <param name="authTwinRefId">Reference id of the digital twin </param>
		/// <param name="cancellation"></param>
		/// <returns>List of testGroups</returns>
		public async Task<List<TestAnalyteGroup>> GetTestGroupsAsync(string authTwinRefId, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/TestGroup/For/{authTwinRefId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetTestGroupsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.TestAnalyteGroups?.Items.ToList();
		}

		/// <summary>
		/// Retrieves a TestGroup based on the provided TestGroup id.
		/// </summary>
		/// <param name="testGroupId">ID of the TestGroup to retrieve</param>
		/// <param name="cancellation"></param>
		/// <returns>TestGroup object</returns>
		public async Task<TestAnalyteGroup> GetOneTestGroupAsync(string testGroupId, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/TestGroup/{testGroupId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetOneTestGroupAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.TestAnalyteGroups?.Items.FirstOrDefault();
		}

		/// <summary>
		/// Determine if analyte is scheduled for use
		/// </summary>
		/// <param name="authTwinRefId"></param>
		/// <param name="analyteId">ID of the analyte to determine</param>
		/// <param name="cancellation"></param>
		public async Task<bool> IsAnalyteScheduledForUseAsync(string authTwinRefId, string analyteId, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/{authTwinRefId}/IsScheduledForUse?EntityType=Analyte&EntityId={analyteId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("IsAnalyteScheduledForUseAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return bool.TryParse(apiResponse?.Content?.KeyValues?.Items?.FirstOrDefault()?.Value, out var result) && result;
		}

		/// <summary>
		/// Determine if testgroup is scheduled for use
		/// </summary>
		/// <param name="authTwinRefId"></param>
		/// <param name="testGroupId">ID of the testgroup to determine</param>
		/// <param name="cancellation"></param>
		public async Task<bool> IsTestGroupScheduledForUseAsync(string authTwinRefId, string testGroupId, CancellationToken cancellation = default)
		{
			var endPointUrl = $"/operations/sample/v1/{authTwinRefId}/IsScheduledForUse?EntityType=TestGroup&EntityId={testGroupId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("IsTestGroupScheduledForUseAsync", HttpMethod.Get, endPointUrl, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return bool.TryParse(apiResponse?.Content?.KeyValues?.Items?.FirstOrDefault()?.Value, out var result) && result;
		}

		/// <summary>
		/// Creates a new sample schedule. 
		/// This method will start a background process that will create new Activities associated
		/// with the schedule. The SampleCache will need to be refreshed to include the new Activities.
		/// </summary>
		/// <param name="schedule">The new schedule to be created.</param>
		/// <param name="cancellation"></param>
		/// <returns>The schedule that was created or null if the schedule could not be created.</returns>
		public async Task<Schedule> CreateSampleScheduleAsync(Schedule schedule, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/schedule?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateSampleScheduleAsync", HttpMethod.Post, endpoint, cancellation, schedule).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Schedules?.Items.FirstOrDefault();
		}

		/// <summary>
		/// Updates a sample schedule. 
		/// This method will start a background process that will update Activities associated
		/// with the schedule. The SampleCache will need to be refreshed to include the updated Activities.
		/// </summary>
		/// <param name="sampleScheduleId">The ID of the schedule to update.</param>
		/// <param name="schedule">The schedule to update.</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the schedule was successfully updated.</returns>
		public async Task<bool> UpdateSampleScheduleAsync(Guid sampleScheduleId, Schedule schedule, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/schedule/{sampleScheduleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateSampleScheduleAsync", HttpMethod.Put, endpoint, cancellation, schedule).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Deletes a sample schedule. 
		/// This method will start a background process that will delete Activities associated
		/// with the schedule. The SampleCache will need to be refreshed to remove the deleted Activities.
		/// </summary>
		/// <param name="sampleScheduleId">The ID of the schedule to delete.</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the schedule was successfully deleted.</returns>
		public async Task<bool> DeleteSampleScheduleAsync(Guid sampleScheduleId, CancellationToken cancellation = default)
		{
			var endpoint = $"/operations/sample/v1/schedule/{sampleScheduleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteSampleScheduleAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = System.Diagnostics.Stopwatch.StartNew();

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
						Module = "SampleApi",
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
						Module = "SampleApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
