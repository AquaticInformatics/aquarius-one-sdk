PlatformEnvironment
====

PlatformEnvironment is a static class

 ..  class:: PlatformEnvironment 
    :module: ONE.Utilities

Methods
-------

GetPlatformEnvironment
^^^^^^^^^^^^^^^^^^^^

.. method:: GetPlatformEnvironment(string name)
   :module: ONE.Utilities.PlatformEnvironment
.. method:: GetPlatformEnvironment(EnumPlatformEnvironment enumPlatformEnvironment)
   :module: ONE.Utilities.PlatformEnvironment

   :param name: The name of the environment.
   :type name: string
   :param enumPlatformEnvironment: The enumerated value of the environment.
   :type enumPlatformEnvironment: EnumPlatformEnvironment

   :returns: PlatformEnvironment class
   :rtype: PlatformEnvironment

   Creates a new user.  The user will automatically be sent an email for creating their username and password, finishing the user creation. 

   `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-core-v1&operation=createuser>`_ 

.. code-block:: C#

   clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.Feature);

Properties
-------

Environments
^^^^^^^^^^^^^^^^^^^^

.. attribute:: Environments
   :module: ONE.Utilities.PlatformEnvironment

   :returns: Static List of environments
   :rtype: List<PlatformEnvironment> 



.. autosummary::
   :toctree: generated
  
