using Operations.Spreadsheet.Protobuf.Models;
using System.Linq;

namespace ONE.Operations.Spreadsheet
{
    public static class SpreadsheetHelper
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
