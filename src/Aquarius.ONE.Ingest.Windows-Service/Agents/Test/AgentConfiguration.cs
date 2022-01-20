using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentConfiguration : IngestAgentConfiguration
    {
        public AgentConfiguration()
        {
            Telemetry = new List<TelemetryConfig>();
        }
        public List<TelemetryConfig> Telemetry { get; set; }
       
        
    }
}
