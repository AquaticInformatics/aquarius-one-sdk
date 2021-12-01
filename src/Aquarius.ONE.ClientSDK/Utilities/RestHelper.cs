using ONE.Enterprise.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Utilities
{
    public class RestHelper
    {
        public RestHelper(AuthenticationApi authentication, PlatformEnvironment environment, bool continueOnCapturedContext, bool logRestfulCalls)
        {
            _authentication = authentication;
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _logRestfulCalls = logRestfulCalls;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private bool _logRestfulCalls;
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

                var response = await _authentication.Client.PostAsync(endpointURL, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";

                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.Client);
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

                var response = await _authentication.Client.PostAsync(endpointURL, form);
                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";

                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.Client);
                error.Response = (JObject)JToken.FromObject(response);


                string file = SaveRestCallData("POST-FILE", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PostRestJSONAsync Failed: {endpointURL}" });

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

                var response = await _authentication.Client.PutAsync(endpointURL, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var respContent = "";
                if (response.IsSuccessStatusCode)
                    respContent = await response.Content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext);
                error.ElapsedMs = elapsedMs;
                error.Client = (JObject)JToken.FromObject(_authentication.Client);
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
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authentication.Token.access_token);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _authentication.Client.SendAsync(request).ConfigureAwait(true);

                //var response = await _authentication.Client.PatchAsync(endpointURL, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(true);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.Client);
                error.Response = (JObject)JToken.FromObject(response);
                error.Content = json; // JObject.Parse(json);
                string file = SaveRestCallData("PATCH", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PatchRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"PatchRestJSONAsync Failed: {endpointURL}" });
                return new ServiceResponse
                {
                    ResponseMessage = response,
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

                var response = await _authentication.Client.DeleteAsync(endpointURL).ConfigureAwait(_continueOnCapturedContext);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.Client);
                error.Response = (JObject)JToken.FromObject(response);
                string file = SaveRestCallData("DELETE", error.ToString(), !response.IsSuccessStatusCode);
                if (response.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"DeleteRestJSONAsync Success: {endpointURL}" });
                else
                    Event(null, new ClientApiLoggerEventArgs { File = file, EventLevel = EnumEventLevel.Warn, HttpStatusCode = response.StatusCode, ElapsedMs = elapsedMs, Module = "RestHelper", Message = $"DeleteRestJSONAsync Failed: {endpointURL}" });
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

                var response = await _authentication.Client.GetAsync(endpointURL).ConfigureAwait(_continueOnCapturedContext);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                error.ElapsedMs = elapsedMs;

                error.Client = (JObject)JToken.FromObject(_authentication.Client);
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

    }
}
