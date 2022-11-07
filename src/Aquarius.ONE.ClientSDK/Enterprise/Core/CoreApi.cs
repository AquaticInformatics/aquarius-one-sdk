using ONE.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ONE.Models.CSharp;

namespace ONE.Enterprise.Core
{
    public class CoreApi
    {
        public CoreApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper, bool throwAPIErrors = false)
        {
            _environment = environment;
            _continueOnCapturedContext = continueOnCapturedContext;
            _restHelper = restHelper;
            _throwAPIErrors = throwAPIErrors;
        }
        private PlatformEnvironment _environment;
        private bool _continueOnCapturedContext;
        private RestHelper _restHelper;
        private readonly bool _throwAPIErrors;
        
        public event EventHandler<ClientApiLoggerEventArgs> Event = delegate { };
        public enum EnumUserExpand
        {
            none = 0,
            role = 1,
            tenant = 2,
            feature = 3,
            userprofile = 4,
            role_tenant = 5,
            role_feature = 6,
            role_userprofile = 7,
            tenant_feature = 8,
            tenant_userprofile = 9,
            tenant_feature_userprofile = 10,
            role_tenant_feature_userProfile = 11
        }
        public enum EnumNavigationProperty
        {
            permissions,
            roles,
            users,
            productofferings
        }
        // **************************** User Methods                *********************************
        public async Task<User> CreateUserAsync(string firstName, string lastName, string email, string tenantId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/user";
            dynamic userJson = new JObject();
            if (string.IsNullOrEmpty(firstName))
                firstName = "New";
            if (string.IsNullOrEmpty(lastName))
                lastName = "User";
            userJson.firstName = firstName;
            userJson.lastName = lastName;
            userJson.email = email;
            userJson.tenantId = tenantId;
            userJson.isActive = true;
            userJson.isHeadlessUser = false;
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, userJson.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Users.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"CreateUserAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"CreateUserAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"CreateUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<User>> GetUsersAsync(EnumUserExpand userExpand = EnumUserExpand.none)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<User> users = new List<User>();
            string expand = "";
            if (userExpand != EnumUserExpand.none)
            {
                expand = Enum.GetName(typeof(EnumUserExpand), userExpand).Replace("_", ",");
            }
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/User?expand={expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Users.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        users.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetUsersAsync Success" });
                    return users;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetUsersAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetUsersAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/user/{userId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteUserAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteUserAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        public async Task<User> GetUserAsync(string userId, EnumUserExpand userExpand = EnumUserExpand.none)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            User user = null;
            string expand = "";
            if (userExpand != EnumUserExpand.none)
            {
                expand = Enum.GetName(typeof(EnumUserExpand), userExpand).Replace("_", ",");
            }
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/User/{userId}?expand={expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Users.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        user = result;
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetUserAsync Success" });
                    return user;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetUserAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        // **************************** Tenant Methods                *********************************

        public async Task<Tenant> CreateTenantAsync(string name, string culture)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/Tenant";
            dynamic tenantJson = new JObject();
            if (string.IsNullOrEmpty(name))
                name = "New Tenant";
            if (string.IsNullOrEmpty(culture))
                culture = "en";
            tenantJson.name = name;
            tenantJson.culture = culture;
            //tenantJson.enumTimeZone = (int)timeZone;
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, tenantJson.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Tenants.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"CreateTenantAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"CreateTenantAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"CreateTenantAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> AddTenantProductOfferingAsync(string tenantId, string productOfferingId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/productoffering/createref?key={productOfferingId}&navigationProperty=tenants&referenceId={tenantId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"AddTenantProductOfferingAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"AddTenantProductOfferingAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"AddTenantProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> RemoveTenantProductOfferingAsync(string tenantId, string productOfferingId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/productoffering/deleteref?key={productOfferingId}&navigationProperty=tenants&referenceId={tenantId}";

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"AddTenantProductOfferingAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"AddTenantProductOfferingAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"AddTenantProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<List<Tenant>> GetTenantsAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<Tenant> tenants = new List<Tenant>();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/Tenant").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Tenants.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        tenants.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetTenantsAsync Success" });
                    return tenants;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetTenantsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetTenantsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Tenant> GetTenantAsync(string tenantId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/Tenant/{tenantId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Tenants.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetTenantAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetTenantAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetTenantAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> UpdateTenantAsync(Tenant tenant)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/Tenant?requestId={requestId}";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(tenant, jsonSettings);

            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UpdateTenantAsync Success" });
                    return true;
                }
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UpdateTenantAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UpdateTenantAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> DeleteTenantAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/Tenant/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteTenantAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteTenantAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteTenantAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public enum EnumProductOfferingExpand
        {
            None = 0,
            Feature = 1,
            Tenant = 2,
            TenantAndFeature = 3
        }
        // **************************** Product Offering Methods       *********************************
        public async Task<ProductOffering> CreateProductOfferingAsync(ProductOffering productOffering)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/ProductOffering";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(productOffering, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ProductOfferings.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateProductOfferingAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateProductOfferingAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"CreateProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ProductOffering> GetProductOfferingAsync(string productOfferingId, EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            string expand = "";
            switch (productOfferingExpand)
            {
                case EnumProductOfferingExpand.Feature:
                    expand = "?expand=feature";
                    break;
                case EnumProductOfferingExpand.Tenant:
                    expand = "?expand=tenant";
                    break;
                case EnumProductOfferingExpand.TenantAndFeature:
                    expand = "?expand=tenant,feature";
                    break;
            }
            ProductOffering productOffering = null;
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/ProductOffering/{productOfferingId}{expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.ProductOfferings.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        productOffering = result;
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingAsync Success" });
                    return productOffering;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<ProductOffering>> GetProductOfferingsAsync(EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            string expand = "";
            switch (productOfferingExpand)
            {
                case EnumProductOfferingExpand.Feature:
                    expand = "?expand=feature";
                    break;
                case EnumProductOfferingExpand.Tenant:
                    expand = "?expand=tenant";
                    break;
                case EnumProductOfferingExpand.TenantAndFeature:
                    expand = "?expand=tenant,feature";
                    break;
            }
            List<ProductOffering> productOfferings = new List<ProductOffering>(); ;
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/ProductOffering{expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.ProductOfferings.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        productOfferings.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingsAsync Success" });
                    return productOfferings;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetProductOfferingsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<ProductOffering> UpdateProductOfferingAsync(ProductOffering productOffering)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/ProductOffering";

            productOffering.UpdateMask = new Google.Protobuf.WellKnownTypes.FieldMask { Paths = { "description", "name", "i18NKeyName", "SprintNumber", "JsonSchema", "UIDefinition", "PropertyBag"} };

            try
            {
                string updatedData = JsonConvert.SerializeObject(productOffering);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.ProductOfferings.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        return result;
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"UpdateProductOfferingAsync Success" });
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"UpdateProductOfferingAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UpdateProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteProductOfferingAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/ProductOffering/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteProductOfferingAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteProductOfferingAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteProductOfferingAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        // **************************** User Feature Methods       *********************************
        public async Task<Feature> CreateFeatureAsync(Feature feature)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/Feature";

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(feature, jsonSettings);

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, json.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Features.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateFeatureAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateFeatureAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"CreateFeatureAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        public async Task<bool> CreateFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/feature/createref?requestId={requestId}&key={featureId}&navigationProperty={enumNavigationProperty.ToString()}&referenceId={referenceId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateFeatureReferenceAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"CreateFeatureReferenceAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"CreateFeatureReferenceAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> DeleteFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/feature/deleteref?requestId={requestId}&key={featureId}&navigationProperty={enumNavigationProperty.ToString()}&referenceId={referenceId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteFeatureReferenceAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteFeatureReferenceAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteFeatureReferenceAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<Feature> GetFeatureAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/Feature/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Features.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetFeatureAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetFeatureAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetFeatureAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<List<Feature>> GetFeaturesAsync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<Feature> features = new List<Feature>(); ;
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/Feature").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Features.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        features.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetFeaturesAsync Success" });
                    return features;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetFeaturesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetFeaturesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Feature> UpdateFeatureAsync(Feature feature)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/Feature/{feature.Id}";
            feature.UpdateMask = new Google.Protobuf.WellKnownTypes.FieldMask { Paths = { "description" } };

            try
            {
                string updatedData = JsonConvert.SerializeObject(feature);
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, updatedData, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Features.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        return result;
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"UpdateFeatureAsync Success" });
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"UpdateFeatureAsync Failed" });
                return null;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UpdateFeatureAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteFeatureAsync(string id)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/Feature/{id}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteFeatureAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreAPI", Message = $"DeleteFeatureAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteFeatureAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<List<Feature>> GetUserFeaturesAsync(string userId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<ProductOffering> productOfferings = new List<ProductOffering>(); ;
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/Feature/byUserId/{userId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Features.Items.Select(x => x).ToList();
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingsAsync Success" });
                    return results;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetProductOfferingsAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetProductOfferingsAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }

        // **************************** Role Methods       *********************************

        public async Task<List<Role>> GetRolesAsync(bool expandFeature = false)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<Role> roles = new List<Role>();
            string expand = "";
            if (expandFeature)
                expand = "?expand=feature";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/role{expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Roles.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        roles.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetRolesAsync Success" });
                    return roles;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetRolesAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetRolesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<Role> GetRoleAsync(string id, bool expandFeature = false)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            List<Role> roles = new List<Role>();
            string expand = "";
            if (expandFeature)
                expand = "?expand=feature";
            try
            {
                var respContent = await _restHelper.GetRestProtocolBufferAsync(requestId, $"enterprise/core/v1/role/{id}{expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    var results = respContent.ApiResponse.Content.Roles.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetRoleAsync Success" });
                        return result;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetRoleAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetRoleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }
        public async Task<bool> DeleteRolesAsync(string roleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, $"enterprise/core/v1/role/{roleId}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteRolesAsync Success" });
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"DeleteRolesAsync Failed" });
                return respContent.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"DeleteRolesAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        // **************************** User Maintenance Methods       *********************************

        public async Task<bool> SendUserNameToEmailAsync(string email)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/core/v1/user/sendusernamestoemail?email={email}");
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (apiResponse == null)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"SendUserNameToEmailAsync Success" });
                        return true;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"SendUserNameToEmailAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"SendUserNameToEmailAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> ResendInvitationAsync(string userId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/core/v1/user/resendInvitation?userId={userId}");
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"ResendInvitationAsync Success" });
                    return true;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"ResendInvitationAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"ResendInvitationAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> UserRequestPasswordResetAsync(string userName)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/user/{userName}/requestpasswordreset";
            dynamic passwordJson = new JObject();
            passwordJson.userName = userName;
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, passwordJson.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (apiResponse == null)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserRequestPasswordResetAsync Success" });
                        return true;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserRequestPasswordResetAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UserRequestPasswordResetAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> UserPasswordUpdateAsync(string userId, string existingPassword, string newPassword)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/user/{userId}/changePassword";
            dynamic passwordJson = new JObject();
            passwordJson.password = existingPassword;
            passwordJson.newPassword = newPassword;
            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, passwordJson.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (apiResponse == null)
                    {
                        Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserPasswordUpdateAsync Success" });
                        return true;
                    }
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserPasswordUpdateAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UserPasswordUpdateAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

        public async Task<bool> UnlockUserAsync(string id)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/User/{id}/UnlockAccount";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UnlockUserAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UnlockUserAsync Failed" });
                    return false;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UnlockUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> ActivateUserAsync(string userName, string password, string userToken)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/User/ActivateUser";

            dynamic userJson = new JObject();
            userJson.userName = userName;
            userJson.password = password;
            userJson.userToken = userToken;

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, userJson.ToString(), endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"ActivateUserAsync Success" });
                    return true;
                }
                else
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"ActivateUserAsync Failed" });
                return false;

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"ActivateUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> UserCreateRoleRefAsync(string userId, string roleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/user/createroleref?key={userId}&referenceId={roleId}";

            try
            {
                var respContent = await _restHelper.PostRestJSONAsync(requestId, "", endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserCreateRoleRefAsync Success" });
                    return true;
                }
                else
                {

                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserCreateRoleRefAsync Failed" });
                    return false;
                }
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UserCreateRoleRefAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<bool> UserDeleteRoleRefAsync(string userId, string roleId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/user/deleteroleref?key={userId}&referenceId={roleId}";

            try
            {
                var respContent = await _restHelper.DeleteRestJSONAsync(requestId, endpoint).ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserRemoveRoleAsync Success" });
                    return true;
                }
                else
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UserRemoveRoleAsync Failed" });
                    return false;
                }

            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UserRemoveRoleAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }
        public async Task<List<User>> GetGlobalUsersAsync(string expand)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var requestId = Guid.NewGuid();
            List<User> users = new List<User>();
            try
            {
                var respContent = await _restHelper.GetRestJSONAsync(requestId, $"enterprise/core/v1/GlobalUser?expand={expand}").ConfigureAwait(_continueOnCapturedContext);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(respContent.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    var results = apiResponse.Content.Users.Items.Select(x => x).ToList();
                    foreach (var result in results)
                    {
                        users.Add(result);
                    }
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetGlobalUsersAsync Success" });
                    return users;
                }
                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"GetGlobalUsersAsync Failed" });
                return null;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"GetGlobalUsersAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return null;
            }
        }


        public async Task<bool> UpdateUserAsync(User user)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var requestId = Guid.NewGuid();
            var endpoint = $"enterprise/core/v1/User/{user.Id}";
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var json = JsonConvert.SerializeObject(user, jsonSettings);
            try
            {
                var respContent = await _restHelper.PatchRestJSONAsync(requestId, json, endpoint);
                if (respContent.ResponseMessage.IsSuccessStatusCode)
                {
                    Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Trace, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UpdateUserAsync Success" });
                    return true;
                }

                Event(null, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Warn, HttpStatusCode = respContent.ResponseMessage.StatusCode, ElapsedMs = watch.ElapsedMilliseconds, Module = "CoreApi", Message = $"UpdateUserAsync Failed" });
                return false;
            }
            catch (Exception e)
            {
                Event(e, new ClientApiLoggerEventArgs { EventLevel = EnumEventLevel.Error, Module = "CoreAPI", Message = $"UpdateUserAsync Failed - {e.Message}" });
                if (_throwAPIErrors) 
					 throw; 
				 return false;
            }
        }

       
    }
}
