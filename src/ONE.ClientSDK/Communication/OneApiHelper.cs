using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enterprise.Core;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ONE.ClientSDK.Communication
{
	public class OneApiHelper : IOneApiHelper
	{
        private const string ModuleName = "OneApiHelper";
        private readonly AuthenticationApi _authentication;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _useProtobufModels;
		private bool _logRestfulCalls;

		// Only PostInsightsAsync uses these.
		private readonly HttpMessageHandler _insightsHandler;
		private readonly object _insightsClientLock = new object();
		private HttpClient _insightsClient;
        
		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public OneApiHelper(AuthenticationApi authentication, bool continueOnCapturedContext, bool useProtobufModels, bool logRestfulCalls)
			: this(authentication, continueOnCapturedContext, useProtobufModels, logRestfulCalls, null)
		{
		}

		// Tests pass a handler to record the insights requests.
		internal OneApiHelper(AuthenticationApi authentication, bool continueOnCapturedContext, bool useProtobufModels, bool logRestfulCalls,
			HttpMessageHandler insightsHandler)
		{
			_authentication = authentication;
			_authentication.TokenExpired += RenewToken;
			_continueOnCapturedContext = continueOnCapturedContext;
			_useProtobufModels = useProtobufModels;
			_logRestfulCalls = logRestfulCalls;
			_insightsHandler = insightsHandler;
        }

		private void RenewToken(object sender, EventArgs _)
		{
			if (sender is AuthenticationApi authentication)
				authentication.LoginAsync().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().GetResult();
		}

		public async Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellation)
		{
			var watch = Stopwatch.StartNew();

			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Accept", "*/*");

			var response = await _authentication.GetBaseClient(uri).SendAsync(request, cancellation);

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

		public async Task<ApiResponse> BuildRequestAndSendAsync(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null, bool useCamelCaseSerialization = false)
		{
			using (var request = CreateRequest(method, uri, content, useCamelCaseSerialization))
			{
				return await SendAsync(request, cancellationToken).ConfigureAwait(_continueOnCapturedContext);
			}
		}

		public HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null, bool useCamelCaseSerialization = false)
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
                    var messageString = content as string ?? JsonConvert.SerializeObject(content,
                        useCamelCaseSerialization
                            ? JsonExtensions.CamelCaseSerializerSettings
                            : JsonExtensions.IgnoreNullSerializerSettings);
					if (!string.IsNullOrEmpty(messageString))
					{
						request.Content = new StringContent(messageString);
						request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
					}
					break;
			}

			return request;
		}

		// **************************** Insights                    *********************************
		private const string InsightsPath = "enterprise/core/v1/Insights";

		// Set by PostInsightsAsync, never by the caller, so exactly one Authorization header is sent and the body type is fixed.
		private static readonly HashSet<string> InsightsReservedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Authorization", "Content-Type", "Content-Length", "Content-Encoding", "Accept", "Host"
		};

		// Posts one JSON body to enterprise/core/v1/Insights with the current token. The body is opaque to the SDK.
		// Returns an outcome for every HTTP status. Throws only for transport failures (including a timeout, as TimeoutException),
		// a cancelled call, a null body, a non-positive timeout, and when the environment is not set or is not https.
		// It reads the token as it is: it never logs in, never raises TokenExpired and never uses the shared base client, so it
		// sends no request when the token is missing or inside minimumTokenLifetime, and never follows a redirect.
		// Headers named Authorization, Content-Type, Content-Length, Content-Encoding, Accept or Host are ignored, as are invalid ones.
		// Unlike SendAsync it writes no REST call log and raises no Event, so telemetry cannot generate telemetry or touch the disk.
		public async Task<InsightsPostResult> PostInsightsAsync(byte[] body, IReadOnlyDictionary<string, string> headers, TimeSpan minimumTokenLifetime,
			TimeSpan timeout, CancellationToken cancellation)
		{
			if (body == null)
				throw new ArgumentNullException(nameof(body));
			if (timeout <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(nameof(timeout), "The timeout must be positive.");

			// Read the token once, so the checks and the header see the same value.
			var token = _authentication.Token;
			var accessToken = token?.access_token;
			if (string.IsNullOrEmpty(accessToken) || token.expires < DateTime.Now + (minimumTokenLifetime > TimeSpan.Zero ? minimumTokenLifetime : TimeSpan.Zero))
				return new InsightsPostResult(InsightsPostOutcome.NotAuthenticated);

			var baseAddress = _authentication.GetBaseAddress(InsightsPath);
			if (baseAddress == null)
				throw new InvalidOperationException("The environment is not set, so there is no host to post insights to.");

			var uri = new Uri(baseAddress, $"{InsightsPath}?requestId={Guid.NewGuid()}");
			if (uri.Scheme != Uri.UriSchemeHttps)
				throw new InvalidOperationException("Insights are only posted over https.");

			using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				AddInsightsHeaders(request, headers);
				request.Content = new ByteArrayContent(body);
				request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				using (var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
				{
					timeoutSource.CancelAfter(timeout);

					try
					{
						// The body of the response is never read.
						using (var response = await GetInsightsClient()
							       .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeoutSource.Token)
							       .ConfigureAwait(_continueOnCapturedContext))
							return new InsightsPostResult(GetInsightsOutcome(response.StatusCode), response.StatusCode);
					}
					catch (OperationCanceledException) when (!cancellation.IsCancellationRequested)
					{
						throw new TimeoutException($"The insights post did not finish within {timeout}.");
					}
				}
			}
		}

		private static void AddInsightsHeaders(HttpRequestMessage request, IReadOnlyDictionary<string, string> headers)
		{
			if (headers == null)
				return;

			foreach (var header in headers)
			{
				if (string.IsNullOrWhiteSpace(header.Key) || InsightsReservedHeaders.Contains(header.Key))
					continue;

				try
				{
					request.Headers.Add(header.Key, header.Value);
				}
				catch (Exception e) when (e is FormatException || e is InvalidOperationException)
				{
					// Not a valid request header: skip it. The post does not depend on it.
				}
			}
		}

		private static InsightsPostOutcome GetInsightsOutcome(HttpStatusCode statusCode)
		{
			var code = (int)statusCode;

			if (code >= 200 && code < 300)
				return InsightsPostOutcome.Accepted;
			if (code >= 300 && code < 400)
				return InsightsPostOutcome.RedirectRefused;
			if (code == 401)
				return InsightsPostOutcome.Unauthorized;
			if (code == 403)
				return InsightsPostOutcome.Forbidden;
			if (code == 429)
				return InsightsPostOutcome.Throttled;
			if (code >= 500)
				return InsightsPostOutcome.ServerError;

			return InsightsPostOutcome.Rejected;
		}

		// Its own client, with no base address and no default headers, so it never carries a token the shared clients hold
		// (finding F-3: Logout() does not clear the shared base client's header). Its timer is the linked source in PostInsightsAsync.
		private HttpClient GetInsightsClient()
		{
			lock (_insightsClientLock)
			{
				return _insightsClient ?? (_insightsClient = new HttpClient(_insightsHandler ?? CreateInsightsHandler()) { Timeout = Timeout.InfiniteTimeSpan });
			}
		}

		// A redirect is not followed, because the token must not go to a host the Core API did not name.
		internal static HttpMessageHandler CreateInsightsHandler() => new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false };

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

			return JsonConvert.DeserializeObject<ApiResponse>(await content.ReadAsStringAsync().ConfigureAwait(_continueOnCapturedContext), JsonExtensions.IgnoreNullSerializerSettings);
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

                if (string.IsNullOrEmpty(dir))
                {
                    Event(this, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = "Unable to determine directory for local log file - temporarily disabling local file logging" });
                    _logRestfulCalls = false;
					return;
                }

                if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				var logFile = Path.Combine(dir, filename);

				//serialize
				dynamic content = new JObject();
				content.ElapsedMs = elapsedMs;
				content.Client = (JObject)JToken.FromObject(_useProtobufModels ? _authentication.HttpProtocolBufferClient : _authentication.HttpJsonClient);
				content.Response = (JObject)JToken.FromObject(response);

				if (string.IsNullOrEmpty(logFile))
				{
					Event(this, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = "Unable to determine filename for local log file - temporarily disabling local file logging" });
					_logRestfulCalls = false;
					return;
				}

				File.WriteAllText(logFile, content.ToString());

                Event(this, new ClientApiLoggerEventArgs { File = logFile, EventLevel = EnumOneLogLevel.OneLogLevelTrace, Module = ModuleName, Message = "SaveRestCallData Succeeded" });
            }
			catch (Exception e)
			{
				Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumOneLogLevel.OneLogLevelError, Module = ModuleName, Message = $"Failed to save local log file - temporarily disabling local file logging - {e.Message}" });
				_logRestfulCalls = false;
            }
		}
    }
}