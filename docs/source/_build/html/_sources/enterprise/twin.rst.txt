Digital Twins APIs
====

Enterprise Twin are digital twins that provide multiple essential purposes including: Organizational Modelling and Security, Resource-Based Access Control, physical world modelling, Instrument relations with Telemetry, and Data Cataloguing through Telemetry Twins.

Digital Twins are classified through a Category (Organizational, Spatial, Instrument and Telemetry) and then by a Type and then by a Sub-Type.

Digital Twin Definition Language (DTDL) is leveraged to describe data schemas that unleash the power of the data stored within the Digital Twins.

 ..  class:: DigitalTwins 
    :module: ClientSDK

Digital Twin Type Methods
---------

CreateDigitalTwinTypeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new digital twin type. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=CreateDigitalTwinType>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: CreateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
   :module: ClientSDK.Library

   :param digitalTwinType: The DigitalTwin Type Object
   :type digitalTwinType: DigitalTwinType

   :returns: The created DigitalTwinType.
   :rtype: Task<DigitalTwinType>

UpdateDigitalTwinTypeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing digital twin type. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=UpdateDigitalTwinType>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: UpdateDigitalTwinTypeAsync(DigitalTwinType digitalTwinType)
   :module: ClientSDK.Library

   :param digitalTwinType: The DigitalTwin Type Object
   :type digitalTwinType: DigitalTwinType

   :returns: The updated DigitalTwinType.
   :rtype: Task<DigitalTwinType>

DeleteDigitalTwinTypeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a digital twin type. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=DeleteDigitalTwinType>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: DeleteDigitalTwinTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The GUID of the DigitalTwinType Object
   :type id: string

   :returns: Whether the deletion was successful.
   :rtype: Task<bool>

GetDigitalTwinTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all digital twin types. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinTypes>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetDigitalTwinTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the DigitalTwinTypes.
   :rtype: Task<List<DigitalTwinType>>


Digital Twin Sub-Type Methods
---------

CreateDigitalTwinSubTypeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new digital twin sub-type. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=CreateDigitalTwinSubtype>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: CreateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
   :module: ClientSDK.Library

   :param digitalTwinSubType: The DigitalTwin Sub-Type Object
   :type digitalTwinSubType: DigitalTwinSubtype

   :returns: The created DigitalTwinSubtype.
   :rtype: Task<DigitalTwinSubtype>

UpdateDigitalTwinSubTypeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing digital twin sub-type. 

- `RESTful API Documentation <hhttps://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=updatetwinsubtype>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: UpdateDigitalTwinSubTypeAsync(DigitalTwinSubtype digitalTwinSubType)
   :module: ClientSDK.Library

   :param digitalTwinSubType: The DigitalTwinSubtype Type Object
   :type digitalTwinSubType: DigitalTwinSubtype

   :returns: The updated DigitalTwinSubtype.
   :rtype: Task<DigitalTwinSubtype>

DeleteDigitalTwinSubTypeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a digital twin sub-type. 

- `RESTful API Documentation <hhttps://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=deletetwinsubtype>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Super Admin

.. method:: DeleteDigitalTwinSubTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The GUID of the DigitalTwinSubtype Object
   :type id: string

   :returns: Whether the deletion was successful.
   :rtype: Task<bool>

GetDigitalTwinSubTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all digital twin types. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- See Also: :doc:`/concepts/digitaltwin` 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetDigitalTwinSubTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the DigitalTwinSubtypes.
   :rtype: Task<List<DigitalTwinSubtype>>    

Digital Twin Methods
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
  
