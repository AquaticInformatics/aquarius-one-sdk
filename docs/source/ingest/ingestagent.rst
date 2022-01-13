IngestAgent
====

Class
----

 ..  class:: IngestAgent
    :module: ClientSDK.Ingest

Methods
------

IngestDataAsync
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client agent and all of it's data.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: IngestDataAsync(string telemetryTwinId, TimeSeriesDatas timeSeriesDatas)
   :module: ClientSDK.Ingest.IngestAgent

   :param telemetryTwinId: The unique Identifier (GUID) of the telemetry or dataset.
   :type telemetryTwinId: string
   :param timeSeriesDatas: The historian data to be uploaded.
   :type timeSeriesDatas: TimeSeriesDatas

   :returns: Whether the load was successful.
   :rtype: Task<bool>

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client agent and all of it's data.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: LoadAsync()
   :module: ClientSDK.Ingest.IngestAgent

   :returns: Whether the load was successful.
   :rtype: Task<bool>

Save
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client agent and its configuration and all of it's data.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: Save()
   :module: ClientSDK.Ingest.IngestAgent

   :returns: Whether the save was successful.
   :rtype: Task<bool>


Properties
------

ConfigurationJson
^^^^^

This property returns the configuration JSON for the Ingest Client agent.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: ConfigurationJson
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The configuration as a JSON string for the IngestClient
   :rtype: string

Name
^^^^^

This property returns the name of the Ingest Client agent.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The name of the IngestAgent
   :rtype: DateTime

Telemetry
^^^^^

This property returns the collection of Digital Twins that represent the data related to Ingest Client agent.  
This property is populated when the LoadAsync is called.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The collection of digital twins that represent the data related to Ingest Client agent.
   :rtype: List<DigitalTwin>

.. autosummary::
   :toctree: generated

  
