using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using Enterprise.Report.Protobuf.Models;
using Common.Core.Protobuf.Models;

namespace ONE.Enterprise.Report
{
    public class ReportApi
    {
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public ReportApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
        }
        public async Task<List<ReportDefinition>> GetDefinitionsAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinition> definitions = new List<ReportDefinition>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/definitions?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        definitions.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetDefinitionsAsync Success" });
                    return definitions;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetDefinitionsAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"GetDefinitionsAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> GetDefinitionAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinition> definitions = new List<ReportDefinition>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/definitions/{id}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetDefinitionsAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetDefinitionsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"GetDefinitionsAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> CreateDefinitionAsync(ReportDefinition reportDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/definitions?requestId={requestId}";

            var json = JsonConvert.SerializeObject(reportDefinition);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"CreateDefinitionAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"CreateDefinitionAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"CreateDefinitionAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteDefinitionAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/report/v1/definitions/{id}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DeleteDefinitionAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DeleteDefinitionAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"DeleteDefinitionAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> UpdateDefinitionAsync(ReportDefinition reportDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/definitions/{reportDefinition.Id}?requestId={requestId}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(reportDefinition, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json.ToString(), endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UpdateDefinitionAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UpdateDefinitionAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"UpdateDefinitionAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> UploadDefinitionTemplateAsync(string id, string filename)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/definitions/upload/{id}?requestId={requestId}";


            try
            {
                var respContent = await _restHelper.UploadFileAsync(requestId, filename, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UploadDefinitionTemplateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UploadDefinitionTemplateAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"UploadDefinitionTemplateAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinitionRun> RenderReportAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/report/render/{id}?requestId={requestId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitionRuns.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"RenderReportAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"RenderReportAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"RenderReportAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> DownloadReportAsync(string id, string filename)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinition> definitions = new List<ReportDefinition>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/report/output/{id}/report?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    using (var fs = new FileStream(filename, FileMode.CreateNew))
                    {
                        await respContent.ResponseMessage.Content.CopyToAsync(fs);
                    }
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DownloadReportAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DownloadReportAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"DownloadReportAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> DownloadTemplateAsync(string id, string filename)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinition> definitions = new List<ReportDefinition>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/report/output/{id}/template?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    using (var fs = new FileStream(filename, FileMode.CreateNew))
                    {
                        await respContent.ResponseMessage.Content.CopyToAsync(fs);
                    }
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DownloadReportAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DownloadReportAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"DownloadReportAsync Failed - {e.Message}" });
                throw;
            }
        }




        public async Task<List<ReportDefinitionTag>> GetReportTagsAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinitionTag> tags = new List<ReportDefinitionTag>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/report/tags?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitionTags.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        tags.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetReportTagsAsync Success" });
                    return tags;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetReportTagsAsync Failed" });
                    return null;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"GetReportTagsAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinitionTag> GetReportTagAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<ReportDefinitionTag> tags = new List<ReportDefinitionTag>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/report/v1/report/tags/{id}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitionTags.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetReportTagAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"GetReportTagAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"GetReportTagAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinition> CreateReportTagAsync(ReportDefinition reportDefinition)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/report/tags?requestId={requestId}";

            var json = JsonConvert.SerializeObject(reportDefinition);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitions.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"CreateReportTagAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"CreateReportTagAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"CreateReportTagAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<bool> DeleteReportTagAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/report/v1/report/tags/{id}?requestId={requestId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DeleteReportTagAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"DeleteReportTagAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"DeleteReportTagAsync Failed - {e.Message}" });
                throw;
            }
        }
        public async Task<ReportDefinitionTag> UpdateReportDefinitionTagAsync(ReportDefinitionTag reportDefinitionTag)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/report/v1/report/tags/{reportDefinitionTag.Id}?requestId={requestId}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(reportDefinitionTag, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json.ToString(), endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ReportDefinitionTags.Items.Distinct().ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UpdateReportDefinitionTagAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "ReportApi", Message = $"UpdateReportDefinitionTagAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "ReportApi", Message = $"UpdateReportDefinitionTagAsync Failed - {e.Message}" });
                throw;
            }
        }


    }
}
