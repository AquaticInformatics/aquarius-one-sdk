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

namespace ONE.ClientSDK.Operations
{
	public class OperationCache
	{
		public OperationCache(OneApi clientSdk, DigitalTwin digitalTwin)
		{
			ClientSdk = clientSdk;
			DigitalTwin = digitalTwin;
			DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
			ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
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
				var allOperationDecendentTwins = LocationTwins.Union(ColumnTwins).ToList();

				ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
				ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
				AddChildren(DigitalTwinItem, allOperationDecendentTwins);
				CacheColumns();
			}
			catch
			{
			}
		}
		public OperationCache()
		{
			DigitalTwinItem = new DigitalTwinItem(DigitalTwin);
			FifteenMinuteWorksheetDefinition = new WorksheetDefinition();
			HourlyWorksheetDefinition = new WorksheetDefinition();
			FourHourWorksheetDefinition = new WorksheetDefinition();
			DailyWorksheetDefinition = new WorksheetDefinition();
			ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
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
			ItemDictionarybyGuid = new Dictionary<string, DigitalTwinItem>();
			ItemDictionarybyLong = new Dictionary<long, DigitalTwinItem>();
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
		public void SetClientSDK(OneApi clientSDK)
		{
			ClientSdk = clientSDK;
		}
		public OneApi GetClientSDK()
		{
			return ClientSdk;
		}
		public Enterprise.Report.ReportCache ReportCache { get; set; }
		private OneApi ClientSdk { get; set; }
		public string Id
		{
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

		public List<User> Users { get; set; }

		private Dictionary<string, DigitalTwinItem> ItemDictionarybyGuid { get; set; }
		private Dictionary<long, DigitalTwinItem> ItemDictionarybyLong { get; set; }

		private Dictionary<long, Column> ColumnsByVariable { get; set; }
		private Dictionary<long, Column> ColumnsByVarNum { get; set; }
		private Dictionary<long, Column> ColumnsById { get; set; }
		private Dictionary<string, Column> ColumnsByGuid { get; set; }

		public Dictionary<string, List<Measurement>> MeasurementCache { get; set; }

		public Dictionary<string, SpreadsheetComputation> SpreadsheetComputations { get; set; }

		public List<Configuration> Sheets { get; set; }
		public List<Configuration> Graphs { get; set; }
		public List<Configuration> Dashboards { get; set; }

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


		public bool IsCached { get; set; }
		public async Task<bool> LoadAsync(bool loadChildCaches = false)
		{
			if (IsCached)
				return true;

			try
			{
				var ColumnTwinsTask = ClientSdk.DigitalTwin.GetDescendantsByTypeAsync(Id, TelemetryConstants.ColumnType.RefId);
				var LocationTwinsTask = ClientSdk.DigitalTwin.GetDescendantsByCategoryAsync(Id, 2);
				var SpreadsheetDefinitionTask = ClientSdk.Spreadsheet.GetSpreadsheetDefinitionAsync(Id);
				var FifteenMinuteWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFifteenMinute);
				var HourlyWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetHour);
				var FourHourWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetFourHour);
				var DailyWorksheetDefinitionTask = ClientSdk.Spreadsheet.GetWorksheetDefinitionAsync(Id, EnumWorksheet.WorksheetDaily);
				var UsersTask = ClientSdk.Core.GetUsersAsync();
				var SheetsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.WorksheetView.Id, Id);
				var GraphsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.Graphs.Id, Id);
				var DashboardsTask = ClientSdk.Configuration.GetConfigurationsAsync(ConfigurationTypeConstants.Dashboards.Id, Id, OrganizationConstants.TenantType.RefId);

				await Task.WhenAll(
					ColumnTwinsTask,
					LocationTwinsTask,
					SpreadsheetDefinitionTask,
					FifteenMinuteWorksheetDefinitionTask,
					HourlyWorksheetDefinitionTask,
					FourHourWorksheetDefinitionTask,
					DailyWorksheetDefinitionTask,
					UsersTask,
					SheetsTask,
					GraphsTask,
					DashboardsTask
					);

				ColumnTwins = ColumnTwinsTask.Result ?? new List<DigitalTwin>();
				LocationTwins = LocationTwinsTask.Result ?? new List<DigitalTwin>();
				SpreadsheetDefinition = SpreadsheetDefinitionTask.Result;
				FifteenMinuteWorksheetDefinition = FifteenMinuteWorksheetDefinitionTask.Result;
				HourlyWorksheetDefinition = HourlyWorksheetDefinitionTask.Result;
				FourHourWorksheetDefinition = FourHourWorksheetDefinitionTask.Result;
				DailyWorksheetDefinition = DailyWorksheetDefinitionTask.Result;
				Users = UsersTask.Result;
				Sheets = SheetsTask.Result;
				Graphs = GraphsTask.Result;
				Dashboards = DashboardsTask.Result;

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
			var allOperationDecendentTwins = LocationTwins.Union(ColumnTwins).ToList();

			AddChildren(DigitalTwinItem, allOperationDecendentTwins);
			IsCached = true;
			CacheColumns();
			return true;
		}

		private async Task<bool> LoadComputations(WorksheetDefinition sourceWorksheetDefinition)
		{
			if (sourceWorksheetDefinition == null) 
				return true;

			foreach (var sourceColumn in sourceWorksheetDefinition.Columns)
			{
				if (sourceColumn.DataSourceBinding?.DataSource != EnumDataSource.DatasourceComputation ||
					string.IsNullOrEmpty(sourceColumn.DataSourceBinding.BindingId)) 
					continue;

				var sourceSpreadsheetComputation = await ClientSdk.Spreadsheet.ComputationGetOneAsync(DigitalTwin.TwinReferenceId, sourceWorksheetDefinition.EnumWorksheet, sourceColumn.DataSourceBinding.BindingId);

				if (sourceSpreadsheetComputation != null && !SpreadsheetComputations.ContainsKey(sourceColumn.DataSourceBinding.BindingId))
				{
					SpreadsheetComputations.Add(sourceColumn.DataSourceBinding.BindingId, sourceSpreadsheetComputation);
				}
			}

			return true;
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
				Column column = GetColumnByColumnNumber((uint)columnTwin.Id);
				ColumnsById.Add(columnTwin.Id, column);
				ColumnsByGuid.Add(columnTwin.TwinReferenceId, column);
				string variableId = DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
				long.TryParse(variableId, out var value);
				if (value != 0 && !ColumnsByVariable.ContainsKey(value))
					ColumnsByVariable.Add(value, column);

				string varNum = DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
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
				var matches = Users.Where(p => String.Equals(p.Id, userId, StringComparison.CurrentCulture));
				if (matches.Count() > 0)
				{
					return matches.First();
				}
				else
				{
					return null;
				}
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
			Column column = Spreadsheet.SpreadsheetHelper.GetColumnByNumber(FifteenMinuteWorksheetDefinition, id);
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
		public DigitalTwin GetLocationTwinByGuid(string guid)
		{
			if (string.IsNullOrEmpty(guid) || LocationTwins == null || LocationTwins.Count == 0)
				return null;
			var matches = LocationTwins.Where(c => c.TwinReferenceId.ToUpper() == guid.ToUpper());
			if (matches.Count() > 0)
				return matches.First();
			return null;
		}
		public long GetColumnTwinDataPropertyLong(string guid, string path, string key)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			if (columnTwin == null)
				return 0;
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, path, key);
		}
		public double GetColumnTwinDataPropertyDouble(string guid, string path, string key)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			if (columnTwin == null)
				return 0;
			return Enterprise.Twin.DigitalTwinHelper.GetDoubleTwinDataProperty(columnTwin, path, key);
		}
		public string GetColumnTwinDataPropertyString(string guid, string path, string key)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			if (columnTwin == null)
				return "";
			return Enterprise.Twin.DigitalTwinHelper.GetTwinDataProperty(columnTwin, path, key);
		}
		public DateTime GetColumnTwinDataPropertyDate(string guid, string path, string key)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			return columnTwin == null ? DateTime.MinValue : DigitalTwinHelper.GetDateTimeTwinDataProperty(columnTwin, path, key);
		}
		public async Task<bool> UpdateTwinDataAsync(string key, string value)
		{
			JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
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
		public long GetTwinDataPropertyLong(string path, string key)
		{
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(DigitalTwin, path, key);
		}
		public double GetTwinDataPropertyDouble(string path, string key)
		{
			return Enterprise.Twin.DigitalTwinHelper.GetDoubleTwinDataProperty(DigitalTwin, path, key);
		}
		public string GetTwinDataPropertyString(string guid, string path, string key)
		{
			return Enterprise.Twin.DigitalTwinHelper.GetTwinDataProperty(DigitalTwin, path, key);
		}
		public DateTime GetTwinDataPropertyDate(string path, string key) => DigitalTwinHelper.GetDateTimeTwinDataProperty(DigitalTwin, path, key);

		public long GetVariableId(string guid)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			if (columnTwin == null)
				return 0;
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
		}
		public long GetVariableId(DigitalTwin columnTwin)
		{
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
		}
		public long GetVariableNum(string guid)
		{
			DigitalTwin columnTwin = GetColumnTwinByGuid(guid);
			if (columnTwin == null)
				return 0;
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
		}
		public long GetVariableNum(DigitalTwin columnTwin)
		{
			return Enterprise.Twin.DigitalTwinHelper.GetLongTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
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
			DigitalTwin digitalTwin = DigitalTwinHelper.GetByRef(ColumnTwins, column.ColumnId);
			return GetWorksheetType(digitalTwin);
		}

		public void AddChildren(DigitalTwinItem digitalTwinTreeItem, List<DigitalTwin> digitalTwins)
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
				if (!string.IsNullOrEmpty(digitalTwin.TwinReferenceId) && !ItemDictionarybyGuid.ContainsKey(digitalTwin.TwinReferenceId))
					ItemDictionarybyGuid.Add(digitalTwin.TwinReferenceId, childDigitalTwinItem);
				if (!ItemDictionarybyLong.ContainsKey(digitalTwin.Id))
					ItemDictionarybyLong.Add(digitalTwin.Id, childDigitalTwinItem);
				AddChildren(childDigitalTwinItem, digitalTwins);
			}
		}
		public string GetColumnGuidByIndex(string index)
		{
			if (!IsCached)
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
		public string Info(string columnIdentifier, string field, LibraryCache library = null)
		{
			if (!IsCached)
				return EnumErrors.ERR_OPERATION_NOT_LOADED.ToString();
			if (library == null && ClientSdk != null && ClientSdk.CacheHelper != null && ClientSdk.CacheHelper.LibraryCache != null)
				library = ClientSdk.CacheHelper.LibraryCache;


			Column column = GetColumnByIdentifier(columnIdentifier);
			if (column == null)
				return EnumErrors.ERR_INVALID_PARAMETER_GUID.ToString();
			var columnTwin = GetColumnTwinByGuid(column.ColumnId);

			Parameter parameter = null;
			Unit unit = null;
			if (library != null)
			{
				parameter = library.GetParameter(column.ParameterId);
				unit = library.GetUnit((long)column.DisplayUnitId);
			}
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
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
					return $"{column.Name}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "SHORTNAME":
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
					return I18NKeyHelper.GetValue("SHORT", parameter.I18NKey);
				case "SHORTNAME.UNITS":
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
					return $"{I18NKeyHelper.GetValue("SHORT", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "VARTYPE":
					return GetWimsVarType(column);
				case "TYPE":
					return GetWimsType(column);
				case "PARAMETERTYPE":
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
					return I18NKeyHelper.GetValue("LONG", parameter.I18NKey);
				case "PARAMETERTYPE.UNITS":
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
					return $"{I18NKeyHelper.GetValue("LONG", parameter.I18NKey)}" + " {" + $"{I18NKeyHelper.GetValue("SHORT", unit.I18NKey)}" + "}";
				case "UNITS":
					if (library == null)
						return EnumErrors.ERR_NOT_AUTHENTICATED.ToString();
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
					return Enterprise.Twin.DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VariableId");
				case "VARNUM":
					return Enterprise.Twin.DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", "VarNum");
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
				return Enterprise.Twin.DigitalTwinHelper.GetTwinDataProperty(columnTwin, "wims\\variable", key);
			}
			return EnumErrors.ERR_NOT_IMPLEMENTED.ToString();
		}

		public string GetWorksheetTypeName(string guid)
		{
			if (!IsCached)
				return EnumErrors.ERR_OPERATION_NOT_LOADED.ToString();

			var column = GetColumnTwinByGuid(guid);
			if (column == null)
				return EnumErrors.ERR_INVALID_PARAMETER_GUID.ToString();
			return GetWorksheetTypeName(column);
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
