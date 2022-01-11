Report
====

Provides human readable rendering of data as file artifacts. 
This service leverages Enterprise Data to assemble datasets required for the rendering of the reports. 
In addition Rendering Engines and Templates are registered within this system.

 ..  class:: Report 
    :module: ClientSDK

Report Definition Methods
------------

CreateDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Creates a report definition. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-create>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: CreateDefinitionAsync(ReportDefinition reportDefinition)
   :module: ClientSDK.Core   

   :param reportDefinition: ReportDefinition object
   :type reportDefinition: ReportDefinition


   :rtype: Task<ReportDefinition>

GetDefinitionsAsync
^^^^^^^^^^^^^^^^^^^^

Gets a list of report definitions. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-getmany>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation


.. method:: GetDefinitionsAsync(string operationId)
   :module: ClientSDK.Core

   :param operationId: The unique identifier (GUID) of the operation.
   :type firstName: string

   :returns: A list of Report Definitions.
   :rtype: Task<List<ReportDefinition>>

GetDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Gets a report definitions using its unique identifier (GUID). 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-get>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.


.. method:: GetDefinitionAsync(string id)
   :module: ClientSDK.Core

   :param id: The unique identifier (GUID) of the report definition.
   :type id: string

   :returns: A Report Definitions.
   :rtype: Task<List<ReportDefinition>>



DeleteDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a previously created report definition.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-delete>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.


.. method:: DeleteDefinitionAsync(string id)
   :module: ClientSDK.Core

   :param operationId: The unique identifier (GUID) of the report definition.
   :type firstName: string

   :returns: Whether or not the report definition was successfully deleted.
   :rtype: Task<bool>

UpdateDefinitionAsync
^^^^^^^^^^^^^^^^^^^^

Updates a previously created report definition.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-update>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.


.. method:: UpdateDefinitionAsync(ReportDefinition reportDefinition)
   :module: ClientSDK.Core   

   :param reportDefinition: ReportDefinition object
   :type reportDefinition: ReportDefinition


   :rtype: Task<ReportDefinition>

Report Methods
------------

DownloadReportAsync
^^^^^^^^^^^^^^^^^^^^

Download a report output (Report).

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=output-download-report>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
  
.. method:: DownloadReportAsync(string id, string filename)
   :module: ClientSDK.Core   

   :param id: The unique identifier (GUID) of the report definition.
   :type id: string
   :param filename: full path to the file to save
   :type filename: string


   :rtype: Task<bool>

Report Template Methods
------------

DownloadTemplateAsync
^^^^^^^^^^^^^^^^^^^^

Download a report template.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-upload>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.
  
.. method:: DownloadTemplateAsync(string id, string filename)
   :module: ClientSDK.Core   

   :param id: The unique identifier (GUID) of the report definition.
   :type id: string
   :param filename: full path to the file to save
   :type filename: string


   :rtype: Task<bool>


UploadDefinitionTemplateAsync
^^^^^^^^^^^^^^^^^^^^

Upload a report template (Excel template or spreadsheet).

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=definitions-upload>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: UploadDefinitionTemplateAsync(string id, string filename)
   :module: ClientSDK.Core   

   :param id: The unique identifier (GUID) of the report definition.
   :type id: string
   :param filename: full path to the file to upload
   :type filename: string

   :rtype: Task<ReportDefinition>

Report Tag Methods
------------

CreateReportTagAsync
^^^^^^^^^^^^^^^^^^^^

Validates and creates a new report definition tag.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=report-tags-create>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: CreateReportTagAsync(string reportDefinitionId, string tag)
   :module: ClientSDK.Core   

   :param reportDefinitionId: The unique identifier (GUID) of the report definition.
   :type reportDefinitionId: string
   :param tag: tag name
   :type tag: string


   :rtype: Task<ReportDefinitionTag>

DeleteReportTagAsync
^^^^^^^^^^^^^^^^^^^^

Deletes a previously created report definition tag.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=report-tags-create>`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: DeleteReportTagAsync(string id)
   :module: ClientSDK.Core   

   :param id: The unique identifier (GUID) of the report definition tag.
   :type id: string

   :rtype: Task<bool>

GetReportTagsAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves a list of all tags the user has access.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=report-tags-getmany>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: GetReportTagsAsync()
   :module: ClientSDK.Core   

   :rtype: Task<List<ReportDefinitionTag>>

GetReportTagAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves a list of all tags the user has access.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=report-tags-get>`_ 
- **Required Role(s)**: Any
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: GetReportTagAsync(string id)
   :module: ClientSDK.Core   

   :param id: The unique identifier (GUID) of the report definition tag.
   :type id: string

   :rtype: Task<ReportDefinitionTag>

UpdateReportDefinitionTagAsync
^^^^^^^^^^^^^^^^^^^^

Updates a previously created report definition tag.

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-report-v1&operation=report-tags-update`_ 
- **Required Role(s)**: Admin, Support Admin, Super Admin
- **Resource Authorization**: Access to the digital twin associated to the operation that the report definition is associated.

.. method:: UpdateReportDefinitionTagAsync(ReportDefinitionTag reportDefinitionTag)
   :module: ClientSDK.Core   

   :param reportDefinitionTag: The report definition tag object.
   :type reportDefinitionTag: ReportDefinitionTag

   :rtype: Task<ReportDefinitionTag>   

.. autosummary::
   :toctree: generated
  
