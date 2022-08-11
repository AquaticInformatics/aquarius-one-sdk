using Newtonsoft.Json;
using ONE.Models.CSharp;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto = ONE.Models.CSharp;

namespace ONE.Common.Logbook
{
    public class LogbookCache
    {
        private readonly ClientSDK _clientSdk;

        private Dictionary<string, Proto.Configuration> Logbooks { get; set; } = new Dictionary<string, Proto.Configuration>();
        private Dictionary<string, List<string>> Tags { get; } = new Dictionary<string, List<string>>();
        private Dictionary<string, List<ConfigurationNote>> LogbookEntries { get; } = new Dictionary<string, List<ConfigurationNote>>();

        public LogbookCache(ClientSDK clientSdk)
        {
            _clientSdk = clientSdk;
        }

        public string OperationId { get; private set; }

        public void SetOperationId(string operationId)
        {
            if (Guid.TryParse(operationId, out _))
                OperationId = operationId;

            throw new ArgumentOutOfRangeException(nameof(operationId), operationId, "OperationId must be a guid");
        }

        /// <summary>
        /// Load Logbook data for an operation
        /// </summary>
        /// <param name="operationId">Identifier of the operation for which to load data, uses <see cref="OperationId"/> if not set and will overwrite the existing OperationId if set.</param>
        public async Task<bool> LoadLogbooksAsync(string operationId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(operationId) && string.IsNullOrEmpty(OperationId)) return false;

                if (!string.IsNullOrEmpty(operationId))
                    SetOperationId(operationId);

                Logbooks = (await _clientSdk.Logbook.GetLogbooksAsync(OperationId)).ToDictionary(k => k.Id, v => v);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads logbookEntries for all logbooks in the cache
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<bool> LoadLogbookEntriesAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                foreach (var logbookId in Logbooks.Keys)
                    await LoadEntriesByLogbookAsync(logbookId, startDate, endDate);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads logbookEntries for a specific logbook in the cache
        /// </summary>
        /// <param name="logbookId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<bool> LoadEntriesByLogbookAsync(string logbookId, DateTime startDate, DateTime endDate)
        {
            try
            {
                LogbookEntries[logbookId] = await _clientSdk.Logbook.GetLogbookEntriesAsync(logbookId, startDate, endDate);

                Tags[logbookId] = new List<string>();

                foreach (var entry in LogbookEntries[logbookId])
                    Tags[logbookId].AddRange(entry.Tags.Select(t => t.Tag));

                Tags[logbookId] = Tags[logbookId].Distinct().ToList();
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
        public List<ConfigurationNote> GetLogbookEntries(string logbookId) => LogbookEntries.ContainsKey(logbookId) ? LogbookEntries[logbookId] : new List<ConfigurationNote>();

        public List<ConfigurationNote> SearchEntriesByTags(string logbookId, params string[] tags) => !LogbookEntries.ContainsKey(logbookId)
            ? new List<ConfigurationNote>()
            : tags.Aggregate(LogbookEntries[logbookId], (current, tag) => current.Where(n => n.Tags.Select(ct => ct.Tag).Contains(tag)).ToList());

        public List<ConfigurationNote> SearchEntriesByText(string logbookId, string searchText) => !LogbookEntries.ContainsKey(logbookId)
            ? new List<ConfigurationNote>()
            : LogbookEntries[logbookId].Where(n => n.Note.ToLower().Contains(searchText.ToLower())).ToList();

        public void ClearCache()
        {
            Logbooks.Clear();
            LogbookEntries.Clear();
            Tags.Clear();
        }

        public void ClearCacheByLogbook(string logbookId)
        {
            Logbooks.Remove(logbookId);
            LogbookEntries.Remove(logbookId);
            Tags.Remove(logbookId);
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
