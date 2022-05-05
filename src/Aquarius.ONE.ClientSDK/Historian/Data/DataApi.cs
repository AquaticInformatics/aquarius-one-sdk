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

        
        public async Task<List<HistorianData>> GetDataAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            List<HistorianData> historianData = new List<HistorianData>();
            try
            {
                string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
                string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/historian/data/v1/{telemetryTwinRefId}?startTime={startDate}&endTime={endDate}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var historianDataItem in respContent.ApiResponse.Content.HistorianDatas.Items)
                        historianData.Add(historianDataItem);
                    
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"GetDataAsync Success" });
                    return historianData;
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
        public async Task<HistorianData> GetOneDataAsync(string telemetryTwinRefId, DateTime dateTime)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            List<HistorianData> historianData = new List<HistorianData>();
            try
            {
                string eDateTime = dateTime.ToString("MM/dd/yyyy HH:mm:ss");
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"/historian/data/v1/{telemetryTwinRefId}/{eDateTime}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var historianDataItem in respContent.ApiResponse.Content.HistorianDatas.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"GetOneDataAsync Success" });
                        return historianDataItem;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"GetOneDataAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"GetOneDataAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> SaveDataAsync(string telemetryTwinRefId, HistorianDatas historianDatas)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/historian/data/v1/{telemetryTwinRefId}";
            if (historianDatas == null || historianDatas.Items == null || historianDatas.Items.Count == 0)
                return true;
            var json = JsonConvert.SerializeObject(historianDatas, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"SaveDataAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"SaveDataAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"SaveDataAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> UpdateDataAsync(string telemetryTwinRefId, HistorianData historianData)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string eDateTime = historianData.DateTimeUTC.ToDateTime().ToString("MM/dd/yyyy HH:mm:ss");
            var endpoint = $"/historian/data/v1/{telemetryTwinRefId}";
            if (historianData == null)
                return true;
            var json = JsonConvert.SerializeObject(historianData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"UpdateDataAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"UpdateDataAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"UpdateDataAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteManyAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            string sDate = startDate.ToString("MM/dd/yyyy HH:mm:ss");
            string eDate = endDate.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"/historian/data/v1/{telemetryTwinRefId}?startTime={startDate}&endTime={endDate}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteManyAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteManyAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "LibraryAPI", Message = $"DeleteManyAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> FlushAsync(string telemetryTwinRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/historian/data/v1/{telemetryTwinRefId}/Flush";
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"FlushAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = $"FlushAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DataApi", Message = $"FlushAsync Failed - {e.Message}" });
                throw;
            }
        }
    }
}
