using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ONE.Models.CSharp;


namespace ONE.Common.Schedule

{
    public class ScheduleApi
    {
        public ScheduleApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        private readonly bool _throwAPIErrors;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<List<ONE.Models.CSharp.Schedule>> GetSchedulesAsync(string authTwinRefId, bool? includeAuthTwinChildren, string scheduleTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string endPointUrl = $"/common/schedule/v1/schedules/{authTwinRefId}";
            if (includeAuthTwinChildren != null)
            {
                endPointUrl += $"?{includeAuthTwinChildren}";
            }

            if (!string.IsNullOrWhiteSpace(scheduleTypeId))
            {
                endPointUrl += includeAuthTwinChildren == null ? $"?{scheduleTypeId}" : $"&{scheduleTypeId}";
            }

            List<ONE.Models.CSharp.Schedule> schedules = new List<ONE.Models.CSharp.Schedule>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId,endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    schedules.AddRange(respContent.ApiResponse.Content.Schedules.Items);

                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", Message = $"GetSchedulesAsync Success"
                    });

                    return schedules;
                }

                Event(null, new ClientApiLoggerEventArgs
                { 
                    EventLevel = EnumEventLevel.Warn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "ScheduleApi",
                    Message = $"GetSchedulesAsync Failed"
                });

                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetSchedulesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ONE.Models.CSharp.Schedule> GetOneScheduleAsync(string scheduleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endPointUrl = $"common/schedule/v1/{scheduleId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var schedule in respContent.ApiResponse.Content.Schedules.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs
                        {
                            EventLevel = EnumEventLevel.Trace, 
                            HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                            ElapsedMs = watch.ElapsedMilliseconds, 
                            Module = "ScheduleApi", 
                            Message = $"GetOneScheduleAsync Success"
                        });
                        return schedule;
                    }
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn, 
                    HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, 
                    Module = "ScheduleApi",
                    Message = $"GetOneScheduleAsync Failed"
                });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetOneScheduleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> SaveScheduleAsync(ONE.Models.CSharp.Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (schedule == null)
                return true;

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/";
            var json = JsonConvert.SerializeObject(schedule, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", 
                        Message = $"SaveScheduleAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs 
                    { 
                        EventLevel = EnumEventLevel.Warn, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", 
                        Message = $"SaveScheduleAsync Failed"
                    });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"SaveScheduleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        public async Task<bool> UpdateScheduleAsync(ONE.Models.CSharp.Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (schedule == null)
                return true;

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/{schedule.Id}";
            var json = JsonConvert.SerializeObject(schedule, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", 
                        Message = $"UpdateScheduleAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn, 
                    HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, 
                    Module = "ScheduleApi", 
                    Message = $"UpdateScheduleAsync Failed"
                });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"UpdateScheduleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(string scheduleId)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/{scheduleId}";

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", 
                        Message = $"DeleteScheduleAsync Success"
                    });

                    return true;
                }
                
                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn, 
                    HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, 
                    Module = "ScheduleApi", 
                    Message = $"DeleteScheduleAsync Failed"
                });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"DeleteScheduleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }
        public async Task<List<ScheduleOccurrence>> GetOccurrencesAsync(ScheduleRecurrencePattern pattern, DateTime afterDate, DateTime beforeDate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/occurrences?afterDate={afterDate:O}&beforeDate={beforeDate:O}";


            try
            {
                List<Models.CSharp.ScheduleOccurrence> scheduleOccurrences = new List<ONE.Models.CSharp.ScheduleOccurrence>();
                var json = JsonConvert.SerializeObject(pattern, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var respContent = await _restHelper.PostRestJSONAsync(requestId,json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse?.Content.ScheduleOccurrences.Items;
                    if (results != null)
                    {
                        scheduleOccurrences.AddRange(results);
                    }

                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace, 
                        HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, 
                        Module = "ScheduleApi", Message = $"GetOccurrencesAsync Success"
                    });
                    return scheduleOccurrences;
                }
                
                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn, 
                    HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, 
                    Module = "ScheduleApi", 
                    Message = $"GetOccurrencesAsync Failed"
                });
                    return null;
            }

            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetOccurrancesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return null;
            }
        }
    }
}
