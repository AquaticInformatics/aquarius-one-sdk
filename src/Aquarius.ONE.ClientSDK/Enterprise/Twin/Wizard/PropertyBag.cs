using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Enterprise.Twin.Wizard
{
    public class PropertyBag            
    {
        //Twin Types to be used
        public string ty { get; set; }
        // List of Suggested Locations for this Digital Twin Sub Type
        public List<Location> ls { get; set; }
        // List of Suggested Parameters
        public List<Parameter> ps { get; set; }
        // List of Suggested Computations

        public override string ToString()
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, jsonSettings);
        }
        public string ToPrettyJson()
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
                 
            };
            return JsonConvert.SerializeObject(this, jsonSettings);
        }
    }
}
