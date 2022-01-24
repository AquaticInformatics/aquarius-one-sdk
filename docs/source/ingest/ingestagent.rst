IngestAgent
=============

Class
------

 ..  class:: IngestAgent
    :module: ClientSDK.Ingest

Methods
---------

IncrementNextUpload
^^^^^^^^^^^^^^^^^^^^

Increments the Next Upload time and sets the Next Upload according to the Configuration Upload Frequency

- **Required Role(s)**: Any

.. method:: IncrementNextUpload(DateTime dateTime)
   :module: ClientSDK.Ingest.IngestAgent

   :param dateTime: The time of the current completed upload
   :type dateTime: DateTime

   :rtype: void

IngestDataAsync
^^^^^^^^^^^^^^^^^^^^

Adds Data to be uploaded to a local cache until the UploadAsync method is called

- **Required Role(s)**: Any

.. method:: IngestData(string telemetryTwinId, DateTime dateTime, double? value, string stringValue= "", object detail = null)
   :module: ClientSDK.Ingest.IngestAgent

   :param telemetryTwinId: Digital Twin Reference Id of the Telemetry Dataset
   :type telemetryTwinId: string
   :param dateTime: Time related to the value
   :type dateTime: DateTime
   :param value: Numerical data to be stored
   :type value: double?
   :param stringValue: (Optional) String equivalent of the Numerical data to be stored
   :type stringValue: string
   :param detail: (Optional) additional data stored as JSON
   :type detail: object

   :rtype: void


.. method:: IngestData(string telemetryTwinId, TimeSeriesData timeSeriesData)
   :module: ClientSDK.Ingest.IngestAgent

   :param telemetryTwinId: Digital Twin Reference Id of the Telemetry Dataset
   :type telemetryTwinId: string
   :param timeSeriesData: Protocol Buffer used to represent Data
   :type timeSeriesData: TimeSeriesData

   :rtype: void

InitializeAsync
^^^^^^^^^^^^^^^^^^^^

Initializes the Class when a new agent is created

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: InitializeAsync(AuthenticationApi authenticationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, string ingestClientId, string ingestAgentName, string agentSubTypeId)
   :module: ClientSDK.Ingest.IngestAgent

   :param authenticationApi: The Authentication Class from the Client SDK
   :type authenticationApi: AuthenticationApi
   :param coreApi: The Core Class from the Client SDK
   :type coreApi: CoreApi
   :param digitalTwinApi: The Digital Twin Class from the Client SDK
   :type digitalTwinApi: DigitalTwinApi
   :param configurationApi: The Configuration Class from the Client SDK
   :type configurationApi: ConfigurationApi
   :param dataApi: The Data Class from the Client SDK
   :type dataApi: DataApi
   :param ingestClientId: Digital Twin Reference Id of the Instrument Ingestion Client
   :type ingestClientId: string
   :param ingestAgentName: Name of this Agent
   :type ingestAgentName: string
   :param agentSubTypeId: Instrument Agent Digital Twin SubType Id
   :type agentSubTypeId: string

   :returns: Whether the agent was successfully initialized
   :rtype: Task<bool>

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the Agent with the information it needs to run

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: LoadAsync(AuthenticationApi authenticationApi, CoreApi coreApi, DigitalTwinApi digitalTwinApi, ConfigurationApi configurationApi, DataApi dataApi, DigitalTwin digitalTwin)
   :module: ClientSDK.Ingest.IngestAgent

   :param authenticationApi: The Authentication Class from the Client SDK
   :type authenticationApi: AuthenticationApi
   :param coreApi: The Core Class from the Client SDK
   :type coreApi: CoreApi
   :param digitalTwinApi: The Digital Twin Class from the Client SDK
   :type digitalTwinApi: DigitalTwinApi
   :param configurationApi: The Configuration Class from the Client SDK
   :type configurationApi: ConfigurationApi
   :param dataApi: The Data Class from the Client SDK
   :type dataApi: DataApi
   :param digitalTwin: The digital Twin that represents the IngestAgent
   :type digitalTwin: DigitalTwin

   :returns: Whether the Agent was successfully loaded
   :rtype: Task<bool>

LoadConfiguration
^^^^^^^^^^^^^^^^^^^^

Loads the configuration object from the ConfigurationJSON

- **Required Role(s)**: Any

.. method:: LoadConfiguration(string json)
   :module: ClientSDK.Ingest.IngestAgent

   :param json: The configuration represented as a JSON string
   :type json: string

   :returns: Whether the load was successful
   :rtype: Task<bool>


RunAsync
^^^^^^^^^^^^^^^^^^^^

Runs the agent to obtain data

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: RunAsync()
   :module: ClientSDK.Ingest.IngestAgent

   :returns: Whether the run was successful
   :rtype: Task<bool>


SaveAsync
^^^^^^^^^^^^^^^^^^^^

Saves all of the Digital Twin and Configuration Data related to the agent.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: SaveAsync()
   :module: ClientSDK.Ingest.IngestAgent

   :returns: Whether the save was successful
   :rtype: Task<bool>

UploadAsync
^^^^^^^^^^^^^^^^^^^^^^^^^^

Uploads the data to a Telemetry Data Set configured by a digital Twin with the same Reference Id

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: UploadAsync()
   :module: ClientSDK.Ingest.IngestAgent

   :returns: Whether the upload was successful
   :rtype: Task<bool>

UpdateTelemetryTwinName
^^^^^^^^^^^^^^^^^^^^^^^^^^

Updates one of the Telemetry Twins Names

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: UpdateTelemetryTwinName(DigitalTwin digitalTwin, string name)
   :module: ClientSDK.Ingest.IngestAgent

   :param digitalTwin: The Telemetry Twin
   :type digitalTwin: DigitalTwin
   :param name: The new name of the Telemetry Twin
   :type name: string

   :returns: Whether the Telemetry Twin was updated
   :rtype: Task<bool>

Properties
------


Configuration
^^^^^

The Configuration Object related to the Agent

.. attribute:: Configuration
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: IngestAgentConfiguration

ConfigurationJson
^^^^^

The JSON representation of the configuration

.. attribute:: ConfigurationJson
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: string

DataSets
^^^^^

Data is a memory cache for the data to be stored by Telemetry GUID

.. attribute:: DataSets
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: Dictionary<string,TimeSeriesDatas>

Enabled
^^^^^

Whether the Agent is Enabled.

.. attribute:: Enabled
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: bool


IsTimeToRun
^^^^^

Whether the Agent is Eligible to run. AKA it is time to run.

.. attribute:: IsTimeToRun
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The name of the IngestAgent
   :rtype: bool

LastRun
^^^^^

Last time the agent was run

.. attribute:: LastRun
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: DateTime

LastUpload
^^^^^

Last time the data in the agent was uploaded

.. attribute:: LastUpload
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: DateTime

Logger
^^^^^

This is a class that manages the logging of information for the Agent

.. attribute:: Logger
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: IngestLogger


Name
^^^^^

This property returns the name of the Ingest Client agent.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The name of the IngestAgent
   :rtype: DateTime

NextRun
^^^^^

Next time the agent should be run

.. attribute:: NextRun
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: DateTime

NextUpload
^^^^^

Next time the data in the agent should be uploaded

.. attribute:: NextUpload
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: DateTime

Telemetry
^^^^^

The Telemetry Datasets that are related to the Agent.
This property is populated when the LoadAsync is called.

.. attribute:: Telemetry
   :module: ClientSDK.Ingest.IngestAgent

   :returns: The collection of digital twin reference ids that represent the data related to Ingest Client agent.
   :rtype: List<string>

TwinSubTypeId
^^^^^

Identifies the unique type of the agent.  This must be set by the implementing class for the agent to be properly registered

.. attribute:: TwinSubTypeId
   :module: ClientSDK.Ingest.IngestAgent

   :rtype: string

.. autosummary::
   :toctree: generated

  
