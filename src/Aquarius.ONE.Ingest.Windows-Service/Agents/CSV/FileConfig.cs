using System;
using System.Collections.Generic;
using System.Linq;

namespace ONE.Ingest.WindowsService.Agents.CSV
{
    public class FileConfig
    {
        public FileConfig()
        {
            Telemetry = new List<TelemetryConfig>();
            Path = "";
            FileName = "";
        }
        public string Path { get; set; }
        public string FileName { get; set; }

        public bool HasHeaderRow { get; set; }

        public int DateColumnNumber { get; set; }
        public List<TelemetryConfig> Telemetry { get; set; }

        public TelemetryConfig? GetById(string Id)
        {
            var matches = Telemetry.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), Id.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
    }
}
