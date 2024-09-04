# Changelog
All notable changes to this project will be documented in this file.

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
