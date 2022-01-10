Core APIs
====

Provides centralized multi-tenancy and multi-product registration and management. 
Provides full APIs for managing Tenants, Users, Products and Features providing GDPR compliance.

 ..  class:: Core 
    :module: ClientSDK

Enterprise Core manages:

*  Users
*  Tenants
*  Products
*  Features

User Methods
------------

CreateUserAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: CreateUserAsync(string firstName, string lastName, string email, string tenantId)
   :module: ClientSDK.Core

   :param firstName: The first name of the user.
   :type firstName: string
   :param lastName: The last name of the user.
   :type lastName: string
   :param email: The email address of the user.  The invitation email will be sent to this address automatically.
   :type email: string
   :param tenantId: The GUID of the tenant that will own the user.
   :type tenantId: string

   :returns: User Object that matches the User Protocol Buffer.
   :rtype: Task<User>

   Creates a new user.  The user will automatically be sent an email for creating their username and password, finishing the user creation. 

   `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createuser>`_ 

.. code-block:: C#

   User user = await _clientSDK.Core.CreateUserAsync(
      "Fred", 
      "Flinstone", 
      "fred@flinstone.me", 
      "38c00913-55c6-4635-8f68-c86acc27c419");

GetUsersAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Any

.. method:: GetUsersAsync(EnumUserExpand userExpand = EnumUserExpand.none)
   :module: ClientSDK.Core     
   
   :param userExpand: Additional information to retrieve with the user.
   :type userExpand: EnumUserExpand

   
   :rtype: Task<List<User>>

.. hint:: 
   **EnumUserExpand**

      1.  **none**: No Expansion
      2.  **role**: Expands with user's roles
      3.  **tenant**: Expands with user's tenant
      4.  **feature**: Expands with user's features unioned from all production offerings the user's tenant owns
      5.  **userprofile**: Expands with the user's profile 
      6.  **role_tenant**: Expands with both the user's role and tenant
      7.  **role_feature**: Expands with both the user's role and features
      8.  **role_userprofile**: Expands with both the user's role and profile
      9.  **tenant_feature**: Expands with both the user's role and profile
      10. **tenant_userprofile**: Expands with both the user's role and profile
      11. **tenant_feature_userprofile**: Expands with the user's tenant, feature and profile
      12. **role_tenant_feature_userProvile**: Expands all

GetUserAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Any

.. method:: GetUserAsync(string userId, EnumUserExpand userExpand = EnumUserExpand.none)
   :module: ClientSDK.Core   

   :rtype: Task<User>
   
DeleteUserAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: DeleteUserAsync(string userId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

SafeDeleteUserAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: SafeDeleteUserAsync(string userId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

Tenant Methods
------------

CreateTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: CreateTenantAsync(string name, string culture)
   :module: ClientSDK.Core   

   :rtype: Task<Tenant>

GetTenantsAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantsAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<Tenant>>

GetTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantAsync(string tenantId)
   :module: ClientSDK.Core   

   :rtype: Task<Tenant>

UpdateTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UpdateTenantAsync(Tenant tenant)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

DeleteTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: DeleteTenantAsync(string id)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

AddTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: AddTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

RemoveTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: RemoveTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>


Product Offering Methods
----------------------

CreateProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Super Admin

.. method:: GetProductOfferingAsync(string productOfferingId, EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
   :module: ClientSDK.Core   

.. method:: CreateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :rtype: Task<ProductOffering>

GetProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Any

.. method:: GetProductOfferingsAsync(EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
   :module: ClientSDK.Core   

   :rtype: Task<List<ProductOffering>>

UpdateProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Super Admin

.. method:: UpdateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :rtype: Task<ProductOffering>

DeleteProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Super Admin

.. method:: DeleteProductOfferingAsync(string id)
   :module: ClientSDK.Core   

   :rtype: Task<bool>


Feature Methods
------------


.. autosummary::
   :toctree: generated

hello  
