IngestClient
====



Class
----

 ..  class:: IngestClient
    :module: ClientSDK.Ingest

Methods
------

LoadAsync
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client and all of it's plugins.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: LoadAsync()
   :module: ClientSDK.Ingest

   :returns: Whether the load was successful.
   :rtype: Task<bool>

Save
^^^^^^^^^^^^^^^^^^^^

Loads the definition of the ingest client, its configuration and all of it's plugins.

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: Save()
   :module: ClientSDK.Ingest

   :returns: Whether the save was successful.
   :rtype: Task<bool>


Properties
------

ConfigurationJson
^^^^^

This property returns the configuration JSON for the Ingest Client.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: ConfigurationJson
   :module: ClientSDK.Ingest.IngestClient

   :returns: The configuration as a JSON string for the IngestClient
   :rtype: string

Name
^^^^^

This property returns the name of the Ingest Client.  
If this property is set, the Save() method will need to be called to persist the change.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestClient

   :returns: The name of the IngestClient
   :rtype: DateTime

Plugins
^^^^^

This property returns the collection of Plugins fo the Ingest Client.  
This property is populated when the LoadAsync is called.

.. attribute:: Name
   :module: ClientSDK.Ingest.IngestClient

   :returns: The name of the IngestClient
   :rtype: DateTime


.. autosummary::
   :toctree: generated

  
