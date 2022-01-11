Digital Twins
====

Enterprise Twin are digital twins that provide multiple essential purposes including: Organizational Modelling and Security, Resource-Based Access Control, physical world modelling, Instrument relations with Telemetry, and Data Cataloguing through Telemetry Twins.

Digital Twins are classified through a Category (Organizational, Spatial, Instrument and Telemetry) and then by a Type and then by a Sub-Type.

Digital Twin Definition Language (DTDL) is leveraged to describe data schemas that unleash the power of the data stored within the Digital Twins.

 ..  class:: DigitalTwin 
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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

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
   :module: ClientSDK.DigitalTwin

   :returns: A list of all of the DigitalTwinSubtypes.
   :rtype: Task<List<DigitalTwinSubtype>>    

Digital Twin Methods
------------

CreateSpaceAsync
^^^^^^^^^^^^^^^^^^^^

 Creates a new space Digital Twin. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=Create>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: CreateSpaceAsync(string parentId, string name, string twinTypeId = Constants.SpaceCategory.LocationType.RefId, string twinSubTypeId = Constants.SpaceCategory.LocationType.OtherSubType.RefId)
   :module: ClientSDK.DigitalTwin

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

CreateAsync
^^^^^^^^^^^^^^^^^^^^

 Creates a new Digital Twin. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=Create>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: CreateAsync(DigitalTwin digitalTwin)
   :module: ClientSDK.DigitalTwin

   :param digitalTwin: Digital Twin Object.
   :type digitalTwin: Digital Twin Object
 
   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>

DeleteTreeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a Digital Twin and its descendants. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=DeleteTwinTree>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: DeleteTreeAsync(string id)
   :module: ClientSDK.DigitalTwin

   :param id: Unique Reference Id (GUID) of the digital twin.
   :type id: string

   :returns: Whether the deletion was successful.
   :rtype: Task<bool>

GetAsync
^^^^^^^^^^^^^^^^^^^^

Returns a digital twin based on the digital twin ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=Get>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the parent digital twin.

.. method:: GetAsync(string twinRefId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Unique Reference Id (GUID) of the digital twin.
   :type twinRefId: string

   :returns: Whether the deletion was successful.
   :rtype: Task<bool>

GetDescendantsByTypeAsync
^^^^^^^^^^^^^^^^^^^^

Returns a collection of digital twins based on the digital twin ID and Type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDescendantsByRefByType>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the parent digital twin.

.. method:: GetDescendantsByTypeAsync(string twinRefId, string twinTypeId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Unique Reference Id (GUID) of the digital twin.
   :type twinRefId: string

   :param twinTypeId: Unique Reference Id (GUID) of the digital twin type.
   :type twinTypeId: string

   :returns: List of Digital Twins.
   :rtype: Task<List<DigitalTwin>>

GetDescendantsAsync
^^^^^^^^^^^^^^^^^^^^

Returns a collection of digital twins based on the digital twin ID and optional Type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDescendantsByRef>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the parent digital twin.

.. method:: GetDescendantsAsync(string twinRefId, string twinTypeId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Unique Reference Id (GUID) of the digital twin.
   :type twinRefId: string

   :param twinTypeId: (Optional) Unique Reference Id (GUID) of the digital twin type.
   :type twinTypeId: string

   :returns: List of Digital Twins.
   :rtype: Task<List<DigitalTwin>>

GetDescendantsBySubTypeAsync
^^^^^^^^^^^^^^^^^^^^

Returns a collection of digital twins based on the digital twin ID and Sub Type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDescendantsByRefByType>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the parent digital twin.

.. method:: GetDescendantsBySubTypeAsync(string twinRefId, string twinSubTypeId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Unique Reference Id (GUID) of the digital twin.
   :type twinRefId: string

   :param twinSubTypeId: Unique Reference Id (GUID) of the digital twin subtype.
   :type twinSubTypeId: string

   :returns: List of Digital Twins.
   :rtype: Task<List<DigitalTwin>>

GetDescendantsBySubTypeAsync
^^^^^^^^^^^^^^^^^^^^

Returns a collection of digital twins based on the digital twin ID and category Id.  See :doc:`/concepts/digitaltwin` 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDescendantsByRefByType>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the parent digital twin.

.. method:: GetDescendantsByCategoryAsync(string twinRefId, uint categoryId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Unique Reference Id (GUID) of the digital twin.
   :type twinRefId: string

   :param categoryId: Unique category Id of the digital twin category.  See :doc:`/concepts/digitaltwin` 
   :type categoryId: uint

   :returns: List of Digital Twins.
   :rtype: Task<List<DigitalTwin>>

MoveAsync
^^^^^^^^^^^^^^^^^^^^

 Moves an existing Digital Twin to a new parent. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=MoveByRef>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: MoveAsync(string twinRefId, string parentRefId)
   :module: ClientSDK.DigitalTwin

   :param twinRefId: Reference ID of the Digital Twin.
   :type twinRefId: string
   :param parentRefId: Reference ID of the Parent Digital Twin.
   :type parentRefId: string

   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>


UpdateAsync
^^^^^^^^^^^^^^^^^^^^

 Updates an existing Digital Twin. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=UpdateDigitalTwin>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: UpdateAsync(DigitalTwin digitalTwin)
   :module: ClientSDK.DigitalTwin

   :param digitalTwin: Digital Twin Object.
   :type digitalTwin: DigitalTwin
 
   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>

UpdateTwinDataAsync
^^^^^^^^^^^^^^^^^^^^

 Updates the twin data on an existing Digital Twin. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=UpdateDigitalTwin>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: UpdateTwinDataAsync(string twinReferenceId, JsonPatchDocument twinData)
   :module: ClientSDK.DigitalTwin

   :param twinReferenceId: Unique Reference Id (GUID) of the digital twin.
   :type twinReferenceId: string
 
   :param twinData: JsonPatchDocument.
   :type twinData: JsonPatchDocument

   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>

UpdateTwinDataManyAsync
^^^^^^^^^^^^^^^^^^^^

 Updates the twin data on multiple existing Digital Twins. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=UpdateDigitalTwin>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: UpdateTwinDataManyAsync(Dictionary<string, JsonPatchDocument> twinDataMany)
   :module: ClientSDK.DigitalTwin

   :param twinDataMany: Dictionary of a collection of twins to be updated.
   :type twinDataMany: Dictionary<string, JsonPatchDocument>

   :returns: Digital Twin Object.
   :rtype: Task<DigitalTwin>

DeleteAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a Digital Twin. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=Delete>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the parent digital twin.

.. method:: DeleteAsync(long twinId)
   :module: ClientSDK.DigitalTwin

   :param twinId: The numeric unique id of the twin.
   :type twinId: long

   :returns: Whether the deletion was successful.
   :rtype: Task<bool>



.. autosummary::
   :toctree: generated
  
