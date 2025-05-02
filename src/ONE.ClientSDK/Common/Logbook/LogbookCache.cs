using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using ONE.Shared.Time;
using Proto = ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Logbook
{
	public class LogbookCache
	{
		private readonly OneApi _clientSdk;

		[JsonProperty] private Dictionary<string, Proto.Configuration> Logbooks { get; set; } = new Dictionary<string, Proto.Configuration>();
		[JsonProperty] private Dictionary<string, List<string>> Tags { get; } = new Dictionary<string, List<string>>();
		[JsonProperty] private Dictionary<string, List<ConfigurationNote>> LogbookEntries { get; } = new Dictionary<string, List<ConfigurationNote>>();

		public LogbookCache(OneApi clientSdk, string serializedCache = "")
		{
			_clientSdk = clientSdk;

			if (string.IsNullOrEmpty(serializedCache)) return;

			var cache = Load(serializedCache);

			if (_clientSdk.ThrowApiErrors && cache == null)
				throw CacheExceptions.NotDeserializedCacheException();

			OperationId = cache?.OperationId ?? string.Empty;
			Logbooks = cache?.Logbooks ?? new Dictionary<string, Proto.Configuration>();
			Tags = cache?.Tags ?? new Dictionary<string, List<string>>();
			LogbookEntries = cache?.LogbookEntries ?? new Dictionary<string, List<ConfigurationNote>>();
		}

		public string OperationId { get; private set; }

		public bool SetOperationId(string operationId)
		{
			if (Guid.TryParse(operationId, out _))
				OperationId = operationId;
			else
				return ErrorResponse(CacheExceptions.IdMustBeGuidException("OperationId"), false);

			return true;
		}

		/// <summary>
		/// Load Logbook data for an operation
		/// </summary>
		/// <param name="operationId">Identifier of the operation for which to load data, uses <see cref="OperationId"/> if not set and will overwrite the existing OperationId if set.</param>
		public async Task<bool> LoadLogbooksAsync(string operationId = "")
		{
			if (string.IsNullOrEmpty(operationId) && string.IsNullOrEmpty(OperationId))
				return ErrorResponse(new ArgumentException("No operationId was provided or previously set"), false);

			if (!string.IsNullOrEmpty(operationId) && !SetOperationId(operationId))
				return ErrorResponse(new ArgumentException("Failed to set OperationId, ensure that it is a valid guid"), false);

			try
			{
				Logbooks = (await _clientSdk.Logbook.GetLogbooksAsync(OperationId)).ToDictionary(k => k.Id, v => v);

				return true;
			}
			catch (Exception ex)
			{
				return ErrorResponse(ex, false);
			}
		}

		/// <summary>
		/// Loads logbookEntries for all logbooks in the cache
		/// </summary>
		/// <param name="startDate">loads logbookEntries on or after this date</param>
		/// <param name="endDate">load logbookEntries before this date</param>
		public async Task<Dictionary<string, bool>> LoadLogbookEntriesAsync(DateTime startDate, DateTime endDate)
		{
			var loaded = new Dictionary<string, bool>();
			
			foreach (var logbookId in Logbooks.Keys)
				loaded.Add(logbookId, await LoadEntriesByLogbookAsync(logbookId, startDate, endDate));

			return loaded;
		}

		/// <summary>
		/// Loads logbookEntries for a specific logbook in the cache
		/// </summary>
		/// <param name="logbookId">identifier of the logbook containing the entries to be loaded</param>
		/// <param name="startDate">loads logbookEntries on or after this date</param>
		/// <param name="endDate">load logbookEntries before this date</param>
		public async Task<bool> LoadEntriesByLogbookAsync(string logbookId, DateTime startDate, DateTime endDate)
		{
			if (!Logbooks.ContainsKey(logbookId))
				return ErrorResponse(CacheExceptions.UnloadedException("Logbook", logbookId), false);

			try
			{
				LogbookEntries[logbookId] = await _clientSdk.Logbook.GetLogbookEntriesAsync(logbookId, startDate, endDate);

				Tags[logbookId] = new List<string>();

				foreach (var entry in LogbookEntries[logbookId])
					Tags[logbookId].AddRange(entry.Tags.Select(t => t.Tag));

				Tags[logbookId] = Tags[logbookId].Distinct().ToList();

				return true;
			}
			catch (Exception ex)
			{
				return ErrorResponse(ex, false);
			}
		}

		/// <summary>
		/// Retrieve a logbook from the cache by ID
		/// </summary>
		/// <param name="logbookId">ID of the logbook to retrieve</param>
		public Proto.Configuration GetLogbook(string logbookId) =>
			ValidLogbook(logbookId) ? Logbooks[logbookId] : ErrorResponse<Proto.Configuration>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

		/// <summary>
		/// Gets all logbooks in the cache
		/// </summary>
		public List<Proto.Configuration> GetLogbooks() => Logbooks.Values.ToList();

		/// <summary>
		/// Get the most recent entry for each logbook in the cache.
		/// </summary>
		public Dictionary<string, ConfigurationNote> GetLatestEntryPerLogbook() =>
			LogbookEntries.ToDictionary(k => k.Key, v => v.Value.OrderByDescending(n => n.NoteTime.ToDateTime()).FirstOrDefault());

		/// <summary>
		/// Retrieve a list of unique tags associated to a specific logbook
		/// </summary>
		/// <param name="logbookId">ID of the logbook containing the tags to be retrieved</param>
		public List<string> GetUniqueTags(string logbookId) => ValidLogbook(logbookId)
			? Tags[logbookId]
			: ErrorResponse<List<string>>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

		/// <summary>
		/// Get all logbookEntries in the cache for a specific logbook.
		/// </summary>
		/// <param name="logbookId">ID of the logbook containing the logbookEntries to retrieve</param>
		public List<ConfigurationNote> GetLogbookEntries(string logbookId) => ValidLogbook(logbookId)
			? LogbookEntries[logbookId]
			: ErrorResponse<List<ConfigurationNote>>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

		/// <summary>
		/// Get all logbookEntries in the cache for a specific logbook within a specific time range.
		/// </summary>
		/// <param name="logbookId">ID of the logbook containing the logbookEntries to retrieve</param>
		/// <param name="startDate">returns logbookEntries on or after this date</param>
		/// <param name="endDate">returns logbookEntries before this date</param>
		public List<ConfigurationNote> GetEntriesByDate(string logbookId, DateTime startDate, DateTime endDate) => ValidLogbook(logbookId)
			? LogbookEntries[logbookId].Where(e => e.NoteTime.ToDateTime() >= startDate && e.NoteTime.ToDateTime() < endDate).ToList()
			: ErrorResponse<List<ConfigurationNote>>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

		/// <summary>
		/// Get all logbookEntries in the cache for a specific logbook containing specific tags.
		/// </summary>
		/// <param name="logbookId">ID of the logbook containing the logbookEntries to retrieve</param>
		/// <param name="tags">tags to filter by, entries must contain all provided tags</param>
		public List<ConfigurationNote> GetEntriesByTags(string logbookId, params string[] tags) => ValidLogbook(logbookId)
			? tags.Aggregate(LogbookEntries[logbookId], (current, tag) => current.Where(n => n.Tags.Select(ct => ct.Tag).Contains(tag)).ToList())
			: ErrorResponse<List<ConfigurationNote>>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

		/// <summary>
		/// Get all logbookEntries in the cache for a specific logbook containing a specific string.
		/// </summary>
		/// <param name="logbookId">ID of the logbook containing the logbookEntries to retrieve</param>
		/// <param name="searchText">text string to search for, this is case-insensitive</param>
		public List<ConfigurationNote> GetEntriesByText(string logbookId, string searchText) => ValidLogbook(logbookId)
			? LogbookEntries[logbookId].Where(n => n.Note.ToLower().Contains(searchText.ToLower())).ToList()
			: ErrorResponse<List<ConfigurationNote>>(CacheExceptions.UnloadedException("Logbook", logbookId), null);

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
			catch (Exception ex)
			{
				return ErrorResponse(ex, base.ToString());
			}
		}

		public LogbookCache Load(string serializedObject)
		{
			try
			{
				return JsonConvert.DeserializeObject<LogbookCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			catch (Exception ex)
			{
				return ErrorResponse<LogbookCache>(ex, null);
			}
		}

		private bool ValidLogbook(string logbookId) =>
			!string.IsNullOrEmpty(logbookId) && Logbooks.ContainsKey(logbookId) && LogbookEntries.ContainsKey(logbookId) && Tags.ContainsKey(logbookId);

		private T ErrorResponse<T>(Exception exception, T result)
		{
			if (_clientSdk.ThrowApiErrors)
				throw exception;

			return result;
		}
	}
}
