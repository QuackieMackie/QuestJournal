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

        List<int> glamourQuestIds = new()
        {
            66746, // "Beauty Is Only Scalp Deep"
            66235, // "Color Your World"
            67094, // "Simply to Dye For"
            68553, // "If I Had a Glamour"
            68554, // "Absolutely Glamourous"
            66957, // "A Self-improving Man"
            66958, // "Submission Impossible"
            66174, // "Forging the Spirit"
            66175, // "Waking the Spirit"
            66999, // "Marvelously Mutable Materia"
            66176, // "Melding Materia Muchly"
            70300, // "A Faerie Tale Come True"
            67896, // "An Egi by Any Other Name"
            70723, // "Bottled Fantasy"
        };

        // Mapping: Identifier (CategoryId or QuestIdList), Folder Name, Grouping Logic
        // Grouping Logic:
        // 0: Use JournalGenre.Name
        // 1: Use JournalCategory.Name
        // 2: Use only FolderName directly
        var categoryFolders = new List<(object Identifier, string FolderName, int GroupBy)>
        {
            (88, "Crystarium Deliveries", 0), // "Crystalline Mean Quests"
    
            (31, "Tribe Quests", 1), // Mamool Ja Quests ?? maybe
            (32, "Tribe Quests", 1), // Pelupelu Quests
            (33, "Tribe Quests", 1), // Intersocietal Quests
            (34, "Tribe Quests", 1), // Amaj'aa Quests
            (35, "Tribe Quests", 1), // Sylph Quests
            (36, "Tribe Quests", 1), // Kobold Quests
            (37, "Tribe Quests", 1), // Sahagin Quests
            (38, "Tribe Quests", 1), // Ixal Quests
            (39, "Tribe Quests", 1), // Vanu Vanu Quests
            (40, "Tribe Quests", 1), // Vath Quests
            (41, "Tribe Quests", 1), // Moogle Quests
            (42, "Tribe Quests", 1), // Kojin Quests
            (43, "Tribe Quests", 1), // Ananta Quests
            (44, "Tribe Quests", 1), // Namazu Quests
            (45, "Tribe Quests", 1), // Pixie Quests
            (46, "Tribe Quests", 1), // Qitari Quests
            (47, "Tribe Quests", 1), // Dwarf Quests
            (48, "Tribe Quests", 1), // Arkasodara Quests
            (49, "Tribe Quests", 1), // Omnicron Quests
            (50, "Tribe Quests", 1), // Loporrit Quests
            (51, "Tribe Quests", 1), // Intersocietal Quests
            
            (glamourQuestIds, "Glamour and Customization", 2),
        };

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            foreach (var (identifier, folderName, groupBy) in categoryFolders)
            {
                bool matches = false;

                if (identifier is int categoryId)
                {
                    matches = category != null && category.Id == categoryId;
                }
                else if (identifier is List<int> questIds)
                {
                    matches = questIds.Contains((int)quest.QuestId);
                }

                if (matches)
                {
                    string subFolderName = groupBy switch
                    {
                        0 => quest.JournalGenre?.Name ?? "Unknown Genre", // Group by JournalGenre.Name
                        1 => quest.JournalGenre?.JournalCategory?.Name ?? "Unknown Category", // Group by JournalCategory.Name
                        _ => folderName // Use FolderName directly
                    };

                    if (!categorizedQuests.ContainsKey(folderName))
                    {
                        categorizedQuests[folderName] = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);
                    }

                    if (!categorizedQuests[folderName].ContainsKey(subFolderName))
                    {
                        categorizedQuests[folderName][subFolderName] = new List<QuestModel>();
                    }

                    categorizedQuests[folderName][subFolderName].Add(quest);
                    break;
                }
            }
        }

        return categorizedQuests;
    }
}
