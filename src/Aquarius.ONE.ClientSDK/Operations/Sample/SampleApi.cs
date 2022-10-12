using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ONE.Models.CSharp;
using ONE.Utilities;
using Proto = ONE.Models.CSharp;

namespace ONE.Operations.Sample
{
    public class SampleApi
    {
        private readonly PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly RestHelper _restHelper;
        private readonly ClientSDK _clientSdk;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public SampleApi(PlatformEnvironment environment, bool continueOnCapturedContext, 
            RestHelper restHelper, ClientSDK clientSdk)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _clientSdk = clientSdk;
        }

        public async Task<List<Proto.Activity>> GetSamplesAsync(string authTwinRefId, 
            string activityTypeId = null, int? statusCode = null, int? priorityCode = null,
            DateTime? startDate = null, DateTime? endDate = null, string scheduleId = null)
        {
            try
            {
                return await _clientSdk.Activity.GetActivitiesAsync(authTwinRefId, 
                    includeActivityDescendants: null, includeAuthTwinDescendants: null, 
                    activityTypeId, statusCode, priorityCode, startDate, endDate, scheduleId);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"GetSamplesAsync Failed - {ex.Message}"));
                throw;
            }
        }

        public async Task<List<Proto.Activity>> GetSampleAsync(string activityId, bool includeDescendants = false)
        {
            try
            {
                return await _clientSdk.Activity.GetOneActivityAsync(activityId, includeDescendants);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"GetSampleAsync Failed - {ex.Message}"));
                throw;
            }
        }

        public async Task<bool> UpdateSamplesAsync(List<Proto.Activity> activities)
        {
            try
            {
                var proto = new Activities();
                proto.Items.AddRange(activities);
                return await _clientSdk.Activity.UpdateActivitiesAsync(proto);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"UpdateSamplesAsync Failed - {ex.Message}"));
                throw;
            }
        }

        private ClientApiLoggerEventArgs CreateLoggerArgs(EnumEventLevel level, string message, HttpStatusCode statusCode = default, long duration = default) => new ClientApiLoggerEventArgs
            { EventLevel = level, HttpStatusCode = statusCode, ElapsedMs = duration, Module = "SampleApi", Message = message };
    }
}
