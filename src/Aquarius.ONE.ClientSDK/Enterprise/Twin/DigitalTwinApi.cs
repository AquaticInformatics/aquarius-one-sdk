using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants.TwinCategory;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Enterprise.Twin
{
    public class DigitalTwinApi
    {
        public DigitalTwinApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _throwAPIErrors;
        private readonly RestHelper _restHelper;

        private const string EndpointRoot = "enterprise/twin/v1";
        private const string ModuleName = nameof(DigitalTwinApi);

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        /********************* DigitalTwinTypes *********************/
        public async Task<DigitalTwinType> CreateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwinType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "CreateDigitalTwinTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "CreateDigitalTwinTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"CreateDigitalTwinTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<DigitalTwinType> UpdateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwinType/{digitalTwinType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "UpdateDigitalTwinTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "UpdateDigitalTwinTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"UpdateDigitalTwinTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<bool> DeleteDigitalTwinTypeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"{EndpointRoot}/DigitalTwinType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "DeleteDigitalTwinTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "DeleteDigitalTwinTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"DeleteDigitalTwinTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }
        public async Task<List<DigitalTwinType>> GetDigitalTwinTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"{EndpointRoot}/DigitalTwinType/?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "GetDigitalTwinTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "GetDigitalTwinTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetDigitalTwinTypesAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        /********************* DigitalTwinSubTypes *********************/
        public async Task<DigitalTwinSubtype> CreateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinSubType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "CreateDigitalTwinSubTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "CreateDigitalTwinSubTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"CreateDigitalTwinSubTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<DigitalTwinSubtype> UpdateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwinSubType/{digitalTwinSubType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinSubType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "UpdateDigitalTwinSubTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "UpdateDigitalTwinSubTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"UpdateDigitalTwinSubTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<bool> DeleteDigitalTwinSubTypeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"{EndpointRoot}/DigitalTwinSubType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "DeleteDigitalTwinSubTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "DeleteDigitalTwinSubTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"DeleteDigitalTwinSubTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }
        public async Task<List<DigitalTwinSubtype>> GetDigitalTwinSubTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"{EndpointRoot}/DigitalTwinSubType/?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var result = respContent.ApiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "GetDigitalTwinSubTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = ModuleName, Message = "GetDigitalTwinSubTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetDigitalTwinSubTypesAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        /********************* DigitalTwins *********************/

        public async Task<DigitalTwin> CreateSpaceAsync(string parentId, string name, 
            string twinTypeId = SpaceConstants.LocationType.RefId, 
            string twinSubTypeId = SpaceConstants.LocationType.OtherSubType.RefId,
            int sortOrder = 0)
        {
            var digitalTwin = new DigitalTwin
            {
                ParentTwinReferenceId = parentId,
                Name = name,
                CategoryId = 2,
                TwinReferenceId = Guid.NewGuid().ToString(),
                TwinTypeId = twinTypeId,
                TwinSubTypeId = twinSubTypeId,
                SortOrder = sortOrder
            };

            return await CreateAsync(digitalTwin);
        }

        public async Task<DigitalTwin> CreateAsync(DigitalTwin digitalTwin)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwin, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "CreateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "CreateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"CreateAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        public async Task<List<DigitalTwin>> CreateManyAsync(List<DigitalTwin> digitalTwins)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin/many";

            var twins = new DigitalTwins();
            twins.Items.AddRange(digitalTwins);
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(twins, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, jsonSettings);
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "CreateAsync Success" });
                    return apiResponse.Content.DigitalTwins.Items.ToList();
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "CreateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"CreateAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        public async Task<DigitalTwin> UpdateAsync(DigitalTwin digitalTwin)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwin, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateAsync Success" });
                    return digitalTwin;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"UpdateAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        public async Task<bool> MoveAsync(string twinRefId, string parentRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin/twinRef/{twinRefId}/parentRef/{parentRefId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "MoveAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "MoveAsync Failed" });

                return respContent.ResponseMessage.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"MoveAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }
        public async Task<bool> MoveAsync(long id, long parentId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin/{id}/parent/{parentId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "MoveAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "MoveAsync Failed" });

                return respContent.ResponseMessage.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"MoveAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }
        public async Task<DigitalTwin> UpdateTwinDataAsync(string twinReferenceId, JsonPatchDocument twinData)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin/twinRefId/{twinReferenceId}/UpdateTwinData";

            try
            {
                string updatedTwinData = JsonConvert.SerializeObject(twinData);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedTwinData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateTwinDataAsync Success" });
                    return await GetAsync(twinReferenceId);
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateTwinDataAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"UpdateTwinDataAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<bool> UpdateTwinDataManyAsync(Dictionary<string, JsonPatchDocument> twinDataMany)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"{EndpointRoot}/DigitalTwin/UpdateTwinData/many";

            try
            {
                string updatedTwinData = JsonConvert.SerializeObject(twinDataMany);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedTwinData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateTwinDataManyAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "UpdateTwinDataManyAsync Failed" });
                return false;


            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"UpdateTwinDataManyAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }

        public async Task<bool> DeleteAsync(long twinId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"{EndpointRoot}/DigitalTwin/{twinId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "DeleteAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "DeleteAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"DeleteAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }
        public async Task<bool> DeleteTreeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"{EndpointRoot}/DigitalTwin/{id}/tree?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "DeleteTreeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "DeleteTreeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"DeleteTreeAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return false;
            }
        }

        public async Task<DigitalTwin> GetAsync(string twinRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                string url = $"{EndpointRoot}/DigitalTwin/Ref/{twinRefId}";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    if (results.Count > 0)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetAsync Success" });
                        return results[0];
                    }

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetAsync Returned No Values" });
                    return null;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetAsync Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

        public async Task<List<DigitalTwin>> GetDescendantsByTypeAsync(string twinRefId, string twinTypeId)
        {
            return await GetDescendantsAsync(twinRefId, twinTypeId);
        }
        public async Task<List<DigitalTwin>> GetDescendantsAsync(string twinRefId, string twinTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                string url = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Type/{twinTypeId}/Descendants";
                if (string.IsNullOrEmpty(twinTypeId))
                    url = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Descendants";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetDescendants Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<List<DigitalTwin>> GetDescendantsBySubTypeAsync(string twinRefId, string twinSubTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                string url = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/SubType/{twinSubTypeId}/Descendants";
                if (string.IsNullOrEmpty(twinSubTypeId))
                    url = $"{EndpointRoot}/DigitalTwin/ref/{twinRefId}/Descendants";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetDescendants Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }
        public async Task<List<DigitalTwin>> GetDescendantsByCategoryAsync(string twinRefId, uint categoryId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"{EndpointRoot}/DigitalTwin/Ref/{twinRefId}/Category/{categoryId}/Descendants").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsByCategoryAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = "GetDescendantsByCategoryAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"GetDescendantsByCategory Failed - {e.Message}" });
                if (_throwAPIErrors)
                     throw;
                return null;
            }
        }

    }
}
