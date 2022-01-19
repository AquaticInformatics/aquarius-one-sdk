using Enterprise.Twin.Protobuf.Models;
using System;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentService : IngestAgent
    {
        public AgentService(ClientSDK clientSDK) 
            : base(clientSDK.Authentication, clientSDK.Core, clientSDK.DigitalTwin, clientSDK.Configuration, clientSDK.Data, new DigitalTwin())
        {
            Configuration = new AgentConfiguration();
        }
        public override async Task<bool> RunAsync()
        {
            var success = true;
            DateTime dateTime = DateTime.Now;
            foreach (TelemetryConfig telemetryConfig in Configuration.Telemetry)
            {
                try
                {
                    Random random = new Random();
                    Double value = random.NextDouble() * (telemetryConfig.MaximumValue - telemetryConfig.MinimumValue) + telemetryConfig.MinimumValue;
                    IngestData(telemetryConfig.Id, dateTime, value, "");
                    IncrementNextRun(dateTime);
                }
                catch (Exception ex)
                {
                    Logger.IngestLogData(ex.Message, ex.StackTrace);
                    success = false;
                }
            }
            return success;
        }
    }

}
