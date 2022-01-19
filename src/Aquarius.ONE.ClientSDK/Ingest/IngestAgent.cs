using Common.Configuration.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.JsonPatch;
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
        private Configuration configuration;
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
        }

        /// <summary>
        /// Initializes the Class when a new agent is created
        /// </summary>
        /// <param name="authentificationApi">The Authentication Class from the Client SDK</param>
        /// <param name="coreApi">The Core Class from the Client SDK</param>
        /// <param name="digitalTwinApi">The Digital Twin Class from the Client SDK</param>
        /// <param name="configurationApi">The Configuration Class from the Client SDK</param>
        /// <param name="dataApi">The Data Class from the Client SDK</param>
        /// <param name="ingestClientId">Digital Twin Refrence Id of the Instrument Ingestion Client</param>
        /// <param name="ingestAgentName">Name of this Agent</param>
        /// <param name="agentSubTypeId">Instrument Agent Digital Twin SubType Id</param>
        /// <returns></returns>
        public async Task<bool> InitializeAsync(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            _authentificationApi = authentificationApi;
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
                    Configuration.Initialize();
                    if (!string.IsNullOrEmpty(ConfigurationJson))
                    {
                        await SaveAsync();

                        // Create Telemetry Twins if they exist
                        foreach (var telemetry in Configuration.Telemetry)
                        {
                            if (!string.IsNullOrEmpty(telemetry.Id) && !string.IsNullOrEmpty(telemetry.Name))
                            {
                                DigitalTwin telemetryTwin = new DigitalTwin
                                {
                                    CategoryId = Enterprise.Twin.Constants.TelemetryCategory.Id,
                                    TwinTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.RefId,
                                    TwinSubTypeId = Enterprise.Twin.Constants.TelemetryCategory.HistorianType.InstrumentMeasurements.RefId,
                                    TwinReferenceId = telemetry.Id,
                                    Name = telemetry.Name,
                                    ParentTwinReferenceId = _digitalTwin.TwinReferenceId
                                };
                                await digitalTwinApi.CreateAsync(telemetryTwin);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the Agent with the information it needs to run
        /// </summary>
        /// <returns>Whether the Agent was successfully loaded</returns>
        public async Task<bool> LoadAsync()
        {
            var configurations = await _configurationApi.GetConfigurationsAsync(1, _digitalTwin.TwinReferenceId);
            Enabled = Helper.GetBoolTwinDataProperty(_digitalTwin, "", "Enabled");
            LastRun = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "", "LastRun");
            NextRun = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "", "NextRun");
            LastUpload = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "", "LastUpload");
            NextUpload = Helper.GetDateTimeTwinDataProperty(_digitalTwin, "", "NextUpload");
            if (configurations != null && configurations.Count > 0)
            {
                configuration = configurations[0];
                ConfigurationJson = configuration.ConfigurationData;
            }
            return false;
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
        /// <param name="propertyBag">(Optional) additional data stored as JSON</param>
        public void IngestData(string telemetryTwinId, DateTime dateTime, double value, string stringValue= "", string propertyBag = "")
        {
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
        /// Whether the Agent is Elegible to run. AKA it is time to run
        /// </summary>
        public bool IsTimeToRun
        {
            get 
            { 
                return NextRun < DateTime.Now; 
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
                return NextUpload < DateTime.Now;
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
            if (_name != Name)
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
            JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
            var existingTwinData = new JObject();
            if (!string.IsNullOrEmpty(_digitalTwin.TwinData))
                existingTwinData = JObject.Parse(_digitalTwin.TwinData);
            jsonPatchDocument = Helper.UpdateJsonDataField(_digitalTwin, jsonPatchDocument, existingTwinData, "Enabled", Enabled.ToString());
            jsonPatchDocument = Helper.UpdateJsonDataField(_digitalTwin, jsonPatchDocument, existingTwinData, "LastRun", LastRun.ToString("MM/dd/yyyy hh:mm:ss"));
            jsonPatchDocument = Helper.UpdateJsonDataField(_digitalTwin, jsonPatchDocument, existingTwinData, "NextRun", NextRun.ToString("MM/dd/yyyy hh:mm:ss"));
            jsonPatchDocument = Helper.UpdateJsonDataField(_digitalTwin, jsonPatchDocument, existingTwinData, "LastUpload", LastUpload.ToString("MM/dd/yyyy hh:mm:ss"));
            jsonPatchDocument = Helper.UpdateJsonDataField(_digitalTwin, jsonPatchDocument, existingTwinData, "NextUpload", NextUpload.ToString("MM/dd/yyyy hh:mm:ss"));
            if (jsonPatchDocument.Operations.Count > 0)
            {
                var updateTwin = await _digitalTwinApi.UpdateTwinDataAsync(_digitalTwin.TwinReferenceId, jsonPatchDocument);
                if (updateTwin != null)
                    _digitalTwin = updateTwin;
                else
                    return false;
            }
            if (configuration != null && ConfigurationJson != configuration.ConfigurationData)
            {
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = _digitalTwin.TwinReferenceId;
                configuration.EnumEntity = EnumEntity.EntityWorksheetview;

                return await _configurationApi.UpdateConfigurationAsync(configuration);

            }
            else if (configuration == null && !string.IsNullOrEmpty(ConfigurationJson))
            {
                configuration = new Configuration();
                configuration.EnumEntity = EnumEntity.EntityWorksheetview;
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = _digitalTwin.TwinReferenceId;
                configuration.IsPublic = true;
                return await _configurationApi.CreateConfigurationAsync(configuration);
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
                if (Configuration != null)
                    if (!Configuration.Load(value))
                        _configurationJson = value;
                    else
                    _configurationJson = value;
            }
        }

        /// <summary>
        /// The Configuration Object related to the Agent
        /// </summary>
        public IngestAgentConfiguration Configuration { get; set; }

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
