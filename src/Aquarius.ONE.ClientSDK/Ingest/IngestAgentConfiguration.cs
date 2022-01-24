using Newtonsoft.Json;
using System;

namespace ONE.Ingest
{
    public class IngestAgentConfiguration
    {
        
        /// <summary>
        /// Frequency the agent should acquire data
        /// </summary>
        public TimeSpan RunFrequency { get; set; }

        /// <summary>
        /// Frequency the agent should upload the data
        /// </summary>
        public TimeSpan UploadFrequency { get; set; }
        
        /// <summary>
        /// Loads the configuration object from JSON
        /// </summary>
        /// <param name="configurationJson">The Configuration represented as a JSON string.</param>
        /// <returns>Whether the Configuration Json was successfully loaded.</returns>
        public virtual bool Load(string configurationJson)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<IngestAgentConfiguration>(configurationJson, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Serializes this class to a string
        /// </summary>
        /// <returns>JSON String version of this class</returns>
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
