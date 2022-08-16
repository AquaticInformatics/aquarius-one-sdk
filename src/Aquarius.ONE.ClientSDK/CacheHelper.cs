using ONE.Operations;

namespace ONE
{
    public class CacheHelper
    {
        public Common.Library.Cache LibaryCache { get; set; }
        public OperationsCache OperationsCache { get; set; }
        public CacheHelper(ClientSDK clientSDK)
        {
            LibaryCache = new Common.Library.Cache(clientSDK);
            OperationsCache = new OperationsCache(clientSDK);
        }
    }
}
