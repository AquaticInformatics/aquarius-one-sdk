﻿using Newtonsoft.Json;
using ONE.ClientSDK.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Operations.Sample
{
	public class SamplesCache
	{
		private readonly OneApi _clientSdk;

		[JsonProperty]
		private Dictionary<string, SampleCache> SampleCaches { get; set; } = new Dictionary<string, SampleCache>();

		[JsonProperty]
		public List<string> OperationIds { get; private set; } = new List<string>();

		public SamplesCache(OneApi clientSdk, string serializedCache = "")
		{
			_clientSdk = clientSdk;

			if (string.IsNullOrEmpty(serializedCache))
				return;

			var cache = Load(serializedCache);

			if (_clientSdk.ThrowApiErrors && cache == null)
				throw CacheExceptions.NotDeserializedCacheException();

			OperationIds = cache?.OperationIds ?? new List<string>();
			SampleCaches = cache?.SampleCaches ?? new Dictionary<string, SampleCache>();
		}

		/// <summary> 
		/// Clears and resets the operationIds that this cache can contain data for 
		/// </summary> 
		/// <param name="operationIds">Identifiers for the operations associated to the data contained in this cache</param> 
		public List<bool> SetOperations(params string[] operationIds)
		{
			OperationIds.Clear();
			ClearCache();

			return operationIds.Select(AddOperation).ToList();
		}

		/// <summary> 
		/// Adds an operation to the list of operations maintained by this cache 
		/// </summary> 
		/// <param name="operationId">ID of the operation to add</param> 
		public bool AddOperation(string operationId)
		{
			if (!Guid.TryParse(operationId, out var guidId))
				return ErrorResponse(new ArgumentException("OperationId must be a guid"), false);

			var normalized = guidId.ToString();

			if (!OperationIds.Contains(normalized))
				OperationIds.Add(normalized);

			return true;
		}

		/// <summary> 
		/// Loads caches for the specified operations for the specified time range. 
		/// </summary> 
		/// <param name="startDate">Load data with dates equal or later than this.</param> 
		/// <param name="endDate">Load data with dates earlier than this.</param> 
		/// <param name="operationIds">Identifiers of the operations to create caches and load data for. 
		/// If no parameters are provided then <see cref="OperationIds"/> will be used, 
		/// otherwise, the provided array will overwrite <see cref="OperationIds"/>. 
		/// </param> 
		public async Task<Dictionary<string, bool>> LoadOperationsAsync(
			DateTime startDate, DateTime endDate, params string[] operationIds)
		{
			var loaded = new Dictionary<string, bool>();

			if (operationIds.Length == 0 && !OperationIds.Any())
				return ErrorResponse(new ArgumentException("No operations were provided or previously set"), loaded);

			if (operationIds.Length > 0 && SetOperations(operationIds).Any(b => !b))
				ErrorResponse<Dictionary<string, bool>>(new ArgumentException("Failed to set one or more operations, please ensure the provided ids are all guids"), null);

			foreach (var operationId in OperationIds)
				loaded.Add(operationId, await LoadByOperationAsync(operationId, startDate, endDate));

			return loaded;
		}

		/// <summary> 
		/// Loads the cache for the specified operation and time range. 
		/// </summary> 
		/// <param name="operationId">ID of the operation to load.</param> 
		/// <param name="startDate">Load data with dates equal or later than this.</param> 
		/// <param name="endDate">Load data with dates earlier than this.</param> 
		public async Task<bool> LoadByOperationAsync(string operationId, DateTime startDate, DateTime endDate)
		{
			if (!AddOperation(operationId))
				return ErrorResponse(CacheExceptions.FailedToAddException(operationId), false);

			var cache = new SampleCache(_clientSdk);
			var loaded = await cache.LoadAsync(startDate, endDate, operationId);

			if (!loaded)
				return ErrorResponse(new ApplicationException($"Failed to load cache for operation ({operationId})"), false);

			SampleCaches[Guid.Parse(operationId).ToString()] = cache;
			return true;
		}

		/// <summary> 
		/// Get the cache for the specified operation. 
		/// </summary> 
		public SampleCache GetCacheByOperation(string operationId) => !ValidOperation(operationId)
			? ErrorResponse<SampleCache>(CacheExceptions.NotInCacheException(operationId), null)
			: SampleCaches[Guid.Parse(operationId).ToString()];

		/// <summary> 
		/// Get all caches. 
		/// </summary> 
		public List<SampleCache> GetAllCaches() => SampleCaches.Values.ToList();

		/// <summary> 
		/// Clear caches for all operations. 
		/// </summary> 
		public void ClearCache() => SampleCaches.Clear();

		/// <summary> 
		/// Clear cache for the specified operation. 
		/// </summary> 
		public void ClearCacheByOperation(string operationId)
		{
			if (Guid.TryParse(operationId, out var guidId))
				SampleCaches.Remove(guidId.ToString());
		}

		/// <summary> 
		/// Get the JSON string for all caches. 
		/// </summary> 
		public override string ToString()
		{
			try
			{
				return JsonConvert.SerializeObject(this, JsonExtensions.CamelCaseSerializerSettings);
			}
			catch (Exception ex)
			{
				return ErrorResponse(ex, base.ToString());
			}
		}

		/// <summary> 
		/// Load all caches from a JSON string. 
		/// </summary> 
		public SamplesCache Load(string serializedObject)
		{
			try
			{
				return JsonConvert.DeserializeObject<SamplesCache>(serializedObject, JsonExtensions.CamelCaseSerializerSettings);
			}
			catch (Exception ex)
			{
				return ErrorResponse<SamplesCache>(ex, null);
			}
		}

		private bool ValidOperation(string operationId)
		{
			if (!Guid.TryParse(operationId, out var guidId))
				return false;

			var normalized = guidId.ToString();

			return OperationIds.Contains(normalized) && SampleCaches.ContainsKey(normalized);
		}

		private T ErrorResponse<T>(Exception exception, T result)
		{
			if (_clientSdk.ThrowApiErrors)
				throw exception;

			return result;
		}
	}
}