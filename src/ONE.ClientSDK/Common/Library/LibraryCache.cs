using Newtonsoft.Json;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Imposed.Internationalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Library
{
	public class LibraryCache
	{
		private readonly OneApi _oneApi;

		public LibraryCache(OneApi oneApi)
		{
			_oneApi = oneApi;
		}

		public List<QuantityType> QuantityTypes { get; set; }
		public List<Parameter> Parameters { get; set; }
		public List<ParameterAgencyCode> ParameterAgencyCodes { get; set; }
		public List<ParameterAgencyCodeType> ParameterAgencyCodeTypes { get; set;}
		public List<Unit> Units { get; set; }
		public List<I18NKey> I18NKeys { get; set; }

		public List<DigitalTwinType> DigitalTwinTypes { get; set; }
		public List<DigitalTwinSubtype> DigitalTwinSubtypes { get; set; }

		public void LoadFromCache(string serializedObject)
        {
            var cache = JsonConvert.DeserializeObject<LibraryCache>(serializedObject, JsonExtensions.IgnoreNullSerializerSettings);
			QuantityTypes = cache.QuantityTypes;
			Parameters = cache.Parameters;
			Units = cache.Units;
			ParameterAgencyCodeTypes = cache.ParameterAgencyCodeTypes;
			ParameterAgencyCodes = cache.ParameterAgencyCodes;
			I18NKeys = cache.I18NKeys;
			DigitalTwinTypes = cache.DigitalTwinTypes;
			DigitalTwinSubtypes = cache.DigitalTwinSubtypes;
			I18NKeyHelper.I18NKeyList = I18NKeys;
		}

		public async Task<bool> LoadAsync(string culture = "en-US", string modules = "AQI_FOUNDATION_LIBRARY")
		{
			try
			{
				var quantityTypesTask = _oneApi.Library.GetQuantityTypesAsync();
				var parametersTask = _oneApi.Library.GetParametersAsync();
				var unitsTask = _oneApi.Library.GetUnitsAsync();
				var parameterAgencyCodeTypesTask = _oneApi.Library.GetParameterAgencyCodeTypesAsync();
				var parameterAgencyCodesTask = _oneApi.Library.GetParameterAgencyCodesAsync();
				var i18NKeysTask = _oneApi.Library.Geti18nKeysAsync(culture, modules);
				var digitalTwinTypesTask = _oneApi.DigitalTwin.GetDigitalTwinTypesAsync();
				var digitalTwinSubtypesTask = _oneApi.DigitalTwin.GetDigitalTwinSubTypesAsync();

				await Task.WhenAll(quantityTypesTask, parametersTask, unitsTask, parameterAgencyCodeTypesTask, parameterAgencyCodesTask, i18NKeysTask, digitalTwinTypesTask, digitalTwinSubtypesTask);

				QuantityTypes = quantityTypesTask.Result;
				Parameters = parametersTask.Result;
				Units = unitsTask.Result;
				ParameterAgencyCodeTypes = parameterAgencyCodeTypesTask.Result;
				ParameterAgencyCodes = parameterAgencyCodesTask.Result;
				I18NKeys = i18NKeysTask.Result;
				DigitalTwinTypes = digitalTwinTypesTask.Result;
				DigitalTwinSubtypes = digitalTwinSubtypesTask.Result;
				I18NKeyHelper.I18NKeyList = I18NKeys;
			}
			catch
			{
				if (_oneApi.ThrowApiErrors)
					throw;
				return false;
			}

			return true;
		}

		public string GetI18NKeyValue(string key, string defaultValue = "")
		{
			try
			{
				if (I18NKeys == null || string.IsNullOrEmpty(key))
					return defaultValue;

				return I18NKeys.FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.CurrentCulture))?.Value ?? defaultValue;
			}
			catch
			{
				if (_oneApi.ThrowApiErrors)
					throw;
				return defaultValue;
			}
		}

		public DigitalTwinType GetDigitalTwinType(string digitalTwinTypeId)
		{
			if (DigitalTwinTypes == null || string.IsNullOrEmpty(digitalTwinTypeId))
				return null;

			return DigitalTwinTypes.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), digitalTwinTypeId.ToUpper(), StringComparison.CurrentCulture));
		}

		public string GetDigitalTwinTypeName(string digitalTwinTypeId)
		{
			var digitalTwinType = GetDigitalTwinType(digitalTwinTypeId);
			return digitalTwinType == null ? string.Empty : GetI18NKeyValue(digitalTwinType.I18NKeyName, digitalTwinType.I18NKeyName);
		}

		public DigitalTwinSubtype GetDigitalTwinSubType(string digitalTwinSubtypeId)
		{
			if (DigitalTwinSubtypes == null || string.IsNullOrEmpty(digitalTwinSubtypeId))
				return null;

			return DigitalTwinSubtypes.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), digitalTwinSubtypeId.ToUpper(), StringComparison.CurrentCulture));
		}

		public string GetDigitalTwinSubtypeName(string digitalTwinSubtypeId)
		{
			var digitalTwinSubtype = GetDigitalTwinSubType(digitalTwinSubtypeId);
			return digitalTwinSubtype == null ? string.Empty : GetI18NKeyValue(digitalTwinSubtype.I18NKeyName, digitalTwinSubtype.I18NKeyName);
		}

		public ParameterAgencyCodeType GetParameterAgencyCodeType(string id)
		{
			if (ParameterAgencyCodeTypes == null || string.IsNullOrEmpty(id))
				return null;

			return ParameterAgencyCodeTypes.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), id.ToUpper(), StringComparison.CurrentCulture));
		}

		public ParameterAgencyCode GetParameterAgencyCode(string id)
		{
			if (ParameterAgencyCodes == null || string.IsNullOrEmpty(id))
				return null;

			return ParameterAgencyCodes.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), id.ToUpper(), StringComparison.CurrentCulture));
		}

		public QuantityType GetQuantityType(string quantityTypeId)
		{
			if (QuantityTypes == null || string.IsNullOrEmpty(quantityTypeId))
				return null;

			return QuantityTypes.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), quantityTypeId.ToUpper(), StringComparison.CurrentCulture));
		}

		public QuantityType GetQuantityTypeByName(string name)
		{
			if (QuantityTypes == null || string.IsNullOrEmpty(name))
				return null;

			return QuantityTypes.FirstOrDefault(p => p.Id != null && string.Equals(p.Name.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));
		}

		public Parameter GetParameter(string parameterId)
		{
			if (Parameters == null || string.IsNullOrEmpty(parameterId))
				return null;

			return Parameters.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), parameterId.ToUpper(), StringComparison.CurrentCulture));
		}

		public Parameter GetParameter(long parameterId)
		{
			if (Parameters == null || parameterId <= 0)
				return null;

			return Parameters.FirstOrDefault(p => p.IntId != 0 && p.IntId == parameterId);
		}

		public Parameter GetParameterByName(string name)
		{
			if (I18NKeys == null || string.IsNullOrEmpty(name))
				return null;

			var i18NKey = I18NKeys.FirstOrDefault(p => p.Value != null && p.Module == "Parameter" && p.Key.StartsWith("PARAMETERTYPE") && string.Equals(p.Value.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));

			if (i18NKey != null)
			{
				var parameter = Parameters.FirstOrDefault(p => p.Id != null && string.Equals(p.I18NKey.ToUpper(), i18NKey.Key.ToUpper(), StringComparison.CurrentCulture));
				if (parameter != null)
					return parameter;
			}

			return null;
		}

		public Unit GetUnit(string unitId)
		{
			if (Units == null || string.IsNullOrEmpty(unitId))
				return null;

			return Units.FirstOrDefault(p => p.Id != null && string.Equals(p.Id.ToUpper(), unitId.ToUpper(), StringComparison.CurrentCulture));
		}

		public Unit GetUnit(long unitId)
		{
			if (Units == null || unitId <= 0)
				return null;

			return Units.FirstOrDefault(p => p.IntId != 0 && p.IntId == unitId);
		}

		public Unit GetUnitByI18NKey(string i18NKey)
		{
			if (Units == null || string.IsNullOrEmpty(i18NKey))
				return null;

			return Units.FirstOrDefault(p => p.Id != null && string.Equals(p.I18NKey.ToUpper(), i18NKey.ToUpper(), StringComparison.CurrentCulture));
		}

		public Unit GetUnitByName(string name)
		{
			if (I18NKeys == null || string.IsNullOrEmpty(name))
				return null;

			var i18NKey = I18NKeys.FirstOrDefault(p => p.Value != null && p.Module == "UnitType" && p.Key.StartsWith("UNIT_TYPE") && string.Equals(p.Value.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));

			return i18NKey != null ? GetUnitByI18NKey(i18NKey.Key) : null;
		}

		public override string ToString()
		{
			try
			{
				return JsonConvert.SerializeObject(this, JsonExtensions.IgnoreNullSerializerSettings);
			}
			catch
			{
				if (_oneApi.ThrowApiErrors)
					throw;
				return base.ToString();
			}
		}

		public static LibraryCache Load(string serializedObject)
		{
			try
			{
				return JsonConvert.DeserializeObject<LibraryCache>(serializedObject, JsonExtensions.IgnoreNullSerializerSettings);
			}
			catch
			{
				return null;
			}
		}
	}
}
