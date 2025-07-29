# Changelog
All notable changes to this project will be documented in this file.

## Version 18.3.0 - 2025-07-29
- Added DigitalTwinApi.UpdateManyAsync() to update many digital twins in a single API call

## Version 18.2.0 - 2025-07-07
### Added
- New SpreadsheetApi.GetRowsAsync overload to support changedSince

## Version 18.1.0 - 2025-05-27
### Added
- New SpreadsheetApi.GetRowsAsync overload to support maxCellDataIncldued

## Version 18.0.1 - 2025-05-14
### Updated
- Updated DigitalTwinApi.CreateSpaceAsync to support the new optional parameter: "sortOrder"

## Version 18.0.0 - 2025-05-05
### Added
- UseProtobufModels configuration option
- OneApiHelper class to standardize api communication and implement the new configuration option

### Changed
- Refactored Api classes to use the new IOneApiHelper interface to allow for unit tests
- Updated namespaces to match file strucutre
- Renamed ClientSDK class to OneApi to avoid namespace conflicts
- Changed DigitalTwinApi.CreateManyAsync to accept an IEnumerable<DigitalTwin> instead of a List

## Version 17.11.1 - 2025-02-21
### Updated
- Updated CSharp dependency

## Version 17.11.0 - 2025-01-28
### Added
- Added ConfigurationApi.CreateConfigurationNote2Async to return the created object.
- Added ConfigurationApi.UpdateConfigurationNote2Async to return the updated object.
- Added LogbookApi.CreateLogbookEntry2Async to return the created object.
- Added LogbookApi.UpdateLogbookEntry2Async to return the updated object.

## Version 17.10.0 - 2025-01-28
### Added
- Added ConfigurationApi.GetConfigurationNotesModifiedSinceUtcAsync to get configuration notes modified since a specified date and time.
- Added LogbookApi.GetModifiedSinceUtcAsync to get entries modified since a specified date and time.

### Updated
- Updated ConfigurationApi.CreateConfigurationNoteAsync to support specifying audit information to save with a configuration note.
- Updated ConfigurationApi.UpdateConfigurationNoteAsync to support specifying audit information to save with a configuration note.
- Updated LogbookApi.CreateLogbookEntryAsync to support specifying audit information to save with a logbook entry.
- Updated LogbookApi.UpdateLogbookEntryAsync to support specifying audit information to save with a logbook entry.

## Version 17.9.0 - 2025-01-20
### Changed
- Update AuthenticationApi to support a custom HTTP client timeout

## Version 17.7.2 - 2024-09-09
### Changed
- Updated SDK to support GetSingleNoteAsync to get single note by configurationid and noteid

## Version 17.6.2 - 2024-08-29
### Changed
- Updated LogbookApi.CreateLogbookEntryAsync to support the ExternalSourceId property.

## Version 17.6.1 - 2024-08-16
### Changed
- Updated LogbookApi.UpdateLogbookEntryAsync to support the ExternalSourceId property.

## Version 17.6.0 - 2024-08-12
### Added
- Added LogbookApi.ImportLogbookEntryAsync. This API supports importing multiple configuration notes

## Version 17.2.0 - 2024-01-18
### Added
- Added DigitalTwinApi.CreateManyAsync. This API supports creating multiple digital twins in a single request.

## Version 17.1.0 - 2024-01-03
### Changed
- Updated ActivityApi.GetActivitiesAsync method to include new optional parameters: descendantTwinType & activeActivitiesOnly.

## Version 17.0.0 - 2023-11-28
### Changed
- Updated dependencies: CSharp 5.0.1, Helpers 1.3.3
### Removed
- Usages to obsolete EnumLogLevel
