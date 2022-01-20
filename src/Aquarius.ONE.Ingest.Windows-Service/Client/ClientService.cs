using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Test = ONE.Ingest.WindowsService.Agents.Test;
using Csv = ONE.Ingest.WindowsService.Agents.CSV;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Client
{
    public sealed class ClientService : BackgroundService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly ClientSDK _clientSDK;
        private Test.AgentService _testAgentService;
        private Csv.AgentService _csvAgentService;
        public ClientService(
            Test.AgentService testAgentService,
            Csv.AgentService csvAgentService,
            ILogger<ClientService> logger, 
            ClientSDK clientSDK) =>
            (_testAgentService, _csvAgentService, _logger, _clientSDK) = (testAgentService, csvAgentService, logger, clientSDK);

     
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // Initialize Configuration
            ClientServiceConfiguration clientServiceConfiguration = new ClientServiceConfiguration();

            _clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(clientServiceConfiguration.Environment);

            // Authenticate with ONE
            await _clientSDK.Authentication.LoginResourceOwnerAsync(clientServiceConfiguration.UserName, clientServiceConfiguration.Password);

            if (!_clientSDK.Authentication.IsAuthenticated)
                _logger.LogCritical($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Unable to Authenticate.  Shutting Service Down.  Check the Aquarius.ONE.Ingest.Windows-Service.dll.config");
            else
            {
                _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Service Authenticated.");
                // Check to see if there is a client configured

                IngestClient ingestClient;

                if (string.IsNullOrEmpty(clientServiceConfiguration.ClientId))
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Registering New Client: {Environment.MachineName}");
                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                    {
                        ingestClient.Logger.IngestLogData($"Registering New Client: { Environment.MachineName}");
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                    }
                }
                else
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Loading Client: {clientServiceConfiguration.ClientId}");
                    ingestClient = await _clientSDK.Ingest.GetClientByIdAsync(clientServiceConfiguration.ClientId);
                }

                if (ingestClient == null)
                {
                    clientServiceConfiguration.ClientId = "";
                    _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Registering New Client: {Environment.MachineName}");

                    ingestClient = await _clientSDK.Ingest.RegisterClientAsync(Environment.MachineName);
                    if (ingestClient != null)
                    {
                        clientServiceConfiguration.ClientId = ingestClient.Id;
                    }
                }
                if (ingestClient != null)
                {
                    _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Loading Client Configuration: {Environment.MachineName}");
                    await ingestClient.LoadAsync(new List<IngestAgent> { _testAgentService, _csvAgentService });
                    ingestClient.Logger.IngestLogData($"Loading Client Configuration: {Environment.MachineName}");
                    if (ingestClient.Agents == null || ingestClient.Agents.Count == 0)
                    {
                        string agentName = "Test Agent";
                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Registering New Agent: {agentName}");
                        ingestClient.Logger.IngestLogData($"Registering New Agent: {agentName}");
                        await ingestClient.RegisterAgentAsync(_testAgentService, agentName, Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentTest.RefId);

                        agentName = "CSV Agent";
                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Registering New Agent: {agentName}");
                        ingestClient.Logger.IngestLogData($"Registering New Agent: {agentName}");
                        await ingestClient.RegisterAgentAsync(_csvAgentService, agentName, Enterprise.Twin.Constants.IntrumentCategory.ClientIngestAgentType.ClientIngestAgentCsv.RefId);
                    }
                    if (ingestClient.Agents != null && ingestClient.Agents.Count > 0)
                    {
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Checking for Agent Activity");

                            try
                            {
                                foreach (var ingestAgent in ingestClient.Agents)
                                {
                                    if (ingestAgent.IsTimeToRun)
                                    {
                                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Running Agent: {ingestAgent.Name}");
                                        bool success = await ingestAgent.RunAsync();
                                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Running Agent Complete: {ingestAgent.Name}");

                                    }
                                    if (ingestAgent.IsTimeToUpload)
                                    {
                                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Uploading Agent Data: {ingestAgent.Name}");
                                        bool succcess = await ingestAgent.UploadAsync();
                                        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}: Uploading Agent Data Complete: {ingestAgent.Name}");
                                        ingestAgent.Logger.IngestLogData("Uploading Agent Data Complete");
                                    }
                                    ingestAgent.Logger.UploadAsync();
                                }
                                ingestClient.Logger.UploadAsync();

                                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }
                        _logger.LogInformation("Shutting Down");
                    }
                }
            }
        }
    }
}
