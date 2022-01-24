using System.Collections.Generic;

namespace ONE.Ingest.WindowsService.Agents.CSV
{
    public class AgentConfiguration : IngestAgentConfiguration
    {
        public AgentConfiguration()
        {
            Files = new List<FileConfig>();
            ProcessedPath = "";
        }
        public List<FileConfig> Files { get; set; }

        public string ProcessedPath { get; set; }


    }
}
