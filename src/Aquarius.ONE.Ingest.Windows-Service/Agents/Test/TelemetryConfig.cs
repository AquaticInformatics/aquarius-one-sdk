using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class TelemetryConfig : IngestTelemetryConfiguration
    {
        public decimal MinimumValue { get; set; }
        public decimal MaximumValue { get; set; }

    }
}
