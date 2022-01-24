namespace ONE.Ingest.WindowsService.Agents.OpenWeatherMap
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

        public string Field { get; set; }

    }
}
