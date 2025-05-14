using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using ONE.ClientSDK.Operations.Spreadsheet;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Constants.TwinCategory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ONE.ClientSDK.Enterprise.Twin.DigitalTwinHelper;

namespace ONE.ClientSDK.Operations
{
	public class SpreadsheetDataImport
	{
		private readonly OneApi _clientSdk;
		private readonly OperationCache _operationCache;
		private readonly CloneOperation _cloneOperation;
		private readonly string _operationId;
		private readonly string _tenantId;
		private List<DigitalTwin> _columnTwins;
		private List<DigitalTwin> _userTwins;
		private List<User> _users;
		private Dictionary<uint, uint> _columnMap;
		private Dictionary<string, User> _userMap;
		public SpreadsheetDataImport(OneApi clientSdk, OperationCache operationCache, string operationId, string tenantId)
		{
			_clientSdk = clientSdk;
			_operationCache = operationCache;
			_operationId = operationId;
			_tenantId = tenantId;
			_cloneOperation = new CloneOperation(_clientSdk, _operationCache);
		}
		public async Task<bool> ImportAsync(EnumWorksheet worksheetType, string json)
		{
			
			//if (!File.Exists(filename))
			//    return false;
			//var worksheetType = GetWorksheetType(filename);
			if (worksheetType == EnumWorksheet.WorksheetUnknown || string.IsNullOrEmpty(json))
				return false;

			if (_columnTwins == null)
			{
				_columnTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(_operationId, TelemetryConstants.ColumnType.RefId);
			}
			_columnMap = await _cloneOperation.GetSourceToDestinationColumnMap(_operationId);

			if (_userTwins == null)
				_userTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(_tenantId, OrganizationConstants.UserType.RefId);
			if (_users == null)
				_users = await _clientSdk.Core.GetUsersAsync();
			CreateUserMap();
			//string json = File.ReadAllText(filename);
			Rows exportedRows = JsonConvert.DeserializeObject<Rows>(json, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

			// Get the worksheet definition so that we can use the Column bindings for any computed Cell
			var worksheetDefinition = await _clientSdk.Spreadsheet.GetWorksheetDefinitionAsync(_operationId, worksheetType);


			foreach (var exportedRow in exportedRows.Items)
			{
				Rows importRows = new Rows();
				importRows.Items.Add(exportedRow.Key, await ConvertRowAsync(exportedRow.Value, worksheetDefinition));
				var succcess = await _clientSdk.Spreadsheet.SaveRowsAsync(importRows, _operationId, worksheetType);
				if (!succcess)
					return false;
			}

			return true;
		}
		public async Task<Dictionary<string, User>> PreviewImportFileAsync(string filename, Dictionary<string, User> userMap)
		{
			_userMap = userMap;
			if (!File.Exists(filename))
				return null;
			var worksheetType = GetWorksheetType(filename);
			if (worksheetType == EnumWorksheet.WorksheetUnknown)
				return null;

			if (_columnTwins == null)
			{
				_columnTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(_operationId, TelemetryConstants.ColumnType.RefId);
			}
			_columnMap = await _cloneOperation.GetSourceToDestinationColumnMap(_operationId);

			if (_userTwins == null)
				_userTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(_tenantId, OrganizationConstants.UserType.RefId);
			if (_users == null)
				_users = await _clientSdk.Core.GetUsersAsync();
			CreateUserMap();
			string json = File.ReadAllText(filename);
			Rows exportedRows = JsonConvert.DeserializeObject<Rows>(json, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

			foreach (var exportedRow in exportedRows.Items)
			{
				Rows importRows = new Rows();
				importRows.Items.Add(exportedRow.Key, PreviewConvertRow(exportedRow.Value));
			}

			return _userMap;
		}
		public EnumWorksheet GetWorksheetType(string filename)
		{
			if (filename.Contains("Daily"))
				return EnumWorksheet.WorksheetDaily;
			if (filename.Contains("FourHour"))
				return EnumWorksheet.WorksheetFourHour;
			if (filename.Contains("Hourly"))
				return EnumWorksheet.WorksheetHour;
			if (filename.Contains("FifteenMinute"))
				return EnumWorksheet.WorksheetFifteenMinute;
			return EnumWorksheet.WorksheetUnknown;
		}
		public User GetUser(string userId)
		{
			if (_users == null)
				return null;
			try
			{
				var matches = _users.Where(p => String.Equals(p.Id, userId, StringComparison.CurrentCulture));
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
				if (_clientSdk.ThrowApiErrors)
					throw;
				return null;
			}
		}
		private void CreateUserMap()
		{
			if (_userMap == null)
				_userMap = new Dictionary<string, User>();
			if (_userTwins != null)
			{
				foreach (var userTwin in _userTwins)
				{
					string cloneId = GetTwinDataProperty(userTwin, "", "CloneId");
					if (!string.IsNullOrEmpty(cloneId))
					{
						var user = GetUser(userTwin.TwinReferenceId);
						if (user != null && !_userMap.ContainsKey(cloneId))
							_userMap.Add(cloneId, user);
					}
				}
			}
		}

		private async Task<Row> ConvertRowAsync(Row exportedRow, WorksheetDefinition worksheetDefinition)
		{
			Row importRow = new Row();
			importRow.RowNumber = exportedRow.RowNumber;
			foreach (Cell cell in exportedRow.Cells)
			{
				importRow.Cells.Add(await ConvertCellAsync(cell, worksheetDefinition));
			}
			return importRow;
		}
		private Row PreviewConvertRow(Row exportedRow)
		{
			Row importRow = new Row();
			importRow.RowNumber = exportedRow.RowNumber;
			foreach (Cell cell in exportedRow.Cells)
			{
				importRow.Cells.Add(PreviewConvertCell(cell));
			}
			return importRow;
		}
		private async Task<Cell> ConvertCellAsync(Cell exportedCell, WorksheetDefinition worksheetDefinition)
		{
			Cell cell = new Cell();
			cell.ColumnNumber = _columnMap[exportedCell.ColumnNumber];
			var column = SpreadsheetHelper.GetColumnByNumber(worksheetDefinition, cell.ColumnNumber);
			if (column.DataSourceBinding?.DataSource == EnumDataSource.DatasourceComputation)
			{
				cell.DataSourceBinding = column.DataSourceBinding;
			}
			//else
			//{
			//    cell.DataSourceBinding = new DataSourceBinding
			//    {
			//        BindingId = Guid.NewGuid().ToString(),
			//        DataSource = EnumDataSource.DatasourceImport,
			//        EnumSamplingStatistic = exportedCell.DataSourceBinding?.EnumSamplingStatistic ?? EnumSamplingStatistic.SamplingStatisticUnknown
			//    };
			//}
			cell = await ConvertCellNotesAsync(exportedCell, cell);
			cell = await ConvertCellDatas(exportedCell, cell);
			return cell;
		}
		private Cell PreviewConvertCell(Cell exportedCell)
		{
			Cell cell = new Cell();
			cell.ColumnNumber = _columnMap[exportedCell.ColumnNumber];
			cell = PreviewConvertCellNotes(exportedCell, cell);
			cell = PreviewConvertCellDatas(exportedCell, cell);
			return cell;
		}
		private async Task<Cell> ConvertCellNotesAsync(Cell exportCell, Cell importCell)
		{
			if (exportCell.Notes != null)
			{
				foreach (var note in exportCell.Notes)
				{
					string userId = note.UserId;
					if (Guid.TryParse(note.UserId, out Guid guidOutput))
					{
						userId = (await ConvertUserAsync(note.UserId)).Id;
					}
					importCell.Notes.Add(new Note { Id = Guid.NewGuid().ToString(), UserId = userId, Text = note.Text, TimeStamp = note.TimeStamp });
				}
			}
			return importCell;
		}
		private Cell PreviewConvertCellNotes(Cell exportCell, Cell importCell)
		{
			if (exportCell.Notes != null)
			{
				foreach (var note in exportCell.Notes)
				{
					if (Guid.TryParse(note.UserId, out Guid guidOutput))
					{
						PreviewConvertUser(note.UserId);
					}
				}
			}
			return importCell;
		}
		private async Task<Cell> ConvertCellDatas(Cell exportCell, Cell importCell)
		{
			if (exportCell.CellDatas != null)
			{
				foreach (var cellData in exportCell.CellDatas)
				{
					importCell.CellDatas.Add(await ConvertCellData(importCell, cellData));
				}
			}
			return importCell;
		}
		private Cell PreviewConvertCellDatas(Cell exportCell, Cell importCell)
		{
			if (exportCell.CellDatas != null)
			{
				foreach (var cellData in exportCell.CellDatas)
				{
					importCell.CellDatas.Add(PreviewConvertCellData(cellData));
				}
			}
			return importCell;
		}
		private async Task<CellData> ConvertCellData(Cell importCell, CellData exportCellData)
		{
			CellData cellData = new CellData();
			cellData.IsLocked = exportCellData.IsLocked;
			cellData.Justification = exportCellData.Justification;
			cellData.StringValue = exportCellData.StringValue;
			cellData.Value = exportCellData.Value;
			cellData.UnitId = exportCellData.UnitId;
			if (importCell.DataSourceBinding?.DataSource == EnumDataSource.DatasourceComputation && exportCellData.DataSourceBinding?.DataSource == EnumDataSource.DatasourceComputation)
			{
				// If it is a computation, make sure the binding is consistent with the Column AND Cell
				cellData.DataSourceBinding = importCell.DataSourceBinding;
				// Only lock if it is a computation
				// cellData.IsLocked = true;
			}
			else if (exportCellData.DataSourceBinding != null)
			{
				// Preserve Binding history from the cell data that is being imported
				cellData.DataSourceBinding = exportCellData.DataSourceBinding;
			}
			else
			{
				// Nothing exists, so set it to import
				cellData.DataSourceBinding = new DataSourceBinding
				{
					BindingId = Guid.NewGuid().ToString(),
					DataSource = EnumDataSource.DatasourceImport,
					EnumSamplingStatistic = exportCellData.DataSourceBinding?.EnumSamplingStatistic ?? EnumSamplingStatistic.SamplingStatisticUnknown
				};
			}
			cellData = await ConvertAuditEvents(exportCellData, cellData);
			return cellData;
		}
		private CellData PreviewConvertCellData(CellData exportCellData)
		{
			CellData cellData = new CellData();
			cellData.IsLocked = exportCellData.IsLocked;
			cellData.Justification = exportCellData.Justification;
			cellData.StringValue = exportCellData.StringValue;
			cellData.Value = exportCellData.Value;
			cellData.UnitId = exportCellData.UnitId;
			cellData = PreviewConvertAuditEvents(exportCellData, cellData);
			return cellData;
		}
		private async Task<CellData> ConvertAuditEvents(CellData exportCellData, CellData cellData)
		{
			if (exportCellData.AuditEvents != null)
			{
				foreach (var auditEvent in exportCellData.AuditEvents)
				{
					cellData.AuditEvents.Add(await ConvertAuditEvent(auditEvent));
				}
			}
			return cellData;
		}
		private CellData PreviewConvertAuditEvents(CellData exportCellData, CellData cellData)
		{
			if (exportCellData.AuditEvents != null)
			{
				foreach (var auditEvent in exportCellData.AuditEvents)
				{
					cellData.AuditEvents.Add(PreviewConvertAuditEvent(auditEvent));
				}
			}
			return cellData;
		}
		private async Task<AuditEvent> ConvertAuditEvent(AuditEvent exportAuditEvent)
		{
			exportAuditEvent.Id = Guid.NewGuid().ToString();
			if (exportAuditEvent.EnumDataSource != EnumDataSource.DatasourceComputation && Guid.TryParse(exportAuditEvent.UserId, out Guid guidOutput))
				exportAuditEvent.UserId = (await ConvertUserAsync(exportAuditEvent.UserId)).Id;
			return exportAuditEvent;

		}
		private AuditEvent PreviewConvertAuditEvent(AuditEvent exportAuditEvent)
		{
			exportAuditEvent.Id = Guid.NewGuid().ToString();
			if (exportAuditEvent.EnumDataSource != EnumDataSource.DatasourceComputation && Guid.TryParse(exportAuditEvent.UserId, out Guid guidOutput))
				exportAuditEvent.UserId = PreviewConvertUser(exportAuditEvent.UserId).Id;
			return exportAuditEvent;

		}
		private async Task<User> ConvertUserAsync(string userId)
		{
			if (_userMap.ContainsKey(userId))
				return _userMap[userId];
			// Create User

			// Get User from OperationExport
			var oldUser = _operationCache.GetUser(userId);
			if (oldUser != null)
			{
				var newuser = await _clientSdk.Core.CreateUserAsync(oldUser.FirstName, oldUser.LastName, oldUser.Email + ".not", _tenantId);
				if (newuser != null)
				{
					_userMap.Add(userId, newuser);
					var userTwin = await _clientSdk.DigitalTwin.GetAsync(newuser.Id);
					if (userTwin != null)
					{
						string twinData = AddUpdateRootValue("CloneId", userId, userTwin.TwinData);
						userTwin.TwinData = twinData;
						userTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
						userTwin = await _clientSdk.DigitalTwin.UpdateAsync(userTwin);
						return newuser;
					}
				}
			}
			else
			{
				//Create / use unknown user
				var newuser = await _clientSdk.Core.CreateUserAsync("AuditUser", "Unknown", "audituser@noone.not", _tenantId);
				if (newuser != null)
				{
					_userMap.Add(userId, newuser);
					var userTwin = await _clientSdk.DigitalTwin.GetAsync(newuser.Id);
					if (userTwin != null)
					{
						string twinData = AddUpdateRootValue("CloneId", userId, userTwin.TwinData);
						userTwin.TwinData = twinData;
						userTwin.UpdateMask = new FieldMask { Paths = { "twinData" } };
						userTwin = await _clientSdk.DigitalTwin.UpdateAsync(userTwin);
						return newuser;

					}
				}
			}
			return null;
		}
		private User PreviewConvertUser(string userId)
		{
			if (_userMap.ContainsKey(userId))
				return _userMap[userId];
			// Create User

			// Get User from OperationExport
			var oldUser = _operationCache.GetUser(userId);
			User newUser;
			if (oldUser != null)
			{
				newUser = new User { FirstName = oldUser.FirstName, LastName = oldUser.LastName, Email = oldUser.Email + "not" };

			}
			else
			{
				newUser = new User { FirstName = "Unknown", LastName = "AuditUser", Email = "audituser@noone.not" };

			}
			_userMap.Add(userId, newUser);

			return newUser;
		}
	}
}
