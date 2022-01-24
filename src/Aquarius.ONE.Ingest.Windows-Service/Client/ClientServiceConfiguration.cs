using ONE.Utilities;
using System.Configuration;

namespace ONE.Ingest.WindowsService.Client
{
    public class ClientServiceConfiguration
    {
        private Configuration _configuration;
        private AppSettingsSection _appSettings;
        private bool _isAppSettingsDirty = false;
        public ClientServiceConfiguration()
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _appSettings = _configuration.AppSettings;
            if (_appSettings.Settings["Username"] == null)
            {
                _appSettings.Settings.Add(new KeyValueConfigurationElement("Username", ""));
                _isAppSettingsDirty = true;
            }
            if (_appSettings.Settings["Password"] == null)
            {
                _appSettings.Settings.Add(new KeyValueConfigurationElement("Password", ""));
                _isAppSettingsDirty = true;
            }
            if (_appSettings.Settings["Environment"] == null)
            {
                _appSettings.Settings.Add(new KeyValueConfigurationElement("Environment", PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature).Name));
                _isAppSettingsDirty = true;
            }
            if (_appSettings.Settings["ClientId"] == null)
            {
                _appSettings.Settings.Add(new KeyValueConfigurationElement("ClientId", ""));
                _isAppSettingsDirty = true;
            }
            if (_isAppSettingsDirty)
                _configuration.Save();
        }
        public string UserName
        {
            get
            {
                return _appSettings.Settings["Username"].Value;
            }
            set
            {
                if (_appSettings.Settings["Username"].Value != value)
                    _isAppSettingsDirty = true;
                _appSettings.Settings["Username"].Value = value;
                Save();
            }
        }
        public string Password
        {
            get
            {
                return _appSettings.Settings["Password"].Value;
            }
            set
            {
                if (_appSettings.Settings["Password"].Value != value)
                    _isAppSettingsDirty = true;
                _appSettings.Settings["Password"].Value = value;
                Save();
            }
        }
        public string Environment 
        { 
            get
            {
                return _appSettings.Settings["Environment"].Value;
            }
            set
            {
                if (_appSettings.Settings["Environment"].Value != value)
                    _isAppSettingsDirty = true;
                _appSettings.Settings["Environment"].Value = value;
                Save();
            }
        }
        public string ClientId
        {
            get
            {
                return _appSettings.Settings["ClientId"].Value;
            }
            set
            {
                if (_appSettings.Settings["ClientId"].Value != value)
                    _isAppSettingsDirty = true;
                _appSettings.Settings["ClientId"].Value = value;
                Save();
            }
        }
        public void Save()
        {
            if (_isAppSettingsDirty)
                _configuration.Save();  
        }
    }
}
