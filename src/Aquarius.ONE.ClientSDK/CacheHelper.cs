using System;
using System.Collections.Generic;
using System.Text;
using ONE;
using ONE.Operations;

namespace ONE
{
    public class CacheHelper
    {
        ClientSDK _clientSDK;
        public ONE.Common.Library.Cache LibaryCache { get; set; }
        public OperationsCache OperationsCache { get; set; }
        public ONE.Enterprise.Report.Cache ReportCache { get; set; }
        public CacheHelper(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
            LibaryCache = new Common.Library.Cache(clientSDK);
            OperationsCache = new OperationsCache(clientSDK);
            ReportCache = new ONE.Enterprise.Report.Cache();

        }
    }
}
