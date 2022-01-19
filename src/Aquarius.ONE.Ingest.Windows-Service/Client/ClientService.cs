using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONE.Ingest.WindowsService.Agents.Test;
using ONE.Utilities;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Client
{
    public sealed class ClientService : BackgroundService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly ClientSDK _clientSDK;
        private AgentService _testAgentService;
        public ClientService(
            AgentService testAgentService,
            ILogger<ClientService> logger, ClientSDK clientSDK) =>
            (_testAgentService, _logger, _clientSDK) = (testAgentService, logger, clientSDK);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // Initialize Configuration

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSettings = configuration.AppSettings;
            bool isAppSettingsDirty = false;
            if (appSettings.Settings["Username"] == null)
            {
                appSettings.Settings.Add(new KeyValueConfigurationElement("Username", ""));
                isAppSettingsDirty = true;
            }
            if (appSettings.Settings["Password"] == null)
            { 
                appSettings.Settings.Add(new KeyValueConfigurationElement("Password", ""));
                isAppSettingsDirty = true;
            }
            if (appSettings.Settings["Environment"] == null)
            {
                appSettings.Settings.Add(new KeyValueConfigurationElement("Environment", PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature).Name));
                isAppSettingsDirty = true;
            }
            if (appSettings.Settings["ClientId"] == null)
            {
                appSettings.Settings.Add(new KeyValueConfigurationElement("ClientId", ""));
                isAppSettingsDirty = true;
            }
            if (isAppSettingsDirty)
                configuration.Save();
            _clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(appSettings.Settings["Environment"].Value);

            // Authenticate with ONE
            await _clientSDK.Authentication.LoginResourceOwnerAsync(appSettings.Settings["Username"].Value, appSettings.Settings["Password"].Value);

            if (_clientSDK.Authentication.IsAuthenticated)
            {
                // Check to see if there is a client configured

                IngestClient ingestClient;

                if (string.IsNullOrEmpty(appSettings.Settings["ClientId"].Value))
                {
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                    {
                        appSettings.Settings["ClientId"].Value = ingestClient.Id;
                        configuration.Save();
                    }
                }
                else
                {
                    ingestClient = await _clientSDK.Ingest.GetClientByIdAsync(appSettings.Settings["ClientId"].Value);
                }

                if (ingestClient == null)
                { 
                    appSettings.Settings["ClientId"].Value = "";
                    configuration.Save();
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                    {
                        appSettings.Settings["ClientId"].Value = ingestClient.Id;
                        configuration.Save();
                    }
                }
                if (ingestClient != null)
                { 
                    await ingestClient.LoadAsync();
                    if (ingestClient.Agents == null || ingestClient.Agents.Count == 0)
                    {
                        await ingestClient.RegisterAgentAsync(_testAgentService, ingestClient.Id, "Test Agent", Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentTest.RefId);
                    }
                    else
                    {
                        foreach (var agent in ingestClient.Agents)
                        {
                            await agent.LoadAsync();
                        }
                    }
                    if (ingestClient.Agents != null && ingestClient.Agents.Count > 0)
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            try
                            {
                                foreach(var ingestAgent in ingestClient.Agents)
                                {
                                    if (ingestAgent.IsTimeToRun)
                                    {
                                        bool success = await ingestAgent.RunAsync();
                                    }
                                    if (ingestAgent.IsTimeToUpload)
                                    {
                                        bool succcess = await ingestAgent.UploadAsync();
                                    }
                                }
                                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
