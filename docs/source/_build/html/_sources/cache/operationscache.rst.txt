Operations
====

The purpose of the Operations Cache is to load the configuration of an ACO operation, which is fairly static into memory, 
so that commonly reused items such as column definitions, views and related location twins are quickly accessible.

OperationsCache Class
----

 ..  class:: OperationsCache 
    :module: ClientSDK.CacheHelper

Methods
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

GuidByIndex
^^^^^^^^^^^^^^^^^^^^

Finds the Operation GUID given the index of the operation.

- **Required Role(s)**: Any
  
.. method:: GuidByIndex(string index)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param index: Zero-based index of the operation.
   :type index: string

   :rtype: Task<bool>

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the operations cache.  After the operations cache is loaded, the user can dive into individual operation cache.

- **Required Role(s)**: Any
  
.. method:: LoadOperationsAsync()
   :module: ClientSDK.CacheHelper.OperationsCache

   :rtype: Task<bool>

Name
^^^^^^^^^^^^^^^^^^^^

Finds the Operation Name given the GUID of the operation.

- **Required Role(s)**: Any
  
.. method:: Name(string guid)
   :module: ClientSDK.CacheHelper.OperationsCache

   :param guid: unique identifier of the operation.
   :type guid: string

   :rtype: Task<bool>



Properties
-----

Units
^^^^^

.. attribute:: DefaultOperation
   :module: ClientSDK.CacheHelper.OperationsCache

   :rtype: OperationCache


.. autosummary::
   :toctree: generated

  
