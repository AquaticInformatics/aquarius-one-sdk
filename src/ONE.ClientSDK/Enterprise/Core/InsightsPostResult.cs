using System.Net;

namespace ONE.ClientSDK.Enterprise.Core
{
	public enum InsightsPostOutcome
	{
		// The Core API took the batch (2xx).
		Accepted,
		// No usable token: none, an empty one, or one that expires within the minimum lifetime. Nothing was sent.
		NotAuthenticated,
		// 401
		Unauthorized,
		// 403
		Forbidden,
		// Any other 4xx (400, 413, ...) or an unexpected status. The same body will not be accepted on a retry.
		Rejected,
		// 429
		Throttled,
		// 5xx
		ServerError,
		// 3xx. The redirect was not followed, so the token was not sent to the redirect target.
		RedirectRefused
	}

	public sealed class InsightsPostResult
	{
		public InsightsPostResult(InsightsPostOutcome outcome, HttpStatusCode? statusCode = null)
		{
			Outcome = outcome;
			StatusCode = statusCode;
		}

		public InsightsPostOutcome Outcome { get; }

		// The HTTP status of the response. Null when no request was made.
		public HttpStatusCode? StatusCode { get; }
	}
}
