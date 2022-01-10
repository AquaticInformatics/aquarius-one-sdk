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

Creates a new user and associates it to a tenant.  The user will automatically be sent an email for creating their username and password, finishing the user creation. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createuser>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

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

.. code-block:: C#

   User user = await _clientSDK.Core.CreateUserAsync(
      "Fred", 
      "Flinstone", 
      "fred@flinstone.me", 
      "38c00913-55c6-4635-8f68-c86acc27c419");

UpdateUserAsync
^^^^^^^^^^^^^^^^^^^^

Updates a User's information.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=patchuser>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UpdateUserAsync(User user)
   :module: ClientSDK.Core   

   :param user: The User Object.
   :type user: User

   :rtype: Task<bool>

GetUsersAsync
^^^^^^^^^^^^^^^^^^^^

Gets all available users including those of sub-tenants.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=usergetmany>`_ 
- **Required Role(s)**: Any

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

Gets a single user from its unique (GUID) identifier.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=usergetbyid>`_ 
- **Required Role(s)**: Any

.. method:: GetUserAsync(string userId, EnumUserExpand userExpand = EnumUserExpand.none)
   :module: ClientSDK.Core   

   :param userId: The GUID for the user.
   :type userId: string
   :param userExpand: Additional information to retrieve with the user.
   :type userExpand: EnumUserExpand

   :rtype: Task<User>
   
DeleteUserAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a single user from its unique (GUID) identifier.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=deleteuser>`_ 
- **Required Role(s)**: Any

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: DeleteUserAsync(string userId)
   :module: ClientSDK.Core   

   :param userId: The GUID for the user.
   :type userId: string

   :rtype: Task<bool>


Extended User Methods
------------

ActivateUserAsync
^^^^^^^^^^^^^^^^^^^^

Completes the user creation process by setting the userName and password.  The user token is sent to the user via email.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=useractivate>`_ 
- **Required Role(s)**: Any
- **Authentication**: Not Required

.. method:: ActivateUserAsync(string userName, string password, string userToken)
   :module: ClientSDK.Core

   :param userName: The new username for the user to be created.
   :type userName: string
   :param password: The password for the user to be created.
   :type password: string
   :param userToken: The token sent to the user via email.
   :type userToken: string

   :returns: Whether the user was successfully activated.
   :rtype: Task<bool>

ResendInvitationAsync
^^^^^^^^^^^^^^^^^^^^

Resend the invitation email to a user who did not complete the registration process in time.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=userresendinvitation>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: ResendInvitationAsync(string userId)
   :module: ClientSDK.Core

   :param userId: The GUID for the user.
   :type userId: string

   :returns: Whether the invitation email was successfully sent.
   :rtype: Task<bool>
   
SendUserNameToEmailAsync
^^^^^^^^^^^^^^^^^^^^

Sends an email to the provided email address with a list of userNames associated to users with the provided email address.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=UserSendUserNamesToEmail>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: SendUserNameToEmailAsync(string email)
   :module: ClientSDK.Core

   :param email: The email for the user.
   :type email: string

   :returns: Whether the email was successfully sent.
   :rtype: Task<bool>

UnlockUserAsync
^^^^^^^^^^^^^^^^^^^^

Used to unlock a user account that has been locked due to too many failed login attempts.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=userunlockaccount>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UnlockUserAsync(string id)
   :module: ClientSDK.Core

   :param id: The GUID for the user.
   :type id: string

   :returns: Whether the user was successfully unlocked.
   :rtype: Task<bool>

UserCreateRoleRefAsync
^^^^^^^^^^^^^^^^^^^^

Assigns a role to a user.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=usercreateroleref>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UserCreateRoleRefAsync(string userId, string roleId)
   :module: ClientSDK.Core

   :param userId: The GUID for the user.
   :type userId: string
   :param roleId: The GUID for the role.
   :type roleId: string

   :returns: Whether the role was successfully added to the user.
   :rtype: Task<bool>

UserDeleteRoleRefAsync
^^^^^^^^^^^^^^^^^^^^

Removes a role from a user.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=userdeleteroleref>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UserDeleteRoleRefAsync(string userId, string roleId)
   :module: ClientSDK.Core

   :param userId: The GUID for the user.
   :type userId: string
   :param roleId: The GUID for the role.
   :type roleId: string

   :returns: Whether the role was successfully removed from the user.
   :rtype: Task<bool>

UserRequestPasswordResetAsync
^^^^^^^^^^^^^^^^^^^^

Sends a password reset email to the email address of the user associated with the provided userName.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=userrequestpasswordreset>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UserRequestPasswordResetAsync(string userName)
   :module: ClientSDK.Core

   :param userName: The username for the user requesting an email to reset their password.
   :type userName: string

   :returns: Whether the password reset email was successfully sent.
   :rtype: Task<bool>

UserPasswordUpdateAsync
^^^^^^^^^^^^^^^^^^^^

Updates the password for the user associated with the provided userId.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=userpasswordupdate>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UserPasswordUpdateAsync(string userId, string existingPassword, string newPassword)
   :module: ClientSDK.Core

   :param userId: The GUID for the user.
   :type userId: string
   :param existingPassword: The current user password.
   :type existingPassword: string
   :param newPassword: The new user password.
   :type newPassword: string

   :returns: Whether the password was successfully updated.
   :rtype: Task<bool>


Tenant Methods
------------

CreateTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: CreateTenantAsync(string name, string culture)
   :module: ClientSDK.Core

   :param name: The name of the tenant.
   :type name: string   
   :param culture: The culture code for the tenant.  See :doc:`/concepts/library` 
   :type culture: string  

   :rtype: Task<Tenant>

GetTenantsAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all tenants the user has rights to see.

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantsAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<Tenant>>

GetTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantAsync(string tenantId)
   :module: ClientSDK.Core   

   :param tenantId: The GUID for the tenant.
   :type tenantId: string

   :rtype: Task<Tenant>

UpdateTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UpdateTenantAsync(Tenant tenant)
   :module: ClientSDK.Core   

   :param tenant: The tenant object.
   :type tenant: Tenant

   :rtype: Task<bool>

DeleteTenantAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: DeleteTenantAsync(string id)
   :module: ClientSDK.Core   

   :param id: The GUID for the tenant.
   :type id: string

   :rtype: Task<bool>

AddTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Assigns a product offering to a tenant.

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: AddTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :param tenantId: The GUID for the tenant.
   :type tenantId: string
   :param productOfferingId: The GUID for the product offering.
   :type productOfferingId: string

   :rtype: Task<bool>

RemoveTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Removes a product offering from a tenant.

**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: RemoveTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core  

   :param tenantId: The GUID for the tenant.
   :type tenantId: string
   :param productOfferingId: The GUID for the product offering.
   :type productOfferingId: string 

   :rtype: Task<bool>


Product Offering Methods
----------------------

CreateProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new product offering.

**Required Role(s)**: Super Admin



.. method:: CreateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :param productOffering: The ProductOffering object.
   :type productOffering: ProductOffering

   :rtype: Task<ProductOffering>

GetProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Any

.. method:: GetProductOfferingAsync(string productOfferingId, EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
   :module: ClientSDK.Core   

.. method:: GetProductOfferingsAsync(EnumProductOfferingExpand productOfferingExpand = EnumProductOfferingExpand.None)
   :module: ClientSDK.Core   

   :param productOfferingId: The GUID for the product offering.
   :type productOfferingId: string 
   :param productOfferingExpand: The additional information to retrieve with the product offering.
   :type productOfferingExpand: EnumProductOfferingExpand 

   :rtype: Task<List<ProductOffering>>

UpdateProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

**Required Role(s)**: Super Admin

.. method:: UpdateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :rtype: Task<ProductOffering>

DeleteProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a product offering.

**Required Role(s)**: Super Admin

.. method:: DeleteProductOfferingAsync(string id)
   :module: ClientSDK.Core   

   :param id: The GUID for the product offering.
   :type id: ProductOffering

   :rtype: Task<bool>


Feature Methods
------------

CreateFeatureAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new Feature

**Required Role(s)**: Super Admin

.. method:: CreateFeatureAsync(Feature feature)
   :module: ClientSDK.Core   

   :param feature: The Feature object.
   :type feature: Feature

   :rtype: Task<ProductOffering>


.. autosummary::
   :toctree: generated

  
