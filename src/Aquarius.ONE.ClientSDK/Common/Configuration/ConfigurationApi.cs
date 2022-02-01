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
    }
}
