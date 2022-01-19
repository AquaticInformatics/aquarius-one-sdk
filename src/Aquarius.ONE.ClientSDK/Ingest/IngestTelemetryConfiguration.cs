using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Ingest
{
    public class IngestTelemetryConfiguration
    {
        /// <summary>
        /// ID of the Telemetry Twin representing the Dataset
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name of the Telemetry Twin representing the Dataset
        /// </summary>
        public string Name { get; set; }
    }
}
