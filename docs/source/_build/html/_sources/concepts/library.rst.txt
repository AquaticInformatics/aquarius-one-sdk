Global Library
====

Provides a global standard set of Water Quality Specific Parameters, Units, Quantity Types, Agencies, Analysis Methods, and the means to map different standards. 
In addition all library items are localized into 24 languages. Additional application localization support is also provided.

Translations (i18n)
-----

Supported languages
^^^^^^
Currently Translations are typically provided for the following:

- Bulgarian (BG)
- Chinese (CN)
- Croatian (HR)
- Czech (CZ)
- Danish (DK)
- Dutch (NL)
- English American (EN-US)
- Great Britain (EN-GB)
- French (FR)
- German (DE)
- Greek (GR)
- Hungarian (HU)
- Italian (IT)
- Japanese (JP)
- Korean (KR)
- Polish (PL)
- Portuguese (PT)
- Romanian (RO)
- Russian (RU)
- Serbian (RS)
- Slovak (SK)
- Slovenian (SI)
- Spanish (ES)
- Swedish (SE)
- Turkish (TR)

Modules
^^^^^^

Translation Keys are grouped into Modules. The current defined modules are:

- **AQI_FOUNDATION_LIBRARY**: Contains translations for items in Common Library
- **AQI_WEB_CLIENT**: Contains translations for Angular Web Clients including Common UI Modules
- **AQI_EMAIL_TEMPLATE**: Contains email templates needed for Enterprise Authorization
- **AQI_MOBILE_RIO**: Contains translations for the Aquatic Compliance and Operations Mobile Data Collection Client

Quantity Types
-----

Physical quantities are those quantities that can be measured. Basically, there are two types of physical quantities (Base quantities or fundamental quantities) and (Derived quantities).
These are quantities that are used to describe the laws of physics. Physical quantities may be divided into six categories.

Units
-----

Units are the specific instance of a quantity type. For example for the quantity type of Mass, units could be: pounds, kilograms, etc.
All Units have quantity types.

Parameters
-----

Parameters are the common identifier of what is being measured. As an example, Flow.
Parameters have an associated Base Unit and quantity type.

Agencies
-----

Agencies are used as a source of authority for Methods, Unit and Parameter Codes.

Example Agencies:

- UGGS
- EPA
- Hach
- Aquatic Informatics

Agency Parameter Code Types
-----

Agency Parameter Codes
-----

Agency Unit Types
-----

Agency Unity Codes
-----

.. autosummary::
   :toctree: generated
  
