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

        public LogbookCache(ClientSDK clientSdk, string serializedCache = "")
        {
            _clientSdk = clientSdk;

            if (string.IsNullOrEmpty(serializedCache)) return;

            var cache = Load(serializedCache);

            if (cache == null)
                throw new ArgumentException("Serialized cache could not be deserialized");

            OperationId = cache.OperationId;
            Logbooks = cache.Logbooks;
            Tags = cache.Tags;
            LogbookEntries = cache.LogbookEntries;
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
        /// <param name="startDate">loads logbookEntries on or after this date</param>
        /// <param name="endDate">load logbookEntries before this date</param>
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
        /// <param name="logbookId">identifier of the logbook containing the entries to be loaded</param>
        /// <param name="startDate">loads logbookEntries on or after this date</param>
        /// <param name="endDate">load logbookEntries before this date</param>
        public async Task<bool> LoadEntriesByLogbookAsync(string logbookId, DateTime startDate, DateTime endDate)
        {
            try
            {
                LogbookEntries[logbookId] = await _clientSdk.Logbook.GetLogbookEntriesAsync(logbookId, startDate, endDate);

                Tags[logbookId] = new List<string>();

                foreach (var entry in LogbookEntries[logbookId])
                    Tags[logbookId].AddRange(entry.Tags.Select(t => t.Tag));

                Tags[logbookId] = Tags[logbookId].Distinct().ToList();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve a logbook from the cache by Id
        /// </summary>
        /// <param name="logbookId">Id of the logbook to retrieve</param>
        public Proto.Configuration GetLogbook(string logbookId)
        {
            if (string.IsNullOrEmpty(logbookId) || !Logbooks.ContainsKey(logbookId))
                return null;

            return Logbooks[logbookId];
        }

        /// <summary>
        /// Gets all logbooks in the cache
        /// </summary>
        public List<Proto.Configuration> GetLogbooks() => Logbooks.Values.ToList();

        /// <summary>
        /// Get the most recent entry for each logbook in the cache.
        /// </summary>
        public Dictionary<string, ConfigurationNote> GetLatestEntryPerLogbook() =>
            LogbookEntries.ToDictionary(k => k.Key, v => v.Value.OrderByDescending(n => n.NoteTime.ToDateTime()).First());

        /// <summary>
        /// Retrieve a list of unique tags associated to a specific logbook
        /// </summary>
        /// <param name="logbookId">Id of the logbook containing the tags to be retrieved</param>
        public List<string> GetUniqueTags(string logbookId)
        {
            if (string.IsNullOrEmpty(logbookId) || !LogbookEntries.ContainsKey(logbookId))
                return null;

            return Tags[logbookId];
        }

        /// <summary>
        /// Get all logbookEntries in the cache for a specific logbook.
        /// </summary>
        /// <param name="logbookId">Id of the logbook containing the logbookEntries to retrieve</param>
        public List<ConfigurationNote> GetLogbookEntries(string logbookId) => LogbookEntries.ContainsKey(logbookId) ? LogbookEntries[logbookId] : new List<ConfigurationNote>();

        /// <summary>
        /// Get all logbookEntries in the cache for a specific logbook within a specific time range.
        /// </summary>
        /// <param name="logbookId">Id of the logbook containing the logbookEntries to retrieve</param>
        /// <param name="startDate">returns logbookEntries on or after this date</param>
        /// <param name="endDate">returns logbookEntries before this date</param>
        public List<ConfigurationNote> GetEntriesByDate(string logbookId, DateTime startDate, DateTime endDate) => LogbookEntries.ContainsKey(logbookId)
            ? LogbookEntries[logbookId].Where(e => e.NoteTime.ToDateTime() >= startDate && e.NoteTime.ToDateTime() < endDate).ToList()
            : new List<ConfigurationNote>();

        /// <summary>
        /// Get all logbookEntries in the cache for a specific logbook containing specific tags.
        /// </summary>
        /// <param name="logbookId">Id of the logbook containing the logbookEntries to retrieve</param>
        /// <param name="tags">tags to filter by, entries must contain all provided tags</param>
        public List<ConfigurationNote> GetEntriesByTags(string logbookId, params string[] tags) => !LogbookEntries.ContainsKey(logbookId)
            ? new List<ConfigurationNote>()
            : tags.Aggregate(LogbookEntries[logbookId], (current, tag) => current.Where(n => n.Tags.Select(ct => ct.Tag).Contains(tag)).ToList());

        /// <summary>
        /// Get all logbookEntries in the cache for a specific logbook containing a specific string.
        /// </summary>
        /// <param name="logbookId">Id of the logbook containing the logbookEntries to retrieve</param>
        /// <param name="searchText">text string to search for, this is case insensitive</param>
        public List<ConfigurationNote> GetEntriesByText(string logbookId, string searchText) => !LogbookEntries.ContainsKey(logbookId)
            ? new List<ConfigurationNote>()
            : LogbookEntries[logbookId].Where(n => n.Note.ToLower().Contains(searchText.ToLower())).ToList();

        public void ClearCache()
        {
            Logbooks.Clear();
            LogbookEntries.Clear();
            Tags.Clear();
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
