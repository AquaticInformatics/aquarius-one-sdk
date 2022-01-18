using Enterprise.Twin.Protobuf.Models;
using Microsoft.Extensions.Logging;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using ONE.Ingest.WindowsService.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TimeSeries.Data.Protobuf.Models;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public class AgentService : IngestAgent
    {
        public AgentService(ClientSDK clientSDK) 
            : base(clientSDK.Authentication, clientSDK.Core, clientSDK.DigitalTwin, clientSDK.Configuration, clientSDK.Data, new DigitalTwin())
        {
            Configuration = new AgentConfiguration();
        }
        public bool Ingest()
        {
            try
            {
              
            }
            catch (Exception ex)
            {
                //return $"That's not funny! {ex}";
            }

            throw new NotImplementedException();
        }
    }

}
