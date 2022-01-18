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

        public List<IngestAgent> Agents { get; set; }

        public IngestLogger Logger { get; set; }

        public async Task<IngestAgent> RegisterAgentAsync(IngestAgent ingestAgent, string ingestClientId, string ingestAgentName, string agentSubTypeId)
        {
            bool success = await ingestAgent.InitializeAsync(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, ingestClientId, ingestAgentName, agentSubTypeId);
            if (success)
            {
                Agents.Add(ingestAgent);

                return ingestAgent;
            }
            return null;
        }
        public async Task<bool> LoadAsync()
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
            
            var configurations = await _configurationApi.GetConfigurationsAsync(1, _ingestClientDigitalTwin.TwinReferenceId);
            if (configurations != null && configurations.Count > 0)
            {
                configuration = configurations[0];
                ConfigurationJson = configuration.ConfigurationData;
            }
            var loggers = await _digitalTwinApi.GetDescendantsBySubTypeAsync(_ingestClientDigitalTwin.TwinReferenceId, ONE.Enterprise.Twin.Constants.TelemetryCategory.HistorianType.Logger.RefId);
            foreach (var logger in loggers)
            {
                if (logger.ParentTwinReferenceId == _ingestClientDigitalTwin.TwinReferenceId)
                {
                    Logger = new IngestLogger(_authentificationApi, _digitalTwinApi, _dataApi, logger);
                    break;
                }
            }
            if (Logger == null)
                Logger = await IngestLogger.InitializeAsync(_authentificationApi, _digitalTwinApi, _dataApi, _ingestClientDigitalTwin.TwinReferenceId, Name + " Logger");
            var pluginTwins = await _digitalTwinApi.GetDescendantsByTypeAsync(_ingestClientDigitalTwin.TwinReferenceId, ONE.Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.RefId);

            if (pluginTwins != null)
            {
                foreach (var pluginTwin in pluginTwins)
                {
                    var plugin = new IngestAgent(_authentificationApi, _coreApi, _digitalTwinApi, _configurationApi, _dataApi, pluginTwin);
                    await plugin.LoadAsync();
                    Agents.Add(plugin);
                }
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
        public string Id
        {
            get
            {
                return _ingestClientDigitalTwin.TwinReferenceId;
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
