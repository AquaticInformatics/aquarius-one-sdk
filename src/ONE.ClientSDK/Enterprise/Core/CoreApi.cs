using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enterprise.Authentication;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Enterprise.Core
{
	public class CoreApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		// Only PostInsightsAsync uses these. A CoreApi built with the public constructor has no AuthenticationApi.
		private readonly AuthenticationApi _authentication;
		private readonly HttpMessageHandler _insightsHandler;
		private readonly object _insightsClientLock = new object();
		private HttpClient _insightsClient;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public CoreApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
		}

		// OneApi.Core uses this one. Tests pass a handler to record the insights requests.
		internal CoreApi(IOneApiHelper apiHelper, AuthenticationApi authentication, bool continueOnCapturedContext, bool throwApiErrors,
			HttpMessageHandler insightsHandler = null) : this(apiHelper, continueOnCapturedContext, throwApiErrors)
		{
			_authentication = authentication;
			_insightsHandler = insightsHandler;
		}

		// **************************** User Methods                *********************************
		public async Task<User> CreateUserAsync(string firstName, string lastName, string email, string tenantId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user?requestId={Guid.NewGuid()}";

			if (string.IsNullOrEmpty(firstName))
				firstName = "New";
			if (string.IsNullOrEmpty(lastName))
				lastName = "User";

			var user = new User
			{
				FirstName = firstName,
				LastName = lastName,
				Email = email,
				TenantId = tenantId,
				IsActive = true,
				IsHeadlessUser = false
			};

			var apiResponse = await ExecuteRequest("", HttpMethod.Post, endpoint, cancellation, user)
				.ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Users?.Items.FirstOrDefault();
		}

		public async Task<List<User>> GetUsersAsync(EnumUserExpand userExpand = EnumUserExpand.none, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/User?requestId={Guid.NewGuid()}";

			if (userExpand != EnumUserExpand.none && !string.IsNullOrEmpty(Enum.GetName(typeof(EnumUserExpand), userExpand)))
				endpoint += $"&expand={Enum.GetName(typeof(EnumUserExpand), userExpand)?.Replace("_", ",")}";

			var apiResponse = await ExecuteRequest("GetUsersAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Users?.Items.ToList();
		}

		public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/{userId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteUserAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<User> GetUserAsync(string userId, EnumUserExpand userExpand = EnumUserExpand.none, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/User/{userId}?requestId={Guid.NewGuid()}";

			if (userExpand != EnumUserExpand.none && !string.IsNullOrEmpty(Enum.GetName(typeof(EnumUserExpand), userExpand)))
				endpoint += $"&expand={Enum.GetName(typeof(EnumUserExpand), userExpand)?.Replace("_", ",")}";

			var apiResponse = await ExecuteRequest("GetUserAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Users?.Items.FirstOrDefault();
		}

		// **************************** Tenant Methods                *********************************

		public async Task<Tenant> CreateTenantAsync(string name, string culture, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Tenant?requestId={Guid.NewGuid()}";

			if (string.IsNullOrEmpty(name))
				name = "New Tenant";
			if (string.IsNullOrEmpty(culture))
				culture = "en";

			var tenant = new Tenant
			{
				Name = name,
				Culture = culture
			};

			var apiResponse = await ExecuteRequest("CreateTenantAsync", HttpMethod.Post, endpoint, cancellation, tenant).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Tenants?.Items.FirstOrDefault();
		}

		public async Task<bool> AddTenantProductOfferingAsync(string tenantId, string productOfferingId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/productoffering/createref?key={productOfferingId}&navigationProperty=tenants&referenceId={tenantId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("AddTenantProductOfferingAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> RemoveTenantProductOfferingAsync(string tenantId, string productOfferingId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/productoffering/deleteref?key={productOfferingId}&navigationProperty=tenants&referenceId={tenantId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("RemoveTenantProductOfferingAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Tenant?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetTenantsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Tenants?.Items.ToList();
		}

		public async Task<Tenant> GetTenantAsync(string tenantId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Tenant/{tenantId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetTenantAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Tenants?.Items.FirstOrDefault();
		}

		public async Task<bool> UpdateTenantAsync(Tenant tenant, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Tenant?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateTenantAsync", new HttpMethod("PATCH"), endpoint, cancellation, tenant).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> DeleteTenantAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Tenant/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteTenantAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}
		
		// **************************** Product Offering Methods       *********************************
		public async Task<ProductOffering> CreateProductOfferingAsync(ProductOffering productOffering, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/ProductOffering?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateProductOfferingAsync", HttpMethod.Post, endpoint, cancellation, productOffering).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ProductOfferings?.Items.FirstOrDefault();
		}

		public async Task<ProductOffering> GetProductOfferingAsync(string productOfferingId, EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/ProductOffering/{productOfferingId}?requestId={Guid.NewGuid()}";

			switch (productOfferingExpand)
			{
				case EnumProductOfferingExpand.Feature:
					endpoint += "&expand=feature";
					break;
				case EnumProductOfferingExpand.Tenant:
					endpoint += "&expand=tenant";
					break;
				case EnumProductOfferingExpand.TenantAndFeature:
					endpoint += "&expand=tenant,feature";
					break;
			}

			var apiResponse = await ExecuteRequest("GetProductOfferingAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ProductOfferings?.Items.FirstOrDefault();
		}

		public async Task<List<ProductOffering>> GetProductOfferingsAsync(EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None, string tenantId = "", CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/ProductOffering?requestId={Guid.NewGuid()}";

			switch (productOfferingExpand)
			{
				case EnumProductOfferingExpand.Feature:
					endpoint += "&expand=feature";
					break;
				case EnumProductOfferingExpand.Tenant:
					endpoint += "&expand=tenant";
					break;
				case EnumProductOfferingExpand.TenantAndFeature:
					endpoint += "&expand=tenant,feature";
					break;
			}

			if (!string.IsNullOrEmpty(tenantId))
				endpoint += $"&tenantId={tenantId}";

			var apiResponse = await ExecuteRequest("GetProductOfferingsAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ProductOfferings?.Items.ToList();
		}

		public async Task<ProductOffering> UpdateProductOfferingAsync(ProductOffering productOffering, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/ProductOffering?requestId={Guid.NewGuid()}";

			productOffering.UpdateMask = new Google.Protobuf.WellKnownTypes.FieldMask { Paths = { "description", "name", "i18NKeyName", "SprintNumber", "JsonSchema", "UIDefinition", "PropertyBag"} };

			var apiResponse = await ExecuteRequest("UpdateProductOfferingAsync", new HttpMethod("PATCH"), endpoint, cancellation, productOffering).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.ProductOfferings?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteProductOfferingAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/ProductOffering/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteProductOfferingAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		// **************************** User Feature Methods       *********************************
		public async Task<Feature> CreateFeatureAsync(Feature feature, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("CreateFeatureAsync", HttpMethod.Post, endpoint, cancellation, feature).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Features?.Items.FirstOrDefault();
		}

		public async Task<bool> CreateFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/feature/createref?requestId={Guid.NewGuid()}&key={featureId}&navigationProperty={enumNavigationProperty.ToString()}&referenceId={referenceId}";

			var apiResponse = await ExecuteRequest("CreateFeatureReferenceAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> DeleteFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/feature/deleteref?requestId={Guid.NewGuid()}&key={featureId}&navigationProperty={enumNavigationProperty.ToString()}&referenceId={referenceId}";

			var apiResponse = await ExecuteRequest("DeleteFeatureReferenceAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<Feature> GetFeatureAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetFeatureAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Features?.Items.FirstOrDefault();
		}

		public async Task<List<Feature>> GetFeaturesAsync(CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetFeaturesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Features?.Items.ToList();
		}

		public async Task<Feature> UpdateFeatureAsync(Feature feature, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature/{feature.Id}?requestId={Guid.NewGuid()}";

			feature.UpdateMask = new Google.Protobuf.WellKnownTypes.FieldMask { Paths = { "description" } };

			var apiResponse = await ExecuteRequest("UpdateFeatureAsync", new HttpMethod("PATCH"), endpoint, cancellation, feature).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Features?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteFeatureAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature/{id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteFeatureAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<List<Feature>> GetUserFeaturesAsync(string userId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/Feature/byUserId/{userId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("GetUserFeaturesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Features?.Items.ToList();
		}

		// **************************** Role Methods       *********************************
		public async Task<List<Role>> GetRolesAsync(bool expandFeature = false, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/role?requestId={Guid.NewGuid()}";
			
			if (expandFeature)
				endpoint += "&expand=feature";

			var apiResponse = await ExecuteRequest("GetRolesAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Roles?.Items.ToList();
		}

		public async Task<Role> GetRoleAsync(string id, bool expandFeature = false, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/role/{id}?requestId={Guid.NewGuid()}";

			if (expandFeature)
				endpoint += "&expand=feature";

			var apiResponse = await ExecuteRequest("GetRoleAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Roles?.Items.FirstOrDefault();
		}

		public async Task<bool> DeleteRolesAsync(string roleId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/role/{roleId}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("DeleteRolesAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		// **************************** User Maintenance Methods       *********************************
		public async Task<bool> SendUserNameToEmailAsync(string email, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/sendusernamestoemail?email={email}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("SendUserNameToEmailAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> ResendInvitationAsync(string userId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/resendInvitation?userId={userId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("ResendInvitationAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UserRequestPasswordResetAsync(string userName, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/{userName}/requestpasswordreset?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UserRequestPasswordResetAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UserPasswordUpdateAsync(string userId, string existingPassword, string newPassword, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/{userId}/changePassword?requestId={Guid.NewGuid()}";

			var passwordManagement = new UserPasswordManagement
			{
				OldPassword = existingPassword,
				NewPassword = newPassword
			};

			var apiResponse = await ExecuteRequest("UserPasswordUpdateAsync", HttpMethod.Post, endpoint, cancellation, passwordManagement).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UnlockUserAsync(string id, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/User/{id}/UnlockAccount?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UnlockUserAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> ActivateUserAsync(string userName, string password, string userToken, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/User/ActivateUser?requestId={Guid.NewGuid()}";

			dynamic userJson = new JObject();
			userJson.userName = userName;
			userJson.password = password;
			userJson.userToken = userToken;

			var apiResponse = await ExecuteRequest("ActivateUserAsync", HttpMethod.Post, endpoint, cancellation, userJson).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UserCreateRoleRefAsync(string userId, string roleId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/createroleref?key={userId}&referenceId={roleId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UserCreateRoleRefAsync", HttpMethod.Post, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<bool> UserDeleteRoleRefAsync(string userId, string roleId, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/user/deleteroleref?key={userId}&referenceId={roleId}&requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UserDeleteRoleRefAsync", HttpMethod.Delete, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		public async Task<List<User>> GetGlobalUsersAsync(string expand, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/GlobalUser?requestId={Guid.NewGuid()}";

			if (!string.IsNullOrEmpty(expand))
				endpoint += $"&expand={expand}";

			var apiResponse = await ExecuteRequest("GetGlobalUsersAsync", HttpMethod.Get, endpoint, cancellation).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse?.Content?.Users?.Items.ToList();
		}
		
		public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellation = default)
		{
			var endpoint = $"enterprise/core/v1/User/{user.Id}?requestId={Guid.NewGuid()}";

			var apiResponse = await ExecuteRequest("UpdateUserAsync", new HttpMethod("PATCH"), endpoint, cancellation, user).ConfigureAwait(_continueOnCapturedContext);

			return apiResponse != null && apiResponse.StatusCode.IsSuccessStatusCode();
		}

		// **************************** Insights Methods            *********************************
		private const string InsightsPath = "enterprise/core/v1/Insights";

		// Set by this method, never by the caller, so exactly one Authorization header is sent and the body type is fixed.
		private static readonly HashSet<string> InsightsReservedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Authorization", "Content-Type", "Content-Length", "Content-Encoding", "Accept", "Host"
		};

		// Posts one JSON body to enterprise/core/v1/Insights with the current token. The body is opaque to the SDK.
		// Returns an outcome for every HTTP status. Throws only for transport failures (including a timeout, as TimeoutException),
		// a cancelled call, a null body, a non-positive timeout, and when the CoreApi was built without an AuthenticationApi.
		// It reads the token as it is: it never logs in, never raises TokenExpired and never uses the shared base client, so it
		// sends no request when the token is missing or inside minimumTokenLifetime, and never follows a redirect.
		// Headers named Authorization, Content-Type, Content-Length, Content-Encoding, Accept or Host are ignored, as are invalid ones.
		public async Task<InsightsPostResult> PostInsightsAsync(byte[] body, IReadOnlyDictionary<string, string> headers, TimeSpan minimumTokenLifetime,
			TimeSpan timeout, CancellationToken cancellation = default)
		{
			if (_authentication == null)
				throw new InvalidOperationException("This CoreApi was built without an AuthenticationApi, so it cannot post insights.");
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

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

				watch.Stop();

				var message = " Success";
				var eventLevel = EnumOneLogLevel.OneLogLevelTrace;

				if (!apiResponse.StatusCode.IsSuccessStatusCode())
				{
					message = " Failed";
					eventLevel = EnumOneLogLevel.OneLogLevelWarn;

					if (_throwApiErrors)
						throw new RestApiException(new ServiceResponse { ApiResponse = apiResponse, ElapsedMs = watch.ElapsedMilliseconds });
				}

				Event(null,
					new ClientApiLoggerEventArgs
					{
						EventLevel = eventLevel,
						HttpStatusCode = (HttpStatusCode)apiResponse.StatusCode,
						ElapsedMs = watch.ElapsedMilliseconds,
						Module = "CoreApi",
						Message = callingMethod + message
					});

				return apiResponse;
			}
			catch (Exception e)
			{
				Event(e,
					new ClientApiLoggerEventArgs
					{
						EventLevel = EnumOneLogLevel.OneLogLevelError,
						Module = "CoreApi",
						Message = $"{callingMethod} Failed - {e.Message}"
					});
				if (_throwApiErrors)
					throw;
				return null;
			}
		}
	}
}
