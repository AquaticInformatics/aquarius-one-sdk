using Common.Configuration.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using Google.Protobuf.WellKnownTypes;
using ONE.Common.Configuration;
using ONE.Common.Historian;
using ONE.Enterprise.Authentication;
using ONE.Enterprise.Core;
using ONE.Enterprise.Twin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ONE.Ingest
{
    public class IngestClient
    {
        private ConfigurationApi _configurationApi;
        private CoreApi _coreApi;
        private DigitalTwinApi _digitalTwinApi;
        private DigitalTwin _ingestClientDigitalTwin;
        private DataApi _dataApi;
        private AuthenticationApi _authentificationApi;
        private Configuration configuration;

        /// <summary>
        /// Instantiates the IngestClientClass
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="ingestClientDigitalTwin">Digital Twin of the Instrument Ingestion Client</param>
        public IngestClient(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin ingestClientDigitalTwin)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _ingestClientDigitalTwin = ingestClientDigitalTwin;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
            Agents = new List<IngestAgent>();
        }

        /// <summary>
        /// A Collection of Ingest Agents that belong to this client
        /// </summary>
        public List<IngestAgent> Agents { get; set; }

        /// <summary>
        /// This is a class that manages the logging of information for the Client
        /// </summary>
        public IngestLogger Logger { get; set; }

        /// <summary>
        /// Registers a new IngestAgent with this Client
        /// </summary>
        /// <param name="ingestAgent">The IngestAgent Object</param>
        /// <param name="ingestAgentName">Name of the new Agent</param>
        /// <param name="agentSubTypeId">Instrument Agent Digital Twin SubType Id</param>
        /// <returns>The newly registered Ingest Agent</returns>
        public async Task<IngestAgent> RegisterAgentAsync(IngestAgent ingestAgent, string ingestAgentName, string agentSubTypeId)
        {
            bool success = await ingestAgent.InitializeAsync(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, Id, ingestAgentName, agentSubTypeId);
            if (success)
            {
                Agents.Add(ingestAgent);

                return ingestAgent;
            }
            return null;
        }
        /// <summary>
        /// Loads the Client with the information it needs to run
        /// </summary>
        /// <param name="ingestAgents">The collection of ingest agent objects</param>
        /// <returns>Whether the Client was successfully loaded</returns>
        public async Task<bool> LoadAsync(List<IngestAgent> ingestAgents)
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
            
            var configurations = await _configurationApi.GetConfigurationsAsync(5, _ingestClientDigitalTwin.TwinReferenceId);
            if (configurations != null && configurations.Count > 0)
            {
                configuration = configurations[0];
                ConfigurationJson = configuration.ConfigurationData;
            }
            Logger = await IngestLogger.GetByParentAsync(_authentificationApi, _digitalTwinApi, _dataApi, _ingestClientDigitalTwin.TwinReferenceId);
            if (Logger == null)
                Logger = await IngestLogger.InitializeAsync(_authentificationApi, _digitalTwinApi, _dataApi, _ingestClientDigitalTwin.TwinReferenceId, Name + " Logger");
            var ingestAgentTwins = await _digitalTwinApi.GetDescendantsByTypeAsync(_ingestClientDigitalTwin.TwinReferenceId, ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.RefId);

            if (ingestAgentTwins != null)
            {
                foreach (var ingestAgentTwin in ingestAgentTwins)
                {
                    foreach (var ingestAgent in ingestAgents)
                    {
                        if (ingestAgent.TwinSubTypeId == ingestAgentTwin.TwinSubTypeId)
                        {
                            await ingestAgent.LoadAsync(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestAgentTwin);
                            Agents.Add(ingestAgent);
                        }
                    }
                    
                }
            }
            return false;
        }

        /// <summary>
        /// Saves all of the Digital Twin and Configuration Data related to the client.
        /// </summary>
        /// <returns>Whether the save was successful</returns>
        public async Task<bool> Save()
        {
            if (_name != Name)
            {
                _ingestClientDigitalTwin.Name = _name;
                _ingestClientDigitalTwin.UpdateMask = new FieldMask { Paths = { "name" } };
                var updatedTWin = await _digitalTwinApi.UpdateAsync(_ingestClientDigitalTwin);
                if (updatedTWin != null)
                    _ingestClientDigitalTwin = updatedTWin;
                else
                    return false;
            }
            if (configuration != null && ConfigurationJson != configuration.ConfigurationData)
            {
                configuration.ConfigurationData = ConfigurationJson;
                configuration.EnumEntity = EnumEntity.EntityWorksheetview;
                configuration.FilterById = _ingestClientDigitalTwin.TwinReferenceId;

                return await _configurationApi.UpdateConfigurationAsync(configuration);
            }
            else if (configuration == null && !string.IsNullOrEmpty(ConfigurationJson))
            {
                configuration = new Configuration();
                configuration.Name = Name;
                configuration.OwnerId = _authentificationApi.User.Id;
                configuration.EnumEntity = EnumEntity.EntityWorksheetview; 
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = _ingestClientDigitalTwin.TwinReferenceId;
                return await _configurationApi.CreateConfigurationAsync(configuration);
            }
            return true;
        }


        private string _name;

        /// <summary>
        /// The name of the Ingest Client.  
        /// </summary>
        public string Name
        {
            get
            {
                return _ingestClientDigitalTwin.Name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// The Twin Reference Id of the Twin that represents the ingest client
        /// </summary>
        public string Id
        {
            get
            {
                return _ingestClientDigitalTwin.TwinReferenceId;
            }
        }

        private string _configurationJson;

        /// <summary>
        /// This property returns the configuration JSON for the Ingest Client.  
        /// </summary>
        public string ConfigurationJson
        {
            get
            {
                return _configurationJson;
            }
            set
            {
                _configurationJson = value;
            }
        }
    }
}
