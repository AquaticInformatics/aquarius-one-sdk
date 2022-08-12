using ONE.Common.Logbook;
using ONE.Operations;

namespace ONE
{
    public class CacheHelper
    {
        ClientSDK _clientSDK;
        public ONE.Common.Library.Cache LibaryCache { get; set; }
        public LogbooksCache LogbooksCache { get; set; }
        public OperationsCache OperationsCache { get; set; }
        public ONE.Enterprise.Report.Cache ReportCache { get; set; }
        public CacheHelper(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
            LibaryCache = new Common.Library.Cache(clientSDK);
            LogbooksCache = new LogbooksCache(clientSDK);
            OperationsCache = new OperationsCache(clientSDK);
            ReportCache = new ONE.Enterprise.Report.Cache();

        }
    }
}
