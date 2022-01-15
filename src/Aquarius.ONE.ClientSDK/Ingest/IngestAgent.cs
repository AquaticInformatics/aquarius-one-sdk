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
using System.Text;
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
        private DigitalTwin _ingestClientAgentDigitalTwin;
        private Configuration configuration;
        private DataApi _dataApi;

        public List<DigitalTwin> Telemetry { get; set; }

        public async Task<bool> IngestDataAsync(string telemetryTwinId, TimeSeriesDatas timeSeriesDatas)
        {
            var result = await _dataApi.SaveDataAsync(telemetryTwinId, timeSeriesDatas);
            return result != null;
        }
        public bool Enabled { get; set; }
        public DateTime LastRun { get; set; }

        public IngestAgent(AuthenticationApi authentificationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin ingestClientDigitalTwin)
        {
            _authentificationApi = authentificationApi;
            _coreApi = coreApi;
            _digitalTwinApi = digitalTwinApi;
            _ingestClientAgentDigitalTwin = ingestClientDigitalTwin;
            _configurationApi = configurationApi;
            _dataApi = dataApi;
        }

        public async Task<bool> LoadAsync()
        {
            var configurations = await _configurationApi.GetConfigurationsAsync(1, _ingestClientAgentDigitalTwin.TwinReferenceId);
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
                _ingestClientAgentDigitalTwin.Name = _name;
                _ingestClientAgentDigitalTwin.UpdateMask = new FieldMask { Paths = { "name" } };
                var updatedTWin = await _digitalTwinApi.UpdateAsync(_ingestClientAgentDigitalTwin);
                if (updatedTWin != null)
                    _ingestClientAgentDigitalTwin = updatedTWin;
                else
                    return false;
            }
            if (configuration != null && ConfigurationJson != configuration.ConfigurationData)
            {
                configuration.ConfigurationData = ConfigurationJson;
                configuration = await _configurationApi.SaveConfiguration(configuration);
                if (configuration == null)
                    return false;
            }
            else if (configuration == null && !string.IsNullOrEmpty(ConfigurationJson))
            {
                configuration = new Configuration();
                configuration.Name = Name;
                configuration.EnumEntity = EnumEntity.EntityFormtemplate;
                configuration.ConfigurationData = ConfigurationJson;
                configuration.FilterById = _ingestClientAgentDigitalTwin.TwinReferenceId;
                configuration = await _configurationApi.SaveConfiguration(configuration);
                if (configuration == null)
                    return false;
            }
            return true;
        }

        private string _name;

        public string Name
        {
            get
            {
                return _ingestClientAgentDigitalTwin.Name;
            }
            set
            {
                _name = value;
            }
        }

        private string _configurationJson;

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
