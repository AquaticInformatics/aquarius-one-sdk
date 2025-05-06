using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Shared.Helpers.JsonPatch;
using Proto = ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Activity
{
	public class ActivityApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public ActivityApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		/// <summary>
		/// Get activities based on criteria provided through optional parameters.
		/// </summary>
		/// <param name="authTwinRefId">Required: The Id of the digital twin to authorize against.</param>
		/// <param name="includeActivityDescendants">Optional: If true, activity descendants will be included.</param>
		/// <param name="includeAuthTwinDescendants">Optional: If true, activities associated with descendants of the authTwin will be included.</param>
		/// <param name="activityTypeId">Optional: If provided, only activities of this type will be returned, otherwise activities with any activity type will be returned.</param>
		/// <param name="statusCode">Optional: If provided, only activities with this statusCode will be return, otherwise activities with any statusCode will be returned.</param>
		/// <param name="priorityCode">Optional: If provided, only activities with this priorityCode will be return, otherwise activities with any priorityCode will be returned.</param>
		/// <param name="startDate">Optional: If provided, only activities equal to or after this time will be returned.</param>
		/// <param name="endDate">Optional: If provided, only activities equal to or before this time will be returned.</param>
		/// <param name="scheduleId">Optional: If provided, only activities associated with this schedule will be returned, otherwise activities associated with any schedule are returned.</param>
		/// <param name="context">Optional: If provided, activities returned are based on a context search on the activity propertyBag.</param>
		/// <param name="descendantTwinType">Optional: If provided and includeAuthTwinDescendants is true, only activities for authTwin descendants of this twin type will be returned. </param>
		/// <param name="activeActivitiesOnly">Optional: If provided, only activities with an active statusCode will be returned.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a list of <see cref="Proto.Activity"/></returns>
		public async Task<List<Proto.Activity>> GetActivitiesAsync(string authTwinRefId = null, 
			bool? includeActivityDescendants = null, bool? includeAuthTwinDescendants = null, 
			string activityTypeId = null, int? statusCode = null, int? priorityCode = null, 
			DateTime? startDate = null, DateTime? endDate = null, string scheduleId = null, 
			string context = null, Guid? descendantTwinType = null, bool? activeActivitiesOnly = null, CancellationToken cancellation = default)
		{
			var queryParameters = new Dictionary<string, string>();

			if (!string.IsNullOrEmpty(authTwinRefId))
				queryParameters.Add(nameof(authTwinRefId), authTwinRefId);

			if (includeActivityDescendants.HasValue)
				queryParameters.Add(nameof(includeActivityDescendants), includeActivityDescendants.Value.ToString());

			if (includeAuthTwinDescendants.HasValue)
				queryParameters.Add(nameof(includeAuthTwinDescendants), includeAuthTwinDescendants.Value.ToString());

			if (!string.IsNullOrEmpty(activityTypeId))
				queryParameters.Add(nameof(activityTypeId), activityTypeId);

			if (statusCode.HasValue)
				queryParameters.Add(nameof(statusCode), statusCode.Value.ToString());

			if (priorityCode.HasValue)
				queryParameters.Add(nameof(priorityCode), priorityCode.Value.ToString());

			if (startDate.HasValue)
				queryParameters.Add("startTime", startDate.Value.ToString("O"));

			if (endDate.HasValue)
				queryParameters.Add("endTime", endDate.Value.ToString("O"));

			if (!string.IsNullOrEmpty(scheduleId))
				queryParameters.Add(nameof(scheduleId), scheduleId);

			if (!string.IsNullOrEmpty(context))
				queryParameters.Add(nameof(context), context);

			if (descendantTwinType.HasValue)
				queryParameters.Add(nameof(descendantTwinType), descendantTwinType.Value.ToString());

			if (activeActivitiesOnly.HasValue)
				queryParameters.Add(nameof(activeActivitiesOnly), activeActivitiesOnly.ToString());

			var sb = new StringBuilder($"/common/activity/v1?requestId={Guid.NewGuid()}");
			
			foreach (var queryParameter in queryParameters)
				sb.Append($"&{queryParameter.Key}={queryParameter.Value}");

			var apiResponse = await ExecuteRequest("GetActivitiesAsync", HttpMethod.Get, sb.ToString(), cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Activities?.Items.ToList();
		}

		/// <summary>
		/// Gets a single activity and optionally, its descendants.
		/// </summary>
		/// <param name="activityId">The Id of the activity to retrieve.</param>
		/// <param name="includeDescendants">Optional: If true, the result will include all activity descendants.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a list of <see cref="Proto.Activity"/></returns>
		public async Task<List<Proto.Activity>> GetOneActivityAsync(string activityId, bool includeDescendants = false, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetOneActivityAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Activities?.Items.ToList();
		}

		/// <summary>
		/// Saves activities.
		/// </summary>
		/// <param name="activities">Activities to save.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> SaveActivitiesAsync(Activities activities, CancellationToken cancellation = default)
		{
			if (activities == null || !activities.Items.Any())
				return true;

			var endpoint = $"/common/activity/v1?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("SaveActivitiesAsync", HttpMethod.Post, endpoint, cancellation, activities).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Updates activities.
		/// </summary>
		/// <param name="activities">Activities to update.</param>
		/// <param name="updatePropertyBag">If true, the activity propertyBags will be replaced.</param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateActivitiesAsync(Activities activities, bool updatePropertyBag = false, CancellationToken cancellation = default)
		{
			if (activities == null || !activities.Items.Any())
				return true;

			var endpoint = $"/common/activity/v1?requestId={Guid.NewGuid()}&updatePropertyBag={updatePropertyBag}";

			var apiResponse = await ExecuteRequest("UpdateActivitiesAsync", HttpMethod.Put, endpoint, cancellation, activities).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Updates the property bag of an activity.
		/// </summary>
		/// <param name="activityId">The Id of the activity.</param>
		/// <param name="propertyBagUpdates">Defines the updates to be performed. <see cref="OneJsonPatchItems"/></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateActivityPropertyBagAsync(Guid activityId, OneJsonPatchItems propertyBagUpdates, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/activity/v1/UpdatePropertyBag/{activityId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateActivityPropertyBagAsync", new HttpMethod("PATCH"), endpoint, cancellation, propertyBagUpdates).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Deletes an activity.
		/// </summary>
		/// <param name="activityId">Id of the activity.</param>
		/// <param name="includeDescendants">Optional: Activity descendants will be deleted unless this is false.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> DeleteActivityAsync(string activityId, bool includeDescendants = true, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteActivityAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

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
						Module = "ActivityApi",
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
						Module = "ActivityApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
