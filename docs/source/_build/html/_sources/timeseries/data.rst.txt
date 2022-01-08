TimeSeries Data
====
Enterprise Core manages:

*  Users
*  Tenants
*  Products
*  Features

User APIS
------------

CreateUserAsync
^^^^^^^^^^^^^^^^^^^^

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

.. method:: GetUsersAsync(EnumUserExpand userExpand = EnumUserExpand.none)
   :module: ClientSDK.Core     
   
   :param userExpand: Additional information to retrieve with the user.
   :type userExpand: EnumUserExpand

   
   :rtype: Task<List<User>>

:note

   none = 0,
   role = 1,
            tenant = 2,
            feature = 3,
            userprovile = 4,
            role_tenant = 5,
            role_feature = 6,
            role_userprofile = 7,
            tenant_feature = 8,
            tenant_userprofile = 9,
            tenant_feature_userprofile = 10,
            role_tenant_feature_userProvile = 11

GetUserAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: GetUserAsync(string userId, EnumUserExpand userExpand = EnumUserExpand.none)
   :module: ClientSDK.Core   

   :rtype: Task<User>
   
DeleteUserAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: DeleteUserAsync(string userId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

SafeDeleteUserAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: SafeDeleteUserAsync(string userId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

Tenant APIS
------------

CreateTenantAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: CreateTenantAsync(string name, string culture)
   :module: ClientSDK.Core   

   :rtype: Task<Tenant>

GetTenantsAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: GetTenantsAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<Tenant>>

GetTenantAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: GetTenantAsync(string tenantId)
   :module: ClientSDK.Core   

   :rtype: Task<Tenant>

UpdateTenantAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: UpdateTenantAsync(Tenant tenant)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

AddTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: AddTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>

RemoveTenantProductOfferingAsync
^^^^^^^^^^^^^^^^^^^^

.. method:: RemoveTenantProductOfferingAsync(string tenantId, string productOfferingId)
   :module: ClientSDK.Core   

   :rtype: Task<bool>


Product APIS
------------

Feature APIS
------------

.. autosummary::
   :toctree: generated

  
