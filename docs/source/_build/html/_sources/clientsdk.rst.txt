ClientSDK
====

The ClientSDK Object is the entry point for using the SDK.

Classes
----------
Once instantiated, this class provides:

Authentication
^^^^^^

..  class:: Authentication 
    
   - See: :doc:`/enterprise/authentication/` 

Configuration
^^^^^^

..  class:: Configuration 

   - See: :doc:`/common/configuration/` 

Core
^^^^^^
  
..  class:: Core 

   - See: :doc:`/enterprise/core/`

Data
^^^^^^
  
..  class:: Data 

   - See: :doc:`/timeseries/data/` 

DigitalTwin
^^^^^^
  
..  class:: DigitalTwin 

   - See: :doc:`/enterprise/twin/` 

Library
^^^^^^
  
..  class:: Library 

   - See: :doc:`/common/library/` 

Report
^^^^^^
   
..  class:: Report 

   - See: :doc:`/enterprise/report/` 

Spreadsheet
^^^^^^
  
..  class:: Spreadsheet 

   - See: :doc:`/operation/spreadsheet/` 


Helper Classes
-------------

CacheHelper
^^^^^^
  
..  class:: CacheHelper 

   - See: :doc:`/helpers/cache/` 

Logger
^^^^^^
  
..  class:: Logger 

   - See: :doc:`/helpers/logger/` 


PoEditor
^^^^^^
  
..  class:: PoEditor 

   - See: :doc:`/helpers/poeditor/` 

Properties
---------------

ContinueOnCapturedContext
^^^^^^
  
.. attribute:: ContinueOnCapturedContext

   :returns: 
   :rtype: bool 


Environment
^^^^^^

The Environment property must be set so that the SDK knows which deployment environment to communicate.
See: :doc:`/concepts/environments`

.. attribute:: Environment



   :returns: See :doc:`/helpers/environment` 
   :rtype: PlatformEnvironment 

.. code-block:: C#

   using ONE.Utilities;
   clientSDK.Environment = PlatformEnvironmentHelper.GetPlatformEnvironment(EnumPlatformEnvironment.AqiFeature);

Logger
^^^^^^
  
.. attribute:: Logger

   :returns: 
   :rtype: bool 

LogRestfulCalls
^^^^^^
  
.. attribute:: LogRestfulCalls

   :returns: 
   :rtype: bool 


.. autosummary::
   :toctree: generated
  
