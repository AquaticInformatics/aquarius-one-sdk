Spreadsheet
====

Water treatment, purpose built data models of tabular data related to water treatment operations. 
Used in conjunction with Enterprise Twins to model the treatment process and Common Computations to calculate data needed to 
operate treatment processes and facilities within regulatory compliance.

 ..  class:: Report 
    :module: ClientSDK.Spreadsheet

Methods
----------

CellValidateAsync
^^^^^^^^^^^^^^^^^^^^

Validates the provided cell. This is the same validation that takes place on a save request but without actually saving the cell.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=cellvalidate>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: CellValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, Cell cell)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param cell: The Cell to validate.
   :type cell: Cell

   :returns: User Object that matches the User Protocol Buffer.
   :rtype: Task<Cell>

ColumnGetByDayAsync
^^^^^^^^^^^^^^^^^^^^

Returns column data based on day, month and year

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=columngetbyday>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-GRAPHS

.. method:: ColumnGetByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param columnId: The numeric id or column number.
   :type columnId: uint
   :param date: The Date to retrieve the Column.
   :type date: DateTime

   :returns: A collection of measurements that match the date specified.
   :rtype: Task<List<Measurement>>

ColumnGetByMonthAsync
^^^^^^^^^^^^^^^^^^^^

Returns column data based on month and year of the passed in date

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=columngetbymonth>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-GRAPHS

.. method:: ColumnGetByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param columnId: The numeric id or column number.
   :type columnId: uint
   :param date: The Date to retrieve the Column.
   :type date: DateTime

   :returns: A collection of measurements that match the date specified.
   :rtype: Task<List<Measurement>>

ColumnGetByYearAsync
^^^^^^^^^^^^^^^^^^^^

Returns column data based on year of the passed in date

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=columngetbyyear>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-GRAPHS

.. method:: ColumnGetByYearAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint columnId, DateTime date)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param columnId: The numeric id or column number.
   :type columnId: uint
   :param date: The Date to retrieve the Column.
   :type date: DateTime

   :returns: A collection of measurements that match the date specified.
   :rtype: Task<List<Measurement>>

ComputationCreateAsync
^^^^^^^^^^^^^^^^^^^^

Creates a new spreadsheet computation.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=computationcreate>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.COMPUTATIONS_WRITE

.. method:: ComputationCreateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param spreadsheetComputation: The computation related to the column.
   :type spreadsheetComputation: SpreadsheetComputation

   :returns: A SpreadsheetComputation object.
   :rtype: Task<SpreadsheetComputation>

ComputationExecuteAsync
^^^^^^^^^^^^^^^^^^^^

Executes all computations based on column number, start row or end row.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=computationcreate>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-EDIT

.. method:: ComputationExecuteAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, DataSourceBinding dataSourceBinding)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param startRow: Start Row number for computation execution.
   :type startRow: uint
   :param endRow: End Row number for computation execution.
   :type endRow: uint

   :rtype: Task<SpreadsheetComputation>

ComputationGetOneAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves a computation based on the Computation Binding ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=computationcreate>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-EDIT

.. method:: ComputationGetOneAsync(string twinReferenceId, EnumWorksheet worksheetType, string computationBindingId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param computationBindingId: Binding ID of the computation to be retrieved.
   :type computationBindingId: string

   :rtype: Task<SpreadsheetComputation>

ComputationGetOneAsync
^^^^^^^^^^^^^^^^^^^^

Validates a computation payload.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=computationcreate>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-EDIT

.. method:: ComputationValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param spreadsheetComputation: The computation related to the column.
   :type spreadsheetComputation: SpreadsheetComputation

   :rtype: Task<List<ApiError>>

DeletePlantAsync
^^^^^^^^^^^^^^^^^^^^

Delete all entities associated with a plant including Spreadsheet/Worksheet Definition, Rows, RowIndices, Computations, Twins, Configurations and Reports.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=deleteplant>`_ 
- **Required Role(s)**: Support Admin, Claros Admin

.. method:: DeletePlantAsync(string operationTwinReferenceId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string

   :returns: Whether the plant was successfully deleted.
   :rtype: Task<bool>

FlushPlantAsync
^^^^^^^^^^^^^^^^^^^^

Flushes data from the speed layer to the batch layer based on Plant ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=flushplant`_ 
- **Required Role(s)**: Claros Admin

.. method:: FlushPlantAsync(string operationTwinReferenceId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string

   :returns: Whether the plant was successfully deleted.
   :rtype: Task<bool>

GetRowsAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves rows based on start row and end row number.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=getrows>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: GetRowsAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, uint startRow, uint endRow, string columnList = null, string viewId = null)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param startRow: Start Row number for computation execution.
   :type startRow: uint
   :param endRow: End Row number for computation execution.
   :type endRow: uint
   :param columnList: (Optional) coma delimited list of the Column Numbers to retrieve.
   :type columnList: string
   :param viewId: (Optional) View ID (GUID) to filter the columns of the rows which are returned.
   :type viewId: string

   :rtype: Task<Rows>

GetRowsByDayAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves rows based on day, month and year.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=getrowsbyday>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: GetRowsByDayAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param date: The Date to retrieve the rows.
   :type date: DateTime
   :param columnList: (Optional) coma delimited list of the Column Numbers to retrieve.
   :type columnList: string
   :param viewId: (Optional) View ID (GUID) to filter the columns of the rows which are returned.
   :type viewId: string

   :rtype: Task<Rows>

GetRowsByMonthAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves rows based on month and year.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=getrowsbymonth>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: GetRowsByMonthAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, DateTime date, string columnList = null, string viewId = null)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param date: The Date to retrieve the rows.
   :type date: DateTime
   :param columnList: (Optional) coma delimited list of the Column Numbers to retrieve.
   :type columnList: string
   :param viewId: (Optional) View ID (GUID) to filter the columns of the rows which are returned.
   :type viewId: string

   :rtype: Task<Rows>

GetSpreadsheetDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves spreadsheet definition based on plant ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=getspreadsheetdefinition>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: GetSpreadsheetDefinitionAsync(string operationTwinReferenceId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string

   :rtype: Task<SpreadsheetDefinition>

GetWorksheetDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves worksheet definition based on plant ID and worksheet type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=getworksheetdefinition>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: GetWorksheetDefinitionAsync(string operationTwinReferenceId, EnumWorksheet worksheetType)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet

   :rtype: Task<WorksheetDefinition>

SaveRowsAsync
^^^^^^^^^^^^^^^^^^^^

Creates new rows or updates existing rows based on Plant ID and Worksheet Type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=saverows>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ

.. method:: SaveRowsAsync(Rows rows, string operationTwinReferenceId, EnumWorksheet worksheetType)
   :module: ClientSDK.Spreadsheet

   :param rows: The rows to be saved.
   :type rows: Rows
   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet

   :rtype: Task<bool>

SaveSpreadsheetDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Creates new spreadsheet definition or updates existing spreadsheet definition based on Plant ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=savespreadsheetdefinition>`_ 
- **Required Role(s)**: Admin, Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-CONFIGURE

.. method:: SaveSpreadsheetDefinitionAsync(string operationTwinReferenceId, SpreadsheetDefinition spreadsheetDefinition)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param spreadsheetDefinition: The definition of the spreadsheet.
   :type spreadsheetDefinition: SpreadsheetDefinition

   :rtype: Task<bool>

WorksheetAddColumnAsync
^^^^^^^^^^^^^^^^^^^^

Adds worksheet definition columns based on plant ID and worksheet type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=addworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Admin, Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-CONFIGURE

.. method:: WorksheetAddColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param worksheetDefinition: The definition of the worksheet.
   :type worksheetDefinition: WorksheetDefinition

   :rtype: Task<bool>

.. note:: 
   This endpoint supports creating 2000 worksheet columns per request. 
   It is expected to create 80-100 columns per second. For scenarios that require creating more than 2000 columns, it is required to create columns in batches.

WorksheetUpdateColumnAsync
^^^^^^^^^^^^^^^^^^^^

Updates worksheet definition columns based on plant ID and worksheet type.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=updateworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Admin, Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-CONFIGURE

.. method:: WorksheetUpdateColumnAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, WorksheetDefinition worksheetDefinition)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param worksheetDefinition: The definition of the worksheet.
   :type worksheetDefinition: WorksheetDefinition

   :rtype: Task<bool>

TODO (Missing) Methods
------

BackupSpreadsheetAsync
^^^^^^^^^^^^^^^^^^^^

This copies the spreadsheet data for a plant/operation to long term storage.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=updateworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: BackupSpreadsheetAsync(string operationTwinReferenceId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string

   :rtype: Task<bool>

RestoreSpreadsheetAsync
^^^^^^^^^^^^^^^^^^^^

This restores the spreadsheet data from a plant/operation to application from backups.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=updateworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
  
.. method:: RestoreSpreadsheetAsync(string operationTwinReferenceId)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string

   :rtype: Task<bool>

BackupSpreadsheetAsync
^^^^^^^^^^^^^^^^^^^^

Cooks the data based on Plant ID.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=updateworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Support Admin, Claros Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
  
.. method:: CookPlantAsync(string operationTwinReferenceId, EnumWorksheet worksheetType)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet

   :rtype: Task<bool>

GetRowIndicesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves the row indices based on the query criteria provided.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-operations-spreadsheet-v1&operation=updateworksheetdefinitioncolumns>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
- **Feature Authorization**: FEATURE.SPREADSHEET-READ
  
.. method:: GetRowIndicesAsync(string operationTwinReferenceId, EnumWorksheet worksheetType,DateTime relativeTime, DateTime utcTime, bool isInSpeed, bool isInCooked, bool is ColumnsCooked)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet

   :rtype: Task<bool>


.. autosummary::
   :toctree: generated
  
