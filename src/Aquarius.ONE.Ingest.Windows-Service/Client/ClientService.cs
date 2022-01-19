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
            ClientServiceConfiguration clientServiceConfiguration = new ClientServiceConfiguration();

            _clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(clientServiceConfiguration.Environment);

            // Authenticate with ONE
            await _clientSDK.Authentication.LoginResourceOwnerAsync(clientServiceConfiguration.UserName, clientServiceConfiguration.Password);

            if (_clientSDK.Authentication.IsAuthenticated)
            {
                // Check to see if there is a client configured

                IngestClient ingestClient;

                if (string.IsNullOrEmpty(clientServiceConfiguration.ClientId))
                {
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                }
                else
                    ingestClient = await _clientSDK.Ingest.GetClientByIdAsync(clientServiceConfiguration.ClientId);

                if (ingestClient == null)
                {
                    clientServiceConfiguration.ClientId = "";
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                }
                if (ingestClient != null)
                { 
                    await ingestClient.LoadAsync();
                    if (ingestClient.Agents == null || ingestClient.Agents.Count == 0)
                        await ingestClient.RegisterAgentAsync(_testAgentService, "Test Agent", Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentTest.RefId);
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
