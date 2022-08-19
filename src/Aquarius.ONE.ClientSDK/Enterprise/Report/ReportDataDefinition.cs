﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Enterprise.Report
{
    public class ReportDataDefinition
    {
        public ReportDataDefinition(string json)
        {
            var definition = JsonConvert.DeserializeObject<ReportDefinitionJson>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Columns = definition.columns;
            
            DateTime.TryParse(definition.endTime, out DateTime eValue);
            EndTime = eValue;

            DateTime.TryParse(definition.startTime, out DateTime sValue);
            StartTime = sValue;

            PlantId = definition.plantId;
            RelativeDateRange = (EnumRelativeDateRange)definition.relativeDateRange;
            RemoveEmptyRows = definition.removeEmptyRows;
        }
        public List<ReportColumn> Columns { get; set; }
        public DateTime EndTime { get; set; }
        public string PlantId { get; set; }
        public DateTime StartTime { get; set; }
        public EnumRelativeDateRange RelativeDateRange { get; set; }
        public bool RemoveEmptyRows { get; set; }

        private Dictionary<string, string> _currentReportColumns;
        public Dictionary<string, string> CurrentReportColumns
        {
            get
            {
                if (_currentReportColumns == null && Columns != null)
                {
                    _currentReportColumns = new Dictionary<string, string>();
                    foreach (var column in Columns)
                        _currentReportColumns.Add(column.id, column.header);
                }
                return _currentReportColumns;
            }
        }
    }
}
