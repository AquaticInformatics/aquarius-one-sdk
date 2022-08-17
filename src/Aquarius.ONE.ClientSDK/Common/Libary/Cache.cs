using ONE.Models.CSharp;
using Newtonsoft.Json;
using ONE;
using ONE.Common.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Common.Library
{
    public class Cache
    {
        ClientSDK _clientSDK;
        public Cache(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
        }
        public List<QuantityType> QuantityTypes { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<ParameterAgencyCode> ParameterAgencyCodes { get; set; }
        public List<ParameterAgencyCodeType> ParameterAgencyCodeTypes { get; set;}
        public List<Unit> Units { get; set; }
        public List<I18NKey> I18Nkeys { get; set; }

        public List<DigitalTwinType> DigitalTwinTypes { get; set; }
        public List<DigitalTwinSubtype> DigitalTwinSubtypes { get; set; }

        public async Task<bool> LoadAsync(string culture = "en-US", string modules = "AQI_FOUNDATION_LIBRARY")
        {
            try
            {
                var QuantityTypesTask = _clientSDK.Library.GetQuantityTypesAsync();
                var ParametersTask = _clientSDK.Library.GetParametersAsync();
                var UnitsTask = _clientSDK.Library.GetUnitsAsync();
                var ParameterAgencyCodeTypesTask = _clientSDK.Library.GetParameterAgencyCodeTypesAsync();
                var ParameterAgencyCodesTask = _clientSDK.Library.GetParameterAgencyCodesAsync();
                var I18NkeysTask = _clientSDK.Library.Geti18nKeysAsync(culture, modules);
                var DigitalTwinTypesTask = _clientSDK.DigitalTwin.GetDigitalTwinTypesAsync();
                var DigitalTwinSubtypesTask = _clientSDK.DigitalTwin.GetDigitalTwinSubTypesAsync();

                await Task.WhenAll(QuantityTypesTask, ParametersTask, UnitsTask, ParameterAgencyCodeTypesTask, ParameterAgencyCodesTask, I18NkeysTask, DigitalTwinTypesTask, DigitalTwinSubtypesTask);
                QuantityTypes = QuantityTypesTask.Result;
                Parameters = ParametersTask.Result;
                Units = UnitsTask.Result;
                ParameterAgencyCodeTypes = ParameterAgencyCodeTypesTask.Result;
                ParameterAgencyCodes = ParameterAgencyCodesTask.Result;
                I18Nkeys = I18NkeysTask.Result;
                DigitalTwinTypes = DigitalTwinTypesTask.Result;
                DigitalTwinSubtypes = DigitalTwinSubtypesTask.Result;

            }
            catch
            {
                return false;
            }

            return true;
        }
        public string GetI18nKeyValue(string key, string defaultValue = "")
        {
            try
            {
                var matches = I18Nkeys.Where(p => String.Equals(p.Key, key, StringComparison.CurrentCulture));
                if (matches.Count() > 0)
                {
                    var item = matches.First();
                    return item.Value;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }
        public DigitalTwinType GetDigitalTwinType(string digitalTwinTypeId)
        {
            if (DigitalTwinTypes == null || string.IsNullOrEmpty(digitalTwinTypeId))
                return null;
            var matches = DigitalTwinTypes.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), digitalTwinTypeId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public string GetDigitalTwinTypeName(string digitalTwinSubtypeId)
        {
            var digitalTwinType = GetDigitalTwinType(digitalTwinSubtypeId);
            if (digitalTwinType == null)
                return "";
            return I18NKeyHelper.GetValue(digitalTwinType.I18NKeyName, digitalTwinType.I18NKeyName);

        }
        public DigitalTwinSubtype GetDigitalTwinSubType(string digitalTwinSubtypeId)
        {
            if (DigitalTwinSubtypes == null || string.IsNullOrEmpty(digitalTwinSubtypeId))
                return null;
            var matches = DigitalTwinSubtypes.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), digitalTwinSubtypeId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public string GetDigitalTwinSubtypeName(string digitalTwinSubtypeId)
        {
            var digitalTwinSubtype = GetDigitalTwinSubType(digitalTwinSubtypeId);
            if (digitalTwinSubtype == null)
                return "";
            return I18NKeyHelper.GetValue(digitalTwinSubtype.I18NKeyName, digitalTwinSubtype.I18NKeyName);

        }
        public ParameterAgencyCodeType GetParameterAgencyCodeType(string id)
        {
            if (ParameterAgencyCodeTypes == null || string.IsNullOrEmpty(id))
                return null;
            var matches = ParameterAgencyCodeTypes.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), id.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public ParameterAgencyCode GetParameterAgencyCode(string id)
        {
            if (ParameterAgencyCodes == null || string.IsNullOrEmpty(id))
                return null;
            var matches = ParameterAgencyCodes.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), id.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public QuantityType GetQuantityType(string quantityTypeId)
        {
            if (QuantityTypes == null || string.IsNullOrEmpty(quantityTypeId))
                return null;
            var matches = QuantityTypes.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), quantityTypeId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public QuantityType GetQuantityTypeByName(string name)
        {
            if (QuantityTypes == null || string.IsNullOrEmpty(name))
                return null;
            var matches = QuantityTypes.Where(p => p.Id != null && String.Equals(p.Name.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
        public Parameter GetParameter(string parameterId)
        {
            if (Parameters == null || string.IsNullOrEmpty(parameterId))
                return null;
            var matches = Parameters.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), parameterId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public Parameter GetParameter(long parameterId)
        {
            if (Parameters == null || parameterId <= 0)
                return null;
            var matches = Parameters.Where(p => p.IntId != 0 && p.IntId == parameterId);
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public Parameter GetParameterByName(string name)
        {
            if (Units == null || string.IsNullOrEmpty(name))
                return null;
            var i18nKeyMatches = I18Nkeys.Where(p => p.Value != null && p.Module == "Parameter" && p.Key.StartsWith("PARAMETERTYPE") && String.Equals(p.Value.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));
            if (i18nKeyMatches.Count() > 0)
            {
                I18NKey i18NKey = i18nKeyMatches.First();
                var matches = Parameters.Where(p => p.Id != null && String.Equals(p.I18NKey.ToUpper(), i18NKey.Key.ToUpper(), StringComparison.CurrentCulture));
                if (matches.Count() > 0)
                    return matches.First();
            }
            return null;
        }
        public Unit GetUnit(string unitId)
        {
            if (Units == null || string.IsNullOrEmpty(unitId))
                return null;
            var matches = Units.Where(p => p.Id != null && String.Equals(p.Id.ToUpper(), unitId.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public Unit GetUnit(long unitId)
        {
            if (Units == null || unitId <= 0)
                return null;
            var matches = Units.Where(p => p.IntId != 0 && p.IntId == unitId);
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public Unit GetUnitByI18nKey(string i18nKey)
        {
            if (Units == null || string.IsNullOrEmpty(i18nKey))
                return null;
            var matches = Units.Where(p => p.Id != null && String.Equals(p.I18NKey.ToUpper(), i18nKey.ToUpper(), StringComparison.CurrentCulture));
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public Unit GetUnitByName(string name)
        {
            if (Units == null || string.IsNullOrEmpty(name))
                return null;
            var i18nKeyMatches = I18Nkeys.Where(p => p.Value != null && p.Module == "UnitType" && p.Key.StartsWith("UNIT_TYPE") && String.Equals(p.Value.ToUpper(), name.ToUpper(), StringComparison.CurrentCulture));
            if (i18nKeyMatches.Count() > 0)
            {
                I18NKey i18NKey = i18nKeyMatches.First();
                var matches = Units.Where(p => p.Id != null && String.Equals(p.I18NKey.ToUpper(), i18NKey.Key.ToUpper(), StringComparison.CurrentCulture));
                if (matches.Count() > 0)
                    return matches.First();
            }
            return null;

        }
        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return base.ToString();
            }
        }
        public static Cache Load(string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<Cache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
    }
}
