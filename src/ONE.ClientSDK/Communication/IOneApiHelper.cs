using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using ONE.ClientSDK.Utilities;

namespace ONE.ClientSDK.Communication
{
	public interface IOneApiHelper
	{
		event EventHandler<ClientApiLoggerEventArgs> Event;

		Task<T> GetAsync<T>(string uri);

		Task<T> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);

		Task<T> BuildRequestAndSendAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null);

		HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null);
	}
}