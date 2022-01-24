IngestAgentConfiguration
============================

This class can be inherited when implementing ingest agents

Class
------

 ..  class:: IngestClient
    :module: ClientSDK.Ingest

Methods
--------

Load
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client and all of it's plugins.
This is a virtual method that can be overridden by the implementing class.

- **Required Role(s)**: Any

.. method:: Load(string configurationJson)
   :module: ClientSDK.Ingest

   :param configurationJson: The Configuration represented as a JSON string.
   :type configurationJson: string

   :returns: Whether the Configuration Json was successfully loaded.
   :rtype: Task<bool>

ToString
^^^^^^^^^^^^^^^^^^^^

Serialized the Configuration to a JSON String.

- **Required Role(s)**: Any

.. method:: ToString()
   :module: ClientSDK.Ingest

   :rtype: string

Properties
----------


RunFrequency
^^^^^^^^^^^^^^^^^^

Frequency the agent should acquire data

.. attribute:: RunFrequency
   :module: ClientSDK.Ingest.IngestAgentConfiguration

   :rtype: TimeSpan

UploadFrequency
^^^^^^^^^^^^^^^^^^

Frequency the agent should upload the data

.. attribute:: UploadFrequency
   :module: ClientSDK.Ingest.IngestAgentConfiguration

   :rtype: TimeSpan


.. autosummary::
   :toctree: generated

  
