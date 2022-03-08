using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Utilities.UI
{
    public class UIDefinition
    {
        public virtual bool Load(string json)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<UIDefinition>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
        public List<Attribute> attributes { get; set; }
        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return base.ToString();
            }
        }
    }
}
