using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ONE.Enterprise.Operations
{
    public class OperationsCache
    {
        List<OperationCache> _operations;

        public OperationsCache(List<OperationCache> operations)
        {
            _operations = operations;
        }
        public string GuidByIndex(string index)
        {
            int.TryParse(index, out int idx);
            if (_operations == null || idx > _operations.Count - 1 || idx < 0 || _operations.Count == 0)
                return EnumErrors.ERR_INDEX_OUT_OF_RANGE.ToString();
            else
                return _operations[idx].Id;
        }
        public string Name(string guid)
        {
            if (_operations == null || string.IsNullOrEmpty(guid))
                return EnumErrors.ERR_INVALID_OPERATION_GUID.ToString();
            var operation = GetOperationById(guid);
            if (operation == null)
                return EnumErrors.ERR_INVALID_OPERATION_GUID.ToString();
            return operation.Name;
        }
       
        public OperationCache GetOperationById(string id)
        {
            if (string.IsNullOrEmpty(id) || _operations == null)
                return null;
            var matches = _operations.Where(c => c.Id.ToUpper() == id.ToUpper());
            if (matches.Count() > 0)
                return matches.First();
            return null;
        }
    }
}
