using ONE.Enterprise.Twin;
using ONE.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONE.Operations
{
    public class OperationsCache
    {
        ClientSDK _clientSDK;

        public List<OperationCache> Operations;

        public OperationsCache(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
        }
        private OperationCache _defaultOperation;
        public OperationCache DefaultOperation
        {
            get
            {
                return _defaultOperation;
            }
            set
            {
                //ValueFunctions = new ValueFunctions(ReportStartDate, Operations, value);
                _defaultOperation = value;
            }
        }
        public async Task<List<OperationCache>> LoadOperationsAsync()
        {
            if (_clientSDK.Authentication.User == null)
            {
                var result = await _clientSDK.Authentication.GetUserInfo();
                _clientSDK.Authentication.User = await _clientSDK.UserHelper.GetUserFromUserInfoAsync(result);
            }
            var operationTwins = await _clientSDK.DigitalTwin.GetDecendantsByTypeAsync(_clientSDK.Authentication.User.TenantId, Constants.SpaceCategory.OperationType.RefId);
            foreach (var operationTwin in operationTwins)
            {
                Operations.Add(new OperationCache(_clientSDK, operationTwin));
            }
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
       
        public OperationCache GetOperationById(string id)
        {
            if (string.IsNullOrEmpty(id) || Operations == null)
                return null;
            var matches = Operations.Where(c => c.Id.ToUpper() == id.ToUpper());
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
    }
}
