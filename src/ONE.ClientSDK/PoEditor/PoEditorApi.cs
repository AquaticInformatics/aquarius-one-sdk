using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.PoEditor
{
	/// <summary>
	/// Class for working with POEditor.
	/// https://poeditor.com/docs/api
	/// </summary>
	public class PoEditorApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public PoEditorApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		private const string PoEditorProjectName = "AQI_FOUNDATION_LIBRARY";
		private const string BaseUrl = "https://api.poeditor.com/v2";
		private const string PoEditorApiKey = "16e260278fde2bccf2b871b1586706c3";
		private const string ApiTokenKey = "api_token";
		private const string IdKey = "id";
		private const string LanguageKey = "language";
		
		private int _projectId = -1;

		#region Projects endpoint

		/// <summary>
		/// Gets a list of projects from the POEditor Api.
		/// </summary>
		/// <returns></returns>
		private async Task<IList<ProjectDto>> GetProjectsAsync()
		{
			try
			{
				var url = $"{BaseUrl}/projects/list";

				var formData = new Dictionary<string, string>
				{
					{ ApiTokenKey, PoEditorApiKey }
				};

				var response = await ExecuteRequestAsync("UpdateLanguageAsync", url, formData);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					dynamic obj = JObject.Parse(content);
					var arr = obj.result.projects as JArray;

					return arr?.ToObject<IList<ProjectDto>>();
				}

				return null;
			}
			catch (Exception ex)
			{
				Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "PoEditorUtility", Message = $"GetProjectsAsync Failed - {ex.Message}" });
				if (_throwApiErrors) 
					 throw;
				return null;
			}
		}

		/// <summary>
		/// Gets a list of translations from the POEditor Api.
		/// </summary>
		/// <param name="languageCode">The language code for the terms. Optional, defaults to en (English).</param>
		/// <returns>Task that returns a list of <see cref="TranslationDto"/>.</returns>
		public async Task<IList<TranslationDto>> GetTranslationsAsync(string languageCode = "en")
		{
			try
			{
				var projectId = await GetProjectIdAsync();
				if (projectId < 0)
					throw new ApplicationException("Could not get POEditor project Id.");

				var url = $"{BaseUrl}/projects/export";

				var formData = new Dictionary<string, string>
				{
					{ ApiTokenKey, PoEditorApiKey },
					{ IdKey, $"{projectId}" },
					{ LanguageKey, languageCode },
					{ "type", "json" },
					{ "filters", "not_proofread" }
				};

				var response = await ExecuteRequestAsync("UpdateLanguageAsync", url, formData);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					dynamic obj = JObject.Parse(content);
					var downloadUrl = (string)obj.result.url;
					var jsonData = await DownloadJsonFileAsync(downloadUrl).ConfigureAwait(_continueOnCapturedContext);
					
					return JsonConvert.DeserializeObject<IList<TranslationDto>>(jsonData);
				}

				return null;
			}
			catch (Exception ex)
			{
				Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "PoEditorUtility", Message = $"GetTranslationsAsync Failed - {ex.Message}" });
				if (_throwApiErrors) 
					 throw;
				return null;
			}
		}

		#endregion Projects endpoint

		#region Languages endpoint

		/// <summary>
		/// Update the language translations for a list of terms.
		/// </summary>
		/// <param name="translations">List of terms containing the translations. The translations must
		/// be in the language specified by 'languageCode'.</param>
		/// <param name="languageCode">The language to update.</param>
		/// <returns>The number of translations modified (added or updated).</returns>
		public async Task<int> UpdateLanguageAsync(IList<TranslationDto> translations, string languageCode = "en")
		{
			try
			{
				var projectId = await GetProjectIdAsync();
				if (projectId < 0)
					throw new ApplicationException("Could not get POEditor project Id.");

				var url = $"{BaseUrl}/languages/update";

				var data = translations
					.Select(x => new
					{
						term = x.Term,
						context = x.Context,
						translation =
							new
							{
								content = x.Definition,
								fuzzy = 0,
								proofread = 1
							}
					});

				var formData = new Dictionary<string, string>
				{
					{ ApiTokenKey, PoEditorApiKey },
					{ IdKey, $"{projectId}" },
					{ LanguageKey, $"{languageCode}" },
					{ "data", JsonConvert.SerializeObject(data) }
				};

				var response = await ExecuteRequestAsync("UpdateLanguageAsync", url, formData);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					dynamic obj = JObject.Parse(content);
					
					return obj.result.translations["added"] + obj.result.translations["updated"];
				}

				return 0;
			}
			catch (Exception ex)
			{
				Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "PoEditorUtility", Message = $"UpdateLanguageAsync Failed - {ex.Message}" });
				if (_throwApiErrors) 
					 throw;
				return 0;
			}
		}

		#endregion Languages endpoint

		#region Internal Helpers

		private async Task<HttpResponseMessage> ExecuteRequestAsync(string callingMethod, string uri, IDictionary<string, string> formData)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var content = new FormUrlEncodedContent(formData);

				var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
				var response = await _apiHelper.SendAsync<HttpResponseMessage>(request, CancellationToken.None, true).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = " Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (!response.IsSuccessStatusCode)
				{
					message = " Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ResponseMessage = response, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = response.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "PoEditorUtility",
						Message = callingMethod + message
					});

				return response;
			}
			catch (Exception ex)
			{
				Event(ex,
					new ClientApiLoggerEventArgs
					{
						EventLevel = EnumOneLogLevel.OneLogLevelError,
						Module = "PoEditorUtility",
						Message = $"{callingMethod} Failed - {ex.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}

		private async Task<string> DownloadJsonFileAsync(string uri)
		{
			var request = await _apiHelper.GetAsync<HttpResponseMessage>(uri, CancellationToken.None).ConfigureAwait(_continueOnCapturedContext);
			var httpStream = await request.Content.ReadAsStreamAsync();

			using (var sr = new StreamReader(httpStream))
			{
				return await sr.ReadToEndAsync();
			}
		}

		private async Task<int> GetProjectIdAsync()
		{
			if (_projectId < 0)
			{
				var projects = await GetProjectsAsync();
				var libraryProject = projects.FirstOrDefault(x => string.Equals(x.Name, PoEditorProjectName, StringComparison.OrdinalIgnoreCase)) ??
				                     throw new ApplicationException("Could not get POEditor project.");
				_projectId = libraryProject.Id;
			}

			return _projectId;
		}

		#endregion Internal Helpers
	}
}
