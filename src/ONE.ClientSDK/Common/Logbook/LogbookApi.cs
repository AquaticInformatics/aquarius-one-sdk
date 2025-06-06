﻿using ONE.ClientSDK.Common.Configuration;
using ONE.Models.CSharp;
using ONE.Shared.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Proto = ONE.Models.CSharp;
// ReSharper disable UnusedMember.Global

namespace ONE.ClientSDK.Common.Logbook
{
	public class LogbookApi
	{
		private readonly ConfigurationApi _configurationApi;

		private const string LogbookTypeId = "c3f5b20a-62d3-4dd2-8430-c0ead8d70a59";

		public LogbookApi(ConfigurationApi configurationApi)
		{
			_configurationApi = configurationApi;
		}

		/// <summary>
		/// Create a named logbook that is associated to a particular operation.
		/// </summary>
		/// <param name="name">Name of the logbook</param>
		/// <param name="operationId">Identifier of the operation to which the logbook is to be associated</param>
		/// <param name="isPublic">IsPublic flag for the logbook</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the logbook was successfully created</returns>
		public async Task<bool> CreateLogbookAsync(string operationId, string name, bool isPublic, CancellationToken cancellation = default)
		{
			var logbook = new Proto.Configuration
				{ Name = name, AuthTwinRefId = operationId, ConfigurationData = "{}", ConfigurationTypeId = LogbookTypeId, IsPublic = isPublic };

			return await _configurationApi.CreateConfigurationAsync(logbook, cancellation);
		}

		/// <summary>
		/// Get logbooks associated to a particular operation
		/// </summary>
		/// <param name="operationId">Identifier of the operation with which the logbooks are associated</param>
		/// <param name="cancellation"></param>
		/// <returns>A list of logbooks</returns>
		public async Task<List<Proto.Configuration>> GetLogbooksAsync(string operationId, CancellationToken cancellation = default)
			=> await _configurationApi.GetConfigurationsAsync(LogbookTypeId, operationId, cancellation: cancellation);

		/// <summary>
		/// Update the name or isPublic flag on a logbook.
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook to be edited</param>
		/// <param name="name">New name of the logbook</param>
		/// <param name="isPublic">New isPublic flag for the logbook</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the logbook was successfully updated</returns>
		public async Task<bool> UpdateLogbookAsync(string logbookId, string name, bool isPublic, CancellationToken cancellation = default)
		{
			// Need to get the existing logbook so we can maintain privileges and other properties that are not editable via this method.
			var existingLogbook = await _configurationApi.GetConfigurationAsync(logbookId, cancellation: cancellation);

			if (existingLogbook == null)
				return false;

			existingLogbook.Name = name;
			existingLogbook.IsPublic = isPublic;
			
			return await _configurationApi.UpdateConfigurationAsync(existingLogbook, cancellation);
		}

		/// <summary>
		/// Deletes all notes in a logbook then deletes the logbook.
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook to be deleted</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the logbook was successfully deleted</returns>
		public async Task<bool> DeleteLogbookAsync(string logbookId, CancellationToken cancellation = default)
		{
			await _configurationApi.DeleteConfigurationNotesAsync(logbookId, "all", cancellation);
			return await _configurationApi.DeleteConfigurationAsync(logbookId, cancellation);
		}

		/// <summary>
		/// Retrieves the entryTime of the latest entry for each logbook available to the user on the provided operation.
		/// If there is an error an empty dictionary will be returned.
		/// </summary>
		/// <param name="operationId">Identifier of the operation that contains the logbooks being queried</param>
		/// <param name="cancellation"></param>
		/// <returns>Dictionary containing the logbookId and dateTime of the last entry in that logbook</returns>
		public async Task<Dictionary<string, DateTime>> GetEntryTimeOfLatestEntriesAsync(string operationId, CancellationToken cancellation = default) =>
			(await _configurationApi.GetConfigurationNotesLastAsync(LogbookTypeId, operationId, cancellation))?.ToDictionary(n => n.ConfigurationId, v => v.NoteTime.ToDateTime()) ??
			new Dictionary<string, DateTime>();

		/// <summary>
		/// Retrieves the latest entry for each logbook available to the user on the provided operation.
		/// If there is an error an empty dictionary will be returned.
		/// </summary>
		/// <param name="operationId">Identifier of the operation that contains the logbooks being queried</param>
		/// <param name="cancellation"></param>
		/// <returns>Dictionary containing the logbookId and the last entry in that logbook</returns>
		public async Task<Dictionary<string, ConfigurationNote>> GetLatestEntriesAsync(string operationId, CancellationToken cancellation = default) =>
			(await _configurationApi.GetConfigurationNotesLastAsync(LogbookTypeId, operationId, cancellation))?.ToDictionary(n => n.ConfigurationId, v => v) ??
			new Dictionary<string, ConfigurationNote>();

		/// <summary>
		/// Retrieves the logbook entries that have been modified since the specified UTC time.
		/// </summary>
		/// <param name="logbookId">Identifies the logbook</param>
		/// <param name="modifiedSinceUtc">The UTC time to compare to the modified time of logbook entries</param>
		/// <param name="cancellation"></param>
		/// <returns>A list of logbook entries that have been modified since the provided UTC time</returns>
		public async Task<List<ConfigurationNote>> GetModifiedSinceUtcAsync(string logbookId, DateTime modifiedSinceUtc, CancellationToken cancellation = default)
			=> await _configurationApi.GetConfigurationNotesModifiedSinceUtcAsync(logbookId, modifiedSinceUtc, cancellation);

		/// <summary>
		/// Retrieves unique tags associated with a specific logbook, if there are no tags on a logbook or there is an error an empty list will be returned.
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook for which to retrieve tags</param>
		/// <param name="cancellation"></param>
		/// <returns>A list of strings that are tags associated to the provided logbook</returns>
		public async Task<IEnumerable<string>> GetUniqueTagsAsync(string logbookId, CancellationToken cancellation = default) =>
			(await _configurationApi.GetConfigurationTagsAsync(logbookId, cancellation))?.Select(t => t.Tag) ?? new List<string>();

		/// <summary>
		/// Create an entry in a logbook
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook in which to create the entry</param>
		/// <param name="entry">Text of the entry to be created</param>
		/// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
		/// <param name="geoPointX">Optional - the WGS84 longitude of the location of this entry</param>
		/// <param name="geoPointY">Optional - the WGS84 latitude of the location of this entry</param>
		/// <param name="tags">Any tags that should be associated to the entry, no spaces allowed</param>
		/// <returns>Boolean indicating whether the logbookEntry was successfully created</returns>
		public async Task<bool> CreateLogbookEntryAsync(string logbookId, string entry, DateTime entryTime,
			float? geoPointX = null, float? geoPointY = null, params string[] tags)
		{
			return await CreateLogbookEntryAsync(logbookId, entry, entryTime, externalSourceId: null,
				externalUsername: null, geoPointX, geoPointY, recordAuditInfo: null, tags);
		}

		/// <summary>
		/// Create an entry in a logbook
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook in which to create the entry</param>
		/// <param name="entry">Text of the entry to be created</param>
		/// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
		/// <param name="externalSourceId">The externalSourceId</param>
		/// <param name="externalUsername">The externalUsername</param>
		/// <param name="geoPointX">Optional - the WGS84 longitude of the location of this entry</param>
		/// <param name="geoPointY">Optional - the WGS84 latitude of the location of this entry</param>
		/// <param name="recordAuditInfo">Optional (defaults to null) - the audit information to save with the entry</param>
		/// <param name="tags">Any tags that should be associated to the entry, no spaces allowed</param>
		/// <returns>Boolean indicating whether the logbookEntry was successfully created</returns>
		public async Task<bool> CreateLogbookEntryAsync(string logbookId, string entry, DateTime entryTime,
			string externalSourceId, string externalUsername, float? geoPointX = null, 
			float? geoPointY = null, RecordAuditInfo recordAuditInfo = null, params string[] tags)
		{
			var result = await CreateLogbookEntry2Async(logbookId, entry, entryTime, externalSourceId,
				externalUsername, geoPointX, geoPointY, recordAuditInfo, tags);

			return result != null;
		}

		/// <summary>
		/// Create an entry in a logbook
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook in which to create the entry</param>
		/// <param name="entry">Text of the entry to be created</param>
		/// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
		/// <param name="externalSourceId">The externalSourceId</param>
		/// <param name="externalUsername">The externalUsername</param>
		/// <param name="geoPointX">Optional - the WGS84 longitude of the location of this entry</param>
		/// <param name="geoPointY">Optional - the WGS84 latitude of the location of this entry</param>
		/// <param name="recordAuditInfo">Optional (defaults to null) - the audit information to save with the entry</param>
		/// <param name="tags">Any tags that should be associated to the entry, no spaces allowed</param>
		/// <returns>Boolean indicating whether the logbookEntry was successfully created</returns>
		public async Task<ConfigurationNote> CreateLogbookEntry2Async(string logbookId, string entry, 
			DateTime entryTime, string externalSourceId, string externalUsername, float? geoPointX = null,
			float? geoPointY = null, RecordAuditInfo recordAuditInfo = null, params string[] tags)
		{
			var logbookEntry = new ConfigurationNote
			{
				ConfigurationId = logbookId,
				Note = entry,
				NoteTime = entryTime.ToOneDateTime(),
				ExternalSourceId = externalSourceId,
				ExternalUsername = externalUsername,
				RecordAuditInfo = recordAuditInfo,
				Tags = { tags.Select(t => new ConfigurationTag { Tag = t }) }
			};

			if (geoPointX != null && geoPointY != null)
			{
				logbookEntry.Geography = new GIS
				{
					Point2D = new Point2D
					{
						X = geoPointX.Value,
						Y = geoPointY.Value
					}
				};
			}

			return await _configurationApi.CreateConfigurationNote2Async(logbookEntry);
		}

		/// <summary>
		/// Retrieves entries belonging to a specific logbook for a specified time span.  Results can be further filtered using the tagString and noteContains parameters.
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook for which to retrieve entries</param>
		/// <param name="startDate">Only retrieve logbookEntries with a noteTime on or after the provided startDate</param>
		/// <param name="endDate">Only retrieve logbookEntries with a noteTime before the provided endDate</param>
		/// <param name="tagString">Space delimited string used to filter logbookEntries, entries must contain all the provided tags, this is case-insensitive.</param>
		/// <param name="noteContains">String used to filter logbookEntries to only those containing the provided text, this is case-insensitive.</param>
		/// <param name="cancellation"></param>
		/// <returns>A list of logbookEntries matching the filter parameters</returns>
		public async Task<List<ConfigurationNote>> GetLogbookEntriesAsync(string logbookId, DateTime startDate, DateTime endDate, string tagString = "",
			string noteContains = "", CancellationToken cancellation = default) => await _configurationApi.GetConfigurationNotesAsync(logbookId, startDate, endDate, tagString, noteContains, cancellation);

		/// <summary>
		/// Edit an entry in a logbook
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook in which to update the entry</param>
		/// <param name="entryId">Identifier of the entry to be edited</param>
		/// <param name="entry">Text of the entry to be edited, this text replaces any existing entry text</param>
		/// /// <param name="externalSourceId">Value used to associate the note with external data</param>
		/// <param name="externalUsername">Value used to associate the note with an external user</param>
		/// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
		/// <param name="geoPointX">Optional - the WGS84 longitude of the location of this entry</param>
		/// <param name="geoPointY">Optional - the WGS84 latitude of the location of this entry</param>
		/// <param name="recordAuditInfo">Audit information to save</param>
		/// <param name="tags">Any tags that should be associated to the entry, no spaces allowed, this list replaces any existing tags</param>
		/// <returns>Boolean value indicating whether the logbookEntry was successfully updated</returns>
		public async Task<bool> UpdateLogbookEntryAsync(string logbookId, string entryId, string entry, 
			string externalSourceId, string externalUsername, DateTime entryTime, float? geoPointX = null,
			float? geoPointY = null, RecordAuditInfo recordAuditInfo = null, params string[] tags)
		{
			var result = await UpdateLogbookEntry2Async(logbookId, entryId, entry, externalSourceId,
				externalUsername, entryTime, geoPointX, geoPointY, recordAuditInfo, tags);

			return result != null;
		}

		/// <summary>
		/// Edit an entry in a logbook
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook in which to update the entry</param>
		/// <param name="entryId">Identifier of the entry to be edited</param>
		/// <param name="entry">Text of the entry to be edited, this text replaces any existing entry text</param>
		/// /// <param name="externalSourceId">Value used to associate the note with external data</param>
		/// <param name="externalUsername">Value used to associate the note with an external user</param>
		/// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
		/// <param name="geoPointX">Optional - the WGS84 longitude of the location of this entry</param>
		/// <param name="geoPointY">Optional - the WGS84 latitude of the location of this entry</param>
		/// <param name="recordAuditInfo">Audit information to save</param>
		/// <param name="tags">Any tags that should be associated to the entry, no spaces allowed, this list replaces any existing tags</param>
		/// <returns>Boolean value indicating whether the logbookEntry was successfully updated</returns>
		public async Task<ConfigurationNote> UpdateLogbookEntry2Async(string logbookId, string entryId, string entry,
			string externalSourceId, string externalUsername, DateTime entryTime, float? geoPointX = null,
			float? geoPointY = null, RecordAuditInfo recordAuditInfo = null, params string[] tags)
		{
			var logbookEntry = new ConfigurationNote
			{
				Id = entryId,
				ConfigurationId = logbookId,
				Note = entry,
				NoteTime = entryTime.ToOneDateTime(),
				Tags = { tags.Select(t => new ConfigurationTag { Tag = t }) },
				ExternalSourceId = externalSourceId,
				ExternalUsername = externalUsername,
				RecordAuditInfo = recordAuditInfo
			};

			if (geoPointX != null && geoPointY != null)
			{
				logbookEntry.Geography = new GIS
				{
					Point2D = new Point2D
					{
						X = geoPointX.Value,
						Y = geoPointY.Value
					}
				};
			}

			return await _configurationApi.UpdateConfigurationNote2Async(logbookEntry);
		}

		/// <summary>
		/// Deletes a single entry from a logbook, to delete all entries use 'all' as the entryId or use
		/// <see cref="DeleteLogbookAsync"/> which will delete all entries and the logbook.
		/// </summary>
		/// <param name="logbookId">Identifier of the logbook containing the entry to be deleted</param>
		/// <param name="entryId">Identifier of the entry to be deleted or 'all' to delete all entries in a logbook</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the entry or entries were successfully deleted</returns>
		public async Task<bool> DeleteLogbookEntryAsync(string logbookId, string entryId, CancellationToken cancellation = default)
			=> await _configurationApi.DeleteConfigurationNotesAsync(logbookId, entryId, cancellation);

		/// <summary>
		/// Import multiple notes
		/// <see>
		///     <cref>ImportConfigurationNotesAsync</cref>
		/// </see>
		/// which add import notes.
		/// </summary>
		/// <param name="configurationNotes">configurationNotes is the notes that is to be imported into the import notes api</param>
		/// <param name="cancellation"></param>
		/// <returns>Boolean value indicating whether the entry or entries were successfully imported</returns>
		public async Task<bool> ImportLogbookEntryAsync(ConfigurationNotes configurationNotes, CancellationToken cancellation = default)
			=> await _configurationApi.ImportConfigurationNotesAsync(configurationNotes, cancellation);

		/// <summary>
		/// get single note
		/// <see>
		///     <cref>GetSingleConfigurationNoteAsync</cref>
		/// </see>
		/// which get single note using configurationId and noteId.
		/// </summary>
		/// <param name="configurationId">Identifies the configuration to be fetched</param>
		/// <param name="noteId">Identifies the note that should be fetched</param>
		/// <param name="cancellation"></param>
		/// <returns>ConfigurationNote</returns>
		public async Task<ConfigurationNote> GetSingleConfigurationNoteAsync(string configurationId, string noteId, CancellationToken cancellation = default)
			=> await _configurationApi.GetSingleConfigurationNoteAsync(configurationId, noteId, cancellation);
	}
}
