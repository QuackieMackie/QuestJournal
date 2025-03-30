using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal;

public class QuestDataFetcher(IDataManager dataManager, IPluginLog log)
{
    private readonly Lazy<ExcelSheet<Quest>?> questSheet = new(() => dataManager.GetExcelSheet<Quest>());
    
    private readonly QuestFetcherUtils questFetcherUtils = new(dataManager, log);

    private ExcelSheet<Quest>? QuestSheet => questSheet.Value;
    
    public List<QuestModel> GetAllQuests()
    {
        Debug.Assert(QuestSheet != null, nameof(QuestSheet) + " != null");
        var questInfoLookup = QuestSheet.ToDictionary(
            quest => quest.RowId,
            questFetcherUtils.BuildQuestInfo
        );

        foreach (var quest in QuestSheet)
        {
            if (questInfoLookup.TryGetValue(quest.RowId, out _) && quest.PreviousQuest.Count > 0)
            {
                var prevQuestIds = questFetcherUtils.GetPrerequisiteQuestIds(quest.PreviousQuest);

                foreach (var prevQuestId in prevQuestIds)
                {
                    if (questInfoLookup.TryGetValue(prevQuestId, out var prevQuestInfo))
                    {
                        prevQuestInfo?.NextQuestIds?.Add(quest.RowId);
                        prevQuestInfo?.NextQuestTitles?.Add(quest.Name.ExtractText());
                    }
                }
            }
        }
        
        return questInfoLookup.Values.Where(quest => quest != null).Select(quest => quest!).ToList();
    }

    public Dictionary<string, List<QuestModel>> GetMainScenarioQuestsByCategory()
    {
        var allQuests = GetAllQuests();
        var msqCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Seventh Umbral Era Main Scenario Quests",
            "Seventh Astral Era Main Scenario Quests",
            "Heavensward Main Scenario Quests",
            "Dragonsong Main Scenario Quests",
            "Post-Dragonsong Main Scenario Quests",
            "Stormblood Main Scenario Quests",
            "Post-Stormblood Main Scenario Quests",
            "Shadowbringers Main Scenario Quests",
            "Post-Shadowbringers Main Scenario Quests",
            "Post-Shadowbringers Main Scenario Quests II",
            "Endwalker Main Scenario Quests",
            "Post-Endwalker Main Scenario Quests",
            "Dawntrail Main Scenario Quests",
            "Post-Dawntrail Main Scenario Quests"
        };

        var categorizedQuests = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);

        foreach (var category in msqCategories)
        {
            categorizedQuests[category] = new List<QuestModel>();
        }

        foreach (var quest in allQuests)
        {
            var categoryName = quest.JournalGenre?.JournalCategory?.Name;
            if (categoryName != null && msqCategories.Contains(categoryName))
            {
                categorizedQuests[categoryName].Add(quest);
            }
        }

        return categorizedQuests;
    }
    
    public Dictionary<string, List<QuestModel>> GetJobQuestsByCategory()
    {
        var allQuests = GetAllQuests();

        var jobCategoryIds = new[]
        {
            84, // "Disciple of War Quests"
            85, // "Disciple of Magic Quests"
            86, // "Disciple of the Hand Quests"
            87, // "Disciple of the Land Quests"
            91, // "Disciple of the War Job Quests"
            92, // "Disciple of the Magic Job Quests"
        };

        var categorizedQuests = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            if (category != null && jobCategoryIds.Contains((int) category.Id))
            {
                var journalGenreName = quest.JournalGenre?.Name;
                if (!string.IsNullOrEmpty(journalGenreName))
                {
                    if (!categorizedQuests.ContainsKey(journalGenreName))
                    {
                        categorizedQuests[journalGenreName] = new List<QuestModel>();
                    }

                    categorizedQuests[journalGenreName].Add(quest);
                }
            }
        }

        return categorizedQuests;
    }

    public Dictionary<string, Dictionary<string, List<QuestModel>>> GetFeatureQuestsByCategory()
    {
        var allQuests = GetAllQuests();
        
        var categorizedQuests = new Dictionary<string, Dictionary<string, List<QuestModel>>>(StringComparer.OrdinalIgnoreCase);
        var categoryFolders = new Dictionary<int, string>
        {
            { 88, "Crystarium Deliveries" }, // "Crystalline Mean Quests"
        };

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            if (category != null && categoryFolders.TryGetValue((int)category.Id, out var folderName))
            {
                var journalGenreName = quest.JournalGenre?.Name;
                if (!string.IsNullOrEmpty(journalGenreName))
                {
                    if (!categorizedQuests.ContainsKey(folderName))
                    {
                        categorizedQuests[folderName] = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);
                    }

                    if (!categorizedQuests[folderName].ContainsKey(journalGenreName))
                    {
                        categorizedQuests[folderName][journalGenreName] = new List<QuestModel>();
                    }

                    categorizedQuests[folderName][journalGenreName].Add(quest);
                }
            }
        }

        return categorizedQuests;
    }
}
