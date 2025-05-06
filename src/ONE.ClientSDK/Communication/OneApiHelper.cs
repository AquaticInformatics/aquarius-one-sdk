using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;

namespace ONE.ClientSDK.Communication
{
	public class OneApiHelper : IOneApiHelper
	{
		private readonly AuthenticationApi _authentication;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _useProtobufModels;
		private readonly bool _logRestfulCalls;
		private readonly JsonSerializerSettings _serializerSettings;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public OneApiHelper(AuthenticationApi authentication, bool continueOnCapturedContext, bool useProtobufModels, bool logRestfulCalls)
		{
			_authentication = authentication;
			_authentication.TokenExpired += RenewToken;
			_continueOnCapturedContext = continueOnCapturedContext;
			_useProtobufModels = useProtobufModels;
			_logRestfulCalls = logRestfulCalls;

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

		public async Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellation)
		{
			var watch = Stopwatch.StartNew();

			var response = await _authentication.GetBaseClient(uri).GetAsync(uri, cancellation);

			watch.Stop();

			SaveRestCallData(response, watch.ElapsedMilliseconds);

			return response;
		}

		public async Task<ApiResponse> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
		{
			var watch = Stopwatch.StartNew();

			var response = await _authentication.GetBaseClient(httpRequestMessage.RequestUri.OriginalString).SendAsync(httpRequestMessage, cancellationToken);

			watch.Stop();

			SaveRestCallData(response, watch.ElapsedMilliseconds);

			if ((response.Content == null || response.StatusCode == HttpStatusCode.NoContent))
				return new ApiResponse { StatusCode = (int)response.StatusCode };

			return await DeserializeApiResponseAsync(response.Content).ConfigureAwait(_continueOnCapturedContext);
		}

		public async Task<ApiResponse> BuildRequestAndSendAsync(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null)
		{
			using (var request = CreateRequest(method, uri, content))
			{
				return await SendAsync(request, cancellationToken).ConfigureAwait(_continueOnCapturedContext);
			}
		}

		public HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null)
		{
			var request = new HttpRequestMessage(method, uri);

			request.Headers.Add("Accept", _useProtobufModels ? "application/x-protobuf" : "application/json");

			switch (content)
			{
				case null:
					break;
				case IMessage message when _useProtobufModels:
					request.Content = new ByteArrayContent(message.ToByteArray());
					request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
					break;
				default:
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

		private async Task<ApiResponse> DeserializeApiResponseAsync(HttpContent content)
		{
			if (content == null)
				return null;

			if (_useProtobufModels)
			{
				var result = new ApiResponse();
				
				result.MergeFrom(await content.ReadAsByteArrayAsync().ConfigureAwait(_continueOnCapturedContext));

				return result;
			}

			return JsonConvert.DeserializeObject<ApiResponse>(await content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext), _serializerSettings);
		}

		private void SaveRestCallData(HttpResponseMessage response, long elapsedMs)
		{
			if (!_logRestfulCalls)
				return;

			try
			{
				var status = response.IsSuccessStatusCode ? "Success" : "Error";

				var filename = $"{response.RequestMessage.Method} {status} - {DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.json";
				var dir = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location)?.FullName ??
				          throw new Exception("Unable to get directory for saving local log files");

				dir = Path.Combine(dir, $"Logs\\{DateTime.Now:yyyy-MM-dd}");
				
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				
				var logFile = Path.Combine(dir, filename);

				//serialize
				dynamic content = new JObject();
				content.ElapsedMs = elapsedMs;
				content.Client = (JObject)JToken.FromObject(_useProtobufModels ? _authentication.HttpProtocolBufferClient : _authentication.HttpJsonClient);
				content.Response = (JObject)JToken.FromObject(response);

				File.WriteAllText(logFile, content.ToString());

				Event(this, new ClientApiLoggerEventArgs { File = logFile, EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = "OneApiHelper", Message = "SaveRestCallData Succeeded" });
			}
			catch (Exception e)
			{
				Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = "OneApiHelper", Message = $"SaveRestCallData Failed - {e.Message}" });
			}
		}
	}
}