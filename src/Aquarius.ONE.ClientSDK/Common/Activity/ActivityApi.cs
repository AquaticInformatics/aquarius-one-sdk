using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Serialization;
using Proto = ONE.Models.CSharp;
using ONE.Models.CSharp.Enums;
using ONE.Shared.Helpers.JsonPatch;

namespace ONE.Common.Activity
{
    public class ActivityApi
    {
        private readonly PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly RestHelper _restHelper;
        private readonly bool _throwAPIErrors;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public ActivityApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
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
        /// <returns>Task that returns a list of <see cref="Proto.Activity"/></returns>
        public async Task<List<Proto.Activity>> GetActivitiesAsync(string authTwinRefId = null, bool? includeActivityDescendants = null, bool? includeAuthTwinDescendants = null, string activityTypeId = null,
            int? statusCode = null, int? priorityCode = null, DateTime? startDate = null, DateTime? endDate = null, string scheduleId = null, string context = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
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

                var sb = new StringBuilder("/common/activity/v1");

                if (queryParameters.Any())
                    sb.Append("?");

                foreach (var queryParameter in queryParameters)
                    sb.Append($"{queryParameter.Key}={queryParameter.Value}&");

                var respContent = await _restHelper.GetRestProtocolBufferAsync(Guid.NewGuid(), sb.ToString().TrimEnd('&')).ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "GetActivitiesAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "GetActivitiesAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return respContent.ResponseMessage.IsSuccessStatusCode ? respContent.ApiResponse.Content.Activities.Items.ToList() : null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"GetActivitiesAsync Failed - {e.Message}"));
                if (_throwAPIErrors) 
					 throw; 
				return null;
            }
        }

        /// <summary>
        /// Gets a single activity and optionally, its decendants.
        /// </summary>
        /// <param name="activityId">The Id of the activity to retrieve.</param>
        /// <param name="includeDescendants">Optional: If true, the result will include all activity descendants.</param>
        /// <returns>Task that returns a list of <see cref="Proto.Activity"/></returns>
        public async Task<List<Proto.Activity>> GetOneActivityAsync(string activityId, bool includeDescendants = false)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(Guid.NewGuid(), $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}")
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "GetOneActivityAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "GetOneActivityAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return respContent.ResponseMessage.IsSuccessStatusCode ? respContent.ApiResponse.Content.Activities.Items.ToList() : null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"GetOneActivityAsync Failed - {e.Message}"));
                if (_throwAPIErrors) 
					 throw; 
				return null;
            }
        }

        /// <summary>
        /// Saves activities.
        /// </summary>
        /// <param name="activities">Activities to save.</param>
        /// <returns>Task that returns a bool indicating success/failure.</returns>
        public async Task<bool> SaveActivitiesAsync(Proto.Activities activities)
        {
            if (activities == null || !activities.Items.Any())
                return true;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var json = JsonConvert.SerializeObject(activities, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var respContent = await _restHelper.PostRestJSONAsync(Guid.NewGuid(), json, "/common/activity/v1/").ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "SaveActivitiesAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "SaveActivitiesAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"SaveActivitiesAsync Failed - {e.Message}"));
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        /// <summary>
        /// Updates activities.
        /// </summary>
        /// <param name="activities">Activities to update.</param>
        /// <returns>Task that returns a bool indicating success/failure.</returns>
        public async Task<bool> UpdateActivitiesAsync(Proto.Activities activities)
        {
            if (activities == null || !activities.Items.Any())
                return true;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var json = JsonConvert.SerializeObject(activities, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var respContent = await _restHelper.PutRestJSONAsync(Guid.NewGuid(), json, "/common/activity/v1/").ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "UpdateActivitiesAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "UpdateActivitiesAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"UpdateActivitiesAsync Failed - {e.Message}"));
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        /// <summary>
        /// Updates the property bag of an activity.
        /// </summary>
        /// <param name="activityId">The Id of the activity.</param>
        /// <param name="propertyBagUpdates">Defines the updates to be performed. <see cref="OneJsonPatchItems"/></param>
        /// <returns>Task that returns a bool indicating success/failure.</returns>
        public async Task<bool> UpdateActivityPropertyBagAsync(Guid activityId, OneJsonPatchItems propertyBagUpdates)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var json = JsonConvert.SerializeObject(propertyBagUpdates, 
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                var respContent = await _restHelper.PatchRestJSONAsync(Guid.NewGuid(), json,
                        $"/common/activity/v1/UpdatePropertyBag/{activityId}")
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "UpdateActivityPropertyBagAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "UpdateActivityPropertyBagAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"UpdateActivityPropertyBagAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Deletes an activity.
        /// </summary>
        /// <param name="activityId">Id of the activity.</param>
        /// <param name="includeDescendants">Optional: Activity descendants will be deleted unless this is false.</param>
        /// <returns>Task that returns a bool indicating success/failure.</returns>
        public async Task<bool> DeleteActivityAsync(string activityId, bool includeDescendants = true)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(Guid.NewGuid(), $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}")
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumLogLevel.Trace, "DeleteActivityAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumLogLevel.Warn, "DeleteActivityAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumLogLevel.Error, $"DeleteActivityAsync Failed - {e.Message}"));
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        private ClientApiLoggerEventArgs CreateLoggerArgs(EnumLogLevel level, string message, HttpStatusCode statusCode = default, long duration = default) => new ClientApiLoggerEventArgs
            { EventLevel = level, HttpStatusCode = statusCode, ElapsedMs = duration, Module = "ActivityApi", Message = message };
    }
}
