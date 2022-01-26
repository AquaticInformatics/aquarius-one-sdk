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
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSeries.Data.Protobuf.Models;

namespace ONE.Ingest
{
    public class IngestAgent
    {
        private AuthenticationApi _authentificationApi;
        private CoreApi _coreApi;
        private ConfigurationApi _configurationApi;
        private DigitalTwinApi _digitalTwinApi;
        private DigitalTwin _digitalTwin = null;
        private Configuration _configuration;
        private DataApi _dataApi;

        /// <summary>
        /// Instantiates the Class
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="ingestAgentDigitalTwin">Digital Twin of the Instrument Ingestion Client</param>
        public IngestAgent(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin ingestAgentDigitalTwin)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _digitalTwin = ingestAgentDigitalTwin;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
            DataSets = new Dictionary<string, TimeSeriesDatas>();
            _name = _digitalTwin.Name;
            Telemetry = new List<string>();
        }

        /// <summary>
        /// Initializes the Class when a new agent is created
        /// </summary>
        /// <param name="authenticationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="ingestClientId">Digital Twin Reference Id of the Instrument Ingestion Client</param>
        /// <param name="ingestAgentName">Name of this Agent</param>
        /// <param name="agentSubTypeId">Instrument Agent Digital Twin SubType Id</param>
        /// <returns>Whether the agent was successfully initialized</returns>
        public virtual async Task<bool> InitializeAsync(AuthenticationApi authenticationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            _authentificationApi = authenticationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
            _name = ingestAgentName;
            
            DigitalTwin ingestAgentTwin = new DigitalTwin
            {
                CategoryId = Enterprise.Twin.Constants.IntrumentCategory.Id,
                TwinTypeId = Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.RefId,
                TwinSubTypeId = agentSubTypeId,
                TwinReferenceId = Guid.NewGuid().ToString(),
                Name = ingestAgentName,
                ParentTwinReferenceId = ingestClientId
            };
            _digitalTwin = await _digitalTwinApi.CreateAsync(ingestAgentTwin);
            if (_digitalTwin != null)
            {
                Logger = await IngestLogger.InitializeAsync(_authentificationApi, _digitalTwinApi, _dataApi, _digitalTwin.TwinReferenceId, ingestAgentName + " Logger");

                if (Configuration != null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationJson))
                    {
                        await SaveAsync();
                        TelemetryTwins = new List<DigitalTwin>();

                        // Create Telemetry Twins if they exist
                        foreach (var telemetry in Telemetry)
                        {
                            if (!string.IsNullOrEmpty(telemetry) && !string.IsNullOrEmpty(telemetry))
                            {
                                DigitalTwin telemetryTwin = new DigitalTwin
                                {
                                    CategoryId = Enterprise.Twin.Constants.TelemetryCategory.Id,
                                    TwinTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.RefId,
                                    TwinSubTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.InstrumentMeasurements.RefId,
                                    TwinReferenceId = telemetry,
                                    Name = telemetry,
                                    ParentTwinReferenceId = _digitalTwin.TwinReferenceId
                                };
                                DigitalTwin newTwin = await digitalTwinApi.CreateAsync(telemetryTwin);
                                TelemetryTwins.Add(newTwin);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates one of the Telemetry Twins Names
        /// </summary>
        /// <param name="digitalTwin">The Telemetry Twin</param>
        /// <param name="name">The new name of the Telemetry Twin</param>
        /// <returns>Whether the Telemetry Twin was updated</returns>
        public async Task<bool> UpdateTelemetryTwinName(DigitalTwin digitalTwin, string name)
        {
            if (digitalTwin != null && !string.IsNullOrEmpty(name) && digitalTwin.Name != name)
            {
                digitalTwin.Name = name;
                digitalTwin.UpdateMask = new FieldMask { Paths = { "name" } };
                var updatedTwin = await _digitalTwinApi.UpdateAsync(digitalTwin);
                if (updatedTwin != null)
                {
                    for (int i = 0; i < TelemetryTwins.Count; i++)
                    {
                        if (TelemetryTwins[i].TwinReferenceId == digitalTwin.TwinReferenceId)
                        {
                            TelemetryTwins[i] = digitalTwin;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Loads the Agent with the information it needs to run
        /// </summary>
        /// <param name="authenticationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="digitalTwin">The digital Twin that represents the IngestAgent</param>
        /// <returns>Whether the Agent was successfully loaded</returns>
        public async Task<bool> LoadAsync(AuthenticationApi authenticationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin digitalTwin)
        {
            _authentificationApi = authenticationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
            _digitalTwin = digitalTwin;
            
            LastRun = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "Agent", "LastRun");
            NextRun = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "Agent", "NextRun");
            LastUpload = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "Agent", "LastUpload");
            NextUpload = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "Agent", "NextUpload");

            await CheckForUpdatedConfigurationsAsync();

            Logger = await IngestLogger.GetByParentAsync(_authentificationApi, _digitalTwinApi, _dataApi, _digitalTwin.TwinReferenceId);
            return false;
        }
        /// <summary>
        /// Checks for new Configurations and loads them into the agent
        /// </summary>
        /// <returns>Whether the check was successful</returns>
        public async Task<bool> CheckForUpdatedConfigurationsAsync(DigitalTwin digitalTwin = null)
        {

            try
            {
                if (digitalTwin != null)
                    _digitalTwin = digitalTwin;
                else
                    _digitalTwin = await _digitalTwinApi.GetAsync(_digitalTwin.TwinReferenceId);
                _name = _digitalTwin.Name;
                Enabled = Helper.GetBoolTwinDataProperty(_digitalTwin, "Agent", "Enabled");
                Telemetry = Helper.GetTwinDataPropertyAslist(_digitalTwin, "Agent", "Telemetry");

                TelemetryTwins = new List<DigitalTwin>();
                foreach (string telemetryId in Telemetry)
                {
                    var telemetryTwin = await _digitalTwinApi.GetAsync(telemetryId);
                    if (telemetryTwin == null)
                        Telemetry.Remove(telemetryId);
                    else
                    {
                        TelemetryTwins.Add(telemetryTwin);
                    }

                }

                var configurations = await _configurationApi.GetConfigurationsAsync(5, _digitalTwin.TwinReferenceId);
                if (configurations != null && configurations.Count > 0)
                {
                    _configuration = configurations[0];
                    ConfigurationJson = _configuration.ConfigurationData;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Data is a memory cache for the data to be stored by Telemetry GUID
        /// </summary>
        public Dictionary<string,TimeSeriesDatas> DataSets { get; set; }

        /// <summary>
        /// Uploads the data to a Telemetry Data Set configured by a digital Twin with the same Reference Id
        /// </summary>
        /// <returns>Whether the upload was successful</returns>
        public async Task<bool> UploadAsync()
        {
            DateTime dateTime = DateTime.Now;
            bool success = true;
            foreach (var dataset in DataSets)
            {
                var result = await _dataApi.SaveDataAsync(dataset.Key, dataset.Value);
                if (result == null)
                    success = false;
                else
                {
                    DataSets[dataset.Key] = new TimeSeriesDatas();
                    IncrementNextUpload(dateTime);
                }
            }
            if (success)
                await SaveAsync();
            return success;
        }

        /// <summary>
        /// Adds Data to be uploaded to a local cache until the UploadAsync method is called
        /// </summary>
        /// <param name="telemetryTwinId">Digital Twin Reference Id of the Telemetry Dataset</param>
        /// <param name="dateTime">Time related to the value</param>
        /// <param name="value">Numerical data to be stored</param>
        /// /// <param name="stringValue">(Optional) String equivalent of the Numerical data to be stored</param>
        /// <param name="detail">(Optional) additional data stored as JSON</param>
        public void IngestData(string telemetryTwinId, DateTime dateTime, double? value, string stringValue= "", object detail = null)
        {
            string propertyBag = "";
            if (detail == null)
                propertyBag = "";
            else if (detail is System.Type String)
            {
                propertyBag = detail.ToString();
            }
            else
            {
                propertyBag = JsonConvert.SerializeObject(detail, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            IngestData(telemetryTwinId, new TimeSeriesData 
            { 
                DateTimeUTC = dateTime.ToJsonTicksDateTime(),
                Value = value,
                StringValue = stringValue,
                PropertyBag = propertyBag
            });
        }

        /// <summary>
        /// Adds Data to be uploaded to a local cache until the UploadAsync method is called
        /// </summary>
        /// <param name="telemetryTwinId">Digital Twin Reference Id of the Telemetry Dataset</param>
        /// <param name="timeSeriesData">Protocol Buffer used to represent Data</param>
        public void IngestData(string telemetryTwinId, TimeSeriesData timeSeriesData)
        {
            if (DataSets == null)
                DataSets = new Dictionary<string,TimeSeriesDatas>();
            if (!DataSets.ContainsKey(telemetryTwinId))
                DataSets.Add(telemetryTwinId, new TimeSeriesDatas());
            DataSets[telemetryTwinId].Items.Add(timeSeriesData);
        }
        /// <summary>
        /// Whether the Agent is Enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Whether the Agent is Eligible to run. AKA it is time to run.
        /// </summary>
        public bool IsTimeToRun
        {
            get 
            { 

                return NextRun < DateTime.Now && Enabled; 
            }
        }

        /// <summary>
        /// Increments the Next Run and sets the Next run according to the Configuration Run Frequency
        /// </summary>
        /// <param name="dateTime">The time of the current completed run</param>
        public void IncrementNextRun(DateTime dateTime)
        {
            NextRun = dateTime.Add(Configuration.RunFrequency);
            LastRun = dateTime;
        }

        /// <summary>
        /// Whether the Agent Data is Elegible to be uploaded. AKA it is time to upload data
        /// </summary>
        public bool IsTimeToUpload
        {
            get
            {
                return NextUpload < DateTime.Now && Enabled;
            }
        }
        /// <summary>
        /// Increments the Next Upload time and sets the Next Upload according to the Configuration Upload Frequency
        /// </summary>
        /// <param name="dateTime">The time of the current completed upload</param>
        public void IncrementNextUpload(DateTime dateTime)
        {
            NextUpload = dateTime.Add(Configuration.UploadFrequency);
            LastUpload = dateTime;
        }
        /// <summary>
        /// The Telemetry Datasets that are related to the Agent
        /// </summary>
        public List<string> Telemetry { get; set; }
        
        /// <summary>
        /// The Twins related to the telemetry for this Agent
        /// </summary>
        public List<DigitalTwin> TelemetryTwins { get; set; }


        /// <summary>
        /// Last time the agent was run
        /// </summary>
        public DateTime LastRun { get; set; }
        
        /// <summary>
        /// Next time the agent should be run
        /// </summary>
        public DateTime NextRun { get; set; }

        /// <summary>
        /// Last time the data in the agent was uploaded
        /// </summary>
        public DateTime LastUpload { get; set; }

        /// <summary>
        /// Next time the data in the agent should be uploaded
        /// </summary>
        public DateTime NextUpload { get; set; }

        /// <summary>
        /// This is a class that manages the logging of information for the Agent
        /// </summary>
        public IngestLogger Logger { get; set; }
        
        /// <summary>
        /// Saves all of the Digital Twin and Configuration Data related to the agent.
        /// </summary>
        /// <returns>Whether the save was successful</returns>
        public async Task<bool> SaveAsync()
        {
            if (!string.IsNullOrEmpty(_name) && _name != Name)
            {
                _digitalTwin.Name = _name;
                _digitalTwin.UpdateMask = new FieldMask { Paths = { "name" } };
                var updatedTWin = await _digitalTwinApi.UpdateAsync(_digitalTwin);
                if (updatedTWin != null)
                    _digitalTwin = updatedTWin;
                else
                    return false;
            }
            // Update The agent twin data
            var properties = new Dictionary<string, object>();

            properties.Add("Enabled", Enabled.ToString());
            properties.Add("LastRun", LastRun.ToString("MM/dd/yyyy hh:mm:ss"));
            properties.Add("NextRun", NextRun.ToString("MM/dd/yyyy hh:mm:ss"));
            properties.Add("LastUpload", LastUpload.ToString("MM/dd/yyyy hh:mm:ss"));
            properties.Add("NextUpload", NextUpload.ToString("MM/dd/yyyy hh:mm:ss"));
            properties.Add("Telemetry", Telemetry);

            JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
            var existingTwinData = new JObject();
            if (!string.IsNullOrEmpty(_digitalTwin.TwinData))
                existingTwinData = JObject.Parse(_digitalTwin.TwinData);
            jsonPatchDocument.Add("/Agent", properties);

            if (jsonPatchDocument.Operations.Count > 0)
            {
                var updateTwin = await _digitalTwinApi.UpdateTwinDataAsync(_digitalTwin.TwinReferenceId, jsonPatchDocument);
                if (updateTwin != null)
                    _digitalTwin = updateTwin;
                else
                    return false;
            }
            if (_configuration != null && ConfigurationJson != _configuration.ConfigurationData)
            {
                _configuration.ConfigurationData = ConfigurationJson;
                _configuration.FilterById = _digitalTwin.TwinReferenceId;
                _configuration.EnumEntity = EnumEntity.EntityWorksheetview;

                return await _configurationApi.UpdateConfigurationAsync(_configuration);

            }
            else if (_configuration == null && !string.IsNullOrEmpty(ConfigurationJson))
            {
                _configuration = new Configuration();
                _configuration.EnumEntity = EnumEntity.EntityWorksheetview;
                _configuration.ConfigurationData = ConfigurationJson;
                _configuration.FilterById = _digitalTwin.TwinReferenceId;
                _configuration.IsPublic = true;
                return await _configurationApi.CreateConfigurationAsync(_configuration);
            }
            return true;
        }

        private string _name;

        /// <summary>
        /// The name of the agent
        /// </summary>
        public string Name
        {
            get
            {
                return _digitalTwin.Name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Identifies the unique type of the agent.  This must be set by the implementing class for the agent to be properly registered
        /// </summary>
        public string TwinSubTypeId { get; set; }

        private string _configurationJson;

        /// <summary>
        /// The JSON representation of the configuration
        /// </summary>
        public virtual string ConfigurationJson
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
                Configuration = JsonConvert.DeserializeObject<IngestAgentConfiguration>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// The Configuration Object related to the Agent
        /// </summary>
        public virtual IngestAgentConfiguration Configuration { get; set; }

        /// <summary>
        /// Runs the agent to obtain data
        /// </summary>
        /// <returns>Whether the run was successful</returns>
        public virtual async Task<bool> RunAsync()
        {
            return true;
        }
    }
}
