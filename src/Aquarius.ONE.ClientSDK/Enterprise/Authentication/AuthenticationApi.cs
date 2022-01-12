using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ONE.Utilities;
using Newtonsoft.Json;
using Enterprise.Core.Protobuf.Models;

namespace ONE.Enterprise.Authentication
{
    public class AuthenticationApi
    {
        public AuthenticationApi(PlatformEnvironment environment, bool continueOnCapturedContext)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        public string UserName { get; set; }

        public string Password { get; set; }
        public User User { get; set; }

        private HttpClient _client;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public Token Token { get; set; }
        public HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    if (_environment != null)
                        _client.BaseAddress = _environment.BaseUri;
                    _client.Timeout = TimeSpan.FromMinutes(10);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                }
                return _client;
            }
        }
   
        public void Logout()
        {
            Token = null;
            _client = null;
        }

        public async Task<bool> LoginResourceOwnerAsync(string userName, string password)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Token = new Token();
            var body = new Dictionary<string, string>
            {
                {
                    "client_id",
                    "VSTestClient"
                },
                {
                    "client_secret",
                    "0CCBB786-9412-4088-BC16-78D3A10158B7"
                },
                {
                    "grant_type",
                    "password"
                },
                {
                    "scope",
                    "FFAccessAPI openid"
                },
                {
                    "username",
                    userName
                },
                {
                    "password",
                    password
                }
            };
            string endpointURL = "/connect/token";
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpointURL))
            {
                request.Content = new FormUrlEncodedContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                using (var respContent = await Client.SendAsync(request).ConfigureAwait(_continueOnCapturedContext))
                {
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    var json = await respContent.Content.ReadAsStringAsync();
                    if (json.Length > 0)
                    {
                        Token = JsonConvert.DeserializeObject<Token>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }

                    if (Token != null && !string.IsNullOrEmpty(Token.access_token))
                    {
                        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", Token.access_token);
                        UserName = userName;
                        Password = password;
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "AuthenticationApi", Message = $"LoginResourceOwnerAsync Success" });
                    }
                    else
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "AuthenticationApi", Message = $"LoginResourceOwnerAsync Failed" });
                    }
                }
            }

            return IsAuthenticated;
        }
        public async Task<string> GetUserInfoAsync()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var response = await Client.GetAsync("/connect/userinfo");
                var elapsedMs = watch.ElapsedMilliseconds;

                if (!response.IsSuccessStatusCode)
                {

                    dynamic error = new JObject();
                    error.Client = (JObject)JToken.FromObject(Client);
                    error.Response = (JObject)JToken.FromObject(response);
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetUserInfoAync Failed" });

                    return null;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetUserInfoAync Success" });

                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "AuthenticationAPI" });
                throw;
            }
        }

        public async Task<string> GetTokenAsync(string userName, string password)
        {
            var body = new Dictionary<string, string>
            {
                {
                    "client_id",
                    "VSTestClient"
                },
                {
                    "client_secret",
                    "0CCBB786-9412-4088-BC16-78D3A10158B7"
                },
                {
                    "grant_type",
                    "password"
                },
                {
                    "scope",
                    "FFAccessAPI openid"
                },
                {
                    "username",
                    userName
                },
                {
                    "password",
                    password
                }
            };
            string endpointURL = "/connect/token";
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpointURL))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                const string tokenKey = "access_token";

                request.Content = new FormUrlEncodedContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                using (var response = await Client.SendAsync(request).ConfigureAwait(_continueOnCapturedContext))
                {
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    var json = await response.Content.ReadAsStringAsync();
                    if (json.Length > 0)
                    {
                        var jObj = JObject.Parse(json);
                        if (jObj.ContainsKey(tokenKey))
                        {
                            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetTokenAsync Success: {endpointURL}" });
                            return (string)jObj[tokenKey];
                        }
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetTokenAsync Failed: {endpointURL}" });

                    return string.Empty;
                }
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Token != null && !string.IsNullOrEmpty(Token.access_token) && Token.expires >= DateTime.Now;
            }
        }
    }
}
