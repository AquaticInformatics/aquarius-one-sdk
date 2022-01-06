using System;
using System.Collections.Generic;
using System.Text;
using ONE;

namespace ONE
{
    public class CacheHelper
    {
        ClientSDK _clientSDK;
        public ONE.Common.Library.Cache LibaryCache { get; set; }
        public CacheHelper(ClientSDK clientSDK)
        {
            _clientSDK = clientSDK;
            LibaryCache = new Common.Library.Cache(clientSDK);
        }
    }
}
