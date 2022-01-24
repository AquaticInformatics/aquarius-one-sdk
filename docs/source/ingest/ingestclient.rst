IngestClient
=============

This is an inheritable class when implementing an ingestion service.

Class
-------

 ..  class:: IngestClient
    :module: ClientSDK.Ingest

Methods
---------

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the Client with the information it needs to run

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: LoadAsync(List<IngestAgent> ingestAgents)
   :module: ClientSDK.Ingest

   :param ingestAgents: The collection of ingest agent objects
   :type ingestAgents: List<IngestAgent>

   :returns: Whether the Client was successfully loaded
   :rtype: Task<bool>

RegisterAgentAsync
^^^^^^^^^^^^^^^^^^^^

Registers a new IngestAgent with this Client.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: RegisterAgentAsync(IngestAgent ingestAgent, string ingestAgentName, string agentSubTypeId)
   :module: ClientSDK.Ingest

   :param ingestAgent: The IngestAgent Object
   :type ingestAgent: IngestAgent
   :param ingestAgentName: Name of the new Agent
   :type ingestAgentName: string
   :param agentSubTypeId: Instrument Agent Digital Twin SubType Id
   :type agentSubTypeId: string

   :returns: The newly registered Ingest Agent.
   :rtype: Task<IngestAgent>

Save
^^^^^^^^^^^^^^^^^^^^

Saves all of the Digital Twin and Configuration Data related to the client.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: Save()
   :module: ClientSDK.Ingest

   :returns: Whether the save was successful.
   :rtype: Task<IngestAgent>


Properties
----------

Agents
^^^^^^^^^

A Collection of Ingest Agents that belong to this client
This property is populated when the LoadAsync is called.

.. attribute:: Agents
   :module: ClientSDK.Ingest.IngestClient

   :rtype: List<IngestAgent>

ConfigurationJson
^^^^^^^^^^^^^^^^^^^^^^^

This property returns the configuration JSON for the Ingest Client.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: ConfigurationJson
   :module: ClientSDK.Ingest.IngestClient

   :returns: The configuration as a JSON string for the IngestClient
   :rtype: string

Id
^^^^^

The Twin Reference Id of the Twin that represents the ingest client

.. attribute:: Id
   :module: ClientSDK.Ingest.IngestClient

   :rtype: string

Logger
^^^^^^^^

This is a class that manages the logging of information for the Client

.. attribute:: Logger
   :module: ClientSDK.Ingest.IngestClient

   :rtype: IngestLogger

Name
^^^^^

The name of the Ingest Client.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestClient

   :returns: The name of the IngestClient
   :rtype: string




.. autosummary::
   :toctree: generated

  
