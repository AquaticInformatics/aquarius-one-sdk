using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Common.Core.Protobuf.Models;
using configProtobuf = Common.Configuration.Protobuf.Models;

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

        public async Task<List<configProtobuf.Configuration>> GetConfigurationsAsync(int entityTypeId, string entityGuid)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<configProtobuf.Configuration> configurations = new List<configProtobuf.Configuration>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"common/configuration/v1/entityType/{entityTypeId}/{entityGuid}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        configProtobuf.Configuration configuration = new configProtobuf.Configuration(result);
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

        public async Task<configProtobuf.Configuration> SaveConfiguration(configProtobuf.Configuration configuration)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"common/configuration/v1/";

            var json = JsonConvert.SerializeObject(configuration);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"SaveConfiguration Success" });
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Configurations.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        return new configProtobuf.Configuration(result);
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ConfigurationApi", Message = $"SaveConfiguration Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ConfigurationApi", Message = $"SaveConfigurationn Failed - {e.Message}" });
                throw;
            }
        }
    }
}
