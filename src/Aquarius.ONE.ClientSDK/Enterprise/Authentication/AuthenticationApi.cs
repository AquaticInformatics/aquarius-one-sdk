using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.Models.CSharp;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ONE.Enterprise.Authentication
{
    public class AuthenticationApi
    {
        public AuthenticationApi(PlatformEnvironment environment, bool continueOnCapturedContext, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _throwAPIErrors = throwAPIErrors;
        }

        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private readonly bool _throwAPIErrors;
        public bool AutoRenewToken { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UsePasswordGrantType { get; set; }
        public User User { get; set; }
        private HttpClient _httpAuthClient;
        private HttpClient _httpJsonClient;
        private HttpClient _httpProtocolBufferClient;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public Token Token { get; set; }
        public event EventHandler TokenExpired = delegate { };
        public string AuthenticationUrl { get => _environment?.AuthenticationUri?.AbsoluteUri; }
        public TimeSpan HttpClientTimeout = TimeSpan.FromMinutes(10);


        private HttpClient HttpAuthClient
        {
            get
            {
                if (_httpAuthClient == null)
                {
                    _httpAuthClient = new HttpClient();
                    if (_environment != null)
                        _httpAuthClient.BaseAddress = _environment.BaseUri;
                    _httpAuthClient.Timeout = HttpClientTimeout;
                    _httpAuthClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _httpAuthClient;
            }
        }

        public HttpClient HttpJsonClient
        {
            get
            {
                if (!IsAuthenticated && AutoRenewToken)
                {
                    TokenExpired(this, null);
                }

                if (_httpJsonClient == null)
                {
                    _httpJsonClient = new HttpClient();
                    if (_environment != null)
                        _httpJsonClient.BaseAddress = _environment.BaseUri;
                    _httpJsonClient.Timeout = HttpClientTimeout;
                    _httpJsonClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                }
                return _httpJsonClient;
            }
        }

        public HttpClient GetLocalHttpJsonClient(string endpointUrl)
        {
            var httpClient = new HttpClient();
            if (_environment != null)
                httpClient.BaseAddress = new Uri($"{_environment.BaseUri.OriginalString.Replace(_environment.BaseUri.Port.ToString(), GetLocalHttpPort(endpointUrl).ToString())}");
            httpClient.Timeout = HttpClientTimeout;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if ((Token == null || Token.expires < DateTime.UtcNow) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                Token = LocalLoginAsync().Result;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token?.access_token);

            return httpClient;
        }

        public HttpClient HttpProtocolBufferClient
        {
            get
            {
                if (!IsAuthenticated && AutoRenewToken)
                {
                    TokenExpired(this, null);
                }

                if (_httpProtocolBufferClient == null)
                {
                    _httpProtocolBufferClient = new HttpClient();
                    if (_environment != null)
                        _httpProtocolBufferClient.BaseAddress = _environment.BaseUri;
                    _httpProtocolBufferClient.Timeout = TimeSpan.FromMinutes(10);
                    _httpProtocolBufferClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/protobuf"));

                }
                return _httpProtocolBufferClient;
            }
        }
        public HttpClient GetLocalHttpProtocolBufferClient(string endpointUrl)
        {
            var httpClient = new HttpClient();
            
            if (_environment != null)
                httpClient.BaseAddress = new Uri($"{_environment.BaseUri.OriginalString.Replace(_environment.BaseUri.Port.ToString(), GetLocalHttpPort(endpointUrl).ToString())}");
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/protobuf"));

            if ((Token == null || Token.expires < DateTime.UtcNow) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                Token = LocalLoginAsync().Result;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token?.access_token);

            return httpClient;
        }

        private const string ActivityPath = "COMMON/ACTIVITY";
        private const string ComputationPath = "COMMON/COMPUTATION";
        private const string ConfigurationPath = "COMMON/CONFIGURATION";
        private const string LibraryPath = "COMMON/LIBRARY";
        private const string NotificationPath = "COMMON/NOTIFICATION";
        private const string SchedulePath = "COMMON/SCHEDULE";
        private const string TimezonePath = "COMMON/TIMEZONE";
        private const string TwinPath = "ENTERPRISE/TWIN";
        private const string CorePath = "ENTERPRISE/CORE";
        private const string ReportPath = "ENTERPRISE/REPORT";
        private const string SpreadsheetPath = "OPERATIONS/SPREADSHEET";
        private const string SamplePath = "OPERATIONS/SAMPLE";
        private const string HistorianDataPath = "HISTORIAN/DATA";

        public int GetLocalHttpPort(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return 0;
            uri = uri.ToUpper();
            if (uri.Contains(ActivityPath))
                return 8918;
            if (uri.Contains(ComputationPath))
                return 9006;
            if (uri.Contains(ConfigurationPath))
                return 9010;
            if (uri.Contains(LibraryPath))
                return 9201;
            if (uri.Contains(NotificationPath))
                return 9004;
            if (uri.Contains(SchedulePath))
                return 6161;
            if (uri.Contains(TimezonePath))
                return 9001;
            if (uri.Contains(TwinPath))
                return 8900;
            if (uri.Contains(CorePath))
                return 9100;
            if (uri.Contains(ReportPath))
                return 9090;
            if (uri.Contains(SpreadsheetPath))
                return 9502;
            if (uri.Contains(SamplePath))
                return 5611;
            if (uri.Contains(HistorianDataPath))
                return 8998;
            return 0;
        }

        public string GetLocalUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return "";
            
            if (uri.Contains("v1"))
                return uri.Substring(uri.IndexOf("v1", StringComparison.InvariantCultureIgnoreCase) + 2); // remove the v1 segment from the local uri
            if (uri.Contains("v2"))
                return uri.Substring(uri.IndexOf("v2", StringComparison.InvariantCultureIgnoreCase)); // keep the v2 segment in the local uri
            return "";
        }

        public void Logout()
        {
            Token = null;
            _httpJsonClient = null;
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

                using (var respContent = await HttpAuthClient.SendAsync(request).ConfigureAwait(_continueOnCapturedContext))
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
                        HttpJsonClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", Token.access_token);
                        HttpProtocolBufferClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", Token.access_token);
                        UserName = userName;
                        Password = password;
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = respContent.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "AuthenticationApi", Message = $"LoginResourceOwnerAsync Success" });
                    }
                    else
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = respContent.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "AuthenticationApi", Message = $"LoginResourceOwnerAsync Failed" });
                    }
                }
            }

            return IsAuthenticated;
        }

        private async Task<Token> LocalLoginAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            var body = new Dictionary<string, string>
            {
                { "client_id", "VSTestClient" }, { "client_secret", "0CCBB786-9412-4088-BC16-78D3A10158B7" }, { "grant_type", "password" }, { "scope", "FFAccessAPI openid" },
                { "username", UserName }, { "password", Password }
            };

            const string endpointUrl = "/connect/token";

            using (var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl))
            {
                request.Content = new FormUrlEncodedContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var localAuthClient = new HttpClient();
                localAuthClient.BaseAddress = _environment.AuthenticationUri;

                using (var respContent = await localAuthClient.SendAsync(request).ConfigureAwait(_continueOnCapturedContext))
                {
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    var json = await respContent.Content.ReadAsStringAsync();
                    if (json.Length > 0)
                        Token = JsonConvert.DeserializeObject<Token>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    var message = "LocalLoginAsync success";
                    var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

                    if (!respContent.IsSuccessStatusCode)
                    {
                        message = "LocalLoginAsync failure";
                        eventLevel = EnumOneLogLevel.OneLogLevelWarn;
                    }

                    Event(null,
                        new ClientApiLoggerEventArgs
                            { EventLevel = eventLevel, HttpStatusCode = respContent.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = message });
                }
            }

            return Token;
        }

        public async Task<string> GetUserInfoAsync()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var response = await HttpJsonClient.GetAsync("/connect/userinfo");
                var elapsedMs = watch.ElapsedMilliseconds;

                if (!response.IsSuccessStatusCode)
                {

                    dynamic error = new JObject();
                    error.Client = (JObject)JToken.FromObject(HttpJsonClient);
                    error.Response = (JObject)JToken.FromObject(response);
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetUserInfoAync Failed" });

                    return null;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetUserInfoAync Success" });

                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "AuthenticationAPI" });
                if (_throwAPIErrors)
                    throw;
                return null;
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

                using (var response = await HttpAuthClient.SendAsync(request).ConfigureAwait(_continueOnCapturedContext))
                {
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    var json = await response.Content.ReadAsStringAsync();
                    if (json.Length > 0)
                    {
                        var jObj = JObject.Parse(json);
                        if (jObj.ContainsKey(tokenKey))
                        {
                            Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetTokenAsync Success: {endpointURL}" });
                            return (string)jObj[tokenKey];
                        }
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "AuthenticationApi", Message = $"GetTokenAsync Failed: {endpointURL}" });

                    return string.Empty;
                }
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Token != null && !string.IsNullOrEmpty(Token.access_token) && Token.expires >= DateTime.Now.AddMinutes(1);
            }
        }

        public void SetToken(string accessToken, DateTime expires, string refreshToken)
        {
            Token = new Token()
            {
                access_token = accessToken,
                expires = expires,
                refresh_token = refreshToken
            };

            HttpJsonClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", Token.access_token);
            HttpProtocolBufferClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", Token.access_token);
        }

        public void SetHttpClientTimeout(TimeSpan timeout)
        {
            HttpClientTimeout = timeout;
            HttpAuthClient.Timeout = HttpClientTimeout;
            HttpJsonClient.Timeout = HttpClientTimeout;
            HttpProtocolBufferClient.Timeout = HttpClientTimeout;
        }
    }
}
