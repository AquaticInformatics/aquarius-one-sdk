using Newtonsoft.Json;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp.Constants.TwinCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.ClientSDK.Operations
{
	public class OperationsCache
	{
		private readonly OneApi _clientSdk;

		public List<OperationCache> Operations { get; set; }

		public OperationsCache(OneApi clientSdk, string serializedObject = "")
		{
			_clientSdk = clientSdk;
			Operations = new List<OperationCache>();
			if (!string.IsNullOrEmpty(serializedObject))
			{
				var operationsCache = JsonConvert.DeserializeObject<OperationsCache>(serializedObject, JsonExtensions.IgnoreNullSerializerSettings);
				Operations = operationsCache.Operations;
				foreach (var operationCache in Operations)
				{
					operationCache.SetClientSdk(clientSdk);
					var allOperationDescendantTwins = operationCache.LocationTwins.Union(operationCache.ColumnTwins).ToList();
					operationCache.AddChildren(operationCache.DigitalTwinItem, allOperationDescendantTwins);
					operationCache.CacheColumns();
				}
			}
		}

		public OperationsCache(string serializedObject)
		{
			try
			{
				var operationsCache = JsonConvert.DeserializeObject<OperationsCache>(serializedObject, JsonExtensions.IgnoreNullSerializerSettings);
				Operations = operationsCache.Operations;
				foreach (var operationCache in Operations)
				{
					var allOperationDescendantTwins = operationCache.LocationTwins.Union(operationCache.ColumnTwins).ToList();
					operationCache.AddChildren(operationCache.DigitalTwinItem, allOperationDescendantTwins);
					operationCache.CacheColumns();
				}
			}
			catch (Exception)
			{
				Operations = new List<OperationCache>();
			}
		}

		public OperationsCache()
		{ }

		public OperationCache CurrentOperation { get; set; }

		public void Unload()
		{
			Operations = new List<OperationCache>();
		}

		public async Task<List<OperationCache>> LoadOperationsAsync(bool loadAllOperationCaches = false)
		{
			if (_clientSdk.Authentication.User == null)
			{
				var result = await _clientSdk.Authentication.GetUserInfoAsync();
				_clientSdk.Authentication.User = await _clientSdk.UserHelper.GetUserFromUserInfoAsync(result);
			}

			var operationTwins = await _clientSdk.DigitalTwin.GetDescendantsByTypeAsync(_clientSdk.Authentication.User.TenantId, SpaceConstants.OperationType.RefId);

			foreach (var operationTwin in operationTwins)
			{
				var operationCache = new OperationCache(_clientSdk, operationTwin);
				Operations.Add(operationCache);
				
				if (loadAllOperationCaches)
					await operationCache.LoadAsync();
			}
			
			Operations = Operations.OrderBy(p => p.Name).ToList();
			return Operations;
		}

		public string GuidByIndex(string index)
		{
			int.TryParse(index, out var idx);
			if (Operations == null || idx > Operations.Count - 1 || idx < 0 || Operations.Count == 0)
				return nameof(EnumErrors.ERR_INDEX_OUT_OF_RANGE);

			return Operations[idx].Id;
		}

		public string Name(string guid)
		{
			if (Operations == null || string.IsNullOrEmpty(guid))
				return nameof(EnumErrors.ERR_INVALID_OPERATION_GUID);
			var operation = GetOperationById(guid);
			return operation == null ? nameof(EnumErrors.ERR_INVALID_OPERATION_GUID) : operation.Name;
		}
	   
		public OperationCache GetOperationById(string guid)
		{
			if (string.IsNullOrEmpty(guid) || Operations == null)
				return null;
			return Operations.FirstOrDefault(c => string.Equals(c.Id, guid, StringComparison.CurrentCultureIgnoreCase));
		}

		public override string ToString()
		{
			try
			{
				return JsonConvert.SerializeObject(this, JsonExtensions.IgnoreNullSerializerSettings);
			}
			catch
			{
				return base.ToString();
			}
		}

		public OperationsCache Load(string serializedObject)
		{
			try
			{
				return JsonConvert.DeserializeObject<OperationsCache>(serializedObject, JsonExtensions.IgnoreNullSerializerSettings);
			}
			catch (Exception)
			{
				if (_clientSdk.ThrowApiErrors)
					throw;

				return null;
			}
		}
	}
}
