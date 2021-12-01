using Newtonsoft.Json;
using Operations.Spreadsheet.Protobuf.Models;
using System;
using System.Collections.Generic;

namespace ONE.Operations.Spreadsheet
{
    [Serializable]
    public class WorksheetViewConfiguration : IWorksheetViewConfiguration
    {
        public WorksheetViewConfiguration()
        { }
        private List<uint> _orderedColumnNumbers;
        public List<uint> OrderedColumnNumbers
        {
            get
            {
                if (_orderedColumnNumbers == null && headers != null)
                {
                    _orderedColumnNumbers = new List<uint>();
                    foreach (var header in headers)
                    {
                        _orderedColumnNumbers.AddRange(GetChildOrderedColumns(header));
                    }
                    //Check to see if any columns are missing
                    if (columnNumbers != null && _orderedColumnNumbers.Count != columnNumbers.Count)
                    {
                        foreach (var column in columnNumbers)
                        {
                            if (!_orderedColumnNumbers.Contains(column))
                                _orderedColumnNumbers.Add(column);
                        }
                    }
                }
                return _orderedColumnNumbers;
            }
        }
        private List<uint> GetChildOrderedColumns(dynamic header)
        {
            List<uint> columns = new List<uint>();
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
        public WorksheetViewConfiguration(string jsonData)
        {
            var view = JsonConvert.DeserializeObject<WorksheetViewConfiguration>(jsonData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            id = view.id;
            name = view.name;
            isOwner = view.isOwner;
            worksheetType = view.worksheetType;
            columnNumbers = view.columnNumbers;
            columns = view.columns;
            createdOn = view.createdOn;
            canDelete = view.canDelete;
            canEdit = view.canEdit;
            headers = new List<dynamic>();
            if (view.headers != null)
            {
                foreach (var header in view.headers)
                {
                    var groupHeaderItem = new GroupHeader(header.ToString());
                    if (groupHeaderItem.name != null)
                    {
                        headers.Add(groupHeaderItem);
                    }
                    else
                    {
                        var columnHeaderItem = JsonConvert.DeserializeObject<ColumnHeader>(header.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        headers.Add(columnHeaderItem);

                    }
                    Console.WriteLine(header);
                }
            }
        }
        public override string ToString()
        {
            string sRet = JsonConvert.SerializeObject(this);
            return sRet;
        }
        public string id { get; set; }
        public string name { get; set; }
        public bool? isOwner { get; set; }
        public EnumWorksheet worksheetType { get; set; }

        public List<uint> columnNumbers { get; set; }
        public Dictionary<uint, ColumnConfig> columns { get; set; }
        public string createdOn { get; set; }
        public bool? canEdit { get; set; }
        public bool? canDelete { get; set; }
        public List<dynamic> headers { get; set; } // (ColumnHeader | GroupHeader)


        [Serializable]
        public class ColumnConfig
        {
            public uint columnWidth { get; set; }
        }
        [Serializable]
        public enum EnumHeaderType
        {
            column = 1,
            group = 2
        }
        public class ColumnHeader
        {
            public uint id { get; set; }
            public EnumHeaderType HeaderType
            {
                get
                {
                    return EnumHeaderType.column;
                }
            }
        }
        [Serializable]
        public class GroupHeader
        {
            public GroupHeader()
            { }
            public GroupHeader(string json)
            {
                var groupHeader = JsonConvert.DeserializeObject<GroupHeader>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                name = groupHeader.name;
                groupId = groupHeader.groupId;
                if (groupHeader.children != null)
                {
                    children = new List<dynamic>();
                    foreach (var child in groupHeader.children)
                    {
                        var groupHeaderItem = new GroupHeader(child.ToString());
                        if (groupHeaderItem.name != null)
                        {
                            children.Add(groupHeaderItem);
                        }
                        else
                        {
                            var columnHeaderItem = JsonConvert.DeserializeObject<ColumnHeader>(child.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                            children.Add(columnHeaderItem);
                        }
                    }
                }

            }
            public string name { get; set; }
            public List<dynamic> children { get; set; }  //(ColumnHeader | GroupHeader)
            public string groupId { get; set; }

            public EnumHeaderType HeaderType
            {
                get
                {
                    return EnumHeaderType.group;
                }
            }

        }
    }
}
