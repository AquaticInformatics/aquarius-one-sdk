using Enterprise.Twin.Protobuf.Models;
using ONE.Common.Configuration;
using ONE.Enterprise.Twin;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Ingest
{
    public class IngestApi
    {
        private DigitalTwinApi _digitalTwinApi;
        private ConfigurationApi _configurationApi;

        public IngestApi(DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi)
        {
            _digitalTwinApi = digitalTwinApi;
            _configurationApi = configurationApi;
        }

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<IngestClient> GetClientByIdAsync(string ingestClientId)
        {
            DigitalTwin ingestClientTwin = await _digitalTwinApi.GetAsync(ingestClientId);
            if (ingestClientTwin != null)
                return new IngestClient(_digitalTwinApi, _configurationApi, ingestClientTwin);
            return null;
        }
        public async Task<IngestClient> RegisterClientAsync(string ingestClientName)
        {
            /*
            DigitalTwin ingestClientTwin = await _digitalTwinApi.GetAsync(id);
            if (ingestClientTwin != null)
                return new IngestClient(_digitalTwinApi, ingestClientTwin);
            */
            return null;
        }
        public async Task<List<IngestClient>> GetAllClientsAsync()
        {
            return null;
        }
    }
}
