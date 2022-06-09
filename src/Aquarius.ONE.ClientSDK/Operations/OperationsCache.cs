using Newtonsoft.Json;
using ONE.Enterprise.Twin;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Operations
{
    public class OperationsCache
    {
        ClientSDK _clientSDK;

        public List<OperationCache> Operations { get; set; }

        public OperationsCache(ClientSDK clientSDK, string serializedObject = "")
        {
            _clientSDK = clientSDK;
            Operations = new List<OperationCache>();
            if (!string.IsNullOrEmpty(serializedObject))
            {
                var operationsCache = JsonConvert.DeserializeObject<OperationsCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Operations = operationsCache.Operations;
                foreach (var operationCache in Operations)
                {
                    operationCache.SetClientSDK(clientSDK);
                    var allOperationDecendentTwins = operationCache.LocationTwins.Union(operationCache.ColumnTwins).ToList();
                    operationCache.AddChildren(operationCache.DigitalTwinItem, allOperationDecendentTwins);
                    operationCache.CacheColumns();
                }
            }
        }
        public OperationsCache(string serializedObject)
        {
            try
            {
                var operationsCache = JsonConvert.DeserializeObject<OperationsCache>(serializedObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Operations = operationsCache.Operations;
                foreach (var operationCache in Operations)
                {
                    var allOperationDecendentTwins = operationCache.LocationTwins.Union(operationCache.ColumnTwins).ToList();
                    operationCache.AddChildren(operationCache.DigitalTwinItem, allOperationDecendentTwins);
                    operationCache.CacheColumns();
                }
            }
            catch (Exception ex)
            {
                Operations = new List<OperationCache>();
            }
        }
        public OperationsCache()
        { }
        private OperationCache _currentOperation;
        public OperationCache CurrentOperation
        {
            get
            {
                return _currentOperation;
            }
            set
            {
                _currentOperation = value;
            }
        }
        public void Unload()
        {
            Operations = new List<OperationCache>();
        }
        public async Task<List<OperationCache>> LoadOperationsAsync(bool loadAllOperationCaches = false)
        {
            if (_clientSDK.Authentication.User == null)
            {
                var result = await _clientSDK.Authentication.GetUserInfoAsync();
                _clientSDK.Authentication.User = await _clientSDK.UserHelper.GetUserFromUserInfoAsync(result);
            }
            var operationTwins = await _clientSDK.DigitalTwin.GetDescendantsByTypeAsync(_clientSDK.Authentication.User.TenantId, Constants.SpaceCategory.OperationType.RefId);
            var cacheTasks = new List<Task>();
            foreach (var operationTwin in operationTwins)
            {
                var operationCache = new OperationCache(_clientSDK, operationTwin);
                Operations.Add(operationCache);
                if (loadAllOperationCaches)
                {
                    var cacheTask = operationCache.LoadAsync();
                    cacheTasks.Add(cacheTask);
                }
            }
            if (loadAllOperationCaches && cacheTasks.Count > 0)
                await Task.WhenAll(cacheTasks);
            Operations = Operations.OrderBy(p => p.Name).ToList();
            return Operations;
        }
        public string GuidByIndex(string index)
        {
            int.TryParse(index, out int idx);
            if (Operations == null || idx > Operations.Count - 1 || idx < 0 || Operations.Count == 0)
                return EnumErrors.ERR_INDEX_OUT_OF_RANGE.ToString();
            else
                return Operations[idx].Id;
        }
        public string Name(string guid)
        {
            if (Operations == null || string.IsNullOrEmpty(guid))
                return EnumErrors.ERR_INVALID_OPERATION_GUID.ToString();
            var operation = GetOperationById(guid);
            if (operation == null)
                return EnumErrors.ERR_INVALID_OPERATION_GUID.ToString();
            return operation.Name;
        }
       
        public OperationCache GetOperationById(string guid)
        {
            if (string.IsNullOrEmpty(guid) || Operations == null)
                return null;
            var matches = Operations.Where(c => c.Id.ToUpper() == guid.ToUpper());
            if (matches.Count() > 0)
                return matches.First();
            return null;
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
        public void Load(string serializedObject)
        {
            
        }
        
    }
}
