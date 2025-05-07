using ONE.Models.CSharp;
using System.Net.Http;

namespace ONE.ClientSDK.Utilities
{
	public class ServiceResponse
	{
		public string Result { get; set; }
		public ApiResponse ApiResponse { get; set; }
		public HttpResponseMessage ResponseMessage { get; set;}
		public long ElapsedMs { get; set; }
	}
}
