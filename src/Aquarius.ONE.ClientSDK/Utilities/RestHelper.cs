using ONE.Enterprise.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ONE.Models.CSharp;

namespace ONE.Utilities
{
    public class RestHelper
    {
        public RestHelper(AuthenticationApi authentication, PlatformEnvironment environment, bool continueOnCapturedContext, bool logRestfulCalls, bool throwAPIErrors)
        {
            _authentication = authentication;
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _logRestfulCalls = logRestfulCalls;
            _throwAPIErrors = throwAPIErrors;

        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private bool _logRestfulCalls;
        private bool _throwAPIErrors;
        private AuthenticationApi _authentication;

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public async Task<ServiceResponse> PostRestJSONAsync(
           Guid requestId,
           string json,
           string endpointURL)
        {
            dynamic error = new JObject();

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).PostAsync(_authentication.GetLocalUri(endpointURL), new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpJsonClient.PostAsync(endpointURL, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";

                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);
                if (string.IsNullOrEmpty(json))
                {
                    error.Content = "";
                }
                else
                {
                    error.Content = JObject.Parse(json);
                }

                string file = SaveRestCallData("POST", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Failed: {endpointURL}" });

                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        Result = respContent,
                        ElapsedMs = elapsedMs
                    });
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    Result = respContent,
                    ElapsedMs = elapsedMs
                };
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"PostRestJSONAsync Failed - {e.Message}" });
                error.Exception = (JObject)JToken.FromObject(e);
                SaveRestCallData("POST", error.ToString(), true);
                throw;
            }
        }
        public async Task<ServiceResponse> UploadFileAsync(Guid requestId,
           string filePath,
           string endpointURL)
        {
            dynamic error = new JObject();

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentNullException(nameof(filePath));
                }

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File [{filePath}] not found.");
                }
                var form = new MultipartFormDataContent();
                byte[] result;
                using (FileStream stream = File.Open(filePath, FileMode.Open))
                {
                    result = new byte[stream.Length];
                    await stream.ReadAsync(result, 0, (int)stream.Length);
                }
                var fileContent = new ByteArrayContent(result);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "file", Path.GetFileName(filePath));
                //form.Add(new StringContent("789"), "userId");
                //form.Add(new StringContent("some comments"), "comment");
                //form.Add(new StringContent("true"), "isPrimary");

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).PostAsync(_authentication.GetLocalUri(endpointURL), form).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpJsonClient.PostAsync(endpointURL, form);
                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";

                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);


                string file = SaveRestCallData("POST-FILE", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        Result = respContent,
                        ElapsedMs = elapsedMs
                    });
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    Result = respContent,
                    ElapsedMs = elapsedMs
                };

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"UploadFileAsync Failed - {e.Message}" });
                error.Exception = (JObject)JToken.FromObject(e);
                SaveRestCallData("POST", error.ToString(), true);
                throw;
            }
        }


        public async Task<ServiceResponse> PutRestJSONAsync(
           Guid requestId,
           string json,
           string endpointURL)
        {
            dynamic error = new JObject();

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).PutAsync(_authentication.GetLocalUri(endpointURL), new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpJsonClient.PutAsync(endpointURL, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";
                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);
                if (string.IsNullOrEmpty(json))
                {
                    error.Content = "";
                }
                else
                {
                    error.Content = JObject.Parse(json);
                }

                string file = SaveRestCallData("PUT", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PutRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PutRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        Result = respContent,
                        ElapsedMs = elapsedMs
                    });
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    Result = respContent,
                    ElapsedMs = elapsedMs
                };

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"PostRestJSONAsync Failed - {e.Message}" });
                error.Exception = (JObject)JToken.FromObject(e);
                SaveRestCallData("POST", error.ToString(), true);
                throw;
            }
        }
        public async Task<ServiceResponse> PatchRestJSONAsync(
            Guid requestId,
            string json,
            string endpointURL)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            dynamic error = new JObject();

            try
            {
                //  For Framework Only
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpointURL);
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    request = new HttpRequestMessage(new HttpMethod("PATCH"), _authentication.GetLocalUri(endpointURL));
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authentication.Token.access_token);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).SendAsync(request).ConfigureAwait(true);
                else
                    response = await _authentication.HttpJsonClient.SendAsync(request).ConfigureAwait(true);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);
                error.Content = json; // JObject.Parse(json);
                string file = SaveRestCallData("PATCH", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PatchRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PatchRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        Result = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext) : null,
                        ElapsedMs = elapsedMs
                    }); 
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    Result = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext) : null,
                    ElapsedMs = elapsedMs
                };
            }
            catch (Exception e)
            {
                error.Exception = (JObject)JToken.FromObject(e);

                SaveRestCallData("PATCH", error.ToString(), true);
                throw;
            }
        }
        public async Task<ServiceResponse> DeleteRestJSONAsync(
            Guid requestId,
            string endpointURL)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            dynamic error = new JObject();

            try
            {

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).DeleteAsync(_authentication.GetLocalUri(endpointURL)).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpJsonClient.DeleteAsync(endpointURL).ConfigureAwait(_continueOnCapturedContext);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);
                string file = SaveRestCallData("DELETE", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"DeleteRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"DeleteRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        ElapsedMs = elapsedMs
                    }); 
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    ElapsedMs = elapsedMs
                };
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"DeleteRestJSONAsync Failed - {e.Message}" });
                error.Exception = (JObject)JToken.FromObject(e);
                SaveRestCallData("DELETE", error.ToString(), true);
                throw;
            }
        }
        public string SaveRestCallData(string prefix, string content, bool error)
        {
            if (!_logRestfulCalls)
                return "";
            try
            {
                string status = "Success";
                if (error)
                    status = "Error";
                string filename = $"{prefix} {status} - {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff")}.json";
                var dir = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
                dir = Path.Combine(dir, "Logs");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                dir = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd"));

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string logFile = Path.Combine(dir, filename);
                //serialize
                File.WriteAllText(logFile, content);
                return logFile;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"SaveRestCallData Failed - {e.Message}" });

                return "";
            }
        }
        public async Task<ServiceResponse> GetRestJSONAsync(
            Guid requestId,
            string endpointURL)
        {
            dynamic error = new JObject();

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpJsonClient(endpointURL).GetAsync(_authentication.GetLocalUri(endpointURL)).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpJsonClient.GetAsync(endpointURL).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.HttpJsonClient);
                error.Response = (JObject)JToken.FromObject(response);
                error.Content = "";

                string file = SaveRestCallData("GET", error.ToString(), !response.IsSuccessStatusCode);

                var respContent = "";
                if (response.IsSuccessStatusCode)
                {
                    respContent = await response.Content.ReadAsStringAsync();
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"GetRestJSONAsync Success: {endpointURL}" });
                }
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"GetRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        Result = respContent,
                        ElapsedMs = elapsedMs
                    }); 
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    Result = respContent,
                    ElapsedMs = elapsedMs
                };
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"GetRestJSONAsync Failed - {e.Message}" });

                error.Exception = (JObject)JToken.FromObject(e);

                SaveRestCallData("GET", error.ToString(), true);
                throw;
            }

        }

        public async Task<ServiceResponse> GetRestProtocolBufferAsync(
            Guid requestId,
            string endpointURL)
        {
            dynamic error = new JObject();

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                HttpResponseMessage response = null;
                if (_environment.PlatformEnvironmentEnum == EnumPlatformEnvironment.Local)
                    response = await _authentication.GetLocalHttpProtocolBufferClient(endpointURL).GetAsync(_authentication.GetLocalUri(endpointURL)).ConfigureAwait(_continueOnCapturedContext);
                else
                    response = await _authentication.HttpProtocolBufferClient.GetAsync(endpointURL).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.HttpProtocolBufferClient);
                error.Response = (JObject)JToken.FromObject(response);
                error.Content = "";

                string file = SaveRestCallData("GET", error.ToString(), !response.IsSuccessStatusCode);

                ApiResponse apiResponse = null;
                if (response.IsSuccessStatusCode)
                {
                    byte[] respContent = await response.Content.ReadAsByteArrayAsync();
                    apiResponse = ApiResponse.Parser.ParseFrom(respContent);

                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"GetRestJSONAsync Success: {endpointURL}" });
                }
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"GetRestJSONAsync Failed: {endpointURL}" });
                if (_throwAPIErrors && !response.IsSuccessStatusCode)
                    throw ExceptionHelper.GetException(new ServiceResponse
                    {
                        ResponseMessage = response,
                        ApiResponse = apiResponse,
                        ElapsedMs = elapsedMs
                    }); 
                return new ServiceResponse
                {
                    ResponseMessage = response,
                    ApiResponse = apiResponse,
                    ElapsedMs = elapsedMs
                };
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "RestHelper", Message = $"GetRestJSONAsync Failed - {e.Message}" });

                error.Exception = (JObject)JToken.FromObject(e);

                SaveRestCallData("GET", error.ToString(), true);
                throw;
            }

        }

    }
}
