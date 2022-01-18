using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentConfiguration : IngestConfiguration
    {
        public AgentConfiguration()
        {
            Telemetry = new List<IngestTelemetryConfiguration>();
        }

        public override void Initialize()
        {
            // To get the user started, stub out two new output telemetries 
            Telemetry.Add(new TelemetryConfig
            {
                 Id = Guid.NewGuid().ToString(),
                 Name = "pH",
                 MinimumValue = 6.5M,
                 MaximumValue = 7.5M
            }
            );
            Telemetry.Add(new TelemetryConfig
            {
                Id = Guid.NewGuid().ToString(),
                Name = "BOD",
                MinimumValue = 22M,
                MaximumValue = 30M
            }
            );
        }
        public override bool Load(string configurationJson)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<AgentConfiguration>(configurationJson, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
