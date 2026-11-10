using ONE.ClientSDK.Enterprise.Core;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ONE.ClientSDK.Communication
{
	public interface IOneApiHelper
	{
		event EventHandler<ClientApiLoggerEventArgs> Event;

		Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellation);

		Task<ApiResponse> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);

		Task<ApiResponse> BuildRequestAndSendAsync(HttpMethod method, string uri, CancellationToken cancellationToken, object content = null, bool useCamelCaseSerialization = false);

		HttpRequestMessage CreateRequest(HttpMethod method, string uri, object content = null, bool useCamelCaseSerialization = false);

		// Posts one JSON body to enterprise/core/v1/Insights with the token as it is: no login, no TokenExpired, no redirect, no Event.
		// Returns an outcome for every HTTP status. See OneApiHelper.PostInsightsAsync for the full contract.
		Task<InsightsPostResult> PostInsightsAsync(byte[] body, IReadOnlyDictionary<string, string> headers, TimeSpan minimumTokenLifetime, TimeSpan timeout,
			CancellationToken cancellation);
	}
}