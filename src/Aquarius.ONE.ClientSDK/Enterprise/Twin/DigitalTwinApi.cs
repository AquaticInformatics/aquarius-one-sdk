
using ONE.Utilities;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ONE.Models.CSharp;

namespace ONE.Enterprise.Twin
{
    public class DigitalTwinApi
    {
        public DigitalTwinApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        /********************* DigitalTwinTypes *********************/
        public async Task<DigitalTwinType> CreateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwinType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<DigitalTwinType> UpdateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwinType/{digitalTwinType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteDigitalTwinTypeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/twin/v1/DigitalTwinType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<DigitalTwinType>> GetDigitalTwinTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<DigitalTwinType> digitalTwinTypes = new List<DigitalTwinType>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/twin/v1/DigitalTwinType/?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.DigitalTwinTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinTypesAsync Failed - {e.Message}" });
                throw;
            }
        }

        /********************* DigitalTwinSubTypes *********************/
        public async Task<DigitalTwinSubtype> CreateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwinSubType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinSubType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinSubTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinSubTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"CreateDigitalTwinSubTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<DigitalTwinSubtype> UpdateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwinSubType/{digitalTwinSubType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwinSubType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinSubTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinSubTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"UpdateDigitalTwinSubTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteDigitalTwinSubTypeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/twin/v1/DigitalTwinSubType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinSubTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinSubTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"DeleteDigitalTwinSubTypeAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<DigitalTwinSubtype>> GetDigitalTwinSubTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<DigitalTwinSubtype> digitalTwinSubTypes = new List<DigitalTwinSubtype>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/twin/v1/DigitalTwinSubType/?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var result = respContent.ApiResponse.Content.DigitalTwinSubtypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinSubTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinSubTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetDigitalTwinSubTypesAsync Failed - {e.Message}" });
                throw;
            }
        }


        /********************* DigitalTwins *********************/

        public async Task<DigitalTwin> CreateSpaceAsync(string parentId, string name, string twinTypeId = Constants.SpaceCategory.WastewaterLocationType.RefId, string twinSubTypeId = Constants.SpaceCategory.WastewaterLocationType.OtherSubType.RefId)
        {
            DigitalTwin digitalTwin = new DigitalTwin();
            digitalTwin.ParentTwinReferenceId = parentId;
            digitalTwin.Name = name;
            digitalTwin.CategoryId = 2;
            digitalTwin.TwinReferenceId = Guid.NewGuid().ToString();
            digitalTwin.TwinTypeId = twinTypeId;
            digitalTwin.TwinSubTypeId = twinSubTypeId;
            return await CreateAsync(digitalTwin);
        }

        public async Task<DigitalTwin> CreateAsync(DigitalTwin digitalTwin)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwin, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"CreateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"CreateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"CreateAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<DigitalTwin> UpdateAsync(DigitalTwin digitalTwin)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(digitalTwin, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json.ToString(), endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateAsync Success" });
                    return digitalTwin;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"UpdateAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<bool> MoveAsync(string twinRefId, string parentRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin/twinRef/{twinRefId}/parentRef/{parentRefId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"MoveAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"MoveAsync Failed" });

                return respContent.ResponseMessage.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"MoveAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> MoveAsync(long id, long parentId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin/{id}/parent/{parentId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"MoveAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"MoveAsync Failed" });

                return respContent.ResponseMessage.IsSuccessStatusCode;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"MoveAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<DigitalTwin> UpdateTwinDataAsync(string twinReferenceId, JsonPatchDocument twinData)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin/twinRefId/{twinReferenceId}/UpdateTwinData";

            try
            {
                string updatedTwinData = JsonConvert.SerializeObject(twinData);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedTwinData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateTwinDataAsync Success" });
                    return await GetAsync(twinReferenceId);
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateTwinDataAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"UpdateTwinDataAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> UpdateTwinDataManyAsync(Dictionary<string, JsonPatchDocument> twinDataMany)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/twin/v1/DigitalTwin/UpdateTwinData/many";
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                string updatedTwinData = JsonConvert.SerializeObject(twinDataMany);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedTwinData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateTwinDataManyAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"UpdateTwinDataManyAsync Failed" });
                return false;


            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"UpdateTwinDataManyAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long twinId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/twin/v1/DigitalTwin/{twinId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"DeleteAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"DeleteAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"DeleteAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteTreeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/twin/v1/DigitalTwin/{id}/tree?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"DeleteTreeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"DeleteTreeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"DeleteTreeAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<DigitalTwin> GetAsync(string twinRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                string url = $"enterprise/twin/v1/DigitalTwin/Ref/{twinRefId}";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    if (results.Count > 0)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetAsync Success" });
                        return results[0];
                    }
                    else
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetAsync Returned No Values" });
                        return null;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetAsync Failed - {e.Message}" });
                throw;
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
                string url = $"enterprise/twin/v1/DigitalTwin/ref/{twinRefId}/Type/{twinTypeId}/Descendants";
                if (string.IsNullOrEmpty(twinTypeId))
                    url = $"enterprise/twin/v1/DigitalTwin/ref/{twinRefId}/Descendants";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetDecendants Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<DigitalTwin>> GetDescendantsBySubTypeAsync(string twinRefId, string twinSubTypeId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                string url = $"enterprise/twin/v1/DigitalTwin/ref/{twinRefId}/SubType/{twinSubTypeId}/Descendants";
                if (string.IsNullOrEmpty(twinSubTypeId))
                    url = $"enterprise/twin/v1/DigitalTwin/ref/{twinRefId}/Descendants";
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetDecendants Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<DigitalTwin>> GetDescendantsByCategoryAsync(string twinRefId, uint categoryId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<DigitalTwin> digitalTwins = new List<DigitalTwin>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/twin/v1/DigitalTwin/Ref/{twinRefId}/Category/{categoryId}/Descendants").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var results = respContent.ApiResponse.Content.DigitalTwins.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        digitalTwins.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsByCategoryAsync Success" });
                    return digitalTwins;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "DigitalTwinApi", Message = $"GetDecendantsByCategoryAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "DigitalTwinAPI", Message = $"GetDecendantsByCategory Failed - {e.Message}" });
                throw;
            }
        }

    }
}
