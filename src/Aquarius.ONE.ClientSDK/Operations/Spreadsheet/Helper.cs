using Operations.Spreadsheet.Protobuf.Models;
using System.Collections.Generic;
using System.Linq;

namespace ONE.Operations.Spreadsheet
{
    public static class Helper
    {
        public static Column GetColumnByNumber(WorksheetDefinition worksheetDefinition, uint columnNumber)
        {
            if (worksheetDefinition != null && worksheetDefinition.Columns.Count > 0)
            {
                var matches = worksheetDefinition.Columns.Where(p => p.ColumnNumber == columnNumber);
                if (matches.Count() > 0)
                {
                    return matches.First();
                }
            }
            return null;
        }
        public static Column GetColumnByDescription(WorksheetDefinition worksheetDefinition, string description)
        {
            if (worksheetDefinition == null || worksheetDefinition.Columns == null)
                return null;
            var matches = worksheetDefinition.Columns.Where(p => p.Description == description);
            if (matches.Count() > 0)
            {
                return matches.First();
            }
            else
            {
                return null;
            }
        }
        public static string GetNewColumnName(string name)
        {
            //Pattern: Name (01)

            //Nameless
            if (string.IsNullOrEmpty(name))
                return "(01)";

            if (name.Length > 4 && name.EndsWith(")") && name.Substring(name.Length - 5, 2) == " (")
            {
                string copyNumString = name.Substring(name.Length - 3, 2);
                int.TryParse(copyNumString, out int copyNumInt);
                if (copyNumInt > 0)
                {
                    copyNumInt++;
                    string rootname = name.Substring(0, name.Length - 4);
                    return $"{rootname}({copyNumInt.ToString().PadLeft(2, '0')})";
                }
            }
            return $"{name} (01)";
        }
        public static WorksheetDefinition EnsureColumnNamesAreUniqueWithinALocation(WorksheetDefinition worksheetDefinition)
        {
            HashSet<string> cache = new HashSet<string>();

            foreach (var column in worksheetDefinition.Columns)
            {
                string hash = $"{column.LocationId} - {column.Name}";
                while (cache.Contains(hash))
                {
                    column.Name = GetNewColumnName(column.Name);
                    hash = $"{column.LocationId} - {column.Name}";
                }
                cache.Add(hash);
            }
            return worksheetDefinition;
        }
        public static CellData GetLatestCellData(Row row, uint columnNumber)
        {
            var cell = GetCellByColumnByNumber(row, columnNumber);
            if (cell != null)
            {
                if (cell.CellDatas != null && cell.CellDatas.Count > 0)
                    return cell.CellDatas[0];
            }
            return null;
            
        }
        public static Cell GetCellByColumnByNumber(Row row, uint columnNumber)
        {
            if (row != null && row.Cells.Count > 0)
            {
                var matches = row.Cells.Where(p => p.ColumnNumber == columnNumber);
                if (matches.Count() > 0)
                {
                    return matches.First();
                }
            }
            return null;
        }
    }
  
}
