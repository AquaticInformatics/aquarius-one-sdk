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

            if (_clientSdk.ThrowAPIErrors && cache == null)
                throw new ArgumentException("Serialized cache could not be deserialized");

            OperationIds = cache?.OperationIds ?? new List<string>();
            LogbookCaches = cache?.LogbookCaches ?? new Dictionary<string, LogbookCache>();
        }

        public List<string> OperationIds { get; } = new List<string>();

        /// <summary>
        /// Clears and resets the operationIds that this cache can contain data for
        /// </summary>
        /// <param name="operationIds">Identifiers for the operations associated to the data contained in this cache</param>
        public List<bool> SetOperations(params string[] operationIds)
        {
            OperationIds.Clear();

            return operationIds.Select(AddOperation).ToList();
        }

        /// <summary>
        /// Adds an operation to the list of operations maintained by this cache
        /// </summary>
        /// <param name="operationId">Id of the operation to add</param>
        public bool AddOperation(string operationId)
        {
            if (Guid.TryParse(operationId, out _))
            {
                if (!OperationIds.Contains(operationId))
                    OperationIds.Add(operationId);
            }
            else
                ErrorResponse(new ArgumentException("OperationId must be a guid"), false);

            return true;
        }

        /// <summary>
        /// Create logbookCaches for multiple operations and load the logbook data
        /// </summary>
        /// <param name="operationIds">Identifiers of the operations for which to create caches and load data. If no parameters are provided then <see cref="OperationIds"/> will be used,
        /// otherwise, the provided array will overwrite <see cref="OperationIds"/>.</param>
        public async Task<Dictionary<string, bool>> LoadAllLogbookCachesAsync(params string[] operationIds)
        {
            var loaded = new Dictionary<string, bool>();

            if (operationIds.Length == 0 && !OperationIds.Any())
                return ErrorResponse(new ArgumentException("No operations were provided or previously set"), loaded);

            if (SetOperations(operationIds).Any(b => !b))
                ErrorResponse<Dictionary<string, bool>>(new ArgumentException("Failed to set one or more operations, please ensure the provided ids are all guids"), null);

            foreach (var operationId in OperationIds)
                loaded.Add(operationId, await LoadLogbookCacheByOperationAsync(operationId));

            return loaded;
        }

        /// <summary>
        /// Create a logbookCache for a single operation and load the logbook data, the operationId is also added to <see cref="OperationIds"/>
        /// </summary>
        /// <param name="operationId">Identifier of the operation for which to create a cache and load data, provided operationId is added to <see cref="OperationIds"/></param>
        public async Task<bool> LoadLogbookCacheByOperationAsync(string operationId)
        {
            if (!AddOperation(operationId))
                return ErrorResponse(new ArgumentException($"Failed to add operation ({operationId})"), false);

            var cache = new LogbookCache(_clientSdk);
            var loaded = await cache.LoadLogbooksAsync(operationId);

            if (loaded)
                LogbookCaches.Add(operationId, cache);
            else
                return ErrorResponse(new ApplicationException($"Failed to load logbooks for operation ({operationId})"), false);

            return true;
        }

        /// <summary>
        /// Loads logbookEntries for all logbooks in all operations in the cache
        /// </summary>
        /// <param name="startDate">loads logbookEntries on or after this date</param>
        /// <param name="endDate">load logbookEntries before this date</param>
        public async Task<Dictionary<string, bool>> LoadEntriesForAllOperationsAsync(DateTime startDate, DateTime endDate)
        {
            var loaded = new Dictionary<string, bool>();

            foreach (var kvp in LogbookCaches)
                loaded.Add(kvp.Key, await LoadEntriesByOperationAsync(kvp.Key, startDate, endDate));

            return loaded;
        }

        /// <summary>
        /// Loads logbookEntries for all logbooks in a specific operation in the cache
        /// </summary>
        /// <param name="operationId">Identifier of the operation for which to load logbookEntry data</param>
        /// <param name="startDate">loads logbookEntries on or after this date</param>
        /// <param name="endDate">load logbookEntries before this date</param>
        public async Task<bool> LoadEntriesByOperationAsync(string operationId, DateTime startDate, DateTime endDate) => ValidOperation(operationId)
            ? (await LogbookCaches[operationId].LoadLogbookEntriesAsync(startDate, endDate)).All(x => x.Value)
            : ErrorResponse(NotInCacheException(operationId), false);

        /// <summary>
        /// Retrieve the logbookCache for a specific operation
        /// </summary>
        /// <param name="operationId">Identifier of the operation associated to teh cache to be retrieved</param>
        public LogbookCache GetLogbookCache(string operationId) =>
            ValidOperation(operationId) ? LogbookCaches[operationId] : ErrorResponse<LogbookCache>(NotInCacheException(operationId), null);

        /// <summary>
        /// Retrieve all logbookCaches contained in this cache
        /// </summary>
        public List<LogbookCache> GetLogbookCaches() => LogbookCaches.Values.ToList();

        public void ClearCache() => LogbookCaches.Clear();

        public void ClearCacheByOperation(string operationId) => LogbookCaches.Remove(operationId);

        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, base.ToString());
            }
        }

        public LogbooksCache Load(string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<LogbooksCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                return ErrorResponse<LogbooksCache>(ex, null);
            }
        }

        private bool ValidOperation(string operationId) => !string.IsNullOrEmpty(operationId) && OperationIds.Contains(operationId) && LogbookCaches.ContainsKey(operationId);

        private static Exception NotInCacheException(string operationId) => new ArgumentException($"Operation ({operationId}) is not part of this cache");

        private T ErrorResponse<T>(Exception exception, T result)
        {
            if (_clientSdk.ThrowAPIErrors)
                throw exception;

            return result;
        }
    }
}
