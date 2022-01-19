using Enterprise.Twin.Protobuf.Models;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Instantiates the IngestApi
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        public IngestApi(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
        }
        /// <summary>
        /// Retrieves the IngestClient by the Twin Reference Id
        /// </summary>
        /// <param name="ingestClientId">Digital Twin Refrence Id of the Instrument Ingestion Client</param>
        /// <returns>IngestClient</returns>
        public async Task<IngestClient> GetClientByIdAsync(string ingestClientId)
        {
            DigitalTwin ingestClientTwin = await _digitalTwinApi.GetAsync(ingestClientId);
            if (ingestClientTwin != null)
                return new IngestClient(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientTwin);
            return null;
        }
        /// <summary>
        /// Creates a Digital Twin for the Ingest Client and does minimal setup
        /// </summary>
        /// <param name="ingestClientName">Name of the Client</param>
        /// <returns>New IngestClient</returns>
        public async Task<IngestClient> RegisterClientAsync(string ingestClientName)
        {
            if (_authentificationApi.User == null)
            {
                string userInfo = await _authentificationApi.GetUserInfoAsync();
                if (!string.IsNullOrEmpty(userInfo))
                {
                    UserHelper userHelper = new UserHelper(_coreApi);
                    _authentificationApi.User = await userHelper.GetUserFromUserInfoAsync(userInfo);
                }
            }

            DigitalTwin ingestClientTwin = new DigitalTwin {
                CategoryId = Enterprise.Twin.Constants.IntrumentCategory.Id,
                TwinTypeId = Enterprise.Twin.Constants.IntrumentCategory.ClientIngestType.RefId,
                TwinSubTypeId = Enterprise.Twin.Constants.IntrumentCategory.ClientIngestType.ClientIngestSubType.RefId,
                TwinReferenceId = Guid.NewGuid().ToString(),
                Name = ingestClientName,
                ParentTwinReferenceId = _authentificationApi.User.TenantId
            };
            ingestClientTwin = await _digitalTwinApi.CreateAsync(ingestClientTwin);
            if (ingestClientTwin != null)
            {
                var ingestClient = new IngestClient(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientTwin);
                return ingestClient;
            }
            return null;
        }
        /// <summary>
        /// Retrieves a collection of all IngestClients the user has rights to retrieve
        /// </summary>
        /// <returns>Collection of IngestClients</returns>
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
