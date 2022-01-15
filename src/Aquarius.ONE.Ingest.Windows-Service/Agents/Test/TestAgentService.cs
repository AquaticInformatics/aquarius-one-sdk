using Microsoft.Extensions.Logging;
using ONE.Ingest.WindowsService.Client;
using System;
using System.Text.Json;

namespace ONE.Ingest.WindowsService.Agents.Test
{
    public sealed class TestAgentService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly ClientSDK _clientSDK;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TestAgentService(ILogger<ClientService> logger, ClientSDK clientSDK) => (_logger, _clientSDK) = (logger, clientSDK);

        public bool Ingest()
        {
            try
            {

            }
            catch (Exception ex)
            {
                //return $"That's not funny! {ex}";
            }

            throw new NotImplementedException();
        }
    }

}
