﻿using ONE.Common.Configuration;
using ONE.Models.CSharp;
using ONE.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto = ONE.Models.CSharp;

namespace ONE.Common.Logbook
{
    public class LogbookApi
    {
        private readonly ConfigurationApi _configurationApi;

        private const string LogbookTypeId = "c3f5b20a-62d3-4dd2-8430-c0ead8d70a59";

        public LogbookApi(PlatformEnvironment environment, bool continueOnCapturedContext, RestHelper restHelper)
        {
            _configurationApi = new ConfigurationApi(environment, continueOnCapturedContext, restHelper);
        }

        /// <summary>
        /// Create a named logbook that is associated to a particular operation.
        /// </summary>
        /// <param name="name">Name of the logbook</param>
        /// <param name="operationId">Identifier of the operation to which the logbook is to be associated</param>
        /// <param name="isPublic">IsPublic flag for the logbook</param>
        /// <returns>Boolean value indicating whether or not the logbook was successfully created</returns>
        public async Task<bool> CreateLogbookAsync(string operationId, string name, bool isPublic)
        {
            var logbook = new Proto.Configuration
                { Name = name, AuthTwinRefId = operationId, ConfigurationData = "{}", ConfigurationTypeId = LogbookTypeId, IsPublic = isPublic };

            return await _configurationApi.CreateConfigurationAsync(logbook, 0);
        }

        /// <summary>
        /// Get logbooks associated to a particular operation
        /// </summary>
        /// <param name="operationId">Identifier of the operation with which the logbooks are associated</param>
        /// <returns>A list of logbooks</returns>
        public async Task<List<Proto.Configuration>> GetLogbooksAsync(string operationId) => await _configurationApi.GetConfigurationsAsync(LogbookTypeId, operationId);

        /// <summary>
        /// Update the name or isPublic flag on a logbook.
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook to be edited</param>
        /// <param name="name">New name of the logbook</param>
        /// <param name="isPublic">New isPublic flag for the logbook</param>
        /// <returns>Boolean value indicating whether or not the logbook was successfully updated</returns>
        public async Task<bool> UpdateLogbookAsync(string logbookId, string name, bool isPublic)
        {
            // Need to get the existing logbook so we can maintain privileges and other properties that are not editable via this method.
            var existingLogbook = await _configurationApi.GetConfigurationAsync(logbookId);

            if (existingLogbook == null)
                return false;

            existingLogbook.Name = name;
            existingLogbook.IsPublic = isPublic;
            
            return await _configurationApi.UpdateConfigurationAsync(existingLogbook, 0);
        }

        /// <summary>
        /// Deletes all notes in a logbook then deletes the logbook.
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook to be deleted</param>
        /// <returns>Boolean value indicating whether or not the logbook was successfully deleted</returns>
        public async Task<bool> DeleteLogbookAsync(string logbookId)
        {
            await _configurationApi.DeleteConfigurationNotesAsync(logbookId, "all");
            return await _configurationApi.DeleteConfigurationAsync(logbookId);
        }

        /// <summary>
        /// Retrieves the entryTime of the latest entry for each logbook available to the user on the provided operation.
        /// If there is an error an empty dictionary will be returned.
        /// </summary>
        /// <param name="operationId">Identifier of the operation that contains the logbooks being queried</param>
        /// <returns>Dictionary containing the logbookId and dateTime of the last entry in that logbook</returns>
        public async Task<Dictionary<string, DateTime>> GetEntryTimeOfLatestEntriesAsync(string operationId) =>
            (await _configurationApi.GetConfigurationNotesLastAsync(LogbookTypeId, operationId))?.ToDictionary(n => n.ConfigurationId, v => v.NoteTime.ToDateTime()) ??
            new Dictionary<string, DateTime>();

        /// <summary>
        /// Retrieves the latest entry for each logbook available to the user on the provided operation.
        /// If there is an error an empty dictionary will be returned.
        /// </summary>
        /// <param name="operationId">Identifier of the operation that contains the logbooks being queried</param>
        /// <returns>Dictionary containing the logbookId and the last entry in that logbook</returns>
        public async Task<Dictionary<string, ConfigurationNote>> GetLatestEntriesAsync(string operationId) =>
            (await _configurationApi.GetConfigurationNotesLastAsync(LogbookTypeId, operationId))?.ToDictionary(n => n.ConfigurationId, v => v) ??
            new Dictionary<string, ConfigurationNote>();

        /// <summary>
        /// Retrieves unique tags associated with a specific logbook, if there are no tags on a logbook or there is an error an empty list will be returned.
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook for which to retrieve tags</param>
        /// <returns>A list of strings that are tags associated to the provided logbook</returns>
        public async Task<IEnumerable<string>> GetUniqueTagsAsync(string logbookId) =>
            (await _configurationApi.GetConfigurationTagsAsync(logbookId))?.Select(t => t.Tag) ?? new List<string>();

        /// <summary>
        /// Create an entry in a logbook
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook in which to create the entry</param>
        /// <param name="entry">Text of the entry to be created</param>
        /// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
        /// <param name="tags">Any tags that should be associated to the entry, no spaces allowed</param>
        /// <returns>Boolean indicating whether or not the logbookEntry was successfully created</returns>
        public async Task<bool> CreateLogbookEntryAsync(string logbookId, string entry, DateTime entryTime, params string[] tags)
        {
            var logbookEntry = new ConfigurationNote
                { ConfigurationId = logbookId, Note = entry, NoteTime = entryTime.ToJsonTicksDateTime(), Tags = { tags.Select(t => new ConfigurationTag { Tag = t }) } };

            return await _configurationApi.CreateConfigurationNoteAsync(logbookEntry);
        }

        /// <summary>
        /// Retrieves entries belonging to a specific logbook for a specified time span.  Results can be further filtered using the tagString and noteContains parameters.
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook for which to retrieve entries</param>
        /// <param name="startDate">Only retrieve logbookEntries with a noteTime on or after the provided startDate</param>
        /// <param name="endDate">Only retrieve logbookEntries with a noteTime before the provided endDate</param>
        /// <param name="tagString">Space delimited string used to filter logbookEntries, entries must contain all of the provided tags, this is case insensitive.</param>
        /// <param name="noteContains">String used to filter logbookEntries to only those containing the provided text, this is case insensitive.</param>
        /// <returns>A list of logbookEntries matching the filter parameters</returns>
        public async Task<List<ConfigurationNote>> GetLogbookEntriesAsync(string logbookId, DateTime startDate, DateTime endDate, string tagString = "",
            string noteContains = "") => await _configurationApi.GetConfigurationNotesAsync(logbookId, startDate, endDate, tagString, noteContains);

        /// <summary>
        /// Edit an entry in a logbook
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook in which to update the entry</param>
        /// <param name="entryId">Identifier of the entry to be edited</param>
        /// <param name="entry">Text of the entry to be edited, this text replaces any existing entry text</param>
        /// <param name="entryTime">Timestamp to be associated to the entry, should be in UTC</param>
        /// <param name="tags">Any tags that should be associated to the entry, no spaces allowed, this list replaces any existing tags</param>
        /// <returns>Boolean value indicating whether or not the logbookEntry was successfully updated</returns>
        public async Task<bool> UpdateLogbookEntryAsync(string logbookId, string entryId, string entry, DateTime entryTime, params string[] tags)
        {
            var logbookEntry = new ConfigurationNote
            {
                Id = entryId, ConfigurationId = logbookId, Note = entry, NoteTime = entryTime.ToJsonTicksDateTime(), Tags = { tags.Select(t => new ConfigurationTag { Tag = t }) }
            };

            return await _configurationApi.UpdateConfigurationNoteAsync(logbookEntry);
        }

        /// <summary>
        /// Deletes a single entry from a logbook, to delete all entries use 'all' as the entryId or use
        /// <see cref="DeleteLogbookAsync"/> which will delete all entries and the logbook.
        /// </summary>
        /// <param name="logbookId">Identifier of the logbook containing the entry to be deleted</param>
        /// <param name="entryId">Identifier of the entry to be deleted or 'all' to delete all entries in a logbook</param>
        /// <returns>Boolean value indicating whether or not the entry or entries were successfully deleted</returns>
        public async Task<bool> DeleteLogbookEntryAsync(string logbookId, string entryId) => await _configurationApi.DeleteConfigurationNotesAsync(logbookId, entryId);
    }
}
