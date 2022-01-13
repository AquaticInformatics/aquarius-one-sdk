using Enterprise.Twin.Protobuf.Models;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
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
        private AuthenticationApi _authentificationApi;
        private CoreApi _coreApi;
        private DigitalTwinApi _digitalTwinApi;
        private ConfigurationApi _configurationApi;
        private DataApi _dataApi;

        public IngestApi(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
        }

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<IngestClient> GetClientByIdAsync(string ingestClientId)
        {
            DigitalTwin ingestClientTwin = await _digitalTwinApi.GetAsync(ingestClientId);
            if (ingestClientTwin != null)
                return new IngestClient(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientTwin);
            return null;
        }
        public async Task<IngestClient> RegisterClientAsync(string ingestClientName)
        {
            DigitalTwin ingestClientTwin = new DigitalTwin {
                CategoryId = Enterprise.Twin.Constants.IntrumentCategory.Id,
                TwinTypeId = Enterprise.Twin.Constants.IntrumentCategory.ClientIngestType.RefId,
                TwinSubTypeId = Enterprise.Twin.Constants.IntrumentCategory.ClientIngestType.ClientIngestSubType.RefId,
                Name = ingestClientName
            };
            ingestClientTwin = await _digitalTwinApi.CreateAsync(ingestClientTwin);
            if (ingestClientTwin != null)
            {
                var ingestClient = new IngestClient(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientTwin);
                return ingestClient;
            }
            return null;
        }
        public async Task<List<IngestClient>> GetAllClientsAsync()
        {
            List<IngestClient> ingestClients = new List<IngestClient>();

            if (_authentificationApi.User == null)
            {
                string userInfo = await _authentificationApi.GetUserInfoAsync();
                if (!string.IsNullOrEmpty(userInfo))
                {
                    UserHelper userHelper = new UserHelper(_coreApi);
                    _authentificationApi.User = await userHelper.GetUserFromUserInfoAsync(userInfo);
                }
            }

            var ingestClientTwins = await _digitalTwinApi.GetDescendantsByTypeAsync(_authentificationApi.User.TenantId, ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestType.RefId);
            if (ingestClientTwins != null)
            {
                foreach (var ingestClientTwin in ingestClientTwins)
                {
                    ingestClients.Add(new IngestClient(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientTwin));
                }
            }
            return ingestClients;
        }
    }
}
