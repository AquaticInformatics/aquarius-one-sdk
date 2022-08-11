using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Common.Logbook
{
    public class LogbooksCache
    {
        private readonly ClientSDK _clientSdk;

        private Dictionary<string, LogbookCache> LogbookCaches { get; } = new Dictionary<string, LogbookCache>();
        
        public LogbooksCache(ClientSDK clientSdk, string serializedCache = "")
        {
            _clientSdk = clientSdk;

            if (string.IsNullOrEmpty(serializedCache)) return;

            var cache = Load(serializedCache);

            if (cache == null)
                throw new ArgumentException("Serialized cache could not be deserialized");

            OperationIds = cache.OperationIds;
            LogbookCaches = cache.LogbookCaches;
        }

        public List<string> OperationIds { get; } = new List<string>();

        /// <summary>
        /// Clears and resets the operationIds that this cache can contain data for
        /// </summary>
        /// <param name="operationIds"></param>
        public void SetOperations(params string[] operationIds)
        {
            OperationIds.Clear();

            foreach (var operationId in operationIds)
                AddOperation(operationId);
        }

        /// <summary>
        /// Adds an operation to the list of operations maintained by this cache
        /// </summary>
        /// <param name="operationId">Id of teh operation to add</param>
        public void AddOperation(string operationId)
        {
            if (Guid.TryParse(operationId, out _))
                OperationIds.Add(operationId);

            throw new ArgumentOutOfRangeException(nameof(operationId), operationId, "OperationId must be a guid");
        }

        /// <summary>
        /// Create logbookCaches for multiple operations and load the logbook data
        /// </summary>
        /// <param name="operationIds">Identifiers of the operations for which to create caches and load data, the provided array will overwrite <see cref="OperationIds"/>.
        /// If no parameters are provided then <see cref="OperationIds"/> will be used</param>
        public async Task<bool> LoadAllLogbookCachesAsync(params string[] operationIds)
        {
            try
            {
                if (operationIds.Length == 0 && !OperationIds.Any()) return false;

                SetOperations(operationIds);

                foreach (var operationId in OperationIds)
                    await LoadLogbookCacheByOperationAsync(operationId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Create logbookCaches for a single operation and load the logbook data
        /// </summary>
        /// <param name="operationId">Identifier of the operations for which to create a cache and load data, provided operationId is added to <see cref="OperationIds"/></param>
        public async Task<bool> LoadLogbookCacheByOperationAsync(string operationId)
        {
            try
            {
                if (string.IsNullOrEmpty(operationId)) return false;

                AddOperation(operationId);

                var cache = new LogbookCache(_clientSdk);
                var loaded = await cache.LoadLogbooksAsync(operationId);

                if (loaded)
                    LogbookCaches.Add(operationId, cache);

                return loaded;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads logbookEntries for all logbooks in all operations in the cache
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<bool> LoadEntriesForAllOperationsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                foreach (var kvp in LogbookCaches)
                    await LoadEntriesByOperationAsync(kvp.Key, startDate, endDate);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads logbookEntries for all logbooks in all operations in the cache
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<bool> LoadEntriesByOperationAsync(string operationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await LogbookCaches[operationId].LoadLogbookEntriesAsync(startDate, endDate);
            }
            catch
            {
                return false;
            }
        }

        public LogbookCache GetLogbookCache(string operationId)
        {
            if (string.IsNullOrEmpty(operationId) || !LogbookCaches.ContainsKey(operationId))
                return null;

            return LogbookCaches[operationId];
        }

        public void ClearCache() => LogbookCaches.Clear();

        public void ClearCacheByOperation(string operationId) => LogbookCaches.Remove(operationId);

        public List<LogbookCache> GetLogbookCaches() => LogbookCaches.Values.ToList();

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

        public static LogbooksCache Load(string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<LogbooksCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch
            {
                return null;
            }
        }
    }
}
