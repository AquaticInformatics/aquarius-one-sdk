using ONE.Common.Logbook;
﻿using ONE.Operations;
using ONE.Operations.Sample;

namespace ONE
{
    public class CacheHelper
    {
        public Common.Library.Cache LibaryCache { get; set; }
        public LogbooksCache LogbooksCache { get; set; }
        public OperationsCache OperationsCache { get; set; }
        public SamplesCache SamplesCache { get; set; }

        public CacheHelper(ClientSDK clientSDK)
        {
            LibaryCache = new Common.Library.Cache(clientSDK);
            LogbooksCache = new LogbooksCache(clientSDK);
            OperationsCache = new OperationsCache(clientSDK);
            SamplesCache = new SamplesCache(clientSDK);
        }
    }
}
