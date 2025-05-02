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
using proto = ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Configuration
{
	public class ConfigurationApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;
		
		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public ConfigurationApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}
		
		public async Task<proto.Configuration> GetConfigurationAsync(string id, int version = 0, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/{id}?requestId={Guid.NewGuid()}";

			if (version > 0)
				endpoint += $"&version={version}";

			var apiResponse = await ExecuteRequest("GetConfigurationAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Configurations?.Items.FirstOrDefault();
		}

		public async Task<List<proto.Configuration>> GetConfigurationsAsync(string configurationTypeId, string authTwinRefId, string descendantTwinTypeId = "", string context = "", CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}&requestId={Guid.NewGuid()}";

			if (!string.IsNullOrEmpty(descendantTwinTypeId))
				endpoint += $"&descendantTwinTypeId={descendantTwinTypeId}";

			if (!string.IsNullOrEmpty(context))
				endpoint += $"&context={context}";

			var apiResponse = await ExecuteRequest("GetConfigurationsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Configurations?.Items.ToList();
		}

		public async Task<List<proto.Configuration>> GetConfigurationsAdminAsync(string configurationTypeId, string authTwinRefId, string descendantTwinTypeId = "", string context = "", string ownerId = "", bool? isPublic = null, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/admin?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}&requestId={Guid.NewGuid()}";

			if (!string.IsNullOrEmpty(descendantTwinTypeId))
				endpoint += $"&descendantTwinTypeId={descendantTwinTypeId}";

			if (!string.IsNullOrEmpty(context))
				endpoint += $"&context={context}";

			if (!string.IsNullOrEmpty(ownerId))
				endpoint += $"&ownerId={ownerId}";

			if (isPublic != null)
				endpoint += $"&isPublic={isPublic}";

			var apiResponse = await ExecuteRequest("GetConfigurationsAdminAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Configurations?.Items.ToList();
		}

		public async Task<bool> CreateConfigurationAsync(proto.Configuration configuration, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateConfigurationAsync", HttpMethod.Post, endpoint, cancellation, configuration).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> CreateConfigurationAsync(string authTwinRefId, string configurationTypeId, string configurationData, bool isPublic, CancellationToken cancellation = default)
		{
			var configuration = new proto.Configuration
			{
				ConfigurationTypeId = configurationTypeId,
				ConfigurationData = configurationData,
				AuthTwinRefId = authTwinRefId,
				IsPublic = isPublic
			};

			return await CreateConfigurationAsync(configuration, cancellation);
		}

		public async Task<bool> UpdateConfigurationAsync(proto.Configuration configuration, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/{configuration.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateConfigurationAsync", HttpMethod.Put, endpoint, cancellation, configuration).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UpdateConfigurationAsync(string id, string configurationTypeId, string configurationData, bool isPublic, CancellationToken cancellation = default)
		{
			var configuration = new proto.Configuration
			{
				Id = id,
				ConfigurationTypeId = configurationTypeId,
				ConfigurationData = configurationData,
				IsPublic = isPublic,
				Version = 0
			};

			return await UpdateConfigurationAsync(configuration, cancellation);
		}

		public async Task<bool> DeleteConfigurationAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteConfigurationAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<List<ConfigurationNote>> GetConfigurationNotesAsync(string configurationId, DateTime startDate, DateTime endDate, string tagString="", string noteContains="", CancellationToken cancellation = default)
		{
			if (startDate.Kind == DateTimeKind.Local)
				startDate = startDate.ToUniversalTime();

			if (endDate.Kind == DateTimeKind.Local)
				endDate = endDate.ToUniversalTime();

			var endpoint = $"common/configuration/v2/notes/{configurationId}?startDate={startDate:O}&endDate={endDate:O}&requestId={Guid.NewGuid()}";

			if (!string.IsNullOrWhiteSpace(tagString))
				endpoint += $"&tagString={tagString}";
			
			if (!string.IsNullOrWhiteSpace(noteContains))
				endpoint += $"&noteContains={noteContains}";

			var apiResponse = await ExecuteRequest("GetConfigurationNotesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.ToList();
		}

		public async Task<List<ConfigurationNote>> GetConfigurationNotesLastAsync(string configurationTypeId, string authTwinRefId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/notes/last?configurationTypeId={configurationTypeId}&authTwinRefId={authTwinRefId}";

			var apiResponse = await ExecuteRequest("GetConfigurationNotesLastAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.ToList();
		}

		public async Task<List<ConfigurationNote>> GetConfigurationNotesModifiedSinceUtcAsync(string configurationId, DateTime modifiedSinceUtc, CancellationToken cancellation = default)
		{
			if (modifiedSinceUtc.Kind == DateTimeKind.Local)
				modifiedSinceUtc = modifiedSinceUtc.ToUniversalTime();

			var endpoint = $"common/configuration/v2/notes/{configurationId}/modifiedSinceUtc?modifiedSinceUtc={modifiedSinceUtc:O}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetConfigurationNotesModifiedSinceUtcAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.ToList();
		}

		public async Task<List<ConfigurationTag>> GetConfigurationTagsAsync(string configurationId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/notes/{configurationId}/uniquetags?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetConfigurationTagsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationTags?.Items.ToList();
		}

		public async Task<bool> CreateConfigurationNoteAsync(ConfigurationNote configurationNote, CancellationToken cancellation = default) 
			=> await CreateConfigurationNote2Async(configurationNote, cancellation) != null;

		public async Task<ConfigurationNote> CreateConfigurationNote2Async(ConfigurationNote configurationNote, CancellationToken cancellation = default)
		{
			var usePayloadAudit = configurationNote.RecordAuditInfo != null;
			var endpoint = $"common/configuration/v2/notes?usePayloadAudit={usePayloadAudit}&requestId={Guid.NewGuid()}";
			
			var apiResponse = await ExecuteRequest("CreateConfigurationNote2Async", HttpMethod.Post, endpoint, cancellation, configurationNote).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.FirstOrDefault();
		}

		public async Task<bool> ImportConfigurationNotesAsync(ConfigurationNotes configurationNotes, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/notes/import?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("ImportConfigurationNotesAsync", HttpMethod.Post, endpoint, cancellation, configurationNotes).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UpdateConfigurationNoteAsync(ConfigurationNote configurationNote, CancellationToken cancellation = default)
			=> await UpdateConfigurationNote2Async(configurationNote, cancellation) != null;

		public async Task<ConfigurationNote> UpdateConfigurationNote2Async(ConfigurationNote configurationNote, CancellationToken cancellation = default)
		{
			var usePayloadAudit = configurationNote.RecordAuditInfo != null;
			var endpoint = $"common/configuration/v2/notes?usePayloadAudit={usePayloadAudit}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("", HttpMethod.Put, endpoint, cancellation, configurationNote).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteConfigurationNotesAsync(string configurationId, string noteId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/notes/{configurationId}?noteId={noteId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteConfigurationNotesAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<ConfigurationNote> GetSingleConfigurationNoteAsync(string configurationId, string noteId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/configuration/v2/notes/{configurationId}/{noteId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetSingleConfigurationNoteAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ConfigurationNotes?.Items.FirstOrDefault();
		}

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync<ApiResponse>(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

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

				Event(null,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "ConfigurationApi",
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
						Module = "ConfigurationApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
