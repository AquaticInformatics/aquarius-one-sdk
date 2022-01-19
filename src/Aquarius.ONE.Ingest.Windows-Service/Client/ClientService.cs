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

            if (!_clientSDK.Authentication.IsAuthenticated)
                _logger.LogCritical("Unable to Authenticate.  Shutting Service Down.  Check the Aquarius.ONE.Ingest.Windows-Service.dll.config");
            else
            {
                _logger.LogInformation("Service Authenticated.");
                // Check to see if there is a client configured

                IngestClient ingestClient;

                if (string.IsNullOrEmpty(clientServiceConfiguration.ClientId))
                {
                    _logger.LogInformation($"Registering New Client: {Environment.MachineName}");
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                }
                else
                {
                    _logger.LogInformation($"Loading Client: {clientServiceConfiguration.ClientId}");
                    ingestClient = await _clientSDK.Ingest.GetClientByIdAsync(clientServiceConfiguration.ClientId);
                }

                if (ingestClient == null)
                {
                    clientServiceConfiguration.ClientId = "";
                    _logger.LogInformation($"Registering New Client: {Environment.MachineName}");

                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                }
                if (ingestClient != null)
                {
                    _logger.LogInformation($"Loading Client Configuration: {Environment.MachineName}");

                    await ingestClient.LoadAsync();
                    if (ingestClient.Agents == null || ingestClient.Agents.Count == 0)
                    {
                        string agentName = "Test Agent";
                        _logger.LogInformation($"Registering New Agent: {agentName}");

                        await ingestClient.RegisterAgentAsync(_testAgentService, agentName, Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentTest.RefId);
                    }
                    else
                    {
                        foreach (var agent in ingestClient.Agents)
                        {
                            _logger.LogInformation($"Loading Agent: {agent.Name}");

                            await agent.LoadAsync();
                        }
                    }
                    if (ingestClient.Agents != null && ingestClient.Agents.Count > 0)
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            try
                            {
                                foreach (var ingestAgent in ingestClient.Agents)
                                {
                                    if (ingestAgent.IsTimeToRun)
                                    {
                                        _logger.LogInformation($"Running Agent: {ingestAgent.Name}");

                                        bool success = await ingestAgent.RunAsync();
                                    }
                                    if (ingestAgent.IsTimeToUpload)
                                    {
                                        _logger.LogInformation($"Uploading Agent Data: {ingestAgent.Name}");

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
