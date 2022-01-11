LibraryCache APIs
====

The purpose of the Library Cache is to load the global library, which is fairly static into memory, 
so that commonly reused items such as quantity types, units and parameters along with their translations can be quickly available.
In addition there are helper methods to retrieve these items efficiently.

LibraryCache Class
----

 ..  class:: LibraryCache 
    :module: ClientSDK.CacheHelper

Methods
----

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the library cache.  After the library cache is loaded, the other methods will perform.

- **Required Role(s)**: Any
  
.. method:: LoadAsync()
   :module: ClientSDK.Spreadsheet

   :rtype: Task<bool>

GetParameter - By GUID
^^^^^^^^^^^^^^^^^^^^

Retrieves a parameter by GUID from Cache.

- **Required Role(s)**: Any
  
.. method:: GetParameter(string parameterId)
   :module: ClientSDK.Spreadsheet

   :param parameterId: Unique Identifier (GUID).
   :type parameterId: string

   :rtype: Parameter

GetParameter - By long Id
^^^^^^^^^^^^^^^^^^^^

Retrieves a parameter by the long id from Cache.

- **Required Role(s)**: Any
  
.. method:: GetParameter(long parameterId)
   :module: ClientSDK.Spreadsheet

   :param parameterId: Unique Identifier (long).
   :type parameterId: long

   :rtype: Parameter

GetParameterByName
^^^^^^^^^^^^^^^^^^^^

Retrieves a parameter by translated Name from Cache.

- **Required Role(s)**: Any
  
.. method:: GetParameterByName(string name)
   :module: ClientSDK.Spreadsheet

   :param name: Translated name of the parameter.
   :type name: string

   :rtype: Parameter


GetParameterAgencyCodeType
^^^^^^^^^^^^^^^^^^^^

Retrieves a Parameter Agency Code Type by GUID from Cache.

- **Required Role(s)**: Any
  
.. method:: GetParameterAgencyCodeType(string id)
   :module: ClientSDK.Spreadsheet

   :param id: Unique Identifier (GUID).
   :type id: string

   :rtype: ParameterAgencyCodeType

GetQuantityType
^^^^^^^^^^^^^^^^^^^^

Retrieves a quantity type by GUID from Cache.

- **Required Role(s)**: Any
  
.. method:: GetQuantityType(string quantityTypeId)
   :module: ClientSDK.Spreadsheet

   :param quantityTypeId: Unique Identifier (GUID).
   :type quantityTypeId: string

   :rtype: QuantityType

GetQuantityTypeByName
^^^^^^^^^^^^^^^^^^^^

Retrieves a quantity type by Name from Cache.

- **Required Role(s)**: Any
  
.. method:: GetQuantityTypeByName(string name)
   :module: ClientSDK.Spreadsheet

   :param name: Display name of the quantity type.
   :type name: string

   :rtype: QuantityType

GetParameterAgencyCode
^^^^^^^^^^^^^^^^^^^^

Retrieves a Parameter Agency Code by GUID from Cache.

- **Required Role(s)**: Any
  
.. method:: GetParameterAgencyCode(string id)
   :module: ClientSDK.Spreadsheet

   :param id: Unique Identifier (GUID).
   :type id: string

   :rtype: ParameterAgencyCode

GetUnit - By GUID
^^^^^^^^^^^^^^^^^^^^

Retrieves a unit by GUID from Cache.

- **Required Role(s)**: Any
  
.. method:: GetUnit(string unitId)
   :module: ClientSDK.Spreadsheet

   :param unitId: Unique Identifier (GUID).
   :type unitId: string

   :rtype: Unit

GetUnit - By long Id
^^^^^^^^^^^^^^^^^^^^

Retrieves a unit by the long id from Cache.

- **Required Role(s)**: Any
  
.. method:: GetUnit(long unitId)
   :module: ClientSDK.Spreadsheet

   :param unitId: Unique Identifier (long).
   :type unitId: long

   :rtype: Unit

GetUnitByI18nKey
^^^^^^^^^^^^^^^^^^^^

Retrieves a unit by I18nKey from Cache.

- **Required Role(s)**: Any
  
.. method:: GetUnitByI18nKey(string i18nKey)
   :module: ClientSDK.Spreadsheet

   :param i18nKey: The i18n Key for the Unit.
   :type i18nKey: string

   :rtype: Unit

GetUnitByName
^^^^^^^^^^^^^^^^^^^^

Retrieves a unit by translated Name from Cache.

- **Required Role(s)**: Any
  
.. method:: GetUnitByName(string name)
   :module: ClientSDK.Spreadsheet

   :param name: Translated name of the unit.
   :type name: string

   :rtype: Unit

ToString
^^^^^^^^^^^^^^^^^^^^

Serializes Cache for local storage.

- **Required Role(s)**: Any
  
.. method:: ToString()
   :module: ClientSDK.Spreadsheet

   :rtype: string


Properties
----

I18Nkeys
^^^^^

.. attribute:: I18Nkeys
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<I18NKey>

Parameters
^^^^^

.. attribute:: Parameters
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<Parameter>

ParameterAgencyCodes
^^^^^

.. attribute:: ParameterAgencyCodes
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<ParameterAgencyCode>

ParameterAgencyCodeTypes
^^^^^

.. attribute:: ParameterAgencyCodeTypes
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<ParameterAgencyCodeType>

QuantityTypes
^^^^^

.. attribute:: QuantityTypes
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<Unit>

Units
^^^^^

.. attribute:: Units
   :module: ClientSDK.CacheHelper.LibraryCache

   :rtype: List<QuantityType>
