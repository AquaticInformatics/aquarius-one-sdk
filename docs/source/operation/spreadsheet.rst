Spreadsheet APIs
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
- 
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
- 
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
- 
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
- 
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
- 
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
- 
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
- 
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
- 
.. method:: ComputationValidateAsync(string operationTwinReferenceId, EnumWorksheet worksheetType, SpreadsheetComputation spreadsheetComputation)
   :module: ClientSDK.Spreadsheet

   :param operationTwinReferenceId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type operationTwinReferenceId: string
   :param worksheetType: The enumeration of the type of worksheet.
   :type worksheetType: EnumWorksheet
   :param spreadsheetComputation: The computation related to the column.
   :type spreadsheetComputation: SpreadsheetComputation

   :rtype: Task<List<ApiError>>



.. autosummary::
   :toctree: generated
  
