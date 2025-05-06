using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Shared.Helpers.JsonPatch;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Schedule
{
	public class ScheduleApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public ScheduleApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		/// <summary>
		/// Gets a list of schedules.
		/// </summary>
		/// <param name="authTwinRefId">The ID of the digital twin to authorize against.</param>
		/// <param name="includeAuthTwinChildren">Indicates whether schedules for children of the authTwin will be returned.</param>
		/// <param name="scheduleTypeId">ID of the schedule type to be returned.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a list of <see cref="Schedule"/>.</returns>
		public async Task<List<Models.CSharp.Schedule>> GetSchedulesAsync(string authTwinRefId, bool? includeAuthTwinChildren, string scheduleTypeId, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/schedule/v1/schedules/{authTwinRefId}?requestId={Guid.NewGuid()}";

			if (includeAuthTwinChildren.HasValue)
				endpoint += $"&includeAuthTwinChildren={includeAuthTwinChildren}";
			
			if (!string.IsNullOrWhiteSpace(scheduleTypeId))
				endpoint += $"&scheduleTypeId={scheduleTypeId}";

			var apiResponse = await ExecuteRequest("GetSchedulesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Schedules?.Items.ToList();
		}

		/// <summary>
		/// Gets a single schedule.
		/// </summary>
		/// <param name="scheduleId">ID of the schedule.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a <see cref="Schedule"/>.</returns>
		public async Task<Models.CSharp.Schedule> GetOneScheduleAsync(string scheduleId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/schedule/v1/{scheduleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetOneScheduleAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Schedules?.Items.FirstOrDefault();
		}

		/// <summary>
		/// Saves a schedule.
		/// </summary>
		/// <param name="schedule">Schedule to save.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> SaveScheduleAsync(Models.CSharp.Schedule schedule, CancellationToken cancellation = default)
		{
			if (schedule == null)
				return true;

			var endpoint = $"/common/schedule/v1?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("SaveScheduleAsync", HttpMethod.Post, endpoint, cancellation, schedule).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Updates a schedule.
		/// </summary>
		/// <param name="schedule">The schedule to update.</param>
		/// <param name="updatePropertyBag">If true, the schedule propertyBag will be replaced.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateScheduleAsync(Models.CSharp.Schedule schedule, bool updatePropertyBag = false, CancellationToken cancellation = default)
		{
			if (schedule == null)
				return true;

			var endpoint = $"/common/schedule/v1/{schedule.Id}?requestId={Guid.NewGuid()}&updatePropertyBag={updatePropertyBag}";

			var apiResponse = await ExecuteRequest("UpdateScheduleAsync", HttpMethod.Put, endpoint, cancellation, schedule).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Deletes a schedule.
		/// </summary>
		/// <param name="scheduleId">Id of schedule to delete.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> DeleteScheduleAsync(string scheduleId, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/schedule/v1/{scheduleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteScheduleAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/// <summary>
		/// Gets schedule occurrences.
		/// </summary>
		/// <param name="pattern">Schedule recurrence pattern. <see cref="ScheduleRecurrencePattern"/>.</param>
		/// <param name="afterDate">The time after which occurrences will start.</param>
		/// <param name="beforeDate">The time before which occurrences will end.</param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a list of <see cref="ScheduleOccurrence"/>.</returns>
		public async Task<List<ScheduleOccurrence>> GetOccurrencesAsync(ScheduleRecurrencePattern pattern, DateTime afterDate, DateTime beforeDate, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/schedule/v1/occurrences?afterDate={afterDate:O}&beforeDate={beforeDate:O}&requestId={Guid.NewGuid()}";
			
			var apiResponse = await ExecuteRequest("GetOccurrencesAsync", HttpMethod.Post, endpoint, cancellation, pattern).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ScheduleOccurrences?.Items.ToList();
		}

		/// <summary>
		/// Updates the property bag of a schedule.
		/// </summary>
		/// <param name="scheduleId">The ID of the schedule.</param>
		/// <param name="propertyBagUpdates">Defines the updates to be performed. <see cref="OneJsonPatchItems"/></param>
		/// <param name="cancellation"></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateSchedulePropertyBagAsync(Guid scheduleId, OneJsonPatchItems propertyBagUpdates, CancellationToken cancellation = default)
		{
			var endpoint = $"/common/schedule/v1/UpdatePropertyBag/{scheduleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateSchedulePropertyBagAsync", new HttpMethod("PATCH"), endpoint, cancellation, propertyBagUpdates).ConfigureAwait(_continueOnCapturedContext);

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
						Module = "ScheduleApi",
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
						Module = "ScheduleApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
