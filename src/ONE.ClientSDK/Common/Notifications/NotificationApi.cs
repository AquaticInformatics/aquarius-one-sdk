using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Notifications
{
	public class NotificationApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public NotificationApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}
		
		/********************* Notification Topics *********************/
		public async Task<NotificationTopic> GetNotificationTopicAsync(string topicId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTopic/{topicId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetNotificationTopicAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTopics?.Items.FirstOrDefault();
		}

		public async Task<List<NotificationTopic>> GetNotificationTopicsAsync(EnumNotificationCategory enumNotificationCategory = EnumNotificationCategory.NotificationCategoryUnknown, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTopic?topicType={enumNotificationCategory}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetNotificationTopicsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTopics?.Items.ToList();
		}

		public async Task<NotificationTopic> CreateNotificationTopicAsync(NotificationTopic notificationTopic, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTopic?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateNotificationTopicAsync", HttpMethod.Post, endpoint, cancellation, notificationTopic).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTopics?.Items.FirstOrDefault();
		}

		public async Task<NotificationTopic> UpdateNotificationTopicAsync(NotificationTopic notificationTopic, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTopic?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateNotificationTopicAsync", HttpMethod.Put, endpoint, cancellation, notificationTopic).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTopics?.Items.FirstOrDefault();
		}

		/********************* User Notification Preferences *********************/
		public async Task<UserNotificationPreferences> GetUserNotificationPreferencesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/UserNotificationPreference?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUserNotificationPreferencesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UserNotificationPreferences;
		}

		public async Task<UserNotificationPreferences> UpdateUserNotificationPreferencesAsync(UserNotificationPreferences userNotificationPreferences, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/UserNotificationPreference?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateUserNotificationPreferencesAsync", HttpMethod.Put, endpoint, cancellation, userNotificationPreferences).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UserNotificationPreferences;
		}

		/********************* Notification Templates *********************/
		public async Task<NotificationTemplate> GetNotificationTemplateAsync(string templateId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTemplate/{templateId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetNotificationTemplateAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTemplates?.Items.FirstOrDefault();
		}

		public async Task<List<NotificationTemplate>> GetNotificationTemplatesAsync(string topicId = "", string cultureCode = "", CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTemplate?topicId={topicId}&cultureCode={cultureCode}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetNotificationTemplatesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTemplates?.Items.ToList();
		}

		public async Task<NotificationTemplate> CreateNotificationTemplateAsync(NotificationTemplate notificationTemplate, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTemplate?topicId={notificationTemplate.NotificationTopic.Id}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateNotificationTemplateAsync", HttpMethod.Post, endpoint, cancellation, notificationTemplate).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTemplates?.Items.FirstOrDefault();
		}

		public async Task<NotificationTemplate> UpdateNotificationTemplateAsync(NotificationTemplate notificationTemplate, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/NotificationTemplate?topicId={notificationTemplate.NotificationTopic.Id}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateNotificationTemplateAsync", HttpMethod.Put, endpoint, cancellation, notificationTemplate).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.NotificationTemplates?.Items.FirstOrDefault();
		}

		/********************* Notifications *********************/
		public async Task<bool> CreateNotificationAsync(NotificationEvent notificationEvent, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/Notification?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateNotificationAsync", HttpMethod.Post, endpoint, cancellation, notificationEvent).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> CreateUserNotificationAsync(string userId, NotificationEvent notificationEvent, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/Notification/User/{userId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateUserNotificationAsync", HttpMethod.Post, endpoint, cancellation, notificationEvent).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/********************* In App Notifications *********************/
		public async Task<List<InAppNotificationMessage>> GetInAppNotificationsAsync(int pageNumber = 0, int pageSize = 0, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/InAppNotification?requestId={Guid.NewGuid()}";

			if (pageNumber > 0 && pageSize > 0)
				endpoint += $"&pageNumber={pageNumber}&pageSize={pageSize}";

			var apiResponse = await ExecuteRequest("GetInAppNotificationsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.InAppNotificationMessages?.Items.ToList();
		}

		public async Task<InAppNotificationMessage> UpdateInAppNotificationAsync(InAppNotificationMessage inAppNotificationMessage, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/InAppNotification?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateInAppNotificationAsync", HttpMethod.Put, endpoint, cancellation, inAppNotificationMessage).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.InAppNotificationMessages?.Items.FirstOrDefault();
		}

		public async Task<List<InAppNotificationMessage>> UpdateInAppNotificationsAsync(InAppNotificationMessages inAppNotificationMessages, CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/InAppNotification?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateInAppNotificationsAsync", HttpMethod.Put, endpoint, cancellation, inAppNotificationMessages).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.InAppNotificationMessages?.Items.ToList();
		}

		public async Task<bool> InAppNotificationsMarkAllReadAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/notification/v1/InAppNotification/MarkAllRead?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("InAppNotificationsMarkAllReadAsync", HttpMethod.Put, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = " Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (!apiResponse.StatusCode.IsSuccessStatusCode())
				{
					message = " Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ApiResponse = apiResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "NotificationApi",
						Message = callingMethod + message
					});

				return apiResponse;
			}
			catch (Exception e)
			{
				Event(e,
					new ClientApiLoggerEventArgs
					{
						EventLevel = EnumOneLogLevel.OneLogLevelError,
						Module = "NotificationApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
