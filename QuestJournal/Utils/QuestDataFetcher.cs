using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public class QuestDataFetcher
{
    private readonly IDataManager dataManager;
    private readonly IPluginLog log;

    public QuestDataFetcher(IDataManager dataManager, IPluginLog log)
    {
        this.dataManager = dataManager;
        this.log = log;
    }

    // Main Public Methods

    /// <summary>
    ///     Fetches all quests and their details.
    /// </summary>
    public List<IQuestInfo> GetAllQuests()
    {
        var questList = new List<IQuestInfo>();

        var exVersionSheet = dataManager.GetExcelSheet<ExVersion>();
        var journalGenreSheet = dataManager.GetExcelSheet<JournalGenre>();
        var questSheet = dataManager.GetExcelSheet<Quest>();

        foreach (var questData in questSheet)
        {
            var questInfo = BuildQuestInfo(questData, exVersionSheet, journalGenreSheet);
            if (questInfo == null) continue;

            questList.Add(questInfo);
        }

        return questList;
    }

    /// <summary>
    ///     Saves a list of quests to a JSON file.
    /// </summary>
    public void SaveQuestDataToJson(List<IQuestInfo> questData, string filePath)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(questData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            log.Error($"Failed to save quest data to JSON file at '{filePath}': {ex.Message}");
            throw;
        }
    }

    // IQuestInfo Builders

    /// <summary>
    ///     Builds a IQuestInfo object from raw quest data.
    /// </summary>
    private IQuestInfo? BuildQuestInfo(
        Quest questData, ExcelSheet<ExVersion>? exVersionSheet, ExcelSheet<JournalGenre>? journalGenreSheet)
    {
        try
        {
            var preQuestIds = GetPrerequisiteQuestIds(questData.PreviousQuest);
            var preQuestTitles = GetPrerequisiteQuestTitles(questData.PreviousQuest);

            var expansionName = GetExpansionName(questData.Expansion, exVersionSheet, questData.Id);
            var journalGenreDetails = GetJournalGenreDetails(questData.JournalGenre, journalGenreSheet, questData.Id);

            var questDetails = new IQuestInfo
            {
                QuestId = questData.RowId,
                QuestTitle = questData.Name.ToString(),
                PreviousQuestIds = preQuestIds,
                PreviousQuestTitles = preQuestTitles,
                StarterNpc = ResolveNpcName(questData.IssuerStart, dataManager),
                FinishNpc = ResolveNpcName(questData.TargetEnd, dataManager),
                IsRepeatable = questData.IsRepeatable,
                Expansion = expansionName,
                JournalGenre = journalGenreDetails
            };

            return questDetails;
        }
        catch (Exception ex)
        {
            log.Error($"Failed to build IQuestInfo for QuestId {questData.RowId}: {ex.Message}");
            return null;
        }
    }

    private JournalGenreDetails? GetJournalGenreDetails(
        RowRef<JournalGenre> journalGenreRef, ExcelSheet<JournalGenre>? journalGenreSheet, ReadOnlySeString questId)
    {
        if (journalGenreSheet == null || journalGenreRef.RowId <= 0) return null;

        try
        {
            var journalGenre = journalGenreRef.Value;

            JournalCategoryDetails? journalCategoryDetails = null;
            if (journalGenre.JournalCategory.RowId > 0)
            {
                var journalCategory = journalGenre.JournalCategory.Value;
                journalCategoryDetails = new JournalCategoryDetails
                {
                    Id = journalCategory.RowId,
                    Name = journalCategory.Name.ToString()
                };
            }

            return new JournalGenreDetails
            {
                Id = journalGenre.RowId,
                Icon = journalGenre.Icon,
                Name = journalGenre.Name.ToString(),
                JournalCategory = journalCategoryDetails
            };
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve JournalGenre for Quest {questId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Validates the contents of a IQuestInfo object.
    /// </summary>
    private bool IsQuestInfoValid(IQuestInfo questInfo)
    {
        return questInfo.QuestId > 0 && !string.IsNullOrEmpty(questInfo.QuestTitle) &&
               (!string.IsNullOrEmpty(questInfo.Expansion) || questInfo.JournalGenre != null);
    }

    // Data Parsers

    /// <summary>
    ///     Resolves the IDs of prerequisite quests.
    /// </summary>
    private List<uint> GetPrerequisiteQuestIds(IEnumerable<RowRef<Quest>> previousQuests)
    {
        var ids = new List<uint>();
        foreach (var preQuestRef in previousQuests)
        {
            if (preQuestRef.RowId <= 0) continue;

            try
            {
                var preQuest = preQuestRef.Value;
                ids.Add(preQuest.RowId);
            }
            catch (Exception ex)
            {
                log.Error($"Failed to resolve PreviousQuestId for RowId {preQuestRef.RowId}: {ex.Message}");
            }
        }

        return ids;
    }

    /// <summary>
    ///     Resolves the titles of prerequisite quests.
    /// </summary>
    private List<string> GetPrerequisiteQuestTitles(IEnumerable<RowRef<Quest>> previousQuests)
    {
        var titles = new List<string>();
        foreach (var preQuestRef in previousQuests)
        {
            if (preQuestRef.RowId <= 0) continue;

            try
            {
                var preQuest = preQuestRef.Value;
                titles.Add(preQuest.Name.ToString());
            }
            catch (Exception ex)
            {
                log.Error($"Failed to resolve PreviousQuestTitle for RowId {preQuestRef.RowId}: {ex.Message}");
            }
        }

        return titles;
    }

    /// <summary>
    ///     Resolves the expansion name for the quest.
    /// </summary>
    private string GetExpansionName(
        RowRef<ExVersion> expansionRef, ExcelSheet<ExVersion>? exVersionSheet, ReadOnlySeString questId)
    {
        if (expansionRef.RowId <= 0 || exVersionSheet == null) return "";

        try
        {
            var expansionRow = exVersionSheet.GetRow(expansionRef.RowId);
            return expansionRow.Name.ToString() ?? "";
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve Expansion for Quest {questId}: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    ///     Resolves the journal genre name for the quest.
    /// </summary>
    private string? GetJournalGenreName(
        RowRef<JournalGenre> journalGenreRef, ExcelSheet<JournalGenre>? journalGenreSheet, ReadOnlySeString questId)
    {
        if (journalGenreSheet == null || journalGenreRef.RowId <= 0) return null;

        try
        {
            var journalGenre = journalGenreRef.Value;
            return journalGenre.Name.ToString();
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve JournalGenre for Quest {questId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Resolves the NPC name for the given RowRef.
    /// </summary>
    private string? ResolveNpcName(RowRef npcRef, IDataManager dataManager)
    {
        if (npcRef.RowId == 0) return null;

        try
        {
            var enpcSheet = dataManager.GetExcelSheet<ENpcResident>();
            var npc = enpcSheet.GetRow(npcRef.RowId);
            return npc.Singular.ToString();
        }
        catch (Exception ex)
        {
            log.Error($"Error resolving NPC name for RowId {npcRef.RowId}: {ex.Message}");
            return null;
        }
    }
}
