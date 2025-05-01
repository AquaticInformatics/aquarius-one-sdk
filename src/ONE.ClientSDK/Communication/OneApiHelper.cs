using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Communication
{
    public class OneApiHelper : IOneApiHelper
    {
        private readonly AuthenticationApi _authentication;
        private readonly bool _continueOnCapturedContext;
        private readonly bool _useProtobufModels;
        private readonly JsonSerializerSettings _serializerSettings;

        public OneApiHelper(AuthenticationApi authentication, bool continueOnCapturedContext, bool useProtobufModels)
        {
            _authentication = authentication;
            _authentication.TokenExpired += RenewToken;
            _continueOnCapturedContext = continueOnCapturedContext;
            _useProtobufModels = useProtobufModels;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private void RenewToken(object sender, EventArgs _)
        {
            if (sender is AuthenticationApi authentication)
                authentication.LoginAsync().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().GetResult();
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = _useProtobufModels
                ? await _authentication.HttpProtocolBufferClient.GetAsync(uri).ConfigureAwait(_continueOnCapturedContext)
                : await _authentication.HttpJsonClient.GetAsync(uri).ConfigureAwait(_continueOnCapturedContext);

            if (typeof(T) == typeof(HttpResponseMessage))
                return (T)(object)response;

            if ((response.Content == null || response.StatusCode == HttpStatusCode.NoContent) && typeof(T) == typeof(ApiResponse))
                return (T)(object)new ApiResponse { StatusCode = (int)response.StatusCode };

            return await DeserializeAsync<T>(response.Content).ConfigureAwait(_continueOnCapturedContext);
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var response = _useProtobufModels
                ? await _authentication.HttpProtocolBufferClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(_continueOnCapturedContext)
                : await _authentication.HttpJsonClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(_continueOnCapturedContext);

            if (typeof(T) == typeof(HttpResponseMessage))
                return (T)(object)response;

            if ((response.Content == null || response.StatusCode == HttpStatusCode.NoContent) && typeof(T) == typeof(ApiResponse))
                return (T)(object)new ApiResponse { StatusCode = (int)response.StatusCode };

            return await DeserializeAsync<T>(response.Content).ConfigureAwait(_continueOnCapturedContext);
        }

        public async Task<T> BuildRequestAndSendAsync<T>(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null)
        {
            using (var request = CreateRequest(method, uri, content))
            {
                return await SendAsync<T>(request, cancellationToken).ConfigureAwait(_continueOnCapturedContext);
            }
        }

        public HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null)
        {
            var request = new HttpRequestMessage(method, uri);

            switch (content)
            {
                case null when _useProtobufModels:
                    request.Headers.Add("Accept", "application/x-protobuf");
                    break;
                case null:
                    request.Headers.Add("Accept", "application/json");
                    break;
                case IMessage message when _useProtobufModels:
                    request.Headers.Add("Accept", "application/x-protobuf");
                    request.Content = new ByteArrayContent(message.ToByteArray());
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
                    break;
                default:
                    request.Headers.Add("Accept", "application/json");
                    var messageString = JsonConvert.SerializeObject(content, _serializerSettings);
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
                    result.MergeFrom(await content.ReadAsByteArrayAsync().ConfigureAwait(_continueOnCapturedContext));

                    return (T)result;
                }
                return default;
            }

            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext), _serializerSettings);
        }
    }
}