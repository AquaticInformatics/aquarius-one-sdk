using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ONE.ClientSDK.Communication;
using ONE.ClientSDK.Enums;
using ONE.ClientSDK.Utilities;
using ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Enterprise.Core
{
	public class CoreApi
	{
		private readonly IOneApiHelper _apiHelper;
		private readonly bool _continueOnCapturedContext;
		private readonly bool _throwApiErrors;

		public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };

		public CoreApi(IOneApiHelper apiHelper, bool continueOnCapturedContext, bool throwApiErrors)
		{
			_apiHelper = apiHelper;
			_continueOnCapturedContext = continueOnCapturedContext;
			_throwApiErrors = throwApiErrors;
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

		private async Task<ApiResponse> ExecuteRequest(string callingMethod, HttpMethod httpMethod, string endpoint, CancellationToken cancellation, object content = null)
		{
			try
			{
				var watch = Stopwatch.StartNew();

				var apiResponse = await _apiHelper.BuildRequestAndSendAsync<ApiResponse>(httpMethod, endpoint, cancellation, content).ConfigureAwait(_continueOnCapturedContext);

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
