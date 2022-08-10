using Newtonsoft.Json;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ONE.Utilities;
using Proto = ONE.Models.CSharp;

namespace ONE.Common.Logbook
{
    public class LogbookCache
    {
        private readonly ClientSDK _clientSdk;
        
        public LogbookCache(ClientSDK clientSdk)
        {
            _clientSdk = clientSdk;
        }

        public Dictionary<string, Proto.Configuration> Logbooks { get; set; }
        public Dictionary<string, List<string>> Tags { get; set; }
        public Dictionary<string, List<ConfigurationNote>> LogbookEntries { get; set; }

        /// <summary>
        /// Load Logbook and logbookEntry data for an operation, by default it will load entries for the last thirty days but that can be overridden
        /// </summary>
        /// <param name="operationId">Identifier of the operation for which to load data</param>
        /// <param name="dataTimeSpan"></param>
        public async Task<bool> LoadAsync(string operationId, TimeSpan dataTimeSpan = default)
        {
            try
            {
                Logbooks = (await _clientSdk.Logbook.GetLogbooksAsync(operationId)).ToDictionary(k => k.Id, v => v);

                var endDate = DateTime.UtcNow;
                var startDate = endDate.Subtract(dataTimeSpan == default ? TimeSpan.FromDays(30) : dataTimeSpan);

                foreach (var logbookId in Logbooks.Keys)
                {
                    LogbookEntries[logbookId] = await _clientSdk.Logbook.GetLogbookEntriesAsync(logbookId, startDate, endDate);

                    if (!Tags.ContainsKey(logbookId))
                        Tags[logbookId] = new List<string>();

                    foreach (var entry in LogbookEntries[logbookId])
                        Tags[logbookId].AddRange(entry.Tags.Select(t => t.Tag));

                    Tags[logbookId] = Tags[logbookId].Distinct().ToList();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Proto.Configuration GetLogbook(string logbookId)
        {
            if (string.IsNullOrEmpty(logbookId) || !Logbooks.ContainsKey(logbookId))
                return null;

            return Logbooks[logbookId];
        }

        public List<Proto.Configuration> GetLogbooks() => Logbooks.Values.ToList();

        // Latest
        public Dictionary<string, ConfigurationNote> GetLatestEntries() =>
            LogbookEntries.ToDictionary(k => k.Key, v => v.Value.OrderByDescending(n => n.NoteTime.ToDateTime()).First());

        // Tags
        public List<string> GetUniqueTags(string logbookId)
        {
            if (string.IsNullOrEmpty(logbookId) || !LogbookEntries.ContainsKey(logbookId))
                return null;

            return Tags[logbookId];
        }

        // Entries
        

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

        public static LogbookCache Load(string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<LogbookCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
    }
}
