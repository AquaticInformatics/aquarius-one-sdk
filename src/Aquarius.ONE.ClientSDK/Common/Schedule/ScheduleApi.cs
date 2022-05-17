using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using ONE.Models.CSharp;


namespace ONE.Common.Schedule

{
    public class ScheduleApi
    {
        public ScheduleApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        
        public async Task<List<ONE.Models.CSharp.Schedule>> GetSchedulesAsync(string twinRefId, string createdById, DateTime startDate, DateTime endDate, string scheduleTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
            string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
            List<ONE.Models.CSharp.Schedule> schedules = new List<ONE.Models.CSharp.Schedule>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/common/schedule/v1/schedule?twinRefId={twinRefId}&createdById={createdById}&startDate={sDate}&endDate={eDate}&scheduleTypeId={scheduleTypeId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var schedule in respContent.ApiResponse.Content.Schedules.Items)
                        schedules.Add(schedule);
                    
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetSchedulesAsync Success" });
                    return schedules;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetSchedulesAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetSchedulesAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ONE.Models.CSharp.Schedule> GetOneScheduleAsync(string scheduleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            List<HistorianData> historianData = new List<HistorianData>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/common/schedule/v1/schedule/{scheduleId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var schedule in respContent.ApiResponse.Content.Schedules.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetOneScheduleAsync Success" });
                        return schedule;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetOneScheduleAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetOneScheduleAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> SaveScheduleAsync(ONE.Models.CSharp.Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/schedule";
            if (schedule == null)
                return true;
            var json = JsonConvert.SerializeObject(schedule, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"SaveScheduleAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"SaveScheduleAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"SaveScheduleAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> UpdateScheduleAsync(ONE.Models.CSharp.Schedule schedule)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/schedule/v1/schedule";
            if (schedule == null)
                return true;
            var json = JsonConvert.SerializeObject(schedule, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"UpdateScheduleAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"UpdateScheduleAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"UpdateScheduleAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteScheduleAsync(string scheduleId)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"/common/schedule/v1/schedule/{scheduleId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"DeleteScheduleAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"DeleteScheduleAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"DeleteScheduleAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<object> GetOccurrancesAsync(string id, string twinRefId, string createdById, DateTime startDate, DateTime endDate, string scheduleTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
            string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
            var endpoint = $"/common/schedule/v1/schedule/{id}/occurrences?twinRefId={twinRefId}&createdById={createdById}&startDate={sDate}&endDate={eDate}&scheduleTypeId={scheduleTypeId}";

            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                /*
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var schedule in respContent.ApiResponse.Content.Schedules.Items)
                        schedules.Add(schedule);

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetSchedulesAsync Success" });
                    return schedules;
                }
                else
                */
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ScheduleApi", Message = $"GetOccurrancesAsync Failed" });
                    return null;
                }
                
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ScheduleApi", Message = $"GetOccurrancesAsync Failed - {e.Message}" });
                throw;
            }
        }
    }
}
