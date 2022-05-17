using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using ONE.Models.CSharp;


namespace ONE.Common.Activity

{
    public class ActivityApi
    {
        public ActivityApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        
        public async Task<List<ONE.Models.CSharp.Activity>> GetActivitiesAsync(string twinRefId, string createdById, DateTime startDate, DateTime endDate, string scheduleTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
            string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
            List<ONE.Models.CSharp.Activity> activities = new List<ONE.Models.CSharp.Activity>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/common/schedule/v1/schedule?twinRefId={twinRefId}&createdById={createdById}&startDate={sDate}&endDate={eDate}&scheduleTypeId={scheduleTypeId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var activity in respContent.ApiResponse.Content.Activities.Items)
                        activities.Add(activity);
                    
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"GetActivitiesAsync Success" });
                    return activities;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"GetActivitiesAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ActivityApi", Message = $"GetActivitiesAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ONE.Models.CSharp.Activity> GetOneActivityAsync(string activityId, bool includeDescendants = false)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            List<HistorianData> historianData = new List<HistorianData>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var activity in respContent.ApiResponse.Content.Activities.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"GetOneActivityAsync Success" });
                        return activity;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"GetOneActivityAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ActivityApi", Message = $"GetOneActivityAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> SaveActivityAsync(ONE.Models.CSharp.Activity activity)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/activity/v1/";
            if (activity == null)
                return true;
            var json = JsonConvert.SerializeObject(activity, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"SaveActivityAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"SaveActivityAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ActivityApi", Message = $"SaveActivityAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> UpdateActivityAsync(ONE.Models.CSharp.Activity activity)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/common/activity/v1/";
            if (activity == null)
                return true;
            var json = JsonConvert.SerializeObject(activity, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"UpdateActivityAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"UpdateActivityAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ActivityApi", Message = $"UpdateActivityAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteActivityAsync(string activityId, bool includeDescendants = false)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"/common/activity/v1/{activityId}?includeDescendants={includeDescendants}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"DeleteActivityAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ActivityApi", Message = $"DeleteActivityAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ActivityApi", Message = $"DeleteActivityAsync Failed - {e.Message}" });
                throw;
            }
        }
       
    }
}
