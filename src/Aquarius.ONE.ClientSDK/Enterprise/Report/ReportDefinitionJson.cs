using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Enterprise.Report
{
    public class ReportDefinitionJson
    {
        public ReportDefinitionJson(string json)
        {
            var definition = JsonConvert.DeserializeObject<ReportDefinitionJson>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            columns = definition.columns;
            endTime = definition.endTime;
            plantId = definition.plantId;
            startTime = definition.startTime;
            relativeDateRange = definition.relativeDateRange;
            removeEmptyRows = definition.removeEmptyRows;
        }
        public ReportDefinitionJson()
        { }
        public List<ReportColumn> columns { get; set; }
        public string endTime { get; set; }
        public string plantId { get; set; }
        public string startTime { get; set; }
        public int relativeDateRange { get; set; }
        public bool removeEmptyRows { get; set; }

    }
}
