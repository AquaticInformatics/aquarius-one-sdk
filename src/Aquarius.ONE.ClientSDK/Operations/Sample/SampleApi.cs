using ONE.Common.Activity;
using ONE.Models.CSharp;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ONE.Operations.Sample
{
    public class SampleApi
    {
        private readonly PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _throwAPIErrors;

        private readonly RestHelper _restHelper;
        private readonly ActivityApi _activityApi;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public SampleApi(PlatformEnvironment environment, bool continueOnCapturedContext,
            RestHelper restHelper, ActivityApi activityApi, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _activityApi = activityApi;
            _throwAPIErrors = throwAPIErrors;
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
        /// <returns>One or more Activities that meet the criteria.</returns>
        public async Task<List<Activity>> GetActivitiesAsync(string authTwinRefId,
            string activityTypeId = null, int? statusCode = null, int? priorityCode = null,
            DateTime? startDate = null, DateTime? endDate = null, string scheduleId = null)
        {
            try
            {
                return await _activityApi.GetActivitiesAsync(authTwinRefId,
                    includeActivityDescendants: null, includeAuthTwinDescendants: null,
                    activityTypeId, statusCode, priorityCode, startDate, endDate, scheduleId);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetActivitiesAsync Failed - {ex.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }

        /// <summary>
        /// Retrieves an activity and its children.
        /// </summary>
        /// <param name="activityId">
        /// Identifier of the Activity being returned.
        /// </param>
        /// <param name="includeDescendants">
        /// Optional: Defaults to false. Determines if child activities are returned.
        /// </param>
        /// <returns>The specified activity and optionally, its child activities.</returns>
        public async Task<List<Activity>> GetActivityAsync(string activityId, bool includeDescendants = false)
        {
            try
            {
                return await _activityApi.GetOneActivityAsync(activityId, includeDescendants);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetActivityAsync Failed - {ex.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }

        /// <summary>
        /// Updates one or more activities.
        /// </summary>
        /// <param name="activities"> List of activities to update. </param>
        /// <param name="updatePropertyBag">If true, the activity propertyBags will be replaced.</param>
        /// <returns>Boolean value indicating whether or not the activities were updated successfully.</returns>
        public async Task<bool> UpdateActivitiesAsync(List<Activity> activities, bool updatePropertyBag = false)
        {
            try
            {
                var proto = new Activities();
                proto.Items.AddRange(activities);
                return await _activityApi.UpdateActivitiesAsync(proto, updatePropertyBag);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateActivitiesAsync Failed - {ex.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Creates an analyte
        /// </summary>
        /// <param name="analyte">Analyte to be created</param>
        /// <returns>Boolean value indicating whether or not the analyte was successfully created</returns>
        public async Task<bool> CreateAnalyteAsync(Analyte analyte)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var endpoint = $"/operations/sample/v1/analyte";

            try
            {
                var respContent = await _restHelper.PostRestProtobufAsync(analyte, endpoint).ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "CreateAnalyteAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "CreateAnalyteAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));


                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"CreateAnalyteAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Creates a testGroup
        /// </summary>
        /// <param name="testGroup">TestGroup to be created</param>
        /// <returns>Boolean value indicating whether or not the testgroup was successfully created</returns>
        public async Task<bool> CreateTestGroupAsync(TestAnalyteGroup testGroup)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/operations/sample/v1/testgroup";

            try
            {
                var respContent = await _restHelper.PostRestProtobufAsync(testGroup, endpoint).ConfigureAwait(_continueOnCapturedContext);
                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "CreateTestGroupAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "CreateTestGroupAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return respContent.ResponseMessage.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"CreateTestGroupAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Updates an analyte
        /// </summary>
        /// <param name="analyte">Analyte to be updated</param>
        /// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are updated associated with the analyte</param>
        /// <returns>Boolean value indicating whether or not the analyte was successfully updated</returns>
        public async Task<bool> UpdateAnalyteAsync(Analyte analyte, DateTime? effectiveDate = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var endpoint = $"/operations/sample/v1/analyte/{analyte.Id}";

            if (effectiveDate.HasValue)
            {
                endpoint += $"?effectivedate={effectiveDate.Value.ToString("O")}";
            }

            try
            {
                var respContent = await _restHelper.PutRestProtobufAsync(analyte, endpoint).ConfigureAwait(_continueOnCapturedContext);

                var message = "UpdateAnalyteAsync Success";
                var eventLevel = EnumOneLogLevel.OneLogLevelTrace;
                var success = true;

                if (!respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    message = "UpdateAnalyteAsync Failed";
                    eventLevel = EnumOneLogLevel.OneLogLevelWarn;
                    success = false;
                }

                Event(null, CreateLoggerArgs(eventLevel, message, respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return success;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateAnalyteAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Deletes an existing analyte
        /// </summary>
        /// <param name="analyteId">Id of the analyte to be deleted</param>
        /// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are deleted associated with the analyte</param>
        /// <returns>Boolean value indicating whether or not the analyte was successfully deleted</returns>
        public async Task<bool> DeleteAnalyteAsync(string analyteId, DateTime? effectiveDate = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            var endpoint = $"/operations/sample/v1/analyte/{analyteId}";

            if (effectiveDate.HasValue)
            {
                endpoint += $"?effectivedate={effectiveDate.Value.ToString("O")}";
            }

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "DeleteAnalyteAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "DeleteAnalyteAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"DeleteAnalyteAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Retrieve all analytes associated to a specific authTwinRefId
        /// </summary>
        /// <param name="authTwinRefId">Reference id of the digital twin </param>
        /// <param name="includeInactive">Optional boolean value to include inactive analytes</param>
        /// <returns>List of analytes</returns>
        public async Task<List<Analyte>> GetAnalytesAsync(string authTwinRefId, bool? includeInactive = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string endPointUrl = $"/operations/sample/v1/analyte/analytes/{authTwinRefId}";
            if (includeInactive != null)
            {
                endPointUrl += $"?{includeInactive}";
            }

            List<Analyte> analytes = new List<Analyte>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    analytes.AddRange(respContent.ApiResponse.Content.Analytes.Items);

                    Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetAnalytesAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                    return analytes;
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetAnalytesAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetAnalytesAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }
        
        /// <summary>
        /// Retrieves an analyte based on the provided analyte id.
        /// </summary>
        /// <param name="analyteId">Id of the analyte to retrieve</param>
        /// <returns>Analyte object</returns>
        public async Task<Analyte> GetOneAnalyteAsync(string analyteId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/analyte/{analyteId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var analyte in respContent.ApiResponse.Content.Analytes.Items)
                    {
                        Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOneAnalyteAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                        return analyte;
                    }
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOneAnalyteAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetOneAnalyteAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }
        
        /// <summary>
        /// Updates an testGroup
        /// </summary>
        /// <param name="testGroup">TestGroup to be updated</param>
        /// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are updated associated with the testGroup</param>
        /// <returns>Boolean value indicating whether or not the testgroup was successfully updated</returns>
        public async Task<bool> UpdateTestGroupAsync(TestAnalyteGroup testGroup, DateTime? effectiveDate = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var endpoint = $"/operations/sample/v1/testgroup/{testGroup.Id}";

            if (effectiveDate.HasValue)
            {
                endpoint += $"?effectivedate={effectiveDate.Value.ToString("O")}";
            }

            try
            {
                var respContent = await _restHelper.PutRestProtobufAsync(testGroup, endpoint).ConfigureAwait(_continueOnCapturedContext);

                var message = "UpdateTestGroupAsync Success";
                var eventLevel = EnumOneLogLevel.OneLogLevelTrace;
                var success = true;

                if (!respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    message = "UpdateTestGroupAsync Failed";
                    eventLevel = EnumOneLogLevel.OneLogLevelWarn;
                    success = false;
                }

                Event(null, CreateLoggerArgs(eventLevel, message, respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return success;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateTestGroupAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return true;
            }
        }

        /// <summary>
        /// Deletes an existing testGroup
        /// </summary>
        /// <param name="testGroupId">Id of the testGroup to be deleted</param>
        /// <param name="effectiveDate">The date from after schedule definitions and existing sample activities are deleted associated with the testGroup</param>
        /// <returns>Boolean value indicating whether or not the testGroup was successfully deleted</returns>
        public async Task<bool> DeleteTestGroupAsync(string testGroupId, DateTime? effectiveDate = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            var endpoint = $"/operations/sample/v1/testgroup/{testGroupId}";

            if (effectiveDate.HasValue)
            {
                endpoint += $"?effectivedate={effectiveDate.Value.ToString("O")}";
            }

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "DeleteTestGroupAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "DeleteTestGroupAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"DeleteTestGroupAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Retrieve all testgroups associated to a specific authTwinRefId
        /// </summary>
        /// <param name="authTwinRefId">Reference id of the digital twin </param>
        /// <returns>List of testgroups</returns>
        public async Task<List<TestAnalyteGroup>> GetTestGroupsAsync(string authTwinRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            string endPointUrl = $"/operations/sample/v1/TestGroup/For/{authTwinRefId}";

            List<TestAnalyteGroup> TestGroups = new List<TestAnalyteGroup>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    TestGroups.AddRange(respContent.ApiResponse.Content.TestAnalyteGroups.Items);

                    Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetTestGroupsAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                    return TestGroups;
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetTestGroupsAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                return null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetTestGroupsAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }
        
        /// <summary>
        /// Retrieves a TestGroup based on the provided TestGroup id.
        /// </summary>
        /// <param name="testGroupId">Id of the TestGroup to retrieve</param>
        /// <returns>TestGroup object</returns>
        public async Task<TestAnalyteGroup> GetOneTestGroupAsync(string testGroupId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/TestGroup/{testGroupId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var testGroup in respContent.ApiResponse.Content.TestAnalyteGroups.Items)
                    {
                        Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOneTestGroupAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                        return testGroup;
                    }
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "GetOneTestGroupAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return null;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"GetOneTestGroupAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return null;
            }
        }

        /// <summary>
        /// Determine if analyte is scheduled for use
        /// </summary>
        /// <param name="analyteId">Id of the analyte to determine</param>
        public async Task<bool> IsAnalyteScheduledForUseAsync(string authTwinRefId, string analyteId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/{authTwinRefId}/IsScheduledForUse?EntityType=Analyte&EntityId={analyteId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var keyValue in respContent.ApiResponse.Content.KeyValues.Items)
                    {
                        Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "IsAnalyteScheduledForUseAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                        if (bool.TryParse(keyValue.Value, out var value))
                            return value;
                        return false;
                    }
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "IsAnalyteScheduledForUseAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return false;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"IsAnalyteScheduledForUseAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Determine if testgroup is scheduled for use
        /// </summary>
        /// <param name="testGroupId">Id of the testgroup to determine</param>
        public async Task<bool> IsTestGroupScheduledForUseAsync(string authTwinRefId, string testGroupId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/{authTwinRefId}/IsScheduledForUse?EntityType=TestGroup&EntityId={testGroupId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var keyValue in respContent.ApiResponse.Content.KeyValues.Items)
                    {
                        Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "IsTestGroupScheduledForUseAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                        if (bool.TryParse(keyValue.Value, out var value))
                            return value;
                        return false;
                    }
                }

                Event(null, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "IsTestGroupScheduledForUseAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                return false;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"IsTestGroupScheduledForUseAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;
                return false;
            }
        }

        /// <summary>
        /// Creates a new sample schedule. 
        /// This method will start a background process that will create new Activities associated
        /// with the schedule. The SampleCache will need to be refreshed to include the new Activities.
        /// </summary>
        /// <param name="schedule">The new schedule to be created.</param>
        /// <returns>The schedule that was created or null if the schedule could not be created.</returns>
        public async Task<Schedule> CreateSampleScheduleAsync(Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            const string endpoint = "/operations/sample/v1/schedule";

            try
            {
                var respContent = await _restHelper.PostRestProtobufAsync(schedule, endpoint)
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null, respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "CreateSampleScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "CreateSampleScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                var createdSchedule = respContent.ApiResponse.Content?.Schedules?.Items.FirstOrDefault();
                var success = respContent.ResponseMessage.IsSuccessStatusCode && createdSchedule != null;

                return success ? createdSchedule : ErrorResponse(respContent, (Schedule)null);
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"CreateSampleScheduleAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;

                return null;
            }
        }

        /// <summary>
        /// Updates a sample schedule. 
        /// This method will start a background process that will update Activities associated
        /// with the schedule. The SampleCache will need to be refreshed to include the updated Activities.
        /// </summary>
        /// <param name="sampleScheduleId">The Id of the schedule to update.</param>
        /// <param name="schedule">The schedule to update.</param>
        /// <returns>Boolean value indicating whether or not the schedule was successfully updated.</returns>
        public async Task<bool> UpdateSampleScheduleAsync(Guid sampleScheduleId, Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var endpoint = $"/operations/sample/v1/schedule/{sampleScheduleId}";

            try
            {
                var respContent = await _restHelper.PutRestProtobufAsync(schedule, endpoint)
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "UpdateSampleScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "UpdateSampleScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));
                
                if (!respContent.ResponseMessage.IsSuccessStatusCode)
                    return ErrorResponse(respContent, false);

                return true;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"UpdateSampleScheduleAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;

                return false;
            }
        }

        /// <summary>
        /// Deletes a sample schedule. 
        /// This method will start a background process that will delete Activities associated
        /// with the schedule. The SampleCache will need to be refreshed to remove the deleted Activities.
        /// </summary>
        /// <param name="sampleScheduleId">The Id of the schedule to delete.</param>
        /// <returns>Boolean value indicating whether or not the schedule was successfully deleted.</returns>
        public async Task<bool> DeleteSampleScheduleAsync(Guid sampleScheduleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var endpoint = $"/operations/sample/v1/schedule/{sampleScheduleId}";

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(Guid.NewGuid(), endpoint)
                    .ConfigureAwait(_continueOnCapturedContext);

                Event(null,
                    respContent.ResponseMessage.IsSuccessStatusCode
                        ? CreateLoggerArgs(EnumOneLogLevel.OneLogLevelTrace, "DeleteSampleScheduleAsync Success", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds)
                        : CreateLoggerArgs(EnumOneLogLevel.OneLogLevelWarn, "DeleteSampleScheduleAsync Failed", respContent.ResponseMessage.StatusCode, watch.ElapsedMilliseconds));

                if (!respContent.ResponseMessage.IsSuccessStatusCode)
                    return ErrorResponse(respContent, false);

                return true;
            }
            catch (Exception e)
            {
                Event(e, CreateLoggerArgs(EnumOneLogLevel.OneLogLevelError, $"DeleteSampleScheduleAsync Failed - {e.Message}"));
                if (_throwAPIErrors)
                    throw;

                return false;
            }
        }

        private T ErrorResponse<T>(ServiceResponse respContent, T result)
        {
            var exceptionMessage = respContent.ApiResponse?.Errors?.FirstOrDefault()?.Detail ?? $"Unknown Error with status code {respContent.ResponseMessage.StatusCode}";

            if (_throwAPIErrors)
                throw new Exception(exceptionMessage);

            return result;
        }

        private ClientApiLoggerEventArgs CreateLoggerArgs(EnumOneLogLevel level, string message, HttpStatusCode statusCode = default, long duration = default) => new ClientApiLoggerEventArgs
        { EventLevel = level, HttpStatusCode = statusCode, ElapsedMs = duration, Module = "SampleApi", Message = message };
    }
}
