using System.Net;

namespace ONE.ClientSDK.Enterprise.Core
{
	// Classifies the HTTP status of the post. It is not a retry decision: use InsightsPostResult.StatusCode for that.
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
		// Any other status: a 4xx not listed here (400, 404, 408, 413, ...) or an unexpected one. This only names the status class.
		// It does not say the body is invalid or that a retry will fail (a 408 may succeed on retry), so check StatusCode before dropping a batch.
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

		// The HTTP status of the response. Null when no request was made. Use it, not Outcome alone, to decide whether to retry a batch.
		public HttpStatusCode? StatusCode { get; }
	}
}
