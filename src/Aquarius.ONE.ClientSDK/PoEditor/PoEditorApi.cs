using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ONE.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ONE.PoEditor
{
    /// <summary>
    /// Class for working with POEditor.
    /// https://poeditor.com/docs/api
    /// </summary>
    public class PoEditorApi
    {
        public PoEditorApi(PlatformEnvironment environment, bool continueOnCapturedContext, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private readonly bool _throwAPIErrors;
        private const string BaseUrl = "https://api.poeditor.com/v2";
        private const string PoEditorApiKey = "16e260278fde2bccf2b871b1586706c3";

        private const string ApiTokenKey = "api_token";
        private const string IdKey = "id";
        private const string LanguageKey = "language";

        #region Properties

        private int _projectId = -1;

        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

        private HttpClient _client;
        private HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri(BaseUrl);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _client;
            }
        }

        private string _poEditorProjectName;
        private string PoEditorProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(_poEditorProjectName))
                {
                    _poEditorProjectName = _environment.PoEditorProjectName;
                }

                return _poEditorProjectName;
            }
        }

        #endregion Properties

        #region Projects endpoint

        /// <summary>
        /// Gets a list of projects from the POEditor Api.
        /// </summary>
        /// <returns></returns>
        private async Task<IList<ProjectDto>> GetProjectsAsync()
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var url = $"{BaseUrl}/projects/list";

                var formData = new Dictionary<string, string>
                {
                    { ApiTokenKey, PoEditorApiKey }
                };

                var response = await ExecuteRequestAsync(url, formData);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic obj = JObject.Parse(content);
                    var arr = obj.result.projects as JArray;

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "GetProjectsAsync Success" });
                    return arr?.ToObject<IList<ProjectDto>>();
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "GetProjectsAsync Failed" });
                return null;
            }
            catch (Exception ex)
            {
                Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "PoEditorUtility", Message = $"GetProjectsAsync Failed - {ex.Message}" });
                if (_throwAPIErrors) 
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
            var watch = Stopwatch.StartNew();

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

                var response = await ExecuteRequestAsync(url, formData);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic obj = JObject.Parse(content);
                    var downloadUrl = (string)obj.result.url;
                    var jsonData = await DownloadJsonFileAsync(downloadUrl);

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "GetTranslationsAsync Success" });
                    return JsonConvert.DeserializeObject<IList<TranslationDto>>(jsonData);
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "GetTranslationsAsync Failed" });
                return null;
            }
            catch (Exception ex)
            {
                Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "PoEditorUtility", Message = $"GetTranslationsAsync Failed - {ex.Message}" });
                if (_throwAPIErrors) 
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
            var watch = Stopwatch.StartNew();

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

                var response = await ExecuteRequestAsync(url, formData);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic obj = JObject.Parse(content);

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "UpdateLanguageAsync Success" });
                    return obj.result.translations["added"] + obj.result.translations["updated"];
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = response.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "PoEditorUtility", Message = "UpdateLanguageAsync Failed" });
                return 0;
            }
            catch (Exception ex)
            {
                Event(ex, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "PoEditorUtility", Message = $"UpdateLanguageAsync Failed - {ex.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return 0;
            }
        }

        #endregion Languages endpoint

        #region Internal Helpers

        private async Task<HttpResponseMessage> ExecuteRequestAsync(string uri, IDictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            return await Client.PostAsync(uri, content);
        }

        private async Task<string> DownloadJsonFileAsync(string uri)
        {
            var request = await Client.GetAsync(uri);
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
                var libraryProject = projects.FirstOrDefault(x =>
                    string.Equals(x.Name, PoEditorProjectName, StringComparison.OrdinalIgnoreCase));
                if (libraryProject == null)
                    throw new ApplicationException("Could not get POEditor project.");

                _projectId = libraryProject.Id;
            }

            return _projectId;
        }

        #endregion Internal Helpers
    }
}
