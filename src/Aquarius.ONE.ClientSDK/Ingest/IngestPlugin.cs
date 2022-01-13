using Common.Configuration.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using Google.Protobuf.WellKnownTypes;
using ONE.Common.Configuration;
using ONE.Enterprise.Twin;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Ingest
{
    public class IngestPlugin
    {
        private ConfigurationApi _configurationApi;
        private DigitalTwinApi _digitalTwinApi;
        private DigitalTwin _ingestClientDigitalTwin;
        private Configuration configuration;

        public List<DigitalTwin> Telemetry { get; set; }

        public IngestPlugin(DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DigitalTwin ingestClientDigitalTwin)
        {
            _digitalTwinApi = digitalTwinApi;
            _ingestClientDigitalTwin = ingestClientDigitalTwin;
            _configurationApi = configurationApi;
        }

        public async Task<bool> LoadAsync()
        {
            var configurations = await _configurationApi.GetConfigurationsAsync(1, _ingestClientDigitalTwin.TwinReferenceId);
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
                configuration.FilterById = _ingestClientDigitalTwin.TwinReferenceId;
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
                return _ingestClientDigitalTwin.Name;
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
