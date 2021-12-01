using Operations.Spreadsheet.Protobuf.Models;
using System.Collections.Generic;

namespace ONE.Operations.Spreadsheet
{
    public interface IWorksheetViewConfiguration
    {
        bool? canDelete { get; set; }
        bool? canEdit { get; set; }
        List<uint> columnNumbers { get; set; }
        Dictionary<uint, WorksheetViewConfiguration.ColumnConfig> columns { get; set; }
        string createdOn { get; set; }
        List<dynamic> headers { get; set; }
        string id { get; set; }
        bool? isOwner { get; set; }
        string name { get; set; }
        EnumWorksheet worksheetType { get; set; }

        string ToString();
    }
}