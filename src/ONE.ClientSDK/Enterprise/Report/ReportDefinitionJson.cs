using Newtonsoft.Json;
using ONE.ClientSDK.Utilities;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace ONE.ClientSDK.Enterprise.Report
{
	public class ReportDefinitionJson
	{
		public ReportDefinitionJson(string json)
		{
			var definition = JsonConvert.DeserializeObject<ReportDefinitionJson>(json, JsonExtensions.IgnoreNullSerializerSettings);
			columns = definition.columns;
			endTime = definition.endTime;
			plantId = definition.plantId;
			startTime = definition.startTime;
			relativeDateRange = definition.relativeDateRange;
			removeEmptyRows = definition.removeEmptyRows;
		}

		public ReportDefinitionJson() { }

		public List<ReportColumn> columns { get; set; }
		public string endTime { get; set; }
		public string plantId { get; set; }
		public string startTime { get; set; }
		public int relativeDateRange { get; set; }
		public bool removeEmptyRows { get; set; }
	}
}
