using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ONE.Ingest.WindowsService.Agents.OpenWeatherMap
{
    public class AgentConfiguration : IngestAgentConfiguration
    {
        public AgentConfiguration()
        {
            Telemetry = new List<TelemetryConfig>();
        }
        public List<TelemetryConfig> Telemetry { get; set; }
        public string ApiKey { get; set; }
        public string Location { get; set; }
    }
}
