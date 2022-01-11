Authentication
====

Provides centralized Identity Service through the implementation of Open ID Connect. 
Based on Identity Server, this service provides single sing-on / sign-out across federated gateways such as Azure Active Directory, Google, etc.

 ..  class:: Authentication 
    :module: ClientSDK


Methods
-----

GetTokenAsync
^^^^
The token endpoint can be used to programmatically request tokens.  (see `Identity Server Documentation <https://docs.identityserver.io/en/latest/endpoints/token.html>`_).

.. method:: GetTokenAsync(string userName, string password)
   :module: ONE.Utilities.PlatformEnvironment

   :param userName: The user name of the user to authenticate.
   :type userName: string
   :param password: The password of the user to authenticate.
   :type password: string

   :returns:  
   :rtype: Task<string>


GetUserInfoAync
^^^^
The UserInfo endpoint can be used to retrieve identity information about a user (see `Identity Server Documentation <https://docs.identityserver.io/en/latest/endpoints/userinfo.html>`_).

.. method:: GetUserInfoAync()
   :module: ONE.Utilities.PlatformEnvironment

   :returns:  the mapped claims (at least the openid scope is required).
   :rtype: Task<string>

LoginResourceOwnerAsync
^^^^
Logs the user in and acquires an access token.  (see `Identity Server Documentation <https://docs.identityserver.io/en/latest/endpoints/token.html>`_).

.. method:: LoginResourceOwnerAsync(string userName, string password)
   :module: ONE.Utilities.PlatformEnvironment

   :param userName: The user name of the user to authenticate.
   :type userName: string
   :param password: The password of the user to authenticate.
   :type password: string

   :returns:  success
   :rtype: Task<bool>

.. code-block:: C#

   if (await clientSDK.Authentication.LoginResourceOwnerAsync(Username, Password))
            // success
   else
            // fail


Logout
^^^^
Clears the token and the Client.

.. method:: Logout()
   :module: ONE.Utilities.PlatformEnvironment

   :rtype: void

Properties
----

IsAuthenticated
^^^^^^^^^^^^^^^^^^^^

.. attribute:: IsAuthenticated
   :module: ClientSDK.Authentication

   :returns:  Returns whether the current user is authenticated.
   :rtype: bool 


Password
^^^^^^^^^^^^^^^^^^^^

.. attribute:: Password
   :module: ClientSDK.Authentication

   :returns:  The password used in authentication.
   :rtype: string 

User
^^^^^^^^^^^^^^^^^^^^
Sets or Returns the user.

.. attribute:: User
   :module: ClientSDK.Authentication

   :returns:  The user used in authentication.
   :rtype: User 

.. code-block:: C#

   // Setting the user property

   if (clientSDK.Authentication.User == null)
         {
               var result = await clientSDK.Authentication.GetUserInfoAync();
               clientSDK.Authentication.User = await clientSDK.UserHelper.GetUserFromUserInfoAsync(result);
         }

UserName
^^^^^^^^^^^^^^^^^^^^

.. attribute:: UserName
   :module: ClientSDK.Authentication

   :returns:  The username used in authentication.
   :rtype: string 

Token
^^^^^^^^^^^^^^^^^^^^

.. attribute:: Token
   :module: ClientSDK.Authentication

   :returns:  The access token returned during authentication.
   :rtype: Token 



.. autosummary::
   :toctree: generated

