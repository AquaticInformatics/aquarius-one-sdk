using Common.Configuration.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            Configuration = new IngestClientConfiguration();

            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _ingestClientDigitalTwin = ingestClientDigitalTwin;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
            Agents = new List<IngestAgent>();
            _name = ingestClientDigitalTwin.Name;
        }

        /// <summary>
        /// The Configuration Object related to the Agent
        /// </summary>
        public virtual IngestClientConfiguration Configuration { get; set; }

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
        /// Checks for new Configurations and loads them into the client
        /// </summary>
        /// <returns>Whether the check was successful</returns>
        public async Task<bool> CheckForUpdatedConfigurationsAsync()
        {
            try
            {
                var configurations = await _configurationApi.GetConfigurationsAsync(5, _ingestClientDigitalTwin.TwinReferenceId);

                if (configurations != null && configurations.Count > 0)
                {
                    configuration = configurations[0];
                    ConfigurationJson = configuration.ConfigurationData;
                }
                if (Configuration.ConfigCheckFrequency < new TimeSpan(0, 5, 0))
                    Configuration.ConfigCheckFrequency = new TimeSpan(0, 10, 0);
                if (Configuration.CycleFrequency < new TimeSpan(0, 0, 30))
                    Configuration.CycleFrequency = new TimeSpan(0, 0, 30);
                IncrementNextTimeToCheckConfigurations(DateTime.Now);
                return true;
            }
            catch
            {
                return false;
            }
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
            if (Name == null)
                Name = "Ingest Client";
            LastConfigCheck = Helper.GetDateTimeTwinDataProperty(_ingestClientDigitalTwin, "Client", "LastConfigCheck");
            NextConfigCheck = Helper.GetDateTimeTwinDataProperty(_ingestClientDigitalTwin, "Client", "NextConfigCheck");
            
            await CheckForUpdatedConfigurationsAsync();

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
                
            return await Save();
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

            // Update The client twin data
            var properties = new Dictionary<string, object>();

            properties.Add("LastConfigCheck", LastConfigCheck.ToString("MM/dd/yyyy hh:mm:ss"));
            properties.Add("NextConfigCheck", NextConfigCheck.ToString("MM/dd/yyyy hh:mm:ss"));

            JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
            var existingTwinData = new JObject();
            if (!string.IsNullOrEmpty(_ingestClientDigitalTwin.TwinData))
                existingTwinData = JObject.Parse(_ingestClientDigitalTwin.TwinData);
            jsonPatchDocument.Add("/Client", properties);

            if (jsonPatchDocument.Operations.Count > 0)
            {
                var updateTwin = await _digitalTwinApi.UpdateTwinDataAsync(_ingestClientDigitalTwin.TwinReferenceId, jsonPatchDocument);
                if (updateTwin != null)
                    _ingestClientDigitalTwin = updateTwin;
                else
                    return false;
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
                if (_ingestClientDigitalTwin != null)
                    return _ingestClientDigitalTwin.Name;
                return "";
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
                if (Configuration != null)
                    return Configuration.ToString();
                else
                    return _configurationJson;
            }
            set
            {
                if (value != null)
                {
                    LoadConfiguration(value);
                    _configurationJson = value;
                }
                else
                    _configurationJson = "";
            }
        }
        /// <summary>
        /// Loads the configuration object from the ConfigurationJSON
        /// </summary>
        /// <param name="json">The configuration represented as a JSON string</param>
        /// <returns>Whether the load was successful</returns>
        public virtual bool LoadConfiguration(string json)
        {
            try
            {
                Configuration = JsonConvert.DeserializeObject<IngestClientConfiguration>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool IsTimeToCheckConfigurations
        {
            get
            {

                return NextConfigCheck < DateTime.Now;
            }
        }

        /// <summary>
        /// Increments the Next Configuration Check and sets the Next Check according to the Configuration Check Frequency
        /// </summary>
        /// <param name="dateTime">The time of the current completed run</param>
        public void IncrementNextTimeToCheckConfigurations(DateTime dateTime)
        {
            NextConfigCheck = dateTime.Add(Configuration.ConfigCheckFrequency);
            LastConfigCheck = dateTime;
        }

        /// <summary>
        /// Last time the configurations was checked
        /// </summary>
        public DateTime LastConfigCheck { get; set; }

        /// <summary>
        /// Next time the configurations should be checked
        /// </summary>
        public DateTime NextConfigCheck { get; set; }
    }
}
