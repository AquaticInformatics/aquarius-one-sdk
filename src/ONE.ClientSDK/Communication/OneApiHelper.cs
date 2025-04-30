using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Communication
{
    public class OneApiHelper : IOneApiHelper
    {
        private readonly AuthenticationApi _authentication;
        private readonly bool _useProtobufModels;

        public OneApiHelper(AuthenticationApi authentication, bool useProtobufModels)
        {
            _authentication = authentication;
            _useProtobufModels = useProtobufModels;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = _useProtobufModels ? await _authentication.HttpProtocolBufferClient.GetAsync(uri) : await _authentication.HttpJsonClient.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.NoContent && typeof(T) == typeof(ApiResponse))
                return (T)(object)new ApiResponse { StatusCode = 204 };

            return await DeserializeAsync<T>(response.Content);
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var response = _useProtobufModels
                ? await _authentication.HttpProtocolBufferClient.SendAsync(httpRequestMessage, cancellationToken)
                : await _authentication.HttpJsonClient.SendAsync(httpRequestMessage, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent && typeof(T) == typeof(ApiResponse))
                return (T)(object)new ApiResponse { StatusCode = 204 };

            return await DeserializeAsync<T>(response.Content);
        }

        public async Task<T> BuildRequestAndSendAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null)
        {
            using (var request = CreateRequest(method, uri, content))
            {
                return await SendAsync<T>(request, cancellationToken);
            }
        }

        public HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null)
        {
            var request = new HttpRequestMessage(method, uri);

            switch (content)
            {
                case null:
                    break;
                case IMessage message when _useProtobufModels:
                    request.Headers.Add("Accept", "application/x-protobuf");
                    if (message != null)
                    {
                        request.Content = new ByteArrayContent(message.ToByteArray());
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
                    }                    
                    break;
                default:
                    request.Headers.Add("Accept", "application/json");
                    var messageString = JsonConvert.SerializeObject(content);
                    if (!string.IsNullOrEmpty(messageString))
                    {
                        request.Content = new StringContent(messageString);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }
                    break;
            }

            return request;
        }

        private async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            if (content == null)
                return default;

            if (_useProtobufModels)
            {
                if (Activator.CreateInstance(typeof(T)) is IMessage result)
                {
                    result.MergeFrom(await content.ReadAsByteArrayAsync());

                    return (T)result;
                }
                return default;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
            }
        }
    }
}