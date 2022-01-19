using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ONE.Ingest
{
    public class IngestConfiguration
    {
        public List<IngestTelemetryConfiguration> Telemetry { get; set; }
        public virtual void Initialize()
        {
        }
        public TimeSpan RunFrequency { get; set; }
        public TimeSpan UploadFrequency { get; set; }
        public virtual bool Load(string configurationJson)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<IngestConfiguration>(configurationJson, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
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
