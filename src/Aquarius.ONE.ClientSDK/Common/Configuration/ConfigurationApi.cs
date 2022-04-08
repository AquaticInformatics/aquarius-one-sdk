using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using ONE.Models.CSharp;
using proto = ONE.Models.CSharp;
using Newtonsoft.Json.Linq;

namespace ONE.Common.Configuration
{
    public class ConfigurationApi
    {
        public ConfigurationApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<proto.Configuration> GetConfigurationAsync(string id, int version = 0)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string url = $"common/configuration/v2/{id}";
            if (version > 0)
                url = $"common/configuration/v2/{id}?version={version}";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        proto.Configuration configuration = new proto.Configuration(result);
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Success" });
                        return configuration;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<proto.Configuration>> GetConfigurationsAsync(string configurationTypeId, string authTwinRefId, string context = "")
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<proto.Configuration> configurations = new List<proto.Configuration>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/configuration/v2/?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}&context={context}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        proto.Configuration configuration = new proto.Configuration(result);
                        configurations.Add(configuration);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Success" });
                    return configurations;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<List<proto.Configuration>> GetConfigurationsAdminAsync(string configurationTypeId, string authTwinRefId, string context = "", string ownerId = "", bool? isPublic = null )
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<proto.Configuration> configurations = new List<proto.Configuration>();
            string url = $"common/configuration/v2/admin?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}&context={context}&ownerId={ownerId}";
            if (isPublic != null)
                url = $"common/configuration/v2/admin?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}&context={context}&ownerId={ownerId}?isPublic={isPublic}";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, url).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        proto.Configuration configuration = new proto.Configuration(result);
                        configurations.Add(configuration);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Success" });
                    return configurations;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed - {e.Message}" });
                throw;
            }
        }
        [ObsoleteAttribute("This is the V1 Method, Use alternative Method.", false)]
        public async Task<List<proto.Configuration>> GetConfigurationsAsync(int entityTypeId, string entityGuid)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<proto.Configuration> configurations = new List<proto.Configuration>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/configuration/v1/entityType/{entityTypeId}/{entityGuid}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        proto.Configuration configuration = new proto.Configuration(result);
                        configurations.Add(configuration);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Success" });
                    return configurations;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"GetConfigurationsAsync Failed - {e.Message}" });
                throw;
            }
        }
        [ObsoleteAttribute("This is the V1 Method, Use alternative Method.", false)]
        public async Task<bool> CreateConfigurationAsync(proto.Configuration configuration)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"common/configuration/v1/";
            
           
            dynamic jObject = new JObject();
            jObject["configurationData"] = configuration.ConfigurationData;
            jObject["enumEntity"] = (int)configuration.EnumEntity;
            jObject["filterById"] = configuration.FilterById;
            jObject["isPublic"] = configuration.IsPublic;
            if (!string.IsNullOrEmpty(configuration.Name))
                jObject["name"] = configuration.Name;

            var json = JsonConvert.SerializeObject(jObject);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> CreateConfigurationAsync(string authTwinRefId, string configurationTypeId, string configurationData, bool isPublic)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"common/configuration/v2/";

            dynamic jObject = new JObject();
            jObject["AuthTwinRefId"] = authTwinRefId;
            jObject["ConfigurationTypeId"] = configurationTypeId;
            jObject["ConfigurationData"] = configurationData;
            jObject["isPublic"] = isPublic;

            var json = JsonConvert.SerializeObject(jObject);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"CreateConfigurationAsync Failed - {e.Message}" });
                throw;
            }
        }
        [ObsoleteAttribute("This is the V1 Method, Use alternative Method.", false)]
        public async Task<bool> UpdateConfigurationAsync(proto.Configuration configuration)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"common/configuration/v1/{configuration.Id}";


            dynamic jObject = new JObject();
            jObject["configurationData"] = configuration.ConfigurationData;
            jObject["enumEntity"] = (int)configuration.EnumEntity;
            jObject["filterById"] = configuration.FilterById;
            jObject["isPublic"] = configuration.IsPublic;
            if (!string.IsNullOrEmpty(configuration.Name))
                jObject["name"] = configuration.Name;

            var json = JsonConvert.SerializeObject(jObject);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> UpdateConfigurationAsync(string id, string configurationTypeId, string configurationData, int version)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"common/configuration/v2/{id}";


            dynamic jObject = new JObject();
            jObject["ConfigurationTypeId"] = configurationTypeId;
            jObject["ConfigurationData"] = configurationData;
            jObject["Version"] = version;


            var json = JsonConvert.SerializeObject(jObject);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"UpdateConfigurationAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteConfigurationAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"common/configuration/v2/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"DeleteConfigurationAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"DeleteConfigurationAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"DeleteConfigurationAsync Failed - {e.Message}" });
                throw;
            }
        }
    }
}
