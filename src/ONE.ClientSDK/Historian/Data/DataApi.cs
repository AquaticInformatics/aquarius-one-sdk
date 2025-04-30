using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Historian.Data
{
    public class DataApi
    {
        private PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _throwApiErrors;
        private readonly RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        private readonly JsonSerializerSettings _serializerSettings;

        public DataApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwApiErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwApiErrors = throwApiErrors;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task<List<HistorianData>> GetDataAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var historianData = new List<HistorianData>();

            try
            {
                var sDate = startDate.ToString("O");
                var eDate = endDate.ToString("O");
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, 
                    $"/historian/data/v1/{telemetryTwinRefId}?startTime={sDate}&endTime={eDate}")
                    .ConfigureAwait(_continueOnCapturedContext);

                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var historianDataItem in respContent.ApiResponse.Content.HistorianDatas.Items)
                        historianData.Add(historianDataItem);
                    
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "GetDataAsync Success"
                    });
                    return historianData;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "GetDataAsync Failed"
                });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"GetDataAsync Failed - {e.Message}"
                });
                if (_throwApiErrors) 
					 throw; 
                return null;
            }
        }

        public async Task<HistorianData> GetOneDataAsync(string telemetryTwinRefId, DateTime dateTime)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                var eDateTime = dateTime.ToString("O");
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId,
                    $"/historian/data/v1/{telemetryTwinRefId}/{eDateTime}").ConfigureAwait(_continueOnCapturedContext);

                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var historianDataItem in respContent.ApiResponse.Content.HistorianDatas.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs
                        {
                            EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode,
                            ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "GetOneDataAsync Success"
                        });
                        return historianDataItem;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "GetOneDataAsync Failed"
                });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"GetOneDataAsync Failed - {e.Message}"
                });
                if (_throwApiErrors) 
					 throw; 
                return null;
            }
        }

        public async Task<bool> SaveDataAsync(string telemetryTwinRefId, HistorianDatas historianDatas)
        {
            if (historianDatas?.Items == null || historianDatas.Items.Count == 0)
                return true;
                                                      
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();          
            var endpoint = $"/historian/data/v1/{telemetryTwinRefId}";
            var json = JsonConvert.SerializeObject(historianDatas, _serializerSettings);
            
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "SaveDataAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "SaveDataAsync Failed"
                });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"SaveDataAsync Failed - {e.Message}"
                });
                if (_throwApiErrors) 
					 throw; 
                return false;
            }
        }
        public async Task<bool> SaveBulkDataAsync(string telemetryTwinRefId, HistorianDatas historianDatas)
        {
            if (historianDatas?.Items == null || historianDatas.Items.Count == 0)
                return true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint =$"/historian/data/v1/{telemetryTwinRefId}/bulkingest";

            var json = JsonConvert.SerializeObject(historianDatas, _serializerSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace,
                        HttpStatusCode = respContent.ResponseMessage.StatusCode,
                        ElapsedMs = watch.ElapsedMilliseconds,
                        Module = "DataApi",
                        Message = "SaveBulkDataAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "DataApi",
                    Message = "SaveBulkDataAsync Failed"
                });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError,
                    Module = "DataApi",
                    Message = $"SaveBulkDataAsync Failed - {e.Message}"
                });
                if (_throwApiErrors)
                    throw;
                return false;
            }
        }

        public async Task<bool> UpdateDataAsync(string telemetryTwinRefId, HistorianData historianData)
        {
            if (historianData == null)
                return true;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"/historian/data/v1/{telemetryTwinRefId}";
            var json = JsonConvert.SerializeObject(historianData, _serializerSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "UpdateDataAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "UpdateDataAsync Failed"
                });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"UpdateDataAsync Failed - {e.Message}"
                });
                if (_throwApiErrors) 
					 throw; 
                return false;
            }
        }

        public async Task<bool> DeleteManyAsync(string telemetryTwinRefId, DateTime startDate, DateTime endDate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var sDate = startDate.ToString("O");
            var eDate = endDate.ToString("O");

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, 
                    $"/historian/data/v1/{telemetryTwinRefId}?startTime={sDate}&endTime={eDate}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode,
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "DeleteManyAsync Success"
                    });
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "DeleteManyAsync Failed"
                    });
                }
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"DeleteManyAsync Failed - {e.Message}" });
                if (_throwApiErrors) 
					 throw; 
                return false;
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
                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                        ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "FlushAsync Success"
                    });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, 
                    ElapsedMs = watch.ElapsedMilliseconds, Module = "DataApi", Message = "FlushAsync Failed"
                });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "DataApi", Message = $"FlushAsync Failed - {e.Message}"
                });
                if (_throwApiErrors) 
					 throw; 
                return false;
            }
        }
    }
}
