using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using ONE.Utilities;
using Google.Protobuf.WellKnownTypes;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Enums;

namespace ONE.Common.Notification
{
    public class NotificationApi
    {
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public NotificationApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        private readonly bool _throwAPIErrors;


        /********************* Notification Topics *********************/

        public async Task<NotificationTopic> GetNotificationTopicAsync(string topicId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/NotificationTopic/{topicId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.NotificationTopics.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<NotificationTopic>> GetNotificationTopicsAsync(EnumNotificationCategory enumNotificationCategoy = EnumNotificationCategory.NotificationCategoryUnknown)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<NotificationTopic> notificationTopics = new List<NotificationTopic>(); 
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/NotificationTopic?topicType={enumNotificationCategoy}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.NotificationTopics.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        notificationTopics.Add(result);                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicsAsync Success" });
                    return notificationTopics;

                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetNotificationTopicsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<NotificationTopic> CreateNotificationTopicAsync(NotificationTopic notificationTopic)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/NotificationTopic";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationTopic, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.NotificationTopics.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationTopicAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationTopicAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"CreateNotificationTopicAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<NotificationTopic> UpdateNotificationTopicAsync(NotificationTopic notificationTopic)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/NotificationTopic";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationTopic, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.NotificationTopics.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateNotificationTopicAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateNotificationTopicAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"UpdateNotificationTopicAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* User Notification Preferences *********************/

        public async Task<UserNotificationPreferences> GetUserNotificationPreferencesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/UserNotificationPreference").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Success" });
                    return respContent.ApiResponse.Content.UserNotificationPreferences;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetNotificationTopicAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
      
      
        public async Task<UserNotificationPreferences> UpdateUserNotificationPreferencesAsync(UserNotificationPreferences userNotificationPreferences)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/UserNotificationPreference";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(userNotificationPreferences, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateUserNotificationPreferencesAsync Success" });
                    return respContent.ApiResponse.Content.UserNotificationPreferences;

                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateUserNotificationPreferencesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"UpdateUserNotificationPreferencesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* Notification Templates *********************/

        public async Task<NotificationTemplate> GetNotificationTemplateAsync(string templateId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();

            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/NotificationTemplate/{templateId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.NotificationTemplates.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTemplateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTemplateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetNotificationTemplateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<NotificationTemplate>> GetNotificationTemplatesAsync(string topicId = "", string cultureCode = "")
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<NotificationTemplate> notificationTemplates = new List<NotificationTemplate>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/NotificationTemplate?topicId={topicId}&cultureCode={cultureCode}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.NotificationTemplates.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        notificationTemplates.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTemplatesAsync Success" });
                    return notificationTemplates;

                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetNotificationTemplatesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetNotificationTemplatesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<NotificationTemplate> CreateNotificationTemplateAsync(NotificationTemplate notificationTemplate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/NotificationTemplate?topicId={notificationTemplate.NotificationTopic.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationTemplate, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.NotificationTemplates.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationTemplateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationTemplateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"CreateNotificationTemplateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<NotificationTemplate> UpdateNotificationTemplateAsync(NotificationTemplate notificationTemplate)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/NotificationTemplate?topicId={notificationTemplate.NotificationTopic.Id}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationTemplate, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.NotificationTemplates.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateNotificationTemplateAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateNotificationTemplateAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"UpdateNotificationTemplateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        /********************* Notifications *********************/
        public async Task<bool> CreateNotificationAsync(NotificationEvent notificationEvent)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/Notification";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationEvent, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationAsync Success" });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"CreateNotificationAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> CreateUserNotificationAsync(string userId, NotificationEvent notificationEvent)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/Notification/User/{userId}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(notificationEvent, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationAsync Success" });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"CreateNotificationAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"CreateNotificationAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				return false;
            }
        }
        /********************* In App Notifications *********************/


        public async Task<List<InAppNotificationMessage>> GetInAppNotificationsAsync(int pageNumber = 0, int pageSize = 0)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<InAppNotificationMessage> inAppNotificationMessages = new List<InAppNotificationMessage>();
            try
            {
                ServiceResponse respContent = null;
                if (pageNumber > 0)
                    respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/InAppNotification?pageNumber={pageNumber}&pageSize={pageSize}").ConfigureAwait(_continueOnCapturedContext);
                else
                    respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"common/notification/v1/InAppNotification").ConfigureAwait(_continueOnCapturedContext);

                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.InAppNotificationMessages.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        inAppNotificationMessages.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetInAppNotificationsAsync Success" });
                    return inAppNotificationMessages;

                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"GetInAppNotificationsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"GetInAppNotificationsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
       
        public async Task<InAppNotificationMessage> UpdateInAppNotificationAsync(InAppNotificationMessage inAppNotificationMessage)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/InAppNotification";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(inAppNotificationMessage, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.InAppNotificationMessages.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateInAppNotificationAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateInAppNotificationAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"UpdateInAppNotificationAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<InAppNotificationMessage>> UpdateInAppNotificationsAsync(InAppNotificationMessages inAppNotificationMessages)
        {
            List<InAppNotificationMessage> inAppNotifications = new List<InAppNotificationMessage>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/InAppNotification";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(inAppNotificationMessages, jsonSettings);

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.InAppNotificationMessages.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        inAppNotifications.Add(result);

                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateInAppNotificationsAsync Success" });
                    return inAppNotifications;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"UpdateInAppNotificationsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"UpdateInAppNotificationsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> InAppNotificationsMarkAllReadAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"common/notification/v1/InAppNotification/MarkAllRead";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            try
            {
                var respContent = await _restHelper.PutRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"InAppNotificationsMarkAllReadAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "NotificationApi", Message = $"InAppNotificationsMarkAllReadAsync Failed" });
                return false; }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumLogLevel.Error, Module = "NotificationApi", Message = $"InAppNotificationsMarkAllReadAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
    }
}
