using Common.Core.Protobuf.Models;
using Common.Library.Protobuf.Models;
using Enterprise.Twin.Protobuf.Models;
using ONE.Common.Library;
using ONE.Enterprise.Twin;
using ONE.Utilities;
using Operations.Spreadsheet.Protobuf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Operations
{
    public class OperationCache
    {
        public OperationCache(ClientSDK clientSDK, DigitalTwin digitalTwin)
        {
            ClientSDK = clientSDK;
            DigitalTwin = digitalTwin;
            DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
            ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
            ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
            FifteenMinuteRows = new Dictionary<uint, Row>();
            HourlyRows = new Dictionary<uint, Row>();
            FourHourRows = new Dictionary<uint, Row>();
            DailyRows = new Dictionary<uint, Row>();
            MeasurementCache = new Dictionary<string, List<Measurement>>();
        }
        public ClientSDK ClientSDK { get; set; }
        public string Id {
            get
            {
                if (DigitalTwin != null)
                    return DigitalTwin.TwinReferenceId;
                return null;
            }
        }
        public string Name
        {
            get
            {
                if (DigitalTwin != null)
                    return DigitalTwin.Name;
                return null;
            }
        }
        public DigitalTwin DigitalTwin { get; set; }
        public DigitalTwinItem DigitalTwinItem { get; set; }
        public SpreadsheetDefinition SpreadsheetDefinition { get; set; }
        public WorksheetDefinition FifteenMinuteWorksheetDefinition { get; set; }
        public WorksheetDefinition HourlyWorksheetDefinition { get; set; }
        public WorksheetDefinition FourHourWorksheetDefinition { get; set; }
        public WorksheetDefinition DailyWorksheetDefinition { get; set; }
        public Dictionary<uint, Row> FifteenMinuteRows { get; set; }
        public Dictionary<uint, Row> HourlyRows { get; set; }
        public Dictionary<uint, Row> FourHourRows { get; set; }
        public Dictionary<uint, Row> DailyRows { get; set; }
        public List<DigitalTwin> LocationTwins { get; set; }
        public List<DigitalTwin> ColumnTwins { get; set; }

        private Dictionary<string, DigitalTwinItem> ItemDictionarybyGuid { get; set; }
        private Dictionary<long, DigitalTwinItem> ItemDictionarybyLong { get; set; }

        private Dictionary<long, Column> ColumnsByVariable { get; set; }
        private Dictionary<long, Column> ColumnsByVarNum { get; set; }
        private Dictionary<long, Column> ColumnsById { get; set; }
        private Dictionary<string, Column> ColumnsByGuid { get; set; }

        public Dictionary<string, List<Measurement>> MeasurementCache {get; set;}

        private string _delimiter = "\\";
        public string Delimiter
        {
            get
            {
                return _delimiter;
            }
            set
            {
                _delimiter = value;
            }
        }
        public void AddRow(EnumWorksheet enumWorksheet, Row row)
        {
            if (row == null)
                return;
            switch (enumWorksheet)
            {
                case EnumWorksheet.WorksheetFifteenMinute:
                    if (!FifteenMinuteRows.ContainsKey(row.RowNumber))
                        FifteenMinuteRows.Add(row.RowNumber, row);
                    break;
                case EnumWorksheet.WorksheetHour:
                    if (!HourlyRows.ContainsKey(row.RowNumber))
                        HourlyRows.Add(row.RowNumber, row);
                    break;
                case EnumWorksheet.WorksheetFourHour:
                    if (!FourHourRows.ContainsKey(row.RowNumber))
                        FourHourRows.Add(row.RowNumber, row);
                    break;
                case EnumWorksheet.WorksheetDaily:
                   
                    if (!DailyRows.ContainsKey(row.RowNumber))
                        HourlyRows.Add(row.RowNumber, row);
                    break;
            }
        }
        public Row GetRow(EnumWorksheet enumWorksheet, uint rowNumber)
        {
            switch (enumWorksheet)
            {
                case EnumWorksheet.WorksheetFifteenMinute:
                    if (FifteenMinuteRows.ContainsKey(rowNumber))
                        return FifteenMinuteRows[rowNumber];
                    else
                        return null;
                case EnumWorksheet.WorksheetHour:
                    if (HourlyRows.ContainsKey(rowNumber))
                        return HourlyRows[rowNumber];
                    else
                        return null;
                case EnumWorksheet.WorksheetFourHour:
                    if (FourHourRows.ContainsKey(rowNumber))
                        return FourHourRows[rowNumber];
                    else
                        return null;
                case EnumWorksheet.WorksheetDaily:
                    if (DailyRows.ContainsKey(rowNumber))
                        return DailyRows[rowNumber];
                    else
                        return null;
            }
            return null;
        }
        
        
        public bool IsInitialized { get; set; }
        public async Task<bool> InitializeAsync()
        {
            if (IsInitialized)
                return true;
            ColumnTwins = await ClientSDK.DigitalTwin.GetDescendantsByCategoryAsync(Id, 4);
            LocationTwins = await ClientSDK.DigitalTwin.GetDescendantsByCategoryAsync(Id, 2);
            SpreadsheetDefinition = await ClientSDK.Spreadsheet.GetSpreadsheetDefinitionAsync(Id);
            FifteenMinuteWorksheetDefinition = await ClientSDK.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFifteenMinute);
            HourlyWorksheetDefinition = await ClientSDK.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetHour);
            FourHourWorksheetDefinition = await ClientSDK.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFourHour);
            DailyWorksheetDefinition = await ClientSDK.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetDaily);

            //Merge the Twins
            var allChildTwins = LocationTwins.Union(ColumnTwins).ToList();
            
            AddChildren(DigitalTwinItem, allChildTwins);
            CacheColumns();
            IsInitialized = true;
            return true;
        }
        private void CacheColumns()
        {
            ColumnsByVariable = new Dictionary<long, Column>();
            ColumnsByVarNum = new Dictionary<long, Column>();
            ColumnsById = new Dictionary<long, Column>();
            ColumnsByGuid = new Dictionary<string, Column>();
            foreach (var columnTwin in ColumnTwins)
            {
                Column column = GetColumnByColumnNumber((uint)columnTwin.Id);
                ColumnsById.Add(columnTwin.Id, column);
                ColumnsByGuid.Add(columnTwin.TwinReferenceId, column);
                string variableId = ONE.Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
                long.TryParse(variableId, out var value);
                if (value != 0)
                    ColumnsByVariable.Add(value, column);

                string varNum = ONE.Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
                long.TryParse(variableId, out var varNumValue);
                if (varNumValue != 0)
                    ColumnsByVarNum.Add(varNumValue, column);
            }
        }
        public Column GetColumnByIdentifier(string sId)
        {
            if (string.IsNullOrEmpty(sId))
                return null;
            
            uint.TryParse(sId, out var uId);
            if (uId != 0)
            {
                return GetColumnByColumnNumber(uId);
            }
            if (sId.Length < 15)
            {
                uint.TryParse(sId.Substring(1), out var wimsId);
                if (wimsId > 0)
                {
                    if (sId.ToUpper().StartsWith("V"))
                    {
                        return GetColumnByVariableId(wimsId);
                    }
                    else if (sId.ToUpper().StartsWith("I"))
                    {
                        return GetColumnByVariableId(wimsId);
                    }
                }
            }
            Guid.TryParse(sId, out var gId);
            if (gId != Guid.Empty)
                return GetColumnByGuid(sId);
            return null;
        }
        public Column GetColumnByColumnNumber(uint id)
        {
            if (id == 0)
                return null;
            if (ColumnsById == null)
                return null;
            if (ColumnsById.ContainsKey(id))
                return ColumnsById[id];
            Column column = ONE.Operations.Spreadsheet.Helper.GetColumnByNumber(FifteenMinuteWorksheetDefinition, id);
            if (column != null)
                return column;
            column = ONE.Operations.Spreadsheet.Helper.GetColumnByNumber(HourlyWorksheetDefinition, id);
            if (column != null)
                return column;
            column = ONE.Operations.Spreadsheet.Helper.GetColumnByNumber(FourHourWorksheetDefinition, id);
            if (column != null)
                return column;
            column = ONE.Operations.Spreadsheet.Helper.GetColumnByNumber(DailyWorksheetDefinition, id);
            return column;

        }
        public Column GetColumnByGuid(string id)
        {
            if (ColumnsByGuid == null || string.IsNullOrEmpty(id))
                return null;
            if (ColumnsByGuid.ContainsKey(id))
                return ColumnsByGuid[id];
            return null;
        }
        public DigitalTwin GetColumnTwinByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid) || ColumnTwins == null || ColumnTwins.Count == 0)
                return null;
            var matches = ColumnTwins.Where(c => c.TwinReferenceId.ToUpper() == guid.ToUpper());
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
        public List<Column> GetColumnByName(string name)
        {
           List<Column> columns = new List<Column>();
           
           return columns;

        }
        public List<Column> GetColumnByFullName(string name)
        {
            List<Column> columns = new List<Column>();

            return columns;

        }
        public long GetColumnTwinDataPropertyLong(string guid, string path, string key)
        {
            DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
            if (columnTwin == null)
                return 0;
            return Enterprise.Twin.Helper.GetLongTwinDataProperty(columnTwin, path, key);
        }
        public double GetColumnTwinDataPropertyDouble(string guid, string path, string key)
        {
            DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
            if (columnTwin == null)
                return 0;
            return Enterprise.Twin.Helper.GetDoubleTwinDataProperty(columnTwin, path, key);
        }
        public string GetColumnTwinDataPropertyString(string guid, string path, string key)
        {
            DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
            if (columnTwin == null)
                return "";
            return Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, path, key);
        }
        public DateTime GetColumnTwinDataPropertyDate(string guid, string path, string key)
        {
            DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
            if (columnTwin == null)
                return DateTime.MinValue;
            DateTime.TryParse(Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, path, key), out DateTime dateTime);
            return dateTime;
        }
        public long GetVariableId(string guid)
        {
            DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
            if (columnTwin == null)
                return 0;
            return Enterprise.Twin.Helper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
        }
        public long GetVariableId(DigitalTwin columnTwin)
        {
            return Enterprise.Twin.Helper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
        }
        public Column GetColumnByVariableId(long variableId)
        {
           if (ColumnsByVariable.ContainsKey(variableId))
               return ColumnsByVariable[variableId];
            return null;
        }
        public Column GetColumnByVarNum(long varNum)
        {
            if (ColumnsByVarNum.ContainsKey(varNum))
                return ColumnsByVarNum[varNum];
            return null;
        }
        public string GetTelemetryPath(string digitalTwinReferenceId, bool includeItem)
        {
            if (ItemDictionarybyGuid.ContainsKey(digitalTwinReferenceId))
            {
                if (includeItem)
                {
                    return ItemDictionarybyGuid[digitalTwinReferenceId].Path;
                }
                else
                {
                    if (ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId != null &&
                        ItemDictionarybyGuid.ContainsKey(ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId))
                    {
                        return ItemDictionarybyGuid[ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId].Path;
                    }
                    else if (ItemDictionarybyLong.ContainsKey((long)ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentId))
                    {
                        return ItemDictionarybyLong[(long)ItemDictionarybyGuid[digitalTwinReferenceId].DigitalTwin.ParentId].Path;
                    }
                }
            }
            return "";
        }
        public string GetWorksheetTypeName(DigitalTwin digitalTwin)
        {
            switch (digitalTwin.TwinSubTypeId)
            {
                case Constants.TelemetryCategory.ColumnType.WorksheetFifteenMinute.RefId:
                    return "15 Minutes";
                case Constants.TelemetryCategory.ColumnType.WorksheetHour.RefId:
                    return "Hourly";
                case Constants.TelemetryCategory.ColumnType.WorksheetFourHour.RefId:
                    return "4 Hour";
                case Constants.TelemetryCategory.ColumnType.WorksheetDaily.RefId:
                    return "Daily";
            }

            return "";
        }
        public EnumWorksheet GetWorksheetType(DigitalTwin digitalTwin)
        {
            switch (digitalTwin.TwinSubTypeId)
            {
                case Constants.TelemetryCategory.ColumnType.WorksheetFifteenMinute.RefId:
                    return EnumWorksheet.WorksheetFifteenMinute;
                case Constants.TelemetryCategory.ColumnType.WorksheetHour.RefId:
                    return EnumWorksheet.WorksheetHour;
                case Constants.TelemetryCategory.ColumnType.WorksheetFourHour.RefId:
                    return EnumWorksheet.WorksheetFourHour;
                case Constants.TelemetryCategory.ColumnType.WorksheetDaily.RefId:
                    return EnumWorksheet.WorksheetDaily;
            }

            return EnumWorksheet.WorksheetUnknown;
        }
        public EnumWorksheet GetWorksheetType(Column column)
        {
            DigitalTwin digitalTwin = ONE.Enterprise.Twin.Helper.GetByRef(ColumnTwins, column.ColumnId);
            return GetWorksheetType(digitalTwin);
        }

        private void AddChildren(DigitalTwinItem digitalTwinTreeItem, List<DigitalTwin> digitalTwins)
        {
            var childDigitalTwins = digitalTwins.Where(p => p.ParentId == digitalTwinTreeItem.DigitalTwin.Id);
            foreach (DigitalTwin digitalTwin in childDigitalTwins)
            {
                var childDigitalTwinItem = new DigitalTwinItem(digitalTwin);
                if (string.IsNullOrEmpty(digitalTwinTreeItem.Path))
                    childDigitalTwinItem.Path = childDigitalTwinItem.DigitalTwin.Name;
                else
                    childDigitalTwinItem.Path = $"{digitalTwinTreeItem.Path}{Delimiter}{childDigitalTwinItem.DigitalTwin.Name}";
                digitalTwinTreeItem.ChildDigitalTwinItems.Add(childDigitalTwinItem);
                if (!string.IsNullOrEmpty(digitalTwin.TwinReferenceId))
                    ItemDictionarybyGuid.Add(digitalTwin.TwinReferenceId, childDigitalTwinItem);
                ItemDictionarybyLong.Add(digitalTwin.Id, childDigitalTwinItem);
                AddChildren(childDigitalTwinItem, digitalTwins);
            }
        }
        public string GetColumnGuidByIndex(string index)
        {
            if (!IsInitialized)
                return EnumErrors.ERR_OPERATION_NOT_LOADED.ToString();

            int.TryParse(index, out int idx);
            if (ColumnTwins == null || idx > ColumnTwins.Count - 1 || idx < 0 || ColumnTwins.Count == 0)
                return EnumErrors.ERR_INDEX_OUT_OF_RANGE.ToString();
            else
                return ColumnTwins[idx].TwinReferenceId;
        }
        
        public string GetWimsVarType(Column column)
        {
            var workSheetType = GetWorksheetType(column);
            switch (workSheetType)
            {
                case EnumWorksheet.WorksheetFifteenMinute:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "W";
                        if (!column.IsNumeric)
                            return "Q";
                        return "F";
                    }
                case EnumWorksheet.WorksheetHour:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "N";
                        if (!column.IsNumeric)
                            return "B";
                        return "H";
                    }
                case EnumWorksheet.WorksheetFourHour:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "G";
                        if (!column.IsNumeric)
                            return "E";
                        return "4";
                    }
                case EnumWorksheet.WorksheetDaily:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "C";
                        if (!column.IsNumeric)
                            return "T";
                        return "P";
                    }
            }
            return "";
        }
        public string GetWimsType(Column column)
        {
            var workSheetType = GetWorksheetType(column);
            switch (workSheetType)
            {
                case EnumWorksheet.WorksheetFifteenMinute:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "15 Minute Calc";
                        if (!column.IsNumeric)
                            return "15 Minute Text";
                        return "Daily Detail variable tracked every 15 minutes";
                    }
                case EnumWorksheet.WorksheetHour:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "Hourly Calc";
                        if (!column.IsNumeric)
                            return "Hourly Text";
                        return "Daily Detail variable tracked every hour";
                    }
                case EnumWorksheet.WorksheetFourHour:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "4 hour calc.";
                        if (!column.IsNumeric)
                            return "4 hour text variable";
                        return "Daily Detail variable tracked every 4 hours";
                    }
                case EnumWorksheet.WorksheetDaily:
                    {
                        if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceComputation)
                            return "Daily calculated variable";
                        if (!column.IsNumeric)
                            return "Daily text variable";
                        return "Daily variable / parameter";
                    }
            }
            return "";
        }
        public string Info(string columnIdentifier, string field)
        {
            if (!IsInitialized)
                return EnumErrors.ERR_OPERATION_NOT_LOADED.ToString();

            Column column = GetColumnByIdentifier(columnIdentifier);
            if (column == null)
                return EnumErrors.ERR_INVALID_PARAMETER_GUID.ToString();
            var columnTwin = GetColumnTwinByGuid(column.ColumnId);

            var library = ClientSDK.CacheHelper.LibaryCache;
            Parameter parameter = library.GetParameter(column.ParameterId);

            Unit unit = library.GetUnit((long)column.DisplayUnitId);
            string path = GetTelemetryPath(column.ColumnId, false);
            string[] anscestors = path.Split('\\');

            if (column == null)
                return EnumErrors.ERR_INVALID_PARAMETER_IDENTIFIER.ToString();
            switch (field.ToUpper())
            {
                case "OPERATION":

                    return Name;
                case "NAME":
                    return column.Name;
                case "LOCATION:VARNAME":
                    return $"{path} {column.Name}";
                case "LOCATION.VARNAME":
                    if (anscestors.Length > 0)
                        return $"{anscestors[anscestors.Length - 1]}.{column.Name}";
                    return column.Name;
                case "NAME.UNITS":
                    return $"{column.Name}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
                case "SHORTNAME":
                    return I18NKeyHelper.GetValue("SHORT", parameter.I18NKey);
                case "SHORTNAME.UNITS":
                    return $"{I18NKeyHelper.GetValue("SHORT", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
                case "VARTYPE":
                    return GetWimsVarType(column);
                case "TYPE":
                    return GetWimsType(column);
                case "PARAMETERTYPE":
                    return I18NKeyHelper.GetValue("LONG", parameter.I18NKey);
                case "PARAMETERTYPE.UNITS":
                    return $"{I18NKeyHelper.GetValue("LONG", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
                case "UNITS":
                    return I18NKeyHelper.GetValue("SHORT", unit.I18NKey);
                case "XREF":
                    if (column.DataSourceBinding != null)
                        return $"{column.DataSourceBinding.BindingId} ({column.DataSourceBinding.EnumSamplingStatistic})";
                    else
                        return "";
                case "SCADATAG":
                    if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && !column.DataSourceBinding.BindingId.Contains("@@"))
                        return column.DataSourceBinding.BindingId;
                    else
                        return "";
                case "LIMS_LOC":
                    if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && column.DataSourceBinding.BindingId.Contains("@@"))
                        return column.DataSourceBinding.BindingId.Substring(0, column.DataSourceBinding.BindingId.IndexOf('@'));
                    else
                        return "";
                case "LIMS_TEST":
                    if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && column.DataSourceBinding.BindingId.Contains("@@"))
                        return column.DataSourceBinding.BindingId.Substring(column.DataSourceBinding.BindingId.IndexOf('@') + 2);
                    else
                        return "";
                case "STATISTIC":
                    if (column.DataSourceBinding != null)
                        return column.DataSourceBinding.EnumSamplingStatistic.ToString();
                    else
                        return "";
                case "STORETCODE":
                    if (column.ParameterAgencyCodeIds != null && column.ParameterAgencyCodeIds.Count > 0)
                        foreach (var parameterAgencyCodeId in column.ParameterAgencyCodeIds)
                        {
                            var parameterAgencyCode = library.GetParameterAgencyCode(parameterAgencyCodeId);
                            if (parameterAgencyCode != null)
                            {
                                if (parameterAgencyCode.ParameterAgencyCodeTypeId == "96db9876-5e8a-4133-b206-7575b9de824c")
                                    return parameterAgencyCode.Code;
                            }
                        }
                    return "";
                case "STORETCODEDESC":
                    if (column.ParameterAgencyCodeIds != null && column.ParameterAgencyCodeIds.Count > 0)
                        foreach (var parameterAgencyCodeId in column.ParameterAgencyCodeIds)
                        {
                            var parameterAgencyCode = library.GetParameterAgencyCode(parameterAgencyCodeId);
                            if (parameterAgencyCode != null)
                            {
                                if (parameterAgencyCode.ParameterAgencyCodeTypeId == "96db9876-5e8a-4133-b206-7575b9de824c")
                                    return parameterAgencyCode.Name;
                            }
                        }
                    return "";
                case "STORETCODE-DESC":
                    if (column.ParameterAgencyCodeIds != null && column.ParameterAgencyCodeIds.Count > 0)
                        foreach (var parameterAgencyCodeId in column.ParameterAgencyCodeIds)
                        {
                            var parameterAgencyCode = library.GetParameterAgencyCode(parameterAgencyCodeId);
                            if (parameterAgencyCode != null)
                            {
                                if (parameterAgencyCode.ParameterAgencyCodeTypeId == "96db9876-5e8a-4133-b206-7575b9de824c")
                                    return parameterAgencyCode.Code + "-" + parameterAgencyCode.Name;
                            }
                        }
                    return "";
                case "ENTRYMIN":
                    if (column.Limits != null && column.Limits.Count > 0)
                    {
                        foreach (var limit in column.Limits)
                        {
                            if (limit.EnumLimit == EnumLimit.LimitLownear)
                                if (limit.HighValue != null)
                                    return limit.HighValue.ToString();
                        }
                    }
                    return "";
                case "ENTRYMAX":
                    if (column.Limits != null && column.Limits.Count > 0)
                    {
                        foreach (var limit in column.Limits)
                        {
                            if (limit.EnumLimit == EnumLimit.LimitHighnear)
                                if (limit.LowValue != null)
                                    return limit.LowValue.ToString();
                        }
                    }
                    return "";
                case "PATH":
                    return path;
                case "LOCATION":
                    if (anscestors.Length > 0)
                        return anscestors[anscestors.Length - 1];
                    return "";
                case "PARENT":
                    if (anscestors.Length > 1)
                        return anscestors[anscestors.Length - 2];
                    return "";
                case "GRANDPARENT":
                    if (anscestors.Length > 2)
                        return anscestors[anscestors.Length - 3];
                    return "";
                case "FREQUENCY":
                    return GetWorksheetTypeName(columnTwin);
                case "VARIABLEID":
                    return Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
                case "VARNUM":
                    return Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
                case "DATATABLE":
                case "AREA.LOCATION.VARNAME":
                    return EnumErrors.ERR_DEPRECATED.ToString();

            }
            if (field.ToUpper().StartsWith("LOCATION.LEVEL"))
            {
                int.TryParse(field.Substring(14), out int level);
                if (anscestors.Length >= level)
                {
                    return anscestors[level - 1];
                }
                return "";
            }
            if (field.ToUpper().StartsWith("VARIABLE."))
            {
                string key = field.Substring(9);
                return Enterprise.Twin.Helper.GetTwinDataProperty(columnTwin, "wims\\variable", key);
            }
            return EnumErrors.ERR_NOT_IMPLEMENTED.ToString();
        }
       
        public string GetWorksheetTypeName(string guid)
        {
            if (!IsInitialized)
                return EnumErrors.ERR_OPERATION_NOT_LOADED.ToString();

            var column = GetColumnTwinByGuid(guid);
            if (column == null)
                return EnumErrors.ERR_INVALID_PARAMETER_GUID.ToString();
            return GetWorksheetTypeName(column);
        }
        

    }
}
