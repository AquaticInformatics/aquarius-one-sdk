Core (User & Tenants)
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

GetUserFeaturesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves Features for the user associated with the provided userId.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featuregetbyuserid`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetUserFeaturesAsync(string userId)
   :module: ClientSDK.Core

   :param userId: The GUID for the user.
   :type userId: string

   :returns: A list of features available to the user.
   :rtype: Task<List<Feature>>

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

Role Methods
------------

GetRolesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all roles.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=rolegetmany>`_ 
- **Required Role(s)**: Any

.. method:: GetRolesAsync(bool expandFeature = false)
   :module: ClientSDK.Core

   :param expandFeature: Whether to expand to all features available to the role.
   :type expandFeature: bool

   :rtype: Task<List<Role>>

GetRolesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieve a role by id.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=rolegetbyid>`_ 
- **Required Role(s)**: Any

.. method:: GetRoleAsync(string id, bool expandFeature = false)
   :module: ClientSDK.Core

   :param id: Unique identifier (GUID) for the role.
   :type id: string
   :param expandFeature: Whether to expand to all features available to the role.
   :type expandFeature: bool

   :rtype: Task<Role>

Tenant Methods
------------

CreateTenantAsync
^^^^^^^^^^^^^^^^^^^^

Creates a tenant (customer).

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createtenant>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

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

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=tenantgetmany>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantsAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<Tenant>>

GetTenantAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves an existing Tenant based on the id provided.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=tenantget>`_ 
**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: GetTenantAsync(string tenantId)
   :module: ClientSDK.Core   

   :param tenantId: The GUID for the tenant.
   :type tenantId: string

   :rtype: Task<Tenant>

UpdateTenantAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing tenant based on the provided id.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=updatetenant>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: UpdateTenantAsync(Tenant tenant)
   :module: ClientSDK.Core   

   :param tenant: The tenant object.
   :type tenant: Tenant

   :rtype: Task<bool>

DeleteTenantAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing tenant based on the provided id.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=deletetenant>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: DeleteTenantAsync(string id)
   :module: ClientSDK.Core   

   :param id: The GUID for the tenant.
   :type id: string

   :rtype: Task<bool>

AddTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Creates a relationship between a Tenant and a Product Offering. 
Note that for a user to assign a Product Offering to a tenant, the user must currently have that Product Offering in their Product claims. 
If no Tenant-Product Offering relationship is present, 
it will be created. If one is present, it will be updated.
Authorization - Role and Twin Policy. The user must have a valid token, must be an admin, and must have access to the related tenant digital twin.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=TenantProductOfferingCreate>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resouce Authorization**: Must have access to the related tenant digital twin

.. method:: AddTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :param tenantId: The GUID for the tenant.
   :type tenantId: string
   :param productOfferingId: The GUID for the product offering.
   :type productOfferingId: string

   :rtype: Task<bool>

RemoveTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a relationship between a tenant and a product offering.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=TenantProductOfferingDelete>`_ 
- - **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resouce Authorization**: Must have access to the related tenant digital twin

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

Creates a ProductOffering that customers can purchase. 
ProductOfferings contain a set of features that can be used to determine what ui components to display or to control access to certain endpoints.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createproductoffering>`_ 
- **Required Role(s)**: Super Admin

.. method:: CreateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :param productOffering: The ProductOffering object.
   :type productOffering: ProductOffering

   :rtype: Task<ProductOffering>

GetProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves an existing ProductOffering based on the id provided.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=productofferinggetbyid>`_ 
- **Required Role(s)**: Any

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

Updates an existing ProductOffering that customers can purchase. 
ProductOfferings contain a set of features that can be used to determine what ui components to display or to control access to certain endpoints.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=patchproductoffering>`_ 
- **Required Role(s)**: Super Admin

.. method:: UpdateProductOfferingAsync(ProductOffering productOffering)
   :module: ClientSDK.Core   

   :param productOfferingId: The GUID for the product offering.
   :type productOfferingId: string 

   :rtype: Task<ProductOffering>

DeleteProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a product offering.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=productofferingdelete>`_ 
- **Required Role(s)**: Super Admin

.. method:: DeleteProductOfferingAsync(string id)
   :module: ClientSDK.Core   

   :param id: The GUID for the product offering.
   :type id: ProductOffering

   :rtype: Task<bool>


Feature Methods
------------

Features are used to determine what ui components to display or to control which users have access to certain endpoints so this can cause components or endpoints to be inaccessable.


CreateFeatureAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new Feature

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createfeature>`_ 
- **Required Role(s)**: Super Admin

.. method:: CreateFeatureAsync(Feature feature)
   :module: ClientSDK.Core   

   :param feature: The Feature object.
   :type feature: Feature

   :rtype: Task<Feature>

UpdateFeatureAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing Feature. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=patchfeature>`_ 
- **Required Role(s)**: Super Admin

.. method:: UpdateFeatureAsync(Feature feature)
   :module: ClientSDK.Core   

   :param feature: The Feature object.
   :type feature: Feature

   :rtype: Task<Feature>

GetFeaturesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all available Features. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featuregetmany>`_ 
- **Required Role(s)**: Super Admin

.. method:: GetFeaturesAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<Feature>>

GetFeatureAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves an existing Feature based on the id provided.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featuregetbyid>`_ 
- **Required Role(s)**: Super Admin

.. method:: GetFeatureAsync(string id)
   :module: ClientSDK.Core   

   :param id: The unique identifier of the feature to delete
   :type id: string

   :rtype: Task<Feature>

DeleteFeatureAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing Feature. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featuredelete>`_ 
- **Required Role(s)**: Super Admin

.. method:: DeleteFeatureAsync(string id)
   :module: ClientSDK.Core   

   :param id: The unique identifier of the feature to delete
   :type id: string

   :rtype: Task<bool>

CreateFeatureReferenceAsync
^^^^^^^^^^^^^^^^^^^^

Creates a reference to a Feature from another entity. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featurecreateref>`_ 
- **Required Role(s)**: Super Admin

.. method:: CreateFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId)
   :module: ClientSDK.Core   

   :param featureId: The unique identifier of the feature
   :type featureId: string
   :param enumNavigationProperty: The unique identifier of the feature
   :type enumNavigationProperty: EnumNavigationProperty
   :param referenceId: The unique identifier of the item to be referenced
   :type referenceId: string

   :rtype: Task<bool>

DeleteFeatureReferenceAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a reference to a Feature from another entity. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=featuredeleteref>`_ 
- **Required Role(s)**: Super Admin

.. method:: DeleteFeatureReferenceAsync(string featureId, EnumNavigationProperty enumNavigationProperty, string referenceId)
   :module: ClientSDK.Core   

   :param featureId: The unique identifier of the feature
   :type featureId: string
   :param enumNavigationProperty: The unique identifier of the feature
   :type enumNavigationProperty: EnumNavigationProperty
   :param referenceId: The unique identifier of the item to be referenced
   :type referenceId: string

   :rtype: Task<bool>



.. autosummary::
   :toctree: generated

  
