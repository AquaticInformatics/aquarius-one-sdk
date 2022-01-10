Digital Twins APIs
====

Enterprise Twin are digital twins that provide multiple essential purposes including: Organizational Modelling and Security, Resource-Based Access Control, physical world modelling, Instrument relations with Telemetry, and Data Cataloguing through Telemetry Twins.

Digital Twins are classified through a Category (Organizational, Spatial, Instrument and Telemetry) and then by a Type and then by a Sub-Type.

Digital Twin Definition Language (DTDL) is leveraged to describe data schemas that unleash the power of the data stored within the Digital Twins.

 ..  class:: DigitalTwins 
    :module: ClientSDK

Methods
------------

CreateSpaceAsync
^^^^^^^^^^^^^^^^^^^^

 Creates a new space Digital Twin. 

   `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=Create>`_ 


**Required Role(s)**: Admin, Support Admin, Super Admin

.. method:: CreateSpaceAsync(string parentId, string name, string twinTypeId = Constants.SpaceCategory.LocationType.RefId, string twinSubTypeId = Constants.SpaceCategory.LocationType.OtherSubType.RefId)
   :module: ClientSDK.Core

   :param parentId: Reference ID of the Parent Digital Twin.
   :type parentId: string
   :param name: The name of the space.
   :type name: string
   :param twinTypeId: The ID of the Type of Twin.
   :type twinTypeId: string
   :param twinSubTypeId: The ID of the SubType.
   :type twinSubTypeId: string

   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>

  
.. code-block:: C#

   var operationDigitalTwin = await clientSDK.DigitalTwin.CreateSpaceAsync(
      _tenant.Id, 
      "New Operation", 
      Constants.SpaceCategory.OperationType.RefId, 
      Constants.SpaceCategory.OperationType.WasterWaterTreatmentPlantSubType.RefId);


.. autosummary::
   :toctree: generated
  
