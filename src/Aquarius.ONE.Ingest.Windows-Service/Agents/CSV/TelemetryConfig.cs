namespace ONE.Ingest.WindowsService.Agents.CSV
{
    public class TelemetryConfig
    {
        /// <summary>
        /// ID of the Telemetry Twin representing the Dataset
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name of the Telemetry Twin representing the Dataset
        /// </summary>
        public string Name { get; set; }

        public int FilterByColumnNumber { get; set; }
        public string Filter { get; set; }

        public int ValueColumnNumber { get; set; }
        public int StringValueColumnNumber { get; set; }
        
        public bool PutAllColumnsIntoPropertyBag { get; set; }

    }
}
