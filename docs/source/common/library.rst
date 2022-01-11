Library
====

Provides a global standard set of Water Quality Specific Parameters, Units, Quantity Types, Agencies, Analysis Methods, and the means to map different standards. 
In addition all library items are localized into 24 languages. Additional application localization support is also provided.

- See Also: :doc:`/concepts/library` 

Agency Methods
---------

CreateAgencyAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new agency. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateAgencyAsync(Agency agency)
   :module: ClientSDK.Library

   :param agency: The Agency as an object.
   :type agency: Agency

   :returns: Newly created Agency.
   :rtype: Task<Agency>

UpdateAgencyAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing Agency. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=agency-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateAgencyAsync(Agency agency)
   :module: ClientSDK.Library

   :param agency: The Agency as an object.
   :type agency: Agency

   :returns: Updated Agency.
   :rtype: Task<Agency>

GetAgencyAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves Parameter by unique Id (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=agency-get>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetAgencyAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the Agency.
   :type id: string

   :returns: An Agency.
   :rtype: Task<Agency>


GetAgenciesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all agencies. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=agency-getmany>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetAgenciesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the Agencies.
   :rtype: Task<List<Agency>>

Agency Parameter Code Type Methods
---------

CreateParameterAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new parameterAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType)
   :module: ClientSDK.Library

   :param parameterAgencyCodeType: The ParameterAgencyCodeType as an object.
   :type parameterAgencyCodeType: ParameterAgencyCodeType

   :returns: Newly created ParameterAgencyCodeType.
   :rtype: Task<ParameterAgencyCodeType>

UpdateParameterAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing ParameterAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateParameterAgencyCodeTypeAsync(ParameterAgencyCodeType parameterAgencyCodeType)
   :module: ClientSDK.Library

   :param parameterAgencyCodeType: The ParameterAgencyCodeType as an object.
   :type parameterAgencyCodeType: ParameterAgencyCodeType

   :returns: Updated ParameterAgencyCodeType.
   :rtype: Task<ParameterAgencyCodeType>

DeleteParameterAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing ParameterAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteParameterAgencyCodeTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the ParameterAgencyCodeType.
   :type id: string

   :returns: Whether the ParameterAgencyCodeType was deleted.
   :rtype: Task<bool>

GetParameterAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves ParameterAgencyCodeType by unique Id (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-get-by-id>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParameterAgencyCodeTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the ParameterAgencyCodeType.
   :type id: string

   :returns: A ParameterAgencyCodeType.
   :rtype: Task<ParameterAgencyCodeType>


GetParameterAgencyCodeTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all ParameterAgencyCodeTypes. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-get`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParameterAgencyCodeTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the ParameterAgencyCodeType.
   :rtype: Task<List<ParameterAgencyCodeType>>



Agency Parameter Code Methods
---------

CreateParameterAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new ParameterAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycode-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode)
   :module: ClientSDK.Library

   :param parameterAgencyCode: The ParameterAgencyCode as an object.
   :type parameterAgencyCode: ParameterAgencyCode

   :returns: Newly created ParameterAgencyCode.
   :rtype: Task<ParameterAgencyCode>

UpdateParameterAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing ParameterAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycode-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateParameterAgencyCodeAsync(ParameterAgencyCode parameterAgencyCode)
   :module: ClientSDK.Library

   :param parameterAgencyCode: The ParameterAgencyCode as an object.
   :type parameterAgencyCode: ParameterAgencyCode

   :returns: Updated ParameterAgencyCode.
   :rtype: Task<ParameterAgencyCode>

DeleteParameterAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing ParameterAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycode-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteParameterAgencyCodeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the ParameterAgencyCode.
   :type id: string

   :returns: Whether the ParameterAgencyCode was deleted.
   :rtype: Task<bool>

GetParameterAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves ParameterAgencyCode by unique Id (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycode-get>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParameterAgencyCodeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the ParameterAgencyCode.
   :type id: string

   :returns: A ParameterAgencyCode.
   :rtype: Task<ParameterAgencyCode>


GetParameterAgencyCodesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all ParameterAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-get>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParameterAgencyCodesAsync(string parameterAgencyCodeTypeId = null)
   :module: ClientSDK.Library

   :param parameterAgencyCodeTypeId: The parameterAgencyCodeTypeId (GUID).
   :type parameterAgencyCodeTypeId: string

   :returns: A list of all of the ParameterAgencyCode.
   :rtype: Task<List<ParameterAgencyCode>>

Agency Unit Type Methods
---------

CreateUnitAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new UnitAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycodetype-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType)
   :module: ClientSDK.Library

   :param unitAgencyCodeType: The UnitAgencyCodeType as an object.
   :type unitAgencyCodeType: UnitAgencyCodeType

   :returns: Newly created UnitAgencyCodeType.
   :rtype: Task<UnitAgencyCodeType>

UpdateUnitAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing UnitAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycodetype-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateUnitAgencyCodeTypeAsync(UnitAgencyCodeType unitAgencyCodeType)
   :module: ClientSDK.Library

   :param unitAgencyCodeType: The UnitAgencyCodeType as an object.
   :type unitAgencyCodeType: UnitAgencyCodeType

   :returns: Updated UnitAgencyCodeType.
   :rtype: Task<UnitAgencyCodeType>

DeleteUnitAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing UnitAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameteragencycodetype-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteUnitAgencyCodeTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the UnitAgencyCodeType.
   :type id: string

   :returns: Whether the UnitAgencyCodeType was deleted.
   :rtype: Task<bool>

GetUnitAgencyCodeTypeAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves ParameterAgencyCodeType by unique Id (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycodetype-get>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitAgencyCodeTypeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the ParameterAgencyCodeType.
   :type id: string

   :returns: A ParameterAgencyCodeType.
   :rtype: Task<ParameterAgencyCodeType>


GetUnitAgencyCodeTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all UnitAgencyCodeType. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycodetype-getmany>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitAgencyCodeTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the UnitAgencyCodeType.
   :rtype: Task<List<UnitAgencyCodeType>>


Agency Unit Code Methods
---------

CreateUnitAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new ParameterAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycode-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode)
   :module: ClientSDK.Library

   :param unitAgencyCode: The UnitAgencyCode as an object.
   :type unitAgencyCode: UnitAgencyCode

   :returns: Newly created UnitAgencyCode.
   :rtype: Task<UnitAgencyCode>

UpdateUnitAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing UnitAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycode-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateUnitAgencyCodeAsync(UnitAgencyCode unitAgencyCode)
   :module: ClientSDK.Library

   :param unitAgencyCode: The UnitAgencyCode as an object.
   :type unitAgencyCode: UnitAgencyCode

   :returns: Updated UnitAgencyCode.
   :rtype: Task<UnitAgencyCode>

DeleteUnitAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing UnitAgencyCode. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycode-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteUnitAgencyCodeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the UnitAgencyCode.
   :type id: string

   :returns: Whether the UnitAgencyCode was deleted.
   :rtype: Task<bool>

GetUnitAgencyCodeAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves UnitAgencyCode by unique Id (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycode-get>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitAgencyCodeAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the UnitAgencyCode.
   :type id: string

   :returns: A UnitAgencyCode.
   :rtype: Task<UnitAgencyCode>


GetUnitAgencyCodesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all UnitAgencyCodes. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unitagencycode-getmany>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitAgencyCodesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the UnitAgencyCode.
   :rtype: Task<List<UnitAgencyCode>>

  
Parameter Methods
---------

CreateParameterAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new parameter. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateParameterAsync(Parameter parameter)
   :module: ClientSDK.Library

   :param parameter: The Parameter as an object.
   :type parameter: Parameter

   :returns: Newly created Unit.
   :rtype: Task<Parameter>

UpdateParameterAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing parameter. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateParameterAsync(Parameter parameter)
   :module: ClientSDK.Library

   :param parameter: The Parameter as an object.
   :type parameter: Parameter

   :returns: Updated Parameter.
   :rtype: Task<Parameter>

DeleteParameterAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing parameter. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteParameterAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the Parameter.
   :type id: string

   :returns: Whether the Parameter was deleted.
   :rtype: Task<bool>

GetParameterAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves Parameter by unique integer Id. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-get-by-id>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParameterAsync(int id)
   :module: ClientSDK.Library

   :param unitId: The unique identifier (int) for the Parameter.
   :type unitId: int

   :returns: A Parameter.
   :rtype: Task<Parameter>


GetParametersAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all parameters. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=parameter-getmany>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetParametersAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the Parameters.
   :rtype: Task<List<Parameter>>

Quantity Type Methods
---------

GetQuantityTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all quantity types. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetQuantityTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the QuantityTypes.
   :rtype: Task<List<QuantityType>>

Translation (i18n) Methods
---------

GetMobilei18nKeysAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves translation keys needed for the mobile application. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetMobilei18nKeysAsync(string language)
   :module: ClientSDK.Library

   :param language: The language culture code.
   :type language: string

   :returns: A list of all of the requested i18nKey objects needed for Mobile.
   :rtype: Task<List<I18NKey>> 

Geti18nKeysAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves translation keys for a list of requested modules. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: Geti18nKeysAsync(string language, string modules)
   :module: ClientSDK.Library

   :param language: The language culture code.
   :type language: string
   :param modules: Comma delimited set of Modules of translations.  Example: AQI_FOUNDATION_LIBRARY, AQI_MOBILE_RIO
   :type modules: string

   :returns: A list of all of the requested i18nKey objects needed for Mobile.
   :rtype: Task<List<I18NKey>> 


Unit Type Methods
---------

CreateUnitAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new unit. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unit-create>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: CreateUnitAsync(Unit unit)
   :module: ClientSDK.Library

   :param unit: The Unit as an object.
   :type unit: Unit

   :returns: Newly created Unit.
   :rtype: Task<Unit>


UpdateUnitAsync
^^^^^^^^^^^^^^^^^^^^

Updates an existing unit. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unit-update>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: UpdateUnitAsync(Unit unit)
   :module: ClientSDK.Library

   :param unit: The Unit as an object.
   :type unit: Unit

   :returns: Updated Unit.
   :rtype: Task<Unit>

DeleteUnitAsync
^^^^^^^^^^^^^^^^^^^^

Deletes an existing unit. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=unit-delete>`_ 
- **Required Role(s)**: Super Admin
- User must be authenticated

.. method:: DeleteUnitAsync(string id)
   :module: ClientSDK.Library

   :param id: The unique identifier (GUID) for the Unit.
   :type id: string

   :returns: Whether the unit was successfully deleted.
   :rtype: Task<bool>

GetUnitAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves Unit by unique integer Id. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=5d6974441f7a0e5d13cedf62>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitAsync(int unitId)
   :module: ClientSDK.Library

   :param unitId: The unique identifier (int) for the Unit.
   :type unitId: int

   :returns: Requested Unit.
   :rtype: Task<Unit>

GetUnitsAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all units. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-common-library-v1&operation=5d69741c4697ae815db391d3>`_ 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetUnitsAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the Units.
   :rtype: Task<List<Unit>>

.. autosummary::
   :toctree: generated