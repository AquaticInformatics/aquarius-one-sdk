using ONE.ClientSDK.Common.Library;
using ONE.ClientSDK.Common.Logbook;
using ONE.ClientSDK.Operations;
using ONE.ClientSDK.Operations.Sample;

namespace ONE.ClientSDK
{
	public class CacheHelper
	{
		public LibraryCache LibraryCache { get; set; }
		public LogbooksCache LogbooksCache { get; set; }
		public OperationsCache OperationsCache { get; set; }
		public SamplesCache SamplesCache { get; set; }

		public CacheHelper(OneApi clientSdk)
		{
			LibraryCache = new LibraryCache(clientSdk);
			LogbooksCache = new LogbooksCache(clientSdk);
			OperationsCache = new OperationsCache(clientSdk);
			SamplesCache = new SamplesCache(clientSdk);
		}
	}
}
