using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Imposed.Internationalization;

namespace ONE.ClientSDK.Common.Library
{
    public class LibraryApi
    {
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public LibraryApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
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

 
        /********************* Quantity Types *********************/
        public async Task<List<QuantityType>> GetQuantityTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/library/v1/quantitytype?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.QuantityTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }


        /********************* Units *********************/

        public async Task<Unit> GetUnitAsync(int unitId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/library/v1/unit/{unitId}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Units.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return null;
            }
        }
        public async Task<Unit> CreateUnitAsync(Unit unit)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unit";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unit, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Units.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateUnitAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Unit> UpdateUnitAsync(Unit unit)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unit/{unit.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unit, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Units.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateUnitAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteUnitAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/unit/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteUnitAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }
        public async Task<List<Unit>> GetUnitsAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();


            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/library/v1/unit?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Units.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* Parameters *********************/
        public async Task<Parameter> GetParameterAsync(int id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameter/{id}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Parameters.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"GetParameterAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParameterAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<Parameter>> GetParametersAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameter?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Parameters.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParametersAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParametersAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParametersAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Parameter> CreateParameterAsync(Parameter parameter)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameter";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameter, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Parameters.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateUnitAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Parameter> UpdateParameterAsync(Parameter parameter)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameter/{parameter.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameter, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Parameters.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateUnitAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteParameterAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/parameter/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteParameterAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }
        /********************* i18n *********************/


        private dynamic GetChild(dynamic parentJsonNode)
        {
            foreach (var child in parentJsonNode.Children())
            {
                return child;
            }
            return null;
        }
        private bool IsArray(dynamic parentJsonNode)
        {
            var child = GetChild(parentJsonNode);
            if (child == null)
                return false;
            if (GetChild(child) == null)
                return false;
            return true;
        }
        private List<I18NKey> Converti18nJSontoExtendedTranslations(string json)
        {
            dynamic jsonResponse = JsonConvert.DeserializeObject(json);
            List<I18NKey> translations = new List<I18NKey>();

            var jsonNode = jsonResponse.FM;
            Dictionary<string, string> items;
            if (jsonNode != null)
            {

                foreach (var translationRootNode in jsonNode.Children())
                {
                    var moduleChild = GetChild(translationRootNode);

                    switch (moduleChild.Name)
                    {
                        case "AQI_FOUNDATION_LIBRARY":
                        case "FOUNDATION_LIBRARY":
                            dynamic foundationLibaryNode = JsonConvert.DeserializeObject(GetChild(moduleChild).ToString());
                            //Parameter - Long
                            if (foundationLibaryNode.Parameter != null && foundationLibaryNode.Parameter.LONG != null)
                            {
                                jsonNode = foundationLibaryNode.Parameter.LONG;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "Parameter", "LONG");
                            }
                            //Parameter - Short
                            if (foundationLibaryNode.Parameter != null && foundationLibaryNode.Parameter.SHORT != null)
                            {
                                jsonNode = foundationLibaryNode.Parameter.SHORT;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "Parameter", "SHORT");
                            }
                            //Unit Type - Long
                            if (foundationLibaryNode.UnitType != null && foundationLibaryNode.UnitType.LONG != null)
                            {
                                jsonNode = foundationLibaryNode.UnitType.LONG;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "UnitType", "LONG");
                            }
                            //Unit Type - Short
                            if (foundationLibaryNode.UnitType != null && foundationLibaryNode.UnitType.SHORT != null)
                            {
                                jsonNode = foundationLibaryNode.UnitType.SHORT;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "UnitType", "SHORT");
                            }
                            //Digital Twin Types
                            if (foundationLibaryNode.DigitalTwinType != null)
                            {
                                jsonNode = foundationLibaryNode.DigitalTwinType;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "DigitalTwinType", "");
                            }
                            //Digital Twin Types
                            if (foundationLibaryNode.DigitalTwinSubType != null)
                            {
                                jsonNode = foundationLibaryNode.DigitalTwinSubType;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "DigitalTwinSubType", "");
                            }
                            //Feature
                            if (foundationLibaryNode.Feature != null)
                            {
                                jsonNode = foundationLibaryNode.Feature;
                                items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
                                translations = I18NKeyHelper.Load(translations, items, "Feature", "");
                            }
                            break;
                        case "AQI_MOBILE_RIO":
                            var typeChild = GetChild(moduleChild);
                            //var jObject = JObject.Parse(typeChild.ToString());
                            foreach (var child in typeChild.Children())
                            {
                                if (IsArray(child))
                                {
                                    var keysChild = GetChild(child);
                                    items = JsonConvert.DeserializeObject<Dictionary<string, string>>(keysChild.ToString());
                                    translations = I18NKeyHelper.Load(translations, items, "AQI_MOBILE_RIO", child.Name);
                                }
                            }
                            break;
                        case "CLAROS_LOGIN":
                            translations = LoadChildJsonTanslations(translations, "CLAROS-LOGIN", moduleChild, "");
                            break;
                    }

                }
            }
            return translations;
        }
        private List<I18NKey> LoadChildJsonTanslations(List<I18NKey> translations, string module, dynamic json, string parentName)
        {
            foreach (var child in json.Children())
            {
                if (IsArray(child))
                {
                    var typeName = parentName;
                    if (string.IsNullOrEmpty(typeName))
                        typeName = child.Name;
                    else if (!string.IsNullOrEmpty(child.Name))
                        typeName = typeName + "." + child.Name;
                    translations = LoadChildJsonTanslations(translations, module, child, typeName);
                }
                else
                {
                    I18NKey translation = new I18NKey(module, parentName, child.Name, child.Value.Value);
                    translations.Add(translation);
                }
            }
            return translations;
        }
        public string FoundationLibraryName
        {
            get
            {
                switch (_environment.PlatformEnvironmentEnum)
                {
                    case EnumPlatformEnvironment.AqiFeature:
                    case EnumPlatformEnvironment.AqiIntegration:
                    case EnumPlatformEnvironment.AqiStage:
                    case EnumPlatformEnvironment.AqiUSProduction:
                        return "AQI_FOUNDATION_LIBRARY";
                    default:
                        return "FOUNDATION_LIBRARY";
                }
            }
        }
        public async Task<List<I18NKey>> GetMobilei18nKeysAsync(string language)
        {
            return await Geti18nKeysAsync(language, $"AQI_MOBILE_RIO,{FoundationLibraryName},claros_login");
        }
        public async Task<List<I18NKey>> Geti18nKeysAsync(string language, string modules)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (!modules.Contains("AQI_FOUNDATION_LIBRARY"))
                modules = modules.ToUpper().Replace("FOUNDATION_LIBRARY", "AQI_FOUNDATION_LIBRARY");
            List<I18NKey> translations = new List<I18NKey>();
            var requestId = Guid.NewGuid();
            //_authentication.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"common/library/v1/i18n?modulecsv={modules}&lang={language}&requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
               
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = Converti18nJSontoExtendedTranslations(respContent.Result);
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"Geti18nKeysAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"Geti18nKeysAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"Geti18nKeysAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* Agencies *********************/
        public async Task<Agency> CreateAgencyAsync(Agency agency)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/Agency";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(agency, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Agencies.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateAgencyAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateAgencyAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateAgencyAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }

        }
        public async Task<List<Agency>> GetAgenciesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/Agency";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Agencies.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetAgenciesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetAgenciesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetAgenciesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Agency> GetAgencyAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/Agency/{id}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.Agencies.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetAgencyAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetAgencyAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetAgencyAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Agency> UpdateAgencyAsync(Agency agency)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/Agency/{agency.Id}";
            agency.UpdateMask = new FieldMask { Paths = { "I18NKeyName" } };
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(agency, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json.ToString(), endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"UpdateAgencyAsync Success" });
                    return agency;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"UpdateAgencyAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"UpdateAgencyAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* Parameter Agency Code Types *********************/
        public async Task<ParameterAgencyCodeType> CreateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/ParameterAgencyCodeType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameterAgencyCodeType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ParameterAgencyCodeTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<ParameterAgencyCodeType>> GetParameterAgencyCodeTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/ParameterAgencyCodeType";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.ParameterAgencyCodeTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ParameterAgencyCodeType> GetParameterAgencyCodeTypeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCodeType/{id}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.ParameterAgencyCodeTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypeAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParameterAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ParameterAgencyCodeType> UpdateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCodeType/{parameterAgencyCodeType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameterAgencyCodeType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ParameterAgencyCodeTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteParameterAgencyCodeTypeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/parameterAgencyCodeType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        /********************* Parameter Agency Codes *********************/
        public async Task<ParameterAgencyCode> CreateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCode";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameterAgencyCode, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ParameterAgencyCodes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateParameterAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<ParameterAgencyCode>> GetParameterAgencyCodesAsync(string parameterAgencyCodeTypeId = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCode?ParameterAgencyCodeTypeId={parameterAgencyCodeTypeId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.ParameterAgencyCodes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParameterAgencyCodesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ParameterAgencyCode> GetParameterAgencyCodeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCode/{id}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.ParameterAgencyCodes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetParameterAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetParameterAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ParameterAgencyCode> UpdateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/parameterAgencyCode/{parameterAgencyCode.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(parameterAgencyCode, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ParameterAgencyCodes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateParameterAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteParameterAgencyCodeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/parameterAgencyCode/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteParameterAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        /********************* Unit Agency Code Types *********************/
        public async Task<UnitAgencyCodeType> CreateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/UnitAgencyCodeType";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unitAgencyCodeType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.UnitAgencyCodeTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<UnitAgencyCodeType>> GetUnitAgencyCodeTypesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/UnitAgencyCodeType";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.UnitAgencyCodeTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<UnitAgencyCodeType> GetUnitAgencyCodeTypeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCodeType/{id}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.UnitAgencyCodeTypes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypeAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<UnitAgencyCodeType> UpdateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCodeType/{unitAgencyCodeType.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unitAgencyCodeType, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.UnitAgencyCodeTypes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeTypeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeTypeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteUnitAgencyCodeTypeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/unitAgencyCodeType/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeTypeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeTypeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeTypeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }

        /********************* Unit Agency Codes *********************/
        public async Task<UnitAgencyCode> CreateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCode";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unitAgencyCode, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.UnitAgencyCodes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"CreateUnitAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<UnitAgencyCode>> GetUnitAgencyCodesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCode";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.UnitAgencyCodes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodesAsync Success" });
                    return result;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitAgencyCodesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<UnitAgencyCode> GetUnitAgencyCodeAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCode/{id}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var result = respContent.ApiResponse.Content.UnitAgencyCodes.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeAsync Success" });
                    if (result.Count == 1)
                        return result[0];
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = $"GetUnitAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"GetUnitAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<UnitAgencyCode> UpdateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/library/v1/unitAgencyCode/{unitAgencyCode.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(unitAgencyCode, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.UnitAgencyCodes.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"UpdateUnitAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteUnitAgencyCodeAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/library/v1/unitAgencyCode/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryAPI", Message = $"DeleteUnitAgencyCodeAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }
    }
}
