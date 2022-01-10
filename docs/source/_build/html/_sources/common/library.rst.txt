Library APIs
====

Provides a global standard set of Water Quality Specific Parameters, Units, Quantity Types, Agencies, Analysis Methods, and the means to map different standards. 
In addition all library items are localized into 24 languages. Additional application localization support is also provided.

Agency Methods
---------

Agency Parameter Code Type Methods
---------

Agency Parameter Code Methods
---------

Agency Unit Type Methods
---------

Agency Unit Code Methods
---------

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
  
Parameter Methods
---------

Quantity Type Methods
---------

GetQuantityTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all quantity types. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- See Also: :doc:`/concepts/library` 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetQuantityTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the QuantityTypes.
   :rtype: Task<List<QuantityType>>

Translation (i18n) Methods
---------

Unit Type Methods
---------


.. autosummary::
   :toctree: generated