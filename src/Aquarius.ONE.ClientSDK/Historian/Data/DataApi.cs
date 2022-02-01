using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using ONE.Models.CSharp;

namespace ONE.Common.Historian

{
    public class DataApi
    {
        public DataApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<List<TimeSeriesData>> GetDataAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<TimeSeriesData> timeSeriesData = new List<TimeSeriesData>();
            try
            {
                string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
                string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"timeSeries/data/v1/{telemetryTwinRefId}/timeSeriesData?startDate={sDate}&endDate={eDate}&requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.TimeSeriesDatas.Items.ToList();
                    
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"GetDataAsync Success" });
                    return results;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"GetDataAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"GetDataAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<List<TimeSeriesData>> SaveDataAsync(string telemetryTwinRefId, TimeSeriesDatas timeSeriesDatas)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"timeseries/data/v1/{telemetryTwinRefId}/timeSeriesData";

            var json = JsonConvert.SerializeObject(timeSeriesDatas, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.TimeSeriesDatas.Items.ToList();

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"SaveDataAsync Success" });
                    return results;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"SaveDataAsync Failed" });
                    return null;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"SaveDataAsync Failed - {e.Message}" });
                throw;
            }
        }
    }
}
