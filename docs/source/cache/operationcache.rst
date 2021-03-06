Operation
====

The purpose of the Operations Cache is to load the configuration of an ACO operation, which is fairly static into memory, 
so that commonly reused items such as column definitions, views and related location twins are quickly accessible.

OperationsCache Class
----

 ..  class:: OperationCache 
    :module: ClientSDK.CacheHelper

InitializeAsync
^^^^^^^^^^^^^^^^^^^^

Loads the operation cache.

- **Required Role(s)**: Any
  
.. method:: InitializeAsync()
   :module: ClientSDK.CacheHelper.OperationsCache

   :rtype: Task<bool>

Column Info Methods
-----

GetTelemetryPath
^^^^^^^^^^^^^^^^^^^^

Loads the operation cache.

- **Required Role(s)**: Any
  
.. method:: GetTelemetryPath(string digitalTwinReferenceId, bool includeItem)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param digitalTwinReferenceId: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type digitalTwinReferenceId: string
   :param includeItem: Whether to include the column name in the path.
   :type includeItem: bool

   :rtype: string

GetWimsVarType
^^^^^^^^^^^^^^^^^^^^

Retrieves the WIMS Var Type for a column.  This is the single letter representation.

- **Required Role(s)**: Any
  
.. method:: GetWimsVarType(Column column)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param column: The Column object.
   :type column: Column

   :rtype: string

GetWimsType
^^^^^^^^^^^^^^^^^^^^

Retrieves the WIMS Var Type for a column.  This is the text representation.

- **Required Role(s)**: Any
  
.. method:: GetWimsType(Column column)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param column: The Column object.
   :type column: Column

   :rtype: string

.. note:: WIMS equivalent variable types:
   - Daily Worksheet
      - P - Daily variable / parameter
      - C - Daily calculated variable
      - T - Daily text variable
   - Four Hour Worksheet
      - 4 - Daily Detail variable tracked every 4 hours
      - G - 4 hour calc.
      - E - 4 hour text variable
   - Hourly Worksheet
      - H - Daily Detail variable tracked every hour
      - N - Hourly Calc
      - B - Hourly Text
   - Hourly Worksheet
      - F - Daily Detail variable tracked every 15 minutes
      - V - 15 Minute Calc
      - X - 15 Minute Text

GetWorksheetType
^^^^^^^^^^^^^^^^^^^^

Returns the enumerated Worksheet Type

- **Required Role(s)**: Any
  
.. method:: GetWorksheetType(DigitalTwin digitalTwin)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param digitalTwin: The column digital twin.
   :type digitalTwin: DigitalTwin

   :rtype: EnumWorksheet

GetWorksheetTypeName
^^^^^^^^^^^^^^^^^^^^

Returns the name of the type of the worksheet.
 - 15 Minutes
 - Hourly
 - 4 Hour
 - Daily

- **Required Role(s)**: Any
  
.. method:: GetWorksheetTypeName(string guid)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type guid: string

   :rtype: string

GetColumnGuidByIndex
^^^^^^^^^^^^^^^^^^^^

Returns a column GUID based on the index of the column digital twin.

- **Required Role(s)**: Any
  
.. method:: GetColumnGuidByIndex(string index)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param index: The index of the column digital twin.
   :type index: string

   :rtype: string

Info
^^^^^^^^^^^^^^^^^^^^

Returns a column info based on any of the following:

- **Column Number**: The unique uint id of the column.  This is also the long id of the column twin.
- **Column GUID**: The unique id (GUID) of the column.  This is also the TwinReferenceId of the twin related to the column.
- **WIMS VarNum**: The Variable Number from WIMS if the operation was converted from WIMS. 
- **WIMS Variable Id**: The Variable Id from WIMS if the operation was converted from WIMS. 


- **Required Role(s)**: Any
  
.. method:: Info(string columnIdentifier, string field)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param columnIdentifier: unique identifier of the column.
   :type columnIdentifier: string
   :param field: The information to retrieve.  See Note below.
   :type field: string

   :rtype: string

.. note:: Info Fields (Field - Info Returned):
   
   - Operation - Operation hame
   - Location:VarName - path:ColumnName
   - Name:Units - ColumnName {Unit Short Name}
   - ShortName - Parameter Short Name
   - ShortName.Units - Parameter Short Name {Unit Short Name}
   - GetWimsVarType
   - Type
   - ParameterType - Parameter Long Name
   - ParameterType.Units - Parameter Long Name {Unit Short Name}
   - Units - Unit Short Name
   - XREF
   - ScadaTag
   - LIMS_LOC
   - LIMS_TEST
   - Statistic
   - StoretCode
   - StoretCodeDesc
   - StoretCode-Desc - Storet Code - Storet Code Description
   - EntryMin
   - EntryMax
   - Path
   - Location
   - Parent
   - GrandParent
   - Frequency
   - VarNum


Get Column Methods
-----

GetColumnByColumnNumber
^^^^^^^^^^^^^^^^^^^^

Returns a column based on the Column Number

- **Required Role(s)**: Any
  
.. method:: GetColumnByColumnNumber(uint id)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param uint: The unique uint id of the column.  This is also the long id of the column twin.
   :type uint: string

   :rtype: Column



GetColumnByIdentifier
^^^^^^^^^^^^^^^^^^^^

Returns a column based on any of the following:

- **Column Number**: The unique uint id of the column.  This is also the long id of the column twin.
- **Column GUID**: The unique id (GUID) of the column.  This is also the TwinReferenceId of the twin related to the column.
- **WIMS VarNum**: The Variable Number from WIMS if the operation was converted from WIMS. 
- **WIMS Variable Id**: The Variable Id from WIMS if the operation was converted from WIMS. 

- **Required Role(s)**: Any
  
.. method:: GetColumnByIdentifier(string sId)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param sId: unique identifier of the column.
   :type sId: string

   :rtype: Column

GetColumnByVarNum
^^^^^^^^^^^^^^^^^^^^

Returns a column based on the WIMS VarNum. 

- **Required Role(s)**: Any
  
.. method:: GetColumnByVarNum(long varNum)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param varNum: The VarNum from WIMS if the operation was converted from WIMS. 
   :type varNum: long

   :rtype: Column

GetColumnByVariableId
^^^^^^^^^^^^^^^^^^^^

Returns a column based on the WIMS Variable ID. 

- **Required Role(s)**: Any
  
.. method:: GetColumnByVariableId(long variableId)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param variableId: The Variable Id from WIMS if the operation was converted from WIMS. 
   :type variableId: long

   :rtype: Column

GetColumnTwinByGuid
^^^^^^^^^^^^^^^^^^^^

Returns a column twin based on the Column GUID.  This is also the TwinReferenceId of the twin related to the column.

- **Required Role(s)**: Any
  
.. method:: GetColumnTwinByGuid(string guid)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type guid: string

   :rtype: DigitalTwin

Column Twin Data Methods
-----

GetColumnTwinDataPropertyLong
^^^^^^^^^^^^^^^^^^^^

Retrieves the value from the Column Twin Data.

- **Required Role(s)**: Any
  
.. method:: GetColumnTwinDataPropertyLong(string guid, string path, string key)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type guid: string
   :param path: JSON Path to the key to be retrieved.
   :type path: string
   :param key: JSON Key that is found under the path.
   :type key: string

   :rtype: long

GetColumnTwinDataPropertyDouble
^^^^^^^^^^^^^^^^^^^^

Retrieves the value from the Column Twin Data.

- **Required Role(s)**: Any
  
.. method:: GetColumnTwinDataPropertyDouble(string guid, string path, string key)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type guid: string
   :param path: JSON Path to the key to be retrieved.
   :type path: string
   :param key: JSON Key that is found under the path.
   :type key: string

   :rtype: double

GetColumnTwinDataPropertyDate
^^^^^^^^^^^^^^^^^^^^

Retrieves the value from the Column Twin Data.

- **Required Role(s)**: Any
  
.. method:: GetColumnTwinDataPropertyDate(string guid, string path, string key)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique Column identifier (GUID).  This is also the TwinReferenceId of the twin related to the column.
   :type guid: string
   :param path: JSON Path to the key to be retrieved.
   :type path: string
   :param key: JSON Key that is found under the path.
   :type key: string

   :rtype: dateTime

GetVariableId by Column Twin
^^^^^^^^^^^^^^^^^^^^

Retrieves the WIMS VariableId from the Column Twin Data.

- **Required Role(s)**: Any
  
.. method:: GetVariableId(DigitalTwin columnTwin)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param columnTwin: The column digital twin.
   :type columnTwin: DigitalTwin

   :rtype: long

GetVariableId by Column GUID
^^^^^^^^^^^^^^^^^^^^

Retrieves the WIMS VariableId from the Column Twin Data.

- **Required Role(s)**: Any
  
.. method:: GetVariableId(string guid)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: The unique id (GUID) of the column digital twin.
   :type guid: string

   :rtype: long


Operation Methods
-----

GetOperationById
^^^^^^^^^^^^^^^^^^^^

Finds the Operation GUID given the index of the operation.

- **Required Role(s)**: Any
  
.. method:: GetOperationById(string guid)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: unique identifier of the operation.
   :type guid: string

   :rtype: OperationCache


Row Cache Methods
-----

AddRow
^^^^^^^^^^^^^^^^^^^^

Adds a row to the operation cache.

- **Required Role(s)**: Any
  
.. method:: AddRow(EnumWorksheet enumWorksheet, Row row)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param enumWorksheet: The Worksheet Type.
   :type enumWorksheet: EnumWorksheet
   :param row: The Worksheet row to be cached.
   :type row: Row

   :rtype: void

GetRow
^^^^^^^^^^^^^^^^^^^^

Gets a row from the operation cache.

- **Required Role(s)**: Any
  
.. method:: GetRow(EnumWorksheet enumWorksheet, uint rowNumber)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param enumWorksheet: The Worksheet Type.
   :type enumWorksheet: EnumWorksheet
   :param rowNumber: The Worksheet row number.
   :type rowNumber: uint

   :rtype: Row




Properties
-----

Id
^^^^^

.. attribute:: Id

   :returns: The unique identifier (GUID) for the current operation.
   :rtype: string

DigitalTwin
^^^^^

.. attribute:: DigitalTwin

   :returns: The digital twin that represents for the current operation.
   :rtype: DigitalTwin

DigitalTwinItem
^^^^^

.. attribute:: DigitalTwinItem

   :returns: The digital twin item (tree item) that represents for the current operation.
   :rtype: DigitalTwinItem


IsInitiated
^^^^^

.. attribute:: IsInitiated

   :returns: Whether the operation cache has been loaded.
   :rtype: bool

Delimiter
^^^^^

.. attribute:: Delimiter

   :returns: The delimiter used to separate the path for locations.
   :rtype: string


Name
^^^^^

.. attribute:: Name

   :returns: The name of the operation.
   :rtype: string

SpreadsheetDefinition
^^^^^

.. attribute:: SpreadsheetDefinition

   :returns: The SpreadsheetDefinition for the current operation.
   :rtype: SpreadsheetDefinition

FifteenMinuteWorksheetDefinition
^^^^^

.. attribute:: FifteenMinuteWorksheetDefinition

   :returns: The FifteenMinuteWorksheetDefinition for the current operation.
   :rtype: WorksheetDefinition

HourlyWorksheetDefinition
^^^^^

.. attribute:: HourlyWorksheetDefinition

   :returns: The HourlyWorksheetDefinition for the current operation.
   :rtype: WorksheetDefinition

FourHourWorksheetDefinition
^^^^^

.. attribute:: FourHourWorksheetDefinition

   :returns: The FourHourWorksheetDefinition for the current operation.
   :rtype: WorksheetDefinition

DailyWorksheetDefinition
^^^^^

.. attribute:: DailyWorksheetDefinition

   :returns: The DailyWorksheetDefinition for the current operation.
   :rtype: WorksheetDefinition

FifteenMinuteRows
^^^^^

.. attribute:: FifteenMinuteRows

   :returns: The Cached FifteenMinuteRows for the current operation.
   :rtype: Dictionary<uint, Row>

HourlyRows
^^^^^

.. attribute:: HourlyRows

   :returns: The Cached HourlyRows for the current operation.
   :rtype: Dictionary<uint, Row>

FourHourRows
^^^^^

.. attribute:: FourHourRows

   :returns: The Cached FourHourRows for the current operation.
   :rtype: Dictionary<uint, Row>

DailyRows
^^^^^

.. attribute:: DailyRows

   :returns: The Cached DailyRows for the current operation.
   :rtype: Dictionary<uint, Row>

LocationTwins
^^^^^

.. attribute:: LocationTwins

   :returns: The location twins for the current operation.
   :rtype: List<DigitalTwin>

ColumnTwins
^^^^^

.. attribute:: ColumnTwins

   :returns: The column twins for the current operation.
   :rtype: List<DigitalTwin>

MeasurementCache
^^^^^

.. attribute:: MeasurementCache

   :returns: The cache of measurements for the current operation.
   :rtype: Dictionary<string, List<Measurement>>


.. autosummary::
   :toctree: generated