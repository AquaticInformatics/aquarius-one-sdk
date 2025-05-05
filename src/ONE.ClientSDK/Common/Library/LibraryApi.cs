using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Imposed.Internationalization;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Library
{
	public class LibraryApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
		public LibraryApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		/********************* Quantity Types *********************/
		public async Task<List<QuantityType>> GetQuantityTypesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/quantitytype?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetQuantityTypesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.QuantityTypes?.Items.ToList();
		}

		/********************* Units *********************/
		public async Task<Unit> GetUnitAsync(int unitId, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unit/{unitId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Units?.Items.FirstOrDefault();
		}

		public async Task<Unit> CreateUnitAsync(Unit unit, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unit?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateUnitAsync", HttpMethod.Post, endpoint, cancellation, unit).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Units?.Items.FirstOrDefault();
		}

		public async Task<Unit> UpdateUnitAsync(Unit unit, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unit/{unit.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateUnitAsync", HttpMethod.Put, endpoint, cancellation, unit).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Units?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteUnitAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unit/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteUnitAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<List<Unit>> GetUnitsAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unit?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Units?.Items.ToList();
		}

		/********************* Parameters *********************/
		public async Task<Parameter> GetParameterAsync(int id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameter/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParameterAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Parameters?.Items.FirstOrDefault();
		}

		public async Task<List<Parameter>> GetParametersAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameter?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParametersAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Parameters?.Items.ToList();
		}

		public async Task<Parameter> CreateParameterAsync(Parameter parameter, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameter?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateParameterAsync", HttpMethod.Post, endpoint, cancellation, parameter).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Parameters?.Items.FirstOrDefault();
		}

		public async Task<Parameter> UpdateParameterAsync(Parameter parameter, CancellationToken cancellation)
		{
			var endpoint = $"common/library/v1/parameter/{parameter.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateParameterAsync", HttpMethod.Put, endpoint, cancellation, parameter).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Parameters?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteParameterAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameter/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteParameterAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public string FoundationLibraryName => "AQI_FOUNDATION_LIBRARY";

		public async Task<List<I18NKey>> GetMobilei18nKeysAsync(string language) => await Geti18nKeysAsync(language, $"AQI_MOBILE_RIO,{FoundationLibraryName},claros_login");

		public async Task<List<I18NKey>> Geti18nKeysAsync(string language, string modules, CancellationToken cancellation = default)
		{
			try
			{
				var endpoint = $"common/library/v1/i18n?modulecsv={modules}&lang={language}&requestId={Guid.NewGuid()}";

				var watch = Stopwatch.StartNew();

				var httpResponse = await _apiHelper.GetAsync(endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);
				
				watch.Stop();

				if (httpResponse.IsSuccessStatusCode)
				{
					var result = ConvertI18NJsonToExtendedTranslations(await httpResponse.Content.ReadAsStringAsync());
					Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelTrace, HttpStatusCode = httpResponse.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = "Geti18nKeysAsync Success" });
					return result;
				}

				Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelWarn, HttpStatusCode = httpResponse.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "LibraryApi", Message = "Geti18nKeysAsync Failed" });
				
				return null;
			}
			catch (Exception e)
			{
				Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "LibraryApi", Message = $"Geti18nKeysAsync Failed - {e.Message}" });
				if (_throwApiErrors) 
					 throw;
				return null;
			}
		}

		/********************* Agencies *********************/
		public async Task<Agency> CreateAgencyAsync(Agency agency, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/Agency?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateAgencyAsync", HttpMethod.Post, endpoint, cancellation, agency).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Agencies?.Items.FirstOrDefault();
		}

		public async Task<List<Agency>> GetAgenciesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/Agency?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetAgenciesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Agencies?.Items.ToList();
		}

		public async Task<Agency> GetAgencyAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/Agency/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetAgencyAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Agencies?.Items.FirstOrDefault();
		}

		public async Task<Agency> UpdateAgencyAsync(Agency agency, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/Agency/{agency.Id}?requestId={Guid.NewGuid()}";

			agency.UpdateMask = new FieldMask { Paths = { "I18NKeyName" } };

			var apiResponse = await ExecuteRequest("", new HttpMethod("PATCH"), endpoint, cancellation, agency).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Agencies?.Items.FirstOrDefault();
		}

		/********************* Parameter Agency Code Types *********************/
		public async Task<ParameterAgencyCodeType> CreateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/ParameterAgencyCodeType?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateParameterAgencyCodeTypeAsync", HttpMethod.Post, endpoint, cancellation, parameterAgencyCodeType).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodeTypes?.Items.FirstOrDefault();
		}

		public async Task<List<ParameterAgencyCodeType>> GetParameterAgencyCodeTypesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/ParameterAgencyCodeType?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParameterAgencyCodeTypesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodeTypes?.Items.ToList();
		}

		public async Task<ParameterAgencyCodeType> GetParameterAgencyCodeTypeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCodeType/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParameterAgencyCodeTypeAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodeTypes?.Items.FirstOrDefault();
		}

		public async Task<ParameterAgencyCodeType> UpdateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCodeType/{parameterAgencyCodeType.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateParameterAgencyCodeTypeAsync", HttpMethod.Put, endpoint, cancellation, parameterAgencyCodeType).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodeTypes?.Items.FirstOrDefault();
		}
		public async Task<bool> DeleteParameterAgencyCodeTypeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCodeType/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteParameterAgencyCodeTypeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/********************* Parameter Agency Codes *********************/
		public async Task<ParameterAgencyCode> CreateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCode?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateParameterAgencyCodeAsync", HttpMethod.Post, endpoint, cancellation, parameterAgencyCode).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<List<ParameterAgencyCode>> GetParameterAgencyCodesAsync(string parameterAgencyCodeTypeId = null, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCode?ParameterAgencyCodeTypeId={parameterAgencyCodeTypeId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParameterAgencyCodesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodes?.Items.ToList();
		}

		public async Task<ParameterAgencyCode> GetParameterAgencyCodeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCode/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetParameterAgencyCodeAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<ParameterAgencyCode> UpdateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCode/{parameterAgencyCode.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateParameterAgencyCodeAsync", HttpMethod.Put, endpoint, cancellation, parameterAgencyCode).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ParameterAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteParameterAgencyCodeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/parameterAgencyCode/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteParameterAgencyCodeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/********************* Unit Agency Code Types *********************/
		public async Task<UnitAgencyCodeType> CreateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/UnitAgencyCodeType?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateUnitAgencyCodeTypeAsync", HttpMethod.Post, endpoint, cancellation, unitAgencyCodeType).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodeTypes?.Items.FirstOrDefault();
		}

		public async Task<List<UnitAgencyCodeType>> GetUnitAgencyCodeTypesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/UnitAgencyCodeType?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitAgencyCodeTypesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodeTypes?.Items.ToList();
		}

		public async Task<UnitAgencyCodeType> GetUnitAgencyCodeTypeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCodeType/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitAgencyCodeTypeAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodeTypes?.Items.FirstOrDefault();
		}

		public async Task<UnitAgencyCodeType> UpdateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCodeType/{unitAgencyCodeType.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateUnitAgencyCodeTypeAsync", HttpMethod.Put, endpoint, cancellation, unitAgencyCodeType).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodeTypes?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteUnitAgencyCodeTypeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCodeType/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteUnitAgencyCodeTypeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/********************* Unit Agency Codes *********************/
		public async Task<UnitAgencyCode> CreateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode, CancellationToken cancellation)
		{
			var endpoint = $"common/library/v1/unitAgencyCode?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateUnitAgencyCodeAsync", HttpMethod.Post, endpoint, cancellation, unitAgencyCode).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<List<UnitAgencyCode>> GetUnitAgencyCodesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCode?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitAgencyCodesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodes?.Items.ToList();
		}

		public async Task<UnitAgencyCode> GetUnitAgencyCodeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCode/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUnitAgencyCodeAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<UnitAgencyCode> UpdateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCode/{unitAgencyCode.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateUnitAgencyCodeAsync", HttpMethod.Put, endpoint, cancellation, unitAgencyCode).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.UnitAgencyCodes?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteUnitAgencyCodeAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"common/library/v1/unitAgencyCode/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteUnitAgencyCodeAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		/********************* i18n *********************/
		private dynamic GetChild(dynamic parentJsonNode)
		{
			foreach (var child in parentJsonNode.Children())
				return child;
			
			return null;
		}

		private bool IsArray(dynamic parentJsonNode)
		{
			var child = GetChild(parentJsonNode);
			if (child == null)
				return false;

			return GetChild(child) != null;
		}

		private List<I18NKey> ConvertI18NJsonToExtendedTranslations(string json)
		{
			dynamic jsonResponse = JsonConvert.DeserializeObject(json);
			var translations = new List<I18NKey>();

			if (jsonResponse == null)
				return translations;

			var jsonNode = jsonResponse.FM;

			if (jsonNode == null)
				return translations;

			foreach (var translationRootNode in jsonNode.Children())
			{
				var moduleChild = GetChild(translationRootNode);
				Dictionary<string, string> items;

				switch (moduleChild.Name)
				{
					case "AQI_FOUNDATION_LIBRARY":
					case "FOUNDATION_LIBRARY":
						var foundationLibraryNode = JsonConvert.DeserializeObject(GetChild(moduleChild).ToString());
						//Parameter - Long
						if (foundationLibraryNode.Parameter != null && foundationLibraryNode.Parameter.LONG != null)
						{
							jsonNode = foundationLibraryNode.Parameter.LONG;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "Parameter", "LONG");
						}
						//Parameter - Short
						if (foundationLibraryNode.Parameter != null && foundationLibraryNode.Parameter.SHORT != null)
						{
							jsonNode = foundationLibraryNode.Parameter.SHORT;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "Parameter", "SHORT");
						}
						//Unit Type - Long
						if (foundationLibraryNode.UnitType != null && foundationLibraryNode.UnitType.LONG != null)
						{
							jsonNode = foundationLibraryNode.UnitType.LONG;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "UnitType", "LONG");
						}
						//Unit Type - Short
						if (foundationLibraryNode.UnitType != null && foundationLibraryNode.UnitType.SHORT != null)
						{
							jsonNode = foundationLibraryNode.UnitType.SHORT;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "UnitType", "SHORT");
						}
						//Digital Twin Types
						if (foundationLibraryNode.DigitalTwinType != null)
						{
							jsonNode = foundationLibraryNode.DigitalTwinType;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "DigitalTwinType", "");
						}
						//Digital Twin Types
						if (foundationLibraryNode.DigitalTwinSubType != null)
						{
							jsonNode = foundationLibraryNode.DigitalTwinSubType;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "DigitalTwinSubType", "");
						}
						//Feature
						if (foundationLibraryNode.Feature != null)
						{
							jsonNode = foundationLibraryNode.Feature;
							items = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNode.ToString());
							translations = I18NKeyHelper.Load(translations, items, "Feature", "");
						}
						break;
					case "AQI_MOBILE_RIO":
						var typeChild = GetChild(moduleChild);
						//var jObject = JObject.Parse(typeChild.ToString());
						foreach (var child in typeChild.Children())
						{
							if (IsArray(child))
							{
								var keysChild = GetChild(child);
								items = JsonConvert.DeserializeObject<Dictionary<string, string>>(keysChild.ToString());
								translations = I18NKeyHelper.Load(translations, items, "AQI_MOBILE_RIO", child.Name);
							}
						}
						break;
					case "CLAROS_LOGIN":
						translations = LoadChildJsonTranslations(translations, "CLAROS-LOGIN", moduleChild, "");
						break;
				}
			}

			return translations;
		}

		private List<I18NKey> LoadChildJsonTranslations(List<I18NKey> translations, string module, dynamic json, string parentName)
		{
			foreach (var child in json.Children())
			{
				if (IsArray(child))
				{
					var typeName = parentName;
					if (string.IsNullOrEmpty(typeName))
						typeName = child.Name;
					else if (!string.IsNullOrEmpty(child.Name))
						typeName = typeName + "." + child.Name;
					translations = LoadChildJsonTranslations(translations, module, child, typeName);
				}
				else
				{
					var translation = new I18NKey(module, parentName, child.Name, child.Value.Value);
					translations.Add(translation);
				}
			}
			return translations;
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

				Event(this,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "LibraryApi",
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
						Module = "LibraryApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
