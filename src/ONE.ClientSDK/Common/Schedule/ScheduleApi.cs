using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Shared.Helpers.JsonPatch;

namespace ONE.ClientSDK.Common.Schedule
{
	public class ScheduleApi
	{
		public ScheduleApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
		{
			_environment = environment;
			_continueOnCapturedContext = continueOnCapturedContext;
			_restHelper = restHelper;
			_throwApiErrors = throwAPIErrors;
		}

		private PlatformEnvironment _environment;
		private readonly bool _continueOnCapturedContext;
		private readonly RestHelper _restHelper;
		private readonly bool _throwApiErrors;
		private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		/// <summary>
		/// Gets a list of schedules.
		/// </summary>
		/// <param name="authTwinRefId">The Id of the digital twin to authorize against.</param>
		/// <param name="includeAuthTwinChildren">Indicates whether schedules for children of the authTwin will be returned.</param>
		/// <param name="scheduleTypeId">Id of the schedule type to be returned.</param>
		/// <returns>Task that returns a list of <see cref="Schedule"/>.</returns>
		public async Task<List<Models.CSharp.Schedule>> GetSchedulesAsync(string authTwinRefId, 
			bool? includeAuthTwinChildren, string scheduleTypeId)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			var requestId = Guid.NewGuid();
			var endPointUrl = $"/common/schedule/v1/schedules/{authTwinRefId}";

			if (includeAuthTwinChildren != null)
			{
				endPointUrl += $"?{includeAuthTwinChildren}";
			}

			if (!string.IsNullOrWhiteSpace(scheduleTypeId))
			{
				endPointUrl += includeAuthTwinChildren == null ? $"?{scheduleTypeId}" : $"&{scheduleTypeId}";
			}

			try
			{
				var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId,endPointUrl)
					.ConfigureAwait(_continueOnCapturedContext);

				Event(null,
					respContent.ResponseMessage.IsSuccessStatusCode
						? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetSchedulesAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
						: CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "GetSchedulesAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

				return respContent.ResponseMessage.IsSuccessStatusCode ? respContent.ApiResponse.Content.Schedules.Items.ToList() : null;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetSchedulesAsync Failed - {e.Message}"));

				if (_throwApiErrors) 
					 throw;

				return null;
			}
		}

		/// <summary>
		/// Gets a single schedule.
		/// </summary>
		/// <param name="scheduleId">Id of the schedule.</param>
		/// <returns>Task that returns a <see cref="Schedule"/>.</returns>
		public async Task<Models.CSharp.Schedule> GetOneScheduleAsync(string scheduleId)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			var requestId = Guid.NewGuid();
			var endPointUrl = $"common/schedule/v1/{scheduleId}";

			try
			{
				var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl)
					.ConfigureAwait(_continueOnCapturedContext);

				if (respContent.ResponseMessage.IsSuccessStatusCode)
				{
					foreach (var schedule in respContent.ApiResponse.Content.Schedules.Items)
					{
						Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOneScheduleAsync Success",
							respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

						return schedule;
					}
				}

				Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "GetOneScheduleAsync Failed",
					respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

				return null;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetOneScheduleAsync Failed - {e.Message}"));

				if (_throwApiErrors) 
					 throw;

				return null;
			}
		}

		/// <summary>
		/// Saves a schedule.
		/// </summary>
		/// <param name="schedule">Schedule to save.</param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> SaveScheduleAsync(Models.CSharp.Schedule schedule)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			if (schedule == null)
				return true;

			var requestId = Guid.NewGuid();
			const string endpoint = "/common/schedule/v1/";
			var json = JsonConvert.SerializeObject(schedule, _serializerSettings);
			
			try
			{
				var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint)
					.ConfigureAwait(_continueOnCapturedContext);

				Event(null,
					respContent.ResponseMessage.IsSuccessStatusCode
						? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "SaveScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
						: CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "SaveScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

				return respContent.ResponseMessage.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"SaveScheduleAsync Failed - {e.Message}"));

				if (_throwApiErrors) 
					 throw;

				return false;
			}
		}

		/// <summary>
		/// Updates a schedule.
		/// </summary>
		/// <param name="schedule">The schedule to update.</param>
		/// <param name="updatePropertyBag">If true, the schedule propertyBag will be replaced.</param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateScheduleAsync(Models.CSharp.Schedule schedule, bool updatePropertyBag = false)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			if (schedule == null)
				return true;

			var requestId = Guid.NewGuid();
			var endpoint = $"/common/schedule/v1/{schedule.Id}";
			if (updatePropertyBag)
				endpoint += "?updatePropertyBag=true";
			var json = JsonConvert.SerializeObject(schedule, _serializerSettings);

			try
			{
				var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint)
					.ConfigureAwait(_continueOnCapturedContext);

				Event(null,
					respContent.ResponseMessage.IsSuccessStatusCode
						? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "UpdateScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
						: CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "UpdateScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

				return respContent.ResponseMessage.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateScheduleAsync Failed - {e.Message}"));

				if (_throwApiErrors) 
					 throw;

				return false;
			}
		}

		/// <summary>
		/// Deletes a schedule.
		/// </summary>
		/// <param name="scheduleId">Id of schedule to delete.</param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> DeleteScheduleAsync(string scheduleId)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();
			var requestId = Guid.NewGuid();
			var endpoint = $"/common/schedule/v1/{scheduleId}";

			try
			{
				var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint)
					.ConfigureAwait(_continueOnCapturedContext);

				Event(null,
					respContent.ResponseMessage.IsSuccessStatusCode
						? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "DeleteScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
						: CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "DeleteScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
				
				return respContent.ResponseMessage.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"DeleteScheduleAsync Failed - {e.Message}"));

				if (_throwApiErrors) 
					 throw; 
				return false;
			}
		}

		/// <summary>
		/// Gets schedule occurrences.
		/// </summary>
		/// <param name="pattern">Schedule recurrence pattern. <see cref="ScheduleRecurrencePattern"/>.</param>
		/// <param name="afterDate">The time after which occurrences will start.</param>
		/// <param name="beforeDate">The time before which occurrences will end.</param>
		/// <returns>Task that returns a list of <see cref="ScheduleOccurrence"/>.</returns>
		public async Task<List<ScheduleOccurrence>> GetOccurrencesAsync(ScheduleRecurrencePattern pattern, 
			DateTime afterDate, DateTime beforeDate)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			var requestId = Guid.NewGuid();
			var endpoint = $"/common/schedule/v1/occurrences?afterDate={afterDate:O}&beforeDate={beforeDate:O}";
			
			try
			{
				var scheduleOccurrences = new List<ScheduleOccurrence>();
				var json = JsonConvert.SerializeObject(pattern, _serializerSettings);
				var respContent = await _restHelper.PostRestJSONAsync(requestId,json, endpoint)
					.ConfigureAwait(_continueOnCapturedContext);
				
				if (respContent.ResponseMessage.IsSuccessStatusCode)
				{
					var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, _serializerSettings);
					var results = apiResponse?.Content.ScheduleOccurrences.Items;
					if (results != null)
					{
						scheduleOccurrences.AddRange(results);
					}

					Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOccurrencesAsync Success",
						respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

					return scheduleOccurrences;
				}

				Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "GetOccurrencesAsync Failed",
					respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

				return null;
			}

			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetOccurrencesAsync Failed - {e.Message}"));
				
				if (_throwApiErrors) 
					 throw;

				return null;
			}
		}

		/// <summary>
		/// Updates the property bag of a schedule.
		/// </summary>
		/// <param name="scheduleId">The Id of the schedule.</param>
		/// <param name="propertyBagUpdates">Defines the updates to be performed. <see cref="OneJsonPatchItems"/></param>
		/// <returns>Task that returns a bool indicating success/failure.</returns>
		public async Task<bool> UpdateSchedulePropertyBagAsync(Guid scheduleId, OneJsonPatchItems propertyBagUpdates)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				var json = JsonConvert.SerializeObject(propertyBagUpdates, _serializerSettings);

				var respContent = await _restHelper.PatchRestJSONAsync(Guid.NewGuid(), json,
						$"/common/schedule/v1/UpdatePropertyBag/{scheduleId}")
					.ConfigureAwait(_continueOnCapturedContext);

				Event(null,
					respContent.ResponseMessage.IsSuccessStatusCode
						? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "UpdateSchedulePropertyBagAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
						: CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "UpdateSchedulePropertyBagAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
				
				return respContent.ResponseMessage.IsSuccessStatusCode;
			}
			catch (Exception e)
			{
				Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateSchedulePropertyBagAsync Failed - {e.Message}"));
				
				if (_throwApiErrors)
					throw;

				return false;
			}
		}

		private static ClientApiLoggerEventArgs CreateLoggerArgs(EnumOneLogLevel level, string message, HttpStatusCode statusCode = default, long duration = default) => new ClientApiLoggerEventArgs
			{ EventLevel = level, HttpStatusCode = statusCode, ElapsedMs = duration, Module = "ScheduleApi", Message = message };
	}
}
