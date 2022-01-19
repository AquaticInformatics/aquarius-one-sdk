using Common.Configuration.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using Google.Protobuf.WellKnownTypes;
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
        public DigitalTwin DigitalTwin;
        private Configuration configuration;
        private DataApi _dataApi;

        /// <summary>
        /// Data is the in memory cache for the data to be stored by Telemetry GUID
        /// </summary>
        public Dictionary<string,TimeSeriesDatas> DataSets { get; set; }

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
                    incrementNextUpload(dateTime);
                }
            }
            return success;
        }

        public void IngestData(string telemetryTwinId, DateTime dateTime, double value, string propertyBag)
        {
            IngestData(telemetryTwinId, new TimeSeriesData 
            { 
                DateTimeUTC = dateTime.ToJsonTicksDateTime(),
                Value = value,
                PropertyBag = propertyBag
            });
        }
        public void IngestData(string telemetryTwinId, TimeSeriesData timeSeriesData)
        {
            if (!DataSets.ContainsKey(telemetryTwinId))
                DataSets.Add(telemetryTwinId, new TimeSeriesDatas());
            DataSets[telemetryTwinId].Items.Add(timeSeriesData);
        }
        public bool Enabled { get; set; }

        public bool IsTimeToRun
        {
            get 
            { 
                return NextRun < DateTime.Now; 
            }
        }
        public void incrementNextRun(DateTime dateTime)
        {
            NextRun = dateTime.Add(Configuration.RunFrequency);
            LastRun = dateTime;
        }
        public bool IsTimeToUpload
        {
            get
            {
                return NextUpload < DateTime.Now;
            }
        }
        public void incrementNextUpload(DateTime dateTime)
        {
            NextUpload = dateTime.Add(Configuration.UploadFrequency);
            LastUpload = dateTime;
        }
        public DateTime LastRun { get; set; }
        public DateTime NextRun { get; set; }
        public DateTime LastUpload { get; set; }
        public DateTime NextUpload { get; set; }
        public IngestLogger Logger { get; set; }
        public IngestAgent(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin ingestClientDigitalTwin)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            DigitalTwin = ingestClientDigitalTwin;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
        }

        public async Task<bool> InitializeAsync(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi= digitalTwinApi;
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
            DigitalTwin = await _digitalTwinApi.CreateAsync(ingestAgentTwin);
            if (DigitalTwin != null)
            {
                Logger = await IngestLogger.InitializeAsync(_authentificationApi, _digitalTwinApi, _dataApi, DigitalTwin.TwinReferenceId, ingestAgentName + " Logger");

                if (Configuration != null)
                {
                    Configuration.Initialize();
                    if (!string.IsNullOrEmpty(ConfigurationJson))
                    {
                        await Save();

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
                                    ParentTwinReferenceId = DigitalTwin.TwinReferenceId
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

        
        public async Task<bool> LoadAsync()
        {
            var configurations = await _configurationApi.GetConfigurationsAsync(1, DigitalTwin.TwinReferenceId);
            if (configurations != null && configurations.Count > 0)
            {
                configuration = configurations[0];
                ConfigurationJson = configuration.ConfigurationData;
            }
            return false;
        }

        public async Task<bool> Save()
        {
            if (_name != Name)
            {
                DigitalTwin.Name = _name;
                DigitalTwin.UpdateMask = new FieldMask { Paths = { "name" } };
                var updatedTWin = await _digitalTwinApi.UpdateAsync(DigitalTwin);
                if (updatedTWin != null)
                    DigitalTwin = updatedTWin;
                else
                    return false;
            }
            if (configuration != null && ConfigurationJson != configuration.ConfigurationData)
            {
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = DigitalTwin.TwinReferenceId;
                configuration.EnumEntity = EnumEntity.EntityWorksheetview;

                return await _configurationApi.UpdateConfigurationAsync(configuration);

            }
            else if (configuration == null && !string.IsNullOrEmpty(ConfigurationJson))
            {
                configuration = new Configuration();
                configuration.EnumEntity = EnumEntity.EntityWorksheetview;
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = DigitalTwin.TwinReferenceId;
                configuration.IsPublic = true;
                return await _configurationApi.CreateConfigurationAsync(configuration);
            }
            return true;
        }

        private string _name;

        public string Name
        {
            get
            {
                return DigitalTwin.Name;
            }
            set
            {
                _name = value;
            }
        }

        private string _configurationJson;

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

        public IngestConfiguration Configuration { get; set; }

        public virtual async Task<bool> RunAsync()
        {
            return true;
        }
    }
}
