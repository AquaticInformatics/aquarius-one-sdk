using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ONE.Ingest.WindowsService.Agents.Joke
{
    public class JokeAgentService
    {
        private readonly ClientSDK _clientSDK;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private const string JokeApiUrl =
            "https://karljoke.herokuapp.com/jokes/programming/random";

        public JokeAgentService(HttpClient httpClient, ClientSDK clientSDK) => (_httpClient, _clientSDK) = (httpClient, clientSDK);

        public string GetAccessToken()
        {
            return _clientSDK.Authentication.Token.access_token;
        }

        public async Task<string> GetJokeAsync()
        {
            try
            {
                // The API returns an array with a single entry.
                Joke[]? jokes = await _httpClient.GetFromJsonAsync<Joke[]>(
                    JokeApiUrl, _options);

                Joke? joke = jokes?[0];

                return joke is not null
                    ? $"{joke.Setup}{Environment.NewLine}{joke.Punchline}"
                    : "No joke here...";
            }
            catch (Exception ex)
            {
                return $"That's not funny! {ex}";
            }
        }
    }

    public record Joke(int Id, string Type, string Setup, string Punchline);
}
