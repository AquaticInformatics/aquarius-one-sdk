using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentConfiguration : IngestAgentConfiguration
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
                 MinimumValue = 6.5,
                 MaximumValue = 7.5
            }
            );
            Telemetry.Add(new TelemetryConfig
            {
                Id = Guid.NewGuid().ToString(),
                Name = "BOD",
                MinimumValue = 22,
                MaximumValue = 30
            }
            );

            RunFrequency = new TimeSpan(0, 1, 0);
            UploadFrequency = new TimeSpan(0, 5, 0);

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
