using Common.Library.Protobuf.Models;
using ONE;
using ONE.Common.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Common.Library
{
    public class Helper
    {
        ClientSDK _clientSDK;
        public Helper(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
        }
        public List<QuantityType> QuantityTypes { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<Unit> Units { get; set; }
        public List<I18NKey> I18Nkeys { get; set; }
        public async Task<bool> LoadAsync()
        {
            QuantityTypes = await _clientSDK.Library.GetQuantityTypesAsync();
            Parameters = await _clientSDK.Library.GetParametersAsync();
            Units = await _clientSDK.Library.GetUnitsAsync();
            I18Nkeys = await _clientSDK.Library.Geti18nKeysAsync("en-US", "FOUNDATION_LIBRARY");
            return true;
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
    }
}
