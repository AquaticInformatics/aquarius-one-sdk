using Enterprise.Twin.Protobuf.Models;
using Newtonsoft.Json;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using ONE.Ingest.WindowsService.Agents.OpenWeatherMap.API;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.OpenWeatherMap
{
    public class AgentService : IngestAgent
    {
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public override Task<bool> InitializeAsync(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            // Initialize Telemetry
            Telemetry.Add(Guid.NewGuid().ToString());
            Telemetry.Add(Guid.NewGuid().ToString());
            Telemetry.Add(Guid.NewGuid().ToString());

            // Initialize Configururation
            var agentConfiguration = new AgentConfiguration 
            {
                ApiKey = "3fddb1fe23832fc9bbb45fb55756624d",
                Location = "Loveland, Colorado",
                RunFrequency = new TimeSpan(0, 1, 0),
                UploadFrequency = new TimeSpan(0, 5, 0)
            };

            agentConfiguration.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[0],
                Name = "Wind Speed",
                Field = "Wind.SpeedFeetPerSecond"
            }
            );
            agentConfiguration.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[1],
                Name = "Temperature",
                Field = "Main.Temperature.FahrenheitCurrent"
            }
            );
            agentConfiguration.Telemetry.Add(new TelemetryConfig
            {
                Id = Telemetry[2],
                Name = "Humidity",
                Field = "Main.Humidity"
            }
            );

            Configuration = agentConfiguration;

            return base.InitializeAsync(authentificationApi, coreApi, digitalTwinApi, configurationApi, dataApi, ingestClientId, ingestAgentName, agentSubTypeId);
        }

        public AgentService(ClientSDK clientSDK)
            : base(clientSDK.Authentication, clientSDK.Core, clientSDK.DigitalTwin, clientSDK.Configuration, clientSDK.Data, new DigitalTwin())
        {
            TwinSubTypeId = ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentOpenWeatherMap.RefId;
        }

        public override async Task<bool> RunAsync()
        {
            bool success = true;

            try
            {
                var agencyConfiguration = ((AgentConfiguration)Configuration);
                await UpdateTelemetryTwinInfoAsync();

                var client = new OpenWeatherApiClient(agencyConfiguration.ApiKey);
                var results = await client.QueryAsync(agencyConfiguration.Location);
                DateTime dateTime = DateTime.Now;

                foreach (string telemetryId in Telemetry)
                {
                    var telemetryConfig = GetById(telemetryId);
                    if (telemetryConfig != null)
                    {
                        try
                        {
                            switch (telemetryConfig.Field.ToUpper())
                            {
                                case "MAIN.TEMPERATURE.FAHRENHEITCURRENT":
                                    IngestData(telemetryConfig.Id, dateTime, results.Main.Temperature.FahrenheitCurrent, "", results.Main.Temperature);
                                    break;
                                case "WIND.SPEEDFEETPERSECOND":
                                    IngestData(telemetryConfig.Id, dateTime, results.Wind.SpeedFeetPerSecond, "", results.Wind);
                                    break;
                                case "MAIN.HUMIDITY":
                                    IngestData(telemetryConfig.Id, dateTime, results.Main.Humidity, "");
                                    break;
                            }

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
                IncrementNextRun(dateTime);
            }
            catch (Exception ex)
            {
                Logger.IngestLogData(ex.Message, ex.StackTrace);
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
