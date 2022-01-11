RowNumber
====

This is a static class that helps convert dates to row numbers and row numbers to dates.

 ..  class:: RowNumber 
    :module: ONE.Operations.Spreadsheet

Overview
-----

For the ACO worksheets, there is a row number assigned to each slot.  These row numbers all originate from Midnight on January 1, 1900.
Example first five row number to date time for each worksheet type:

Daily

#. - 1/1/1900 0:00 (Midnight)
#. - 1/2/1900 0:00 (Midnight)
#. - 1/3/1900 0:00 (Midnight)
#. - 1/4/1900 0:00 (Midnight)
#. - 1/5/1900 0:00 (Midnight)

Hourly

#. - 1/1/1900 0:00 (Midnight)
#. - 1/1/1900 1:00
#. - 1/1/1900 2:00
#. - 1/1/1900 3:00
#. - 1/1/1900 4:00

Four Hour

#. - 1/1/1900 0:00 (Midnight)
#. - 1/1/1900 4:00
#. - 1/1/1900 8:00
#. - 1/1/1900 12:00
#. - 1/1/1900 16:00

15 Minute

#. - 1/1/1900 0:00 (Midnight)
#. - 1/1/1900 0:15
#. - 1/1/1900 0:30
#. - 1/1/1900 0:45
#. - 1/1/1900 1:00

Methods
-----

GetDateTimeFromRowNumberAndWorksheetType
^^^^^^^^^^^^^^^^^^^^

Gets the Date Time from a row number.  The worksheet type is also required because the row number sequence is different for each worksheet type. 

.. method:: GetDateTimeFromRowNumberAndWorksheetType(uint rowNumber, EnumWorksheet worksheetType)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param rowNumber: The number of the row.  See overview above.
   :type rowNumber: uint
   :param worksheetType: The type of the worksheet.
   :type worksheetType: EnumWorksheet

   :returns: The row number given the date and worksheet type.  Null is returned if there is an error.
   :rtype: DateTime?

GetRowNumberFromDateTimeAndWorksheetType
^^^^^^^^^^^^^^^^^^^^

Gets the Date Time from a row number.  The worksheet type is also required because the row number sequence is different for each worksheet type. 

.. method:: GetRowNumberFromDateTimeAndWorksheetType(DateTime localDateTime, EnumWorksheet worksheetType)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param localDateTime: The local Date and time for the operation.
   :type localDateTime: DateTime
   :param worksheetType: The type of the worksheet.
   :type worksheetType: EnumWorksheet

   :returns: The row number given the date and worksheet type.  Null is returned if there is an error.
   :rtype: DateTime?

TimeSpanOfWorksheetType
^^^^^^^^^^^^^^^^^^^^

Gets the Date Time from a row number.  The worksheet type is also required because the row number sequence is different for each worksheet type. 

.. method:: TimeSpanOfWorksheetType(EnumWorksheet worksheetType)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param worksheetType: The type of the worksheet.
   :type worksheetType: EnumWorksheet

   :returns: The time span of a row, given the worksheet type.
   :rtype: TimeSpan

Properties
-----

.. attribute:: BaseTime
   :module: ONE.Operations.Spreadsheet.RowNumber

   :returns: The beginning of time for row numbering
   :rtype: DateTime

.. autosummary::
   :toctree: generated
  
