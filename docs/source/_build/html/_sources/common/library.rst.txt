Library APIs
====

Provides a global standard set of Water Quality Specific Parameters, Units, Quantity Types, Agencies, Analysis Methods, and the means to map different standards. 
In addition all library items are localized into 24 languages. Additional application localization support is also provided.

Agency Methods
---------

Agency Parameter Code Type Methods
---------

Agency Parameter Code Methods
---------

Agency Unit Type Methods
---------

Agency Unit Code Methods
---------


  
Parameter Methods
---------

Quantity Type Methods
---------

GetQuantityTypesAsync
^^^^^^^^^^^^^^^^^^^^

Retrieves all quantity types. 

- `RESTful API Documentation <https://aqi-feature-api-mgmt.developer.azure-api.net/api-details#api=claros-enterprise-twin-v1&operation=GetDigitalTwinSubtypes>`_ 
- See Also: :doc:`/concepts/library` 
- **Required Role(s)**: Any
- User must be authenticated

.. method:: GetQuantityTypesAsync()
   :module: ClientSDK.Library

   :returns: A list of all of the QuantityTypes.
   :rtype: Task<List<QuantityType>>

Translation (i18n) Methods
---------

Unit Type Methods
---------


.. autosummary::
   :toctree: generated