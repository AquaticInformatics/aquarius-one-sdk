CacheHelper APIs
====

 ..  class:: CacheHelper 
    :module: ClientSDK

The purpose of the Cache Helper is to load fairly static data that is accessed commonly into memory.
In addition there are helper methods to retrieve these items efficiently.


LibraryCache Class
----

 ..  class:: LibraryCache 
    :module: ClientSDK.CacheHelper


The purpose of the Library Cache is to load the global library, which is fairly static into memory, 
so that commonly reused items such as quantity types, units and paramters along with their translations can be quickly available.
In addition there are helper methods to retrieve these items efficiently.


LibraryCache Class Methods
----


LibraryCache Class Properties
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


OperationsCache Class
----

 ..  class:: OperationsCache 
    :module: ClientSDK.CacheHelper

ReportCache Class
----

 ..  class:: ReportCache 
    :module: ClientSDK.CacheHelper

.. autosummary::
   :toctree: generated

  
