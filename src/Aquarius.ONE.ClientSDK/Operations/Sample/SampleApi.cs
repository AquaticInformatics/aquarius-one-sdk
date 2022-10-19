using System;
using System.Collections.Generic;
using ONE.Utilities;
using System.Threading.Tasks;
using System.Net;
using ONE.Models.CSharp;

namespace ONE.Operations.Sample
{
    public class SampleApi
    {
        private readonly PlatformEnvironment _environment;
        private readonly bool _continueOnCapturedContext;
        private readonly RestHelper _restHelper;
        private readonly ClientSDK _clientSdk;
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        public SampleApi(PlatformEnvironment environment, bool continueOnCapturedContext,
            RestHelper restHelper, ClientSDK clientSdk)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _clientSdk = clientSdk;
        }

        public async Task<List<Activity>> GetActivitiesAsync(string authTwinRefId,
            string activityTypeId = null, int? statusCode = null, int? priorityCode = null,
            DateTime? startDate = null, DateTime? endDate = null, string scheduleId = null)
        {
            try
            {
                return await _clientSdk.Activity.GetActivitiesAsync(authTwinRefId,
                    includeActivityDescendants: null, includeAuthTwinDescendants: null,
                    activityTypeId, statusCode, priorityCode, startDate, endDate, scheduleId);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"GetActivitiesAsync Failed - {ex.Message}"));
                throw;
            }
        }

        public async Task<List<Activity>> GetActivityAsync(string activityId, bool includeDescendants = false)
        {
            try
            {
                return await _clientSdk.Activity.GetOneActivityAsync(activityId, includeDescendants);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"GetActivityAsync Failed - {ex.Message}"));
                throw;
            }
        }

        public async Task<bool> UpdateActivitiesAsync(List<Activity> activities)
        {
            try
            {
                var proto = new Activities();
                proto.Items.AddRange(activities);
                return await _clientSdk.Activity.UpdateActivitiesAsync(proto);
            }
            catch (Exception ex)
            {
                Event(ex, CreateLoggerArgs(EnumEventLevel.Error, $"UpdateActivitiesAsync Failed - {ex.Message}"));
                throw;
            }
        }

        public async Task<bool> CreateAnalyteAsync(Analyte analyte)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/operations/sample/v1/analyte";

            try
            {
                var respContent = await _restHelper.PostRestProtobufAsync(analyte, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SampleApi", Message = $"CreateAnalyteAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SampleApi", Message = $"CreateAnalyteAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"CreateAnalyteAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<bool> CreateTestGroupAsync(TestAnalyteGroup testGroup)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"/operations/sample/v1/testgroup";

            try
            {
                var respContent = await _restHelper.PostRestProtobufAsync(testGroup, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SampleApi", Message = $"CreateTestGroupAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "SampleApi", Message = $"CreateTestGroupAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"CreateAnalyteAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<List<Analyte>> GetAnalytesAsync(string authTwinRefId, bool? includeInactive = null)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string endPointUrl = $"/operations/sample/v1/analyte/analytes/{authTwinRefId}";
            if (includeInactive != null)
            {
                endPointUrl += $"?{includeInactive}";
            }

            List<Analyte> analytes = new List<Analyte>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    analytes.AddRange(respContent.ApiResponse.Content.Analytes.Items);

                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace,
                        HttpStatusCode = respContent.ResponseMessage.StatusCode,
                        ElapsedMs = watch.ElapsedMilliseconds,
                        Module = "SampleApi",
                        Message = $"GetAnalytesAsync Success"
                    });

                    return analytes;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "SampleApi",
                    Message = $"GetAnalytesAsync Failed"
                });

                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"GetAnalytesAsync Failed - {e.Message}" });
                throw;
            }
        }


        public async Task<Analyte> GetOneAnalyteAsync(string analyteId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/analyte/{analyteId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var analyte in respContent.ApiResponse.Content.Analytes.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs
                        {
                            EventLevel = EnumEventLevel.Trace,
                            HttpStatusCode = respContent.ResponseMessage.StatusCode,
                            ElapsedMs = watch.ElapsedMilliseconds,
                            Module = "SampleApi",
                            Message = $"GetOneAnalyteAsync Success"
                        });
                        return analyte;
                    }
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "SampleApi",
                    Message = $"GetOneAnalyteAsync Failed"
                });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"GetOneAnalyteAsync Failed - {e.Message}" });
                throw;
            }
        }

        public async Task<List<TestAnalyteGroup>> GetTestGroupsAsync(string authTwinRefId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            string endPointUrl = $"/operations/sample/v1/TestGroup/For/{authTwinRefId}";

            List<TestAnalyteGroup> TestGroups = new List<TestAnalyteGroup>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    TestGroups.AddRange(respContent.ApiResponse.Content.TestAnalyteGroups.Items);

                    Event(null, new ClientApiLoggerEventArgs
                    {
                        EventLevel = EnumEventLevel.Trace,
                        HttpStatusCode = respContent.ResponseMessage.StatusCode,
                        ElapsedMs = watch.ElapsedMilliseconds,
                        Module = "SampleApi",
                        Message = $"GetTestGroupsAsync Success"
                    });

                    return TestGroups;
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "SampleApi",
                    Message = $"GetTestGroupsAsync Failed"
                });

                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"GetTestGroupsAsync Failed - {e.Message}" });
                throw;
            }
        }


        public async Task<TestAnalyteGroup> GetOneTestGroupAsync(string testGroupId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endPointUrl = $"/operations/sample/v1/TestGroup/{testGroupId}";

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, endPointUrl).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    foreach (var testGroup in respContent.ApiResponse.Content.TestAnalyteGroups.Items)
                    {
                        Event(null, new ClientApiLoggerEventArgs
                        {
                            EventLevel = EnumEventLevel.Trace,
                            HttpStatusCode = respContent.ResponseMessage.StatusCode,
                            ElapsedMs = watch.ElapsedMilliseconds,
                            Module = "SampleApi",
                            Message = $"GetOneTestGroupAsync Success"
                        });
                        return testGroup;
                    }
                }

                Event(null, new ClientApiLoggerEventArgs
                {
                    EventLevel = EnumEventLevel.Warn,
                    HttpStatusCode = respContent.ResponseMessage.StatusCode,
                    ElapsedMs = watch.ElapsedMilliseconds,
                    Module = "SampleApi",
                    Message = $"GetOneTestGroupAsync Failed"
                });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "SampleApi", Message = $"GetOneTestGroupAsync Failed - {e.Message}" });
                throw;
            }
        }

        private ClientApiLoggerEventArgs CreateLoggerArgs(EnumEventLevel level, string message, HttpStatusCode statusCode = default, long duration = default) => new ClientApiLoggerEventArgs
        { EventLevel = level, HttpStatusCode = statusCode, ElapsedMs = duration, Module = "SampleApi", Message = message };
    }
}
