using Enterprise.Twin.Protobuf.Models;
using Newtonsoft.Json;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentService : IngestAgent
    {
        public AgentService(ClientSDK clientSDK)
            : base(clientSDK.Authentication, clientSDK.Core, clientSDK.DigitalTwin, clientSDK.Configuration, clientSDK.Data, new DigitalTwin())
        {
            TwinSubTypeId = ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentTest.RefId;
        }
        public override Task<bool> InitializeAsync(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            // Initialize Telemetry
            Telemetry.Add(Guid.NewGuid().ToString());
            Telemetry.Add(Guid.NewGuid().ToString());

            // Initialize Configururation
            var agentConfiguration = new AgentConfiguration();

            agentConfiguration.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[0],
                Name = "pH",
                MinimumValue = 6.5,
                MaximumValue = 7.5
            }
            );
            agentConfiguration.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[1],
                Name = "BOD",
                MinimumValue = 22,
                MaximumValue = 30
            }
            );

            agentConfiguration.RunFrequency = new TimeSpan(0, 1, 0);
            agentConfiguration.UploadFrequency = new TimeSpan(0, 5, 0);

            Configuration = agentConfiguration;

            return base.InitializeAsync(authentificationApi, coreApi, digitalTwinApi, configurationApi, dataApi, ingestClientId, ingestAgentName, agentSubTypeId);
        }
        public override async Task<bool> RunAsync()
        {
            var success = true;
            DateTime dateTime = DateTime.Now;
            await UpdateTelemetryTwinInfoAsync();
            foreach (string telemetryId in Telemetry)
            {
                var telemetryConfig = GetById(telemetryId);
                if (telemetryConfig != null)
                {
                    try
                    {
                        Random random = new Random();
                        Double value = random.NextDouble() * (telemetryConfig.MaximumValue - telemetryConfig.MinimumValue) + telemetryConfig.MinimumValue;
                        IngestData(telemetryConfig.Id, dateTime, value, "");
                        IncrementNextRun(dateTime);
                    }
                    catch (Exception ex)
                    {
                        Logger.IngestLogData(ex.Message, ex.StackTrace);
                        success = false;
                    }
                }
                else
                    return false;
            }
            return success;
        }
        private async Task<bool> UpdateTelemetryTwinInfoAsync()
        {
            if (TelemetryTwins != null)
            {
                for (int i = 0; i < TelemetryTwins.Count; i++)
                {
                    var telemetryConfig = GetById(TelemetryTwins[i].TwinReferenceId);
                    if (telemetryConfig == null)
                        return false;
                    if (!string.IsNullOrEmpty(telemetryConfig.Name) && telemetryConfig.Name != TelemetryTwins[i].Name)
                        await UpdateTelemetryTwinName(TelemetryTwins[i], telemetryConfig.Name);
                }
            }
            return true;
        }
        public TelemetryConfig? GetById(string Id)
        {
            AgentConfiguration configuration = (AgentConfiguration)Configuration;
            var matches = configuration.Telemetry.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), Id.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
        public override bool LoadConfiguration(string json)
        {
            try
            {
                var configuration = JsonConvert.DeserializeObject<AgentConfiguration>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                if (configuration == null)
                    return false;
                configuration.Telemetry = configuration.Telemetry;
                configuration.RunFrequency = configuration.RunFrequency;
                configuration.UploadFrequency = configuration.UploadFrequency;
                Configuration = configuration;

            }
            catch
            {
                return false;
            }
            return true;
        }
    }

}
