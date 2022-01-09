Getting Started
====

Pre-requisites
You must have a development account on the Aquatic Informatics "Integration" Environment.

Have reviewed concepts of:

   - :doc:`/concepts/environments` 
   - Operations, Locations and Column Telemetry using Digital Twins
   - Spreadsheets, Worksheets, Sheets, Cells, Cell Data
   - Parameters, Units and Quantity Types

Development Tools: `Visual Studio 2019 or above <https://visualstudio.microsoft.com/>`_ 

Typical SDK Call Order
------------------------

Instantiate SDK
^^^^^^^^^^^^^^^^^^^^^^

.. code-block:: C#

   using ONE;
   ClientSDK clientSDK = new ClientSDK();

Set Environment
^^^^^^^^^^^^^^^^^^^^^^

This is your development environment or the production environment. 

See Also: :doc:`/concepts/environments`, :doc:`/helpers/environment` 

.. code-block:: C#

   using ONE.Utilities;
   clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);

Login
^^^^^^^^^^^^^^^^^^^^^^

.. code-block:: C#

   if (await clientSDK.Authentication.LoginResourceOwnerAsync(Username, Password))
            // success
   else
            // fail

sThis will also populate some handy information on the clientSDK.Authentication Object:

Get User Information
^^^^^^^^^^^^^^^^^^^^^^

This is useful for other calls in the future. This call will return the user ID of the user that you logged in with as well as your tenant ID. Often these IDs are very handy in other API calls.

.. code-block:: C#

   var result = await clientSDK.Authentication.GetUserInfo();

This will also populate some handy information on the clientSDK.Authentication Object:

Do fun stuff
^^^^^^^^^^^^^^^^^^^^^^

.. autosummary::
   :toctree: generated
  
