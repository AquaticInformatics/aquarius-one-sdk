using Operations.Spreadsheet.Protobuf.Models;
using System.Collections.Generic;

namespace ONE.Enterprise.Twin.Wizard
{
    public class Parameter
    {
        // Is this a default in the wizard
        public bool d { get; set; }
        // Worksheet Type
        public EnumWorksheet t { get; set; }
        //Sampling Statistic
        public EnumSamplingStatistic s { get; set; }
        // List of Agency Codes
        // Example Wizard Code: "w", "INF_FLOW"
        public Dictionary<string, string> ac { get; set; }
        // Optional Formula              
        public string f { get; set; }

    }
}
