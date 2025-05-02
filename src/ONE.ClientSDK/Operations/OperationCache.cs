using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Common.Library;
using ONE.ClientSDK.Enterprise.Twin;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants;
using ONE.Models.CSharp.Constants.TwinCategory;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Operations
{
	public class OperationCache
	{
		public OperationCache(OneApi clientSdk, DigitalTwin digitalTwin)
		{
			ClientSdk = clientSdk;
			DigitalTwin = digitalTwin;
			DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
			ItemDictionaryByGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionaryByLong = new Dictionary<long, DigitalTwinItem>();
			FifteenMinuteRows = new Dictionary<uint, Row>();
			HourlyRows = new Dictionary<uint, Row>();
			FourHourRows = new Dictionary<uint, Row>();
			DailyRows = new Dictionary<uint, Row>();
			MeasurementCache = new Dictionary<string, List<Measurement>>();
			LocationTwins = new List<DigitalTwin>();
			ColumnTwins = new List<DigitalTwin>();
			Users = new List<User>();
			SpreadsheetComputations = new Dictionary<string, SpreadsheetComputation>();
			Sheets = new List<Configuration>();
			Graphs = new List<Configuration>();
			Dashboards = new List<Configuration>();
			ReportCache = new Enterprise.Report.ReportCache(ClientSdk, DigitalTwin.TwinReferenceId);
		}

		public OperationCache(string serializedObject)
		{
			try
			{
				var operationCache = JsonConvert.DeserializeObject<OperationCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				DigitalTwin = operationCache.DigitalTwin;
				DigitalTwinItem = operationCache.DigitalTwinItem;

				SpreadsheetDefinition = operationCache.SpreadsheetDefinition;
				FifteenMinuteWorksheetDefinition = operationCache.FifteenMinuteWorksheetDefinition;
				HourlyWorksheetDefinition = operationCache.HourlyWorksheetDefinition;
				FourHourWorksheetDefinition = operationCache.FourHourWorksheetDefinition;
				DailyWorksheetDefinition = operationCache.DailyWorksheetDefinition;

				FifteenMinuteRows = operationCache.FifteenMinuteRows;
				HourlyRows = operationCache.HourlyRows;
				FourHourRows = operationCache.FourHourRows;
				DailyRows = operationCache.DailyRows;

				MeasurementCache = operationCache.MeasurementCache;

				Delimiter = operationCache.Delimiter;
				IsCached = operationCache.IsCached;

				LocationTwins = operationCache.LocationTwins ?? new List<DigitalTwin>();
				ColumnTwins = operationCache.ColumnTwins ?? new List<DigitalTwin>();
				Users = operationCache.Users;

				SpreadsheetComputations = operationCache.SpreadsheetComputations;
				Sheets = operationCache.Sheets;
				Graphs = operationCache.Graphs;
				Dashboards = operationCache.Dashboards;
				ReportCache = operationCache.ReportCache;
				var allOperationDescendantTwins = LocationTwins.Union(ColumnTwins).ToList();

				ItemDictionaryByGuid = new Dictionary<string, DigitalTwinItem>();
				ItemDictionaryByLong = new Dictionary<long, DigitalTwinItem>();
				AddChildren(DigitalTwinItem, allOperationDescendantTwins);
				CacheColumns();
			}
			catch
			{
				// ignored
			}
		}

		public OperationCache()
		{
			DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
			FifteenMinuteWorksheetDefinition = new WorksheetDefinition();
			HourlyWorksheetDefinition = new WorksheetDefinition();
			FourHourWorksheetDefinition = new WorksheetDefinition();
			DailyWorksheetDefinition = new WorksheetDefinition();
			ItemDictionaryByGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionaryByLong = new Dictionary<long, DigitalTwinItem>();
			FifteenMinuteRows = new Dictionary<uint, Row>();
			HourlyRows = new Dictionary<uint, Row>();
			FourHourRows = new Dictionary<uint, Row>();
			DailyRows = new Dictionary<uint, Row>();
			MeasurementCache = new Dictionary<string, List<Measurement>>();
			LocationTwins = new List<DigitalTwin>();
			ColumnTwins = new List<DigitalTwin>();
			Users = new List<User>();
			SpreadsheetComputations = new Dictionary<string, SpreadsheetComputation>();
			Sheets = new List<Configuration>();
			Graphs = new List<Configuration>();
			Dashboards = new List<Configuration>();
			ReportCache = new Enterprise.Report.ReportCache();
		}

		public void Unload()
		{
			DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
			SpreadsheetDefinition = new SpreadsheetDefinition();
			FifteenMinuteWorksheetDefinition = new WorksheetDefinition();
			HourlyWorksheetDefinition = new WorksheetDefinition();
			FourHourWorksheetDefinition = new WorksheetDefinition();
			DailyWorksheetDefinition = new WorksheetDefinition();
			ItemDictionaryByGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionaryByLong = new Dictionary<long, DigitalTwinItem>();
			FifteenMinuteRows = new Dictionary<uint, Row>();
			HourlyRows = new Dictionary<uint, Row>();
			FourHourRows = new Dictionary<uint, Row>();
			DailyRows = new Dictionary<uint, Row>();
			MeasurementCache = new Dictionary<string, List<Measurement>>();

			ColumnsByVariable = new Dictionary<long, Column>();
			ColumnsByVarNum = new Dictionary<long, Column>();
			ColumnsById = new Dictionary<long, Column>();
			ColumnsByGuid = new Dictionary<string, Column>();

			LocationTwins = new List<DigitalTwin>();
			ColumnTwins = new List<DigitalTwin>();
			Users = new List<User>();
			SpreadsheetComputations = new Dictionary<string, SpreadsheetComputation>();
			Sheets = new List<Configuration>();
			Graphs = new List<Configuration>();
			Dashboards = new List<Configuration>();
			ReportCache = new Enterprise.Report.ReportCache();
		}

		public void SetClientSdk(OneApi clientSdk) => ClientSdk = clientSdk;

		public OneApi GetClientSdk() => ClientSdk;

		public Enterprise.Report.ReportCache ReportCache { get; set; }
		private OneApi ClientSdk { get; set; }
		public string Id => DigitalTwin?.TwinReferenceId;
		public string Name => DigitalTwin?.Name;
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

		public List<User> Users { get; set; }

		private Dictionary<string, DigitalTwinItem> ItemDictionaryByGuid { get; set; }
		private Dictionary<long, DigitalTwinItem> ItemDictionaryByLong { get; set; }

		private Dictionary<long, Column> ColumnsByVariable { get; set; }
		private Dictionary<long, Column> ColumnsByVarNum { get; set; }
		private Dictionary<long, Column> ColumnsById { get; set; }
		private Dictionary<string, Column> ColumnsByGuid { get; set; }

		public Dictionary<string, List<Measurement>> MeasurementCache { get; set; }

		public Dictionary<string, SpreadsheetComputation> SpreadsheetComputations { get; set; }

		public List<Configuration> Sheets { get; set; }
		public List<Configuration> Graphs { get; set; }
		public List<Configuration> Dashboards { get; set; }

		public string Delimiter { get; set; } = "\\";

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
					return FifteenMinuteRows.TryGetValue(rowNumber, out var fifteenMinuteRow) ? fifteenMinuteRow : null;
				case EnumWorksheet.WorksheetHour:
					return HourlyRows.TryGetValue(rowNumber, out var hourlyRow) ? hourlyRow : null;
				case EnumWorksheet.WorksheetFourHour:
					return FourHourRows.TryGetValue(rowNumber, out var fourHourRow) ? fourHourRow : null;
				case EnumWorksheet.WorksheetDaily:
					return DailyRows.TryGetValue(rowNumber, out var dailyRow) ? dailyRow : null;
			}

			return null;
		}

		public bool IsCached { get; set; }
		public async Task<bool> LoadAsync(bool loadChildCaches = false)
		{
			if (IsCached)
				return true;

			try
			{
				var columnTwinsTask = ClientSdk.DigitalTwin.GetDescendantsByTypeAsync(Id, TelemetryConstants.ColumnType.RefId);
				var locationTwinsTask = ClientSdk.DigitalTwin.GetDescendantsByCategoryAsync(Id, 2);
				var spreadsheetDefinitionTask = ClientSdk.Spreadsheet.GetSpreadsheetDefinitionAsync(Id);
				var fifteenMinuteWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFifteenMinute);
				var hourlyWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetHour);
				var fourHourWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFourHour);
				var dailyWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetDaily);
				var usersTask = ClientSdk.Core.GetUsersAsync();
				var sheetsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.WorksheetView.Id, Id);
				var graphsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.Graphs.Id, Id);
				var dashboardsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.Dashboards.Id, Id, OrganizationConstants.TenantType.RefId);

				await Task.WhenAll(
					columnTwinsTask,
					locationTwinsTask,
					spreadsheetDefinitionTask,
					fifteenMinuteWorksheetDefinitionTask,
					hourlyWorksheetDefinitionTask,
					fourHourWorksheetDefinitionTask,
					dailyWorksheetDefinitionTask,
					usersTask,
					sheetsTask,
					graphsTask,
					dashboardsTask
					);

				ColumnTwins = columnTwinsTask.Result ?? new List<DigitalTwin>();
				LocationTwins = locationTwinsTask.Result ?? new List<DigitalTwin>();
				SpreadsheetDefinition = spreadsheetDefinitionTask.Result;
				FifteenMinuteWorksheetDefinition = fifteenMinuteWorksheetDefinitionTask.Result;
				HourlyWorksheetDefinition = hourlyWorksheetDefinitionTask.Result;
				FourHourWorksheetDefinition = fourHourWorksheetDefinitionTask.Result;
				DailyWorksheetDefinition = dailyWorksheetDefinitionTask.Result;
				Users = usersTask.Result;
				Sheets = sheetsTask.Result;
				Graphs = graphsTask.Result;
				Dashboards = dashboardsTask.Result;

				SpreadsheetComputations = new Dictionary<string, SpreadsheetComputation>();
				await LoadComputations(FifteenMinuteWorksheetDefinition);
				await LoadComputations(HourlyWorksheetDefinition);
				await LoadComputations(FourHourWorksheetDefinition);
				await LoadComputations(DailyWorksheetDefinition);

				if (loadChildCaches)
					await ReportCache.LoadAsync();
			}
			catch
			{
				if (ClientSdk.ThrowApiErrors)
					throw;
				return false;
			}

			//Merge the Twins
			var allOperationDescendantTwins = LocationTwins.Union(ColumnTwins).ToList();

			AddChildren(DigitalTwinItem, allOperationDescendantTwins);
			IsCached = true;
			CacheColumns();
			return true;
		}

		private async Task LoadComputations(WorksheetDefinition sourceWorksheetDefinition)
		{
			if (sourceWorksheetDefinition == null) 
				return;

			foreach (var sourceColumn in sourceWorksheetDefinition.Columns)
			{
				if (sourceColumn.DataSourceBinding?.DataSource != EnumDataSource.DatasourceComputation ||
					string.IsNullOrEmpty(sourceColumn.DataSourceBinding.BindingId))
				{
					continue;
				}

				var sourceSpreadsheetComputation = await ClientSdk.Spreadsheet.ComputationGetOneAsync(DigitalTwin.TwinReferenceId, sourceWorksheetDefinition.EnumWorksheet, sourceColumn.DataSourceBinding.BindingId);

				if (sourceSpreadsheetComputation != null && !SpreadsheetComputations.ContainsKey(sourceColumn.DataSourceBinding.BindingId))
				{
					SpreadsheetComputations.Add(sourceColumn.DataSourceBinding.BindingId, sourceSpreadsheetComputation);
				}
			}
		}

		public void CacheColumns()
		{
			if (!IsCached)
				return;

			ColumnsByVariable = new Dictionary<long, Column>();
			ColumnsByVarNum = new Dictionary<long, Column>();
			ColumnsById = new Dictionary<long, Column>();
			ColumnsByGuid = new Dictionary<string, Column>();
			foreach (var columnTwin in ColumnTwins)
			{
				var column = GetColumnByColumnNumber((uint)columnTwin.Id);
				ColumnsById.Add(columnTwin.Id, column);
				ColumnsByGuid.Add(columnTwin.TwinReferenceId, column);
				var variableId = DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
				long.TryParse(variableId, out var value);
				if (value != 0 && !ColumnsByVariable.ContainsKey(value))
					ColumnsByVariable.Add(value, column);

				var varNum = DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
				long.TryParse(variableId, out var varNumValue);
				if (varNumValue != 0 && !ColumnsByVarNum.ContainsKey(varNumValue))
					ColumnsByVarNum.Add(varNumValue, column);
			}
		}

		public User GetUser(string userId)
		{
			if (Users == null)
				return null;

			try
			{
				return Users.FirstOrDefault(p => string.Equals(p.Id, userId, StringComparison.CurrentCulture));
			}
			catch
			{
				return null;
			}
		}

		public Column GetColumnByIdentifier(string sId)
		{
			if (string.IsNullOrEmpty(sId))
				return null;

			uint.TryParse(sId, out var uId);
			if (uId != 0)
				return GetColumnByColumnNumber(uId);
			
			if (sId.Length < 15)
			{
				uint.TryParse(sId.Substring(1), out var wimsId);

				if (wimsId > 0)
					if (sId.ToUpper().StartsWith("V") || sId.ToUpper().StartsWith("I"))
						return GetColumnByVariableId(wimsId);
			}

			Guid.TryParse(sId, out var gId);
			
			return gId != Guid.Empty ? GetColumnByGuid(sId) : null;
		}
		public Column GetColumnByColumnNumber(uint id)
		{
			if (id == 0 || ColumnsById == null)
				return null;
			
			if (ColumnsById.TryGetValue(id, out var column))
				return column;

			column = Spreadsheet.SpreadsheetHelper.GetColumnByNumber(FifteenMinuteWorksheetDefinition, id);
			
			if (column != null)
				return column;

			column = Spreadsheet.SpreadsheetHelper.GetColumnByNumber(HourlyWorksheetDefinition, id);
			
			if (column != null)
				return column;

			column = Spreadsheet.SpreadsheetHelper.GetColumnByNumber(FourHourWorksheetDefinition, id);
			
			if (column != null)
				return column;

			column = Spreadsheet.SpreadsheetHelper.GetColumnByNumber(DailyWorksheetDefinition, id);
			
			return column;
		}

		public Column GetColumnByGuid(string id)
		{
			if (ColumnsByGuid == null || string.IsNullOrEmpty(id))
				return null;

			return ColumnsByGuid.TryGetValue(id, out var column) ? column : null;
		}

		public DigitalTwin GetColumnTwinByGuid(string guid)
		{
			if (string.IsNullOrEmpty(guid) || ColumnTwins == null || ColumnTwins.Count == 0)
				return null;

			return ColumnTwins.FirstOrDefault(c => string.Equals(c.TwinReferenceId, guid, StringComparison.CurrentCultureIgnoreCase));
		}

		public DigitalTwin GetLocationTwinByGuid(string guid)
		{
			if (string.IsNullOrEmpty(guid) || LocationTwins == null || LocationTwins.Count == 0)
				return null;

			return LocationTwins.FirstOrDefault(c => string.Equals(c.TwinReferenceId, guid, StringComparison.CurrentCultureIgnoreCase));
		}

		public long GetColumnTwinDataPropertyLong(string guid, string path, string key)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? 0 : DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, path, key);
		}

		public double GetColumnTwinDataPropertyDouble(string guid, string path, string key)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? 0d : DigitalTwinHelper.GetDoubleTwinDataProperty(columnTwin, path, key);
		}

		public string GetColumnTwinDataPropertyString(string guid, string path, string key)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? string.Empty : DigitalTwinHelper.GetTwinDataProperty(columnTwin, path, key);
		}

		public DateTime GetColumnTwinDataPropertyDate(string guid, string path, string key)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? DateTime.MinValue : DigitalTwinHelper.GetDateTimeTwinDataProperty(columnTwin, path, key);
		}

		public async Task<bool> UpdateTwinDataAsync(string key, string value)
		{
			var jsonPatchDocument = new JsonPatchDocument();
			var existingTwinData = new JObject();

			if (!string.IsNullOrEmpty(DigitalTwin.TwinData))
				existingTwinData = JObject.Parse(DigitalTwin.TwinData);

			jsonPatchDocument = DigitalTwinHelper.UpdateJsonDataField(DigitalTwin, jsonPatchDocument, existingTwinData, key, value);
			var digitalTwin = await ClientSdk.DigitalTwin.UpdateTwinDataAsync(DigitalTwin.TwinReferenceId, jsonPatchDocument);

			if (digitalTwin != null)
			{
				DigitalTwin = digitalTwin;
				return true;
			}
			return false;
		}

		public long GetTwinDataPropertyLong(string path, string key) => DigitalTwinHelper.GetLongTwinDataProperty(DigitalTwin, path, key);

		public double GetTwinDataPropertyDouble(string path, string key) => DigitalTwinHelper.GetDoubleTwinDataProperty(DigitalTwin, path, key);

		public string GetTwinDataPropertyString(string guid, string path, string key) => DigitalTwinHelper.GetTwinDataProperty(DigitalTwin, path, key);

		public DateTime GetTwinDataPropertyDate(string path, string key) => DigitalTwinHelper.GetDateTimeTwinDataProperty(DigitalTwin, path, key);

		public long GetVariableId(string guid)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? 0 : DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
		}

		public long GetVariableId(DigitalTwin columnTwin) => DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");

		public long GetVariableNum(string guid)
		{
			var columnTwin = GetColumnTwinByGuid(guid);

			return columnTwin == null ? 0 : DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
		}

		public long GetVariableNum(DigitalTwin columnTwin) => DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VarNum");

		public Column GetColumnByVariableId(long variableId) => ColumnsByVariable.TryGetValue(variableId, out var column) ? column : null;

		public Column GetColumnByVarNum(long varNum) => ColumnsByVarNum.TryGetValue(varNum, out var column) ? column : null;

		public string GetTelemetryPath(string digitalTwinReferenceId, bool includeItem)
		{
			if (ItemDictionaryByGuid.ContainsKey(digitalTwinReferenceId))
			{
				if (includeItem)
					return ItemDictionaryByGuid[digitalTwinReferenceId].Path;

				if (ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId != null &&
				    ItemDictionaryByGuid.ContainsKey(ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId))
				{
					return ItemDictionaryByGuid[ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentTwinReferenceId].Path;
				}

				if (ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId.HasValue &&
				    ItemDictionaryByLong.ContainsKey((long)ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId))
				{
					return ItemDictionaryByLong[(long)ItemDictionaryByGuid[digitalTwinReferenceId].DigitalTwin.ParentId].Path;
				}
			}

			return string.Empty;
		}

		public string GetWorksheetTypeName(DigitalTwin digitalTwin)
		{
			switch (digitalTwin.TwinSubTypeId)
			{
				case TelemetryConstants.ColumnType.WorksheetFifteenMinute.RefId:
					return "15 Minutes";
				case TelemetryConstants.ColumnType.WorksheetHour.RefId:
					return "Hourly";
				case TelemetryConstants.ColumnType.WorksheetFourHour.RefId:
					return "4 Hour";
				case TelemetryConstants.ColumnType.WorksheetDaily.RefId:
					return "Daily";
			}

			return "";
		}

		public EnumWorksheet GetWorksheetType(DigitalTwin digitalTwin)
		{
			switch (digitalTwin.TwinSubTypeId)
			{
				case TelemetryConstants.ColumnType.WorksheetFifteenMinute.RefId:
					return EnumWorksheet.WorksheetFifteenMinute;
				case TelemetryConstants.ColumnType.WorksheetHour.RefId:
					return EnumWorksheet.WorksheetHour;
				case TelemetryConstants.ColumnType.WorksheetFourHour.RefId:
					return EnumWorksheet.WorksheetFourHour;
				case TelemetryConstants.ColumnType.WorksheetDaily.RefId:
					return EnumWorksheet.WorksheetDaily;
			}

			return EnumWorksheet.WorksheetUnknown;
		}

		public EnumWorksheet GetWorksheetType(Column column)
		{
			var digitalTwin = DigitalTwinHelper.GetByRef(ColumnTwins, column.ColumnId);
			return GetWorksheetType(digitalTwin);
		}

		public void AddChildren(DigitalTwinItem digitalTwinTreeItem, List<DigitalTwin> digitalTwins)
		{
			var childDigitalTwins = digitalTwins.Where(p => p.ParentId == digitalTwinTreeItem.DigitalTwin.Id);
			foreach (var digitalTwin in childDigitalTwins)
			{
				var childDigitalTwinItem = new DigitalTwinItem(digitalTwin);
				if (string.IsNullOrEmpty(digitalTwinTreeItem.Path))
					childDigitalTwinItem.Path = childDigitalTwinItem.DigitalTwin.Name;
				else
					childDigitalTwinItem.Path = $"{digitalTwinTreeItem.Path}{Delimiter}{childDigitalTwinItem.DigitalTwin.Name}";
				digitalTwinTreeItem.ChildDigitalTwinItems.Add(childDigitalTwinItem);
				if (!string.IsNullOrEmpty(digitalTwin.TwinReferenceId) && !ItemDictionaryByGuid.ContainsKey(digitalTwin.TwinReferenceId))
					ItemDictionaryByGuid.Add(digitalTwin.TwinReferenceId, childDigitalTwinItem);
				if (!ItemDictionaryByLong.ContainsKey(digitalTwin.Id))
					ItemDictionaryByLong.Add(digitalTwin.Id, childDigitalTwinItem);
				AddChildren(childDigitalTwinItem, digitalTwins);
			}
		}

		public string GetColumnGuidByIndex(string index)
		{
			if (!IsCached)
				return nameof(EnumErrors.ERR_OPERATION_NOT_LOADED);

			int.TryParse(index, out var idx);
			if (ColumnTwins == null || idx > ColumnTwins.Count - 1 || idx < 0 || ColumnTwins.Count == 0)
				return nameof(EnumErrors.ERR_INDEX_OUT_OF_RANGE);

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

		public string Info(string columnIdentifier, string field, LibraryCache library = null)
		{
			if (!IsCached)
				return nameof(EnumErrors.ERR_OPERATION_NOT_LOADED);
			if (library == null && ClientSdk != null && ClientSdk.CacheHelper != null && ClientSdk.CacheHelper.LibraryCache != null)
				library = ClientSdk.CacheHelper.LibraryCache;


			var column = GetColumnByIdentifier(columnIdentifier);
			if (column == null)
				return nameof(EnumErrors.ERR_INVALID_PARAMETER_GUID);
			var columnTwin = GetColumnTwinByGuid(column.ColumnId);

			Parameter parameter = null;
			Unit unit = null;
			if (library != null)
			{
				parameter = library.GetParameter(column.ParameterId);
				unit = library.GetUnit((long)column.DisplayUnitId);
			}
			var path = GetTelemetryPath(column.ColumnId, false);
			var ancestors = path.Split('\\');

			if (column == null)
				return nameof(EnumErrors.ERR_INVALID_PARAMETER_IDENTIFIER);

			switch (field.ToUpper())
			{
				case "OPERATION":

					return Name;
				case "NAME":
					return column.Name;
				case "LOCATION:VARNAME":
					return $"{path} {column.Name}";
				case "LOCATION.VARNAME":
					return ancestors.Length > 0 ? $"{ancestors[ancestors.Length - 1]}.{column.Name}" : column.Name;
				case "NAME.UNITS":
					return library == null ? nameof(EnumErrors.ERR_NOT_AUTHENTICATED) : $"{column.Name}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "SHORTNAME":
					return library == null ? nameof(EnumErrors.ERR_NOT_AUTHENTICATED) : I18NKeyHelper.GetValue("SHORT", parameter.I18NKey);
				case "SHORTNAME.UNITS":
					return library == null 
						? nameof(EnumErrors.ERR_NOT_AUTHENTICATED)
						: $"{I18NKeyHelper.GetValue("SHORT", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "VARTYPE":
					return GetWimsVarType(column);
				case "TYPE":
					return GetWimsType(column);
				case "PARAMETERTYPE":
					return library == null ? nameof(EnumErrors.ERR_NOT_AUTHENTICATED) : I18NKeyHelper.GetValue("LONG", parameter.I18NKey);
				case "PARAMETERTYPE.UNITS":
					if (library == null)
						return nameof(EnumErrors.ERR_NOT_AUTHENTICATED);
					return $"{I18NKeyHelper.GetValue("LONG", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "UNITS":
					if (library == null)
						return nameof(EnumErrors.ERR_NOT_AUTHENTICATED);
					return I18NKeyHelper.GetValue("SHORT", unit.I18NKey);
				case "XREF":
					return column.DataSourceBinding != null ? $"{column.DataSourceBinding.BindingId} ({column.DataSourceBinding.EnumSamplingStatistic})" : "";
				case "SCADATAG":
					if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && !column.DataSourceBinding.BindingId.Contains("@@"))
						return column.DataSourceBinding.BindingId;
					return "";
				case "LIMS_LOC":
					if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && column.DataSourceBinding.BindingId.Contains("@@"))
						return column.DataSourceBinding.BindingId.Substring(0, column.DataSourceBinding.BindingId.IndexOf('@'));
					return "";
				case "LIMS_TEST":
					if (column.DataSourceBinding != null && column.DataSourceBinding.DataSource == EnumDataSource.DatasourceImport && column.DataSourceBinding.BindingId.Contains("@@"))
						return column.DataSourceBinding.BindingId.Substring(column.DataSourceBinding.BindingId.IndexOf('@') + 2);
					return "";
				case "STATISTIC":
					return column.DataSourceBinding != null ? column.DataSourceBinding.EnumSamplingStatistic.ToString() : "";
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
					return ancestors.Length > 0 ? ancestors[ancestors.Length - 1] : "";
				case "PARENT":
					return ancestors.Length > 1 ? ancestors[ancestors.Length - 2] : "";
				case "GRANDPARENT":
					return ancestors.Length > 2 ? ancestors[ancestors.Length - 3] : "";
				case "FREQUENCY":
					return GetWorksheetTypeName(columnTwin);
				case "VARIABLEID":
					return DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
				case "VARNUM":
					return DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
				case "DATATABLE":
				case "AREA.LOCATION.VARNAME":
					return nameof(EnumErrors.ERR_DEPRECATED);

			}
			if (field.ToUpper().StartsWith("LOCATION.LEVEL"))
			{
				int.TryParse(field.Substring(14), out var level);
				return ancestors.Length >= level ? ancestors[level - 1] : "";
			}
			if (field.ToUpper().StartsWith("VARIABLE."))
			{
				var key = field.Substring(9);
				return DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", key);
			}
			return nameof(EnumErrors.ERR_NOT_IMPLEMENTED);
		}

		public string GetWorksheetTypeName(string guid)
		{
			if (!IsCached)
				return nameof(EnumErrors.ERR_OPERATION_NOT_LOADED);

			var column = GetColumnTwinByGuid(guid);
			return column == null ? nameof(EnumErrors.ERR_INVALID_PARAMETER_GUID) : GetWorksheetTypeName(column);
		}

		public override string ToString()
		{
			try
			{
				return JsonExtensions.SerializeObjectNoCache(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			catch
			{
				return base.ToString();
			}
		}
	}
}
