using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Communication
{
	public interface IOneApiHelper
	{
		event EventHandler<ClientApiLoggerEventArgs> Event;

		Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellation);

		Task<ApiResponse> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);

		Task<ApiResponse> BuildRequestAndSendAsync(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null);

		HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null);
	}
}