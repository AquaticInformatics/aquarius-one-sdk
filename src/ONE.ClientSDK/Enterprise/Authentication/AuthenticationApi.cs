using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ONE.ClientSDK.Enterprise.Authentication
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
        private HttpClient _baseHttpClient;
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
                    _httpProtocolBufferClient.Timeout = HttpClientTimeout;
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
            httpClient.Timeout = HttpClientTimeout;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/protobuf"));

            if ((Token == null || Token.expires < DateTime.UtcNow) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                Token = LocalLoginAsync().Result;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token?.access_token);

            return httpClient;
        }

        // Returns a client with the base address set and the Authorization header if authenticated
        // The base address is set to the environment's base URI or the local URI if the environment is local
        // This is the preferred method to get a client as it prevents conflicts with the Accept headers and provides a consistent base address for both remote and local environments.
        // The other clients are left in for backwards compatibility
        public HttpClient GetBaseClient(string relativeUri)
        {
            if (!IsAuthenticated && AutoRenewToken)
                TokenExpired(this, null);

            return _baseHttpClient ?? (_baseHttpClient = GetHttpClient(null, relativeUri));
        }

        private HttpClient GetHttpClient(bool? acceptProtobuf, string endpointUrl = null)
        {
            var client = new HttpClient
            {
                Timeout = HttpClientTimeout
            };

            if (_environment != null)
                client.BaseAddress = _environment.PlatformEnvironmentEnum != EnumPlatformEnvironment.Local
                    ? _environment.BaseUri
                    : new Uri($"{_environment.BaseUri.OriginalString.Replace(_environment.BaseUri.Port.ToString(), GetLocalHttpPort(endpointUrl).ToString())}");

            if (acceptProtobuf.HasValue)
                client.DefaultRequestHeaders.Accept.Add(acceptProtobuf.Value
                    ? new MediaTypeWithQualityHeaderValue("application/protobuf")
                    : new MediaTypeWithQualityHeaderValue("application/json"));

            if (IsAuthenticated)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.access_token);

            return client;
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

        public async Task<bool> LoginAsync(string userName = null, string password = null)
        {
            try
            {
                // Temporary start. For troubleshooting login issue. Do not commit to code base.
                LogRequestData(null, 0, userName, password);
                // Temporary end

                if (userName == null)
                    userName = UserName;
                if (password == null)
                    password = Password;

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    return false;

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

                        // Temporary start. For troubleshooting login issue. Do not commit to code base.
                        LogRequestData(respContent, watch.ElapsedMilliseconds, userName, password);
                        // Temporary end

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
            catch (Exception ex)
            {
                // Temporary start. For troubleshooting login issue. Do not commit to code base.
                LogRequestData(null, 0, userName, password, ex);
                // Temporary end
                return false;
            }
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

        // Temporary method for troubleshooting login issues, should not be committed to code base
        private void LogRequestData(HttpResponseMessage response, long elapsedMs,
            string userName, string password, Exception ex = null)
        {
            try
            {
                var status = response == null
                    ? "INFO"
                    : response.IsSuccessStatusCode ? "Success" : "Error";

                var filename = $"AUTH {status} - {DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.json";
                var dir = AppContext.BaseDirectory;
                if (string.IsNullOrEmpty(dir))
                    dir = Path.Combine(Path.GetTempPath(), "WIMSSync");

                dir = Path.Combine(dir, $"Logs\\{DateTime.Now:yyyy-MM-dd}");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var logFile = Path.Combine(dir, filename);

                //serialize
                dynamic content = new JObject();
                content.ElapsedMs = elapsedMs;
                content.UserName = userName;
                content.Password = password;
                content.Client = (JObject)JToken.FromObject(HttpAuthClient);
                if (response != null)
                    content.Response = (JObject)JToken.FromObject(response);
                if (ex != null)
                    content.Exception = (JObject)JToken.FromObject(ex);

                File.WriteAllText(logFile, content.ToString());

                Event(this, new ClientApiLoggerEventArgs { File = logFile, EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "AuthenticationApi", Message = "LogRequestData Succeeded" });
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "AuthenticationApi", Message = $"LogRequestData Failed - {e.Message}" });
            }
        }
    }
}
