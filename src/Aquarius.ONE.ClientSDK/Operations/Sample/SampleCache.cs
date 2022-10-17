﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ONE.Utilities;
using Proto = ONE.Models.CSharp;

namespace ONE.Operations.Sample
{
    public class SampleCache
    {
        private readonly ClientSDK _clientSdk;
        private readonly JsonSerializerSettings _jsonSettings;

        [JsonProperty]
        private Dictionary<string, Proto.Activity> Activities { get; set; } = new Dictionary<string, Proto.Activity>();

        /// <summary>
        /// The operation Id of cached data.
        /// </summary>
        [JsonProperty]
        public string OperationId { get; private set; }

        /// <summary>
        /// The start date of cached data.
        /// </summary>
        [JsonProperty]
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// The end date of cached data.
        /// </summary>
        [JsonProperty]
        public DateTime? EndDate { get; private set; }

        public SampleCache(ClientSDK clientSdk, string serializedCache = "")
        {
            _clientSdk = clientSdk;
            _jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            if (string.IsNullOrEmpty(serializedCache)) 
                return;

            var cache = Load(serializedCache);

            if (_clientSdk.ThrowAPIErrors && cache == null)
                throw new ArgumentException("Serialized cache could not be deserialized");

            OperationId = cache?.OperationId ?? string.Empty;
            StartDate = cache?.StartDate;
            EndDate = cache?.EndDate;
            Activities = cache?.Activities ?? new Dictionary<string, Proto.Activity>();
        }

        /// <summary>
        /// Sets the operation Id.
        /// </summary>
        public bool SetOperationId(string operationId)
        {
            if (!Guid.TryParse(operationId, out var guidId))
                return ErrorResponse(new ArgumentException("OperationId must be a guid"), false);

            var changed = string.IsNullOrEmpty(OperationId) || guidId != Guid.Parse(OperationId);

            if (changed)
            {
                ClearCache();
                OperationId = guidId.ToString();
            }

            return true;
        }

        /// <summary>
        /// Load data for an operation.
        /// </summary>
        /// <param name="startDate">Loads data on or after this date.</param>
        /// <param name="endDate">Loads data before this date.</param>
        /// <param name="operationId">Identifier of the operation for which to load data, uses <see cref="OperationId"/> if not set and will overwrite the existing OperationId if set.</param>
        public async Task<bool> LoadAsync(DateTime startDate, DateTime endDate, string operationId = "")
        {
            if (string.IsNullOrEmpty(operationId) && string.IsNullOrEmpty(OperationId))
                return ErrorResponse(new ArgumentException("No operationId was provided or previously set"), false);

            if (!string.IsNullOrEmpty(operationId) && !SetOperationId(operationId))
                return ErrorResponse(new ArgumentException("Failed to set OperationId, ensure that it is a valid guid"), false);

            if (startDate > endDate)
                return ErrorResponse(new ArgumentException("endDate must be greater than startDate"), false);

            try
            {
                Activities = 
                    (await _clientSdk.Sample.GetActivitiesAsync(OperationId, startDate: startDate, endDate: endDate)) 
                    .ToDictionary(k => k.Id, v => v);

                StartDate = startDate;
                EndDate = endDate;

                return true;
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, false);
            }
        }

        /// <summary>
        /// Retrieve data from the cache by activity Id.
        /// </summary>
        public Proto.Activity GetByActivity(string activityId)
        {
            if (!ValidActivity(activityId))
                return ErrorResponse<Proto.Activity>(UnloadedException(activityId), null);

            return Activities[Guid.Parse(activityId).ToString()];
        }

        /// <summary>
        /// Gets all activities in the cache.
        /// </summary>
        public List<Proto.Activity> GetAllActivities() => Activities.Values.ToList();

        /// <summary>
        /// Gets activities in the cache based on input criteria.
        /// </summary>
        public List<Proto.Activity> QueryActivities(string activityTypeId = null, int? statusCode = null, 
            int? priorityCode = null, DateTime? startDate = null, DateTime? endDate = null, 
            string scheduleId = null)
        {
            try
            {
                var activities = Activities.Values.AsQueryable();

                if (Guid.TryParse(activityTypeId, out var aId))
                    activities = activities.Where(x => x.ActivityTypeId == aId.ToString());

                if (statusCode.HasValue)
                    activities = activities.Where(x => x.StatusCode == statusCode.Value);

                if (priorityCode.HasValue)
                    activities = activities.Where(x => x.PriorityCode == priorityCode.Value);

                if (startDate.HasValue)
                    activities = activities.Where(x => x.ScheduledStart.ToDateTime() >= startDate.Value);

                if (endDate.HasValue)
                    activities = activities.Where(x => x.ScheduledEnd.ToDateTime() < endDate.Value);

                if (Guid.TryParse(scheduleId, out var sId))
                    activities = activities.Where(x => x.ScheduleId == sId.ToString());

                return activities.ToList();
            }
            catch (Exception ex)
            {
                return ErrorResponse<List<Proto.Activity>>(ex, null);
            }
        }

        /// <summary>
        /// Clear the cache.
        /// </summary>
        public void ClearCache()
        {
            Activities.Clear();
            OperationId = string.Empty;
            StartDate = null;
            EndDate = null;
        }

        /// <summary>
        /// Get the serialized cache.
        /// </summary>
        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this, _jsonSettings);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, base.ToString());
            }
        }

        public SampleCache Load(string serializedCache)
        {
            try
            {
                return JsonConvert.DeserializeObject<SampleCache>(serializedCache, _jsonSettings);
            }
            catch (Exception ex)
            {
                return ErrorResponse<SampleCache>(ex, null);
            }
        }

        private bool ValidActivity(string activityId)
        {
            if (!Guid.TryParse(activityId, out var guidId))
                return false;

            var normalized = guidId.ToString();

            return Activities.ContainsKey(normalized);
        }

        private static Exception UnloadedException(string activityId) => new ArgumentException($"Activity ({activityId}) is either not loaded or not part of this operation");

        private T ErrorResponse<T>(Exception exception, T result)
        {
            if (_clientSdk.ThrowAPIErrors)
                throw exception;

            return result;
        }
    }
}