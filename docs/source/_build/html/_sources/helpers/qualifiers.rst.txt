Qualifiers
====

This is a static helper class to assist with laboratory qualifiers.

Class
----

 ..  class:: Qualifiers 
    :module: ONE.Operations.Spreadsheet

Concepts
-----

Cascade rule
^^^^^

CASCADE RULE: Sets when to display the data qualifiers (<,>, ND) with the calculated statistic.  Valid settings are:

0. Display no data qualifiers in result.
1. Display the data qualifier if one of the values contains the qualifier.
2. Display data qualifier if over half the values contains the qualifier.
3. Display data qualifier if all of the values contains the qualifier.

Qualifier Rules
^^^^^

0. Zero
1. Entered Value 
2. Half of <, twice of >


.. list-table:: Qualifier Rule Example
   :widths: 25 25 25 25
   :header-rows: 1

   * - Qualifier
     - Rule 0
     - Rule 1
     - Rule 2
   * - <
     - 0
     - Entered Value
     - Half Value
   * - >
     - Entered Value
     - Entered Value
     - Double Value
   * - ND
     - 0
     - 0
     - 0
   * - E
     - 0
     - Entered Value
     - Half Value
   * - DNQ
     - 0
     - Entered Value
     - Half Value
   * - TNTC
     - Enter Value or 10000 if no value
     - Enter Value or 10000 if no value
     - Enter Value or 10000 if no value
   * - P
     - 1
     - 1
     - 1
   * - A
     - 0
     - 0
     - 0

Sort Rules
^^^^^

**California Sort Rule**

California requires the Average or the Geometric Mean to be reported as a Median if any values in the data set contains a data qualifier (i.e. <,ND, DNQ, etc...).  WIMS Spread Functions GAVGMEDZ, GGMMEDZ, and STATZ support these calculations. 

The following is the Rule as stated in a typical California Permit:

When determining compliance with an average monthly limit and more than one sample result is available in a month, the discharger shall compute the arithmetic mean unless the data set contains one or more reported determinations of detected but not quantified (DNQ) or not detected (ND). In those cases, the discharger shall compute the median in place of the arithmetic mean in accordance with the following procedure:

(1) The data set shall be ranked from low to high, reported ND determinations lowest, DNQ determinations next, followed by quantified values (if any). The order of the individual ND or DNQ determinations is unimportant.

(2) The median value of the data set shall be determined. If the data set has an odd number of data points, then the median is the middle value. If the data set has an even number of data points, then the median is the average of the two values around the middle unless one or both of the points are ND or DNQ, in which case the median value shall be the lower of the two data points where DNQ is lower than a value and ND is lower than DNQ. If a sample result, or the arithmetic mean or median of multiple sample results, is below the reported ML, and there is evidence that the priority pollutant is present in the effluent above an effluent limitation and the discharger conducts a pollutant minimization program (PMP)16 (as described in Section I.7.), the discharger shall not be deemed out of compliance.

Other permits state: (which implies Geometric Mean should also be calculated as a Median if data qualifiers exist)

When determining compliance with a measure of central tendency (arithmetic mean, geometric mean, median, etc...) of multiple sample analyses and the data set contains one or more reported determinations of detected but not quantified (DNQ) or not detected (ND), the Permittee shall compute the median in place of the arithmetic mean in accordance with the following procedure:

(1)  The data set shall be ranked from low to high, ranking the ND determinations lowest, DNQ determinations next, followed by quantified values (if any). The order of the individual ND or DNQ determinations is unimportant.

(2) The median value of the data set shall be determined. If the data set has an odd number of data points, then the median is the middle value. If the data set has an even number of data points, then the median is the average of the two values around the middle unless one or both of the points are ND or DNQ, in which case the median value shall be the lower of the two data points where DNQ is lower than a value and ND is lower than DNQ.



Methods
-----

GetAggregateQualifer
^^^^^^^^^^^^^^^^^^^^

Gets the qualifier that should be applied to an aggregate result.

.. method:: GetAggregateQualifer(List<Measurement> measurements, int cascadeRule)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param measurements: A collection of measurements.
   :type measurements: List<Measurement> 
   :param cascadeRule: Cascade rule:  See concepts above.
   :type cascadeRule: int

   :returns: The resulting qualifier from the aggregate.
   :rtype: string

GetRowNumberFromDateTimeAndWorksheetType
^^^^^^^^^^^^^^^^^^^^

Gets the count of each of the qualifiers from the QualifierList.  
This is helpful for many statistical queries that will bias the result based on the weight of the qualifiers.

.. method:: GetQualifierCount(List<Measurement> measurements)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param measurements: A collection of measurements to get the qualifier count.
   :type measurements: List<Measurement> 

   :returns: An array of the counts of qualifiers.  The array matches the QualifierList.
   :rtype: int[]

GetSymbol
^^^^^^^^^^^^^^^^^^^^

Retrieves the symbol from a string value.

.. method:: GetSymbol(string value)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param value: The value to retrieve the symbol.
   :type value: string

   :returns: The symbol if one exists in the input value.
   :rtype: string

GetValue
^^^^^^^^^^^^^^^^^^^^

Retrieves the numeric value from a string value taking into consideration the qualifier rules.

.. method:: GetValue(string enteredValue, EnumQualifierRule qualifierRule)
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param enteredValue: The value to retrieve the numeric value.
   :type enteredValue: string

   :returns: The value from the string value.
   :rtype: object

Sort
^^^^^^^^^^^^^^^^^^^^

Returns a sorted list of items based on qualifier and sort rules.

.. method:: Sort(object range, EnumQualifierRule qualifierRule, string sortRule, string sortOrder = "")
   :module: ONE.Operations.Spreadsheet.RowNumber

   :param range: A collection of values.  See Range under concepts.
   :type range: object

   :returns: The value from the string value.
   :rtype: object


Properties
-----

.. attribute:: QualifierList
   :module: ONE.Operations.Spreadsheet.Qualifiers

   :returns: A comma delimited list of supported qualifiers.
   :rtype: string

.. autosummary::
   :toctree: generated
    
