using Newtonsoft.Json;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Models.CSharp.Imposed.Enums;
using ONE.Models.CSharp.Imposed.WorksheetView;
using ONE.Shared.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using WorksheetView = ONE.Models.CSharp.Imposed.WorksheetView.WorksheetView;

namespace ONE.ClientSDK.Operations.Spreadsheet
{
	public static class SpreadsheetHelper
	{
		public static Column GetColumnByNumber(WorksheetDefinition worksheetDefinition, uint columnNumber) =>
			worksheetDefinition?.Columns?.FirstOrDefault(p => p.ColumnNumber == columnNumber);

		public static Column GetColumnByDescription(WorksheetDefinition worksheetDefinition, string description) =>
			worksheetDefinition?.Columns?.FirstOrDefault(p => p.Description == description);

		public static string GetNewColumnName(string name)
		{
			//Pattern: Name (01)

			//Nameless
			if (string.IsNullOrEmpty(name))
				return "(01)";

			if (name.Length > 4 && name.EndsWith(")") && name.Substring(name.Length - 5, 2) == " (")
			{
				var copyNumString = name.Substring(name.Length - 3, 2);
				int.TryParse(copyNumString, out var copyNumInt);
				if (copyNumInt > 0)
				{
					copyNumInt++;
					var rootName = name.Substring(0, name.Length - 4);
					return $"{rootName}({copyNumInt.ToString().PadLeft(2, '0')})";
				}
			}
			return $"{name} (01)";
		}

		public static WorksheetDefinition EnsureColumnNamesAreUniqueWithinALocation(WorksheetDefinition worksheetDefinition)
		{
			var cache = new HashSet<string>();

			foreach (var column in worksheetDefinition.Columns)
			{
				var hash = $"{column.LocationId} - {column.Name}";
				while (cache.Contains(hash))
				{
					column.Name = GetNewColumnName(column.Name);
					hash = $"{column.LocationId} - {column.Name}";
				}
				cache.Add(hash);
			}
			return worksheetDefinition;
		}

		public static CellData GetLatestCellData(Row row, uint columnNumber) => GetCellByColumnByNumber(row, columnNumber)?.CellDatas?.FirstOrDefault();

		public static Cell GetCellWithLatestCellDataAndNotes(Row row, uint columnNumber)
		{
			var cell = GetCellByColumnByNumber(row, columnNumber);
			var newCell = new Cell();
			if (cell != null)
			{
				if (cell.Notes != null && cell.Notes.Count > 0)
				{
					foreach (var note in cell.Notes)
					{
						newCell.Notes.Add(note);
					}
				}
				if (cell.CellDatas != null && cell.CellDatas.Count > 0)
				{
					newCell.CellDatas.Add(cell.CellDatas[0]);
				}
				return newCell;
			}
			return null;
		}

		public static Cell GetCellByColumnByNumber(Row row, uint columnNumber) => row?.Cells?.Count > 0
			? row.Cells.FirstOrDefault(p => p.ColumnNumber == columnNumber)
			: null;

		public static DateTime GetDate(object dateAsObject)
		{
			if (dateAsObject == null)
				return DateTime.MinValue;

			if (DateTimeHelper.TryParse(dateAsObject.ToString(), out var date))
				return date;

			double.TryParse(dateAsObject.ToString(), out var oaDate);
			if (oaDate > 0)
			{
				try
				{
					date = DateTime.FromOADate(oaDate);
				}
				catch
				{
					return DateTime.MinValue;
				}
			}

			return date;
		}

		public static object GetDoubleValue(string value)
		{
			if (value == null)
				return null;
			if (value.StartsWith("ERR_"))
				return value;
			double.TryParse(value, out var result);
			return result;
		}

		public static bool IsNumeric(string value) => !string.IsNullOrEmpty(value) &&
		                                              value.Replace(".", "").Replace("-", "").All(char.IsNumber);

		public static double? TryParseDouble(string value)
		{
			if (string.IsNullOrEmpty(value) || !IsNumeric(value))
				return null;
			return double.TryParse(value, out var temp) ? temp : (double?)null;
		}

		public static double? TryParseDouble(object value) => value != null ? TryParseDouble(value.ToString()) : null;

		public static object[,] ConvertToArray(object value)
		{
			if (value is string)
			{
				var arr = new object[1, 1];
				arr[0, 0] = value;
				return arr;
			}
			return (object[,])value;
		}

		public static DateTime LastDOMValue(DateTime date) => date.LastDayOfMonth();

		public static List<uint> GetOrderedColumnNumbers(WorksheetView worksheetView)
		{
			var orderedColumnNumbers = new List<uint>();
			if (worksheetView.headers != null)
			{

				foreach (var header in worksheetView.headers)
				{
					orderedColumnNumbers.AddRange(GetChildOrderedColumns(header));
				}
				//Check to see if any columns are missing
				if (worksheetView.columnNumbers != null && orderedColumnNumbers.Count != worksheetView.columnNumbers.Count)
				{
					foreach (var column in worksheetView.columnNumbers)
					{
						if (!orderedColumnNumbers.Contains(column))
							orderedColumnNumbers.Add(column);
					}
				}
			}
			return orderedColumnNumbers;
		}

		private static List<uint> GetChildOrderedColumns(dynamic header)
		{
			var columns = new List<uint>();
			if (header.HeaderType == EnumHeaderType.column) //column
			{
				columns.Add(header.id);
			}
			else // Group Header
			{
				foreach (var item in header.children)
				{
					columns.AddRange(GetChildOrderedColumns(item));
				}
			}
			return columns;
		}

		public static WorksheetView GetWorksheetView(string jsonData)
		{
			var worksheetView = new WorksheetView();
			var view = JsonConvert.DeserializeObject<WorksheetView>(jsonData, JsonExtensions.IgnoreNullSerializerSettings);
			worksheetView.id = view.id;
			worksheetView.name = view.name;
			worksheetView.isOwner = view.isOwner;
			worksheetView.worksheetType = view.worksheetType;
			worksheetView.columnNumbers = view.columnNumbers;
			worksheetView.columns = view.columns;
			worksheetView.createdOn = view.createdOn;
			worksheetView.canDelete = view.canDelete;
			worksheetView.canEdit = view.canEdit;
			worksheetView.headers = new List<dynamic>();
			if (view.headers != null)
			{
				foreach (var header in view.headers)
				{
					var groupHeaderItem = GetGroupHeader(header.ToString());
					if (groupHeaderItem.name != null)
					{
						worksheetView.headers.Add(groupHeaderItem);
					}
					else
					{
						var columnHeaderItem = JsonConvert.DeserializeObject<ColumnHeader>(header.ToString(), JsonExtensions.IgnoreNullSerializerSettings);
						worksheetView.headers.Add(columnHeaderItem);

					}
					Console.WriteLine(header);
				}
			}
			return worksheetView;
		}

		public static GroupHeader GetGroupHeader(string json)
		{
			var newGroupHeader = new GroupHeader();
			var groupHeader = JsonConvert.DeserializeObject<GroupHeader>(json, JsonExtensions.IgnoreNullSerializerSettings);
			newGroupHeader.name = groupHeader.name;
			newGroupHeader.groupId = groupHeader.groupId;
			if (groupHeader.children != null)
			{
				newGroupHeader.children = new List<dynamic>();
				foreach (var child in groupHeader.children)
				{
					var groupHeaderItem = GetGroupHeader(child.ToString());
					if (groupHeaderItem.name != null)
					{
						newGroupHeader.children.Add(groupHeaderItem);
					}
					else
					{
						var columnHeaderItem = JsonConvert.DeserializeObject<ColumnHeader>(child.ToString(), JsonExtensions.IgnoreNullSerializerSettings);
						newGroupHeader.children.Add(columnHeaderItem);
					}
				}
			}
			return newGroupHeader;
		}
	}
}
