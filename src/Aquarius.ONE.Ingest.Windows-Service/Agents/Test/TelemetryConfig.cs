namespace ONE.Ingest.WindowsService.Agents.Test
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
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }

    }
}
