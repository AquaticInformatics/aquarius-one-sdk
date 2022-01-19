using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class TelemetryConfig : IngestTelemetryConfiguration
    {
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }

    }
}
