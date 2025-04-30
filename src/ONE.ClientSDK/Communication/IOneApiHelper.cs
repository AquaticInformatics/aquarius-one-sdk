using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace ONE.ClientSDK.Communication
{
    public interface IOneApiHelper
    {
        Task<T> GetAsync<T>(string uri);

        Task<T> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);

        Task<T> BuildRequestAndSendAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null);

        HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null);
    }
}