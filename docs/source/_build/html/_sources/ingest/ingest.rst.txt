Ingest
===========

The Ingest API is intended to provide an SDK for client applications installed at customer locations to ingest data from local data sources.

Class
----------

 ..  class:: Ingest
    :module: ClientSDK

Child Classes
-------------

- :doc:`/ingest/ingestclient` 
- :doc:`/ingest/ingestagent` 
- :doc:`/ingest/ingestagentconfiguration` 

Methods
----------

GetClientByIdAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves the IngestClient by the Twin Reference Id

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: GetClientByIdAsync(string ingestClientId)
   :module: ClientSDK.Ingest

   :param ingestClientId: The unique Identifier (GUID) of the Operation Digital Twin related to the spreadsheet.
   :type ingestClientId: string

   :returns: An IngestClient object that contains all of the configuration for the client.
   :rtype: Task<IngestClient>

RegisterClientAsync
^^^^^^^^^^^^^^^^^^^^

Creates a Digital Twin for the Ingest Client and does minimal setup

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: RegisterClientAsync(string ingestClientName)
   :module: ClientSDK.Ingest

   :param ingestClientName: The name of the ingest client.
   :type ingestClientName: string

   :returns: An IngestClient object that contains all of the configuration for the client.
   :rtype: Task<IngestClient>

GetAllClientsAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves a collection of all IngestClients the user has rights to retrieve

- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: GetAllClientsAsync()
   :module: ClientSDK.Ingest

   :returns: A collection of all of the ingest clients the user has access.
   :rtype: Task<List<IngestClient>>


.. autosummary::
   :toctree: generated

  
