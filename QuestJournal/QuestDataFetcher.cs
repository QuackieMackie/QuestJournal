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

        List<int> glamourQuestIds =
        [
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
            70723  // "Bottled Fantasy"
        ];

        List<int> locationsQuestIds =
        [
            66749, // "Where the Heart Is (The Goblet)"
            66750, // "Where the Heart Is (Mist)"
            66748, // "Where the Heart Is (The Lavender Beds)"
            65970, // "It Could Happen to You"
            66338, // "Broadening Horizons"
            69708, // "Ascending to Empyreum"
            68167  // "I Dream of Shirogane"
        ];

        List<int> collectableQuestIds =
        [
            67631, // "Inscrutable Tastes"
            67633, // "No Longer a Collectable"
            67634, // "Go West, Craftsman"
            68477, // "Reach Long and Prosper"
            69139, // "The Boutique Always Wins"
            69711, // "Expanding House of Splendors"
            70544  // "Dawn of a New Deal"
        ];
        // Custom Deliveries clients require that you first unlock an expansion's inventory with one of the above quests. 
        List<int> customDeliveriesQuestIds =
        [
            67087, // "Arms Wide Open"
            68541, // "None Forgotten, None Forsaken"
            68675, // "The Seaweed Is Always Greener"
            68713, // "Between a Rock and the Hard Place"
            69265, // "Oh, Beehive Yourself"
            69425, // "O Crafter, My Crafter"
            69615, // "You Can Count on It"
            70059, // "Of Mothers and Merchants"
            70251, // "That's So Anden"
            70351, // "A Request of One's Own"
            70775  // "Laying New Tracks"
        ];
        
        // Mapping: Identifier (CategoryId or QuestIdList), Folder Name, Grouping Logic, Manual Name
        // Grouping Logic:
        // 0: Use JournalGenre.Name
        // 1: Use JournalCategory.Name
        // 2: Use only FolderName directly
        // 3: Use a JsonName for grouping, the JsonName defines the JSON file name
        var categoryFolders = new List<(object Identifier, string FolderName, int GroupBy, string? JsonName)>
        {
            (88, "Crystarium Deliveries", 0, null), // "Crystalline Mean Quests"

            (31, "Tribe Quests", 1, null), // Mamool Ja Quests
            (32, "Tribe Quests", 1, null), // Pelupelu Quests
            (33, "Tribe Quests", 1, null), // Intersocietal Quests
            (34, "Tribe Quests", 1, null), // Amaj'aa Quests
            (35, "Tribe Quests", 1, null), // Sylph Quests
            (36, "Tribe Quests", 1, null), // Kobold Quests
            (37, "Tribe Quests", 1, null), // Sahagin Quests
            (38, "Tribe Quests", 1, null), // Ixal Quests
            (39, "Tribe Quests", 1, null), // Vanu Vanu Quests
            (40, "Tribe Quests", 1, null), // Vath Quests
            (41, "Tribe Quests", 1, null), // Moogle Quests
            (42, "Tribe Quests", 1, null), // Kojin Quests
            (43, "Tribe Quests", 1, null), // Ananta Quests
            (44, "Tribe Quests", 1, null), // Namazu Quests
            (45, "Tribe Quests", 1, null), // Pixie Quests
            (46, "Tribe Quests", 1, null), // Qitari Quests
            (47, "Tribe Quests", 1, null), // Dwarf Quests
            (48, "Tribe Quests", 1, null), // Arkasodara Quests
            (49, "Tribe Quests", 1, null), // Omnicron Quests
            (50, "Tribe Quests", 1, null), // Loporrit Quests
            (51, "Tribe Quests", 1, null), // Intersocietal Quests
            
            (glamourQuestIds, "Glamour and Customization", 2, null),
    
            (locationsQuestIds, "Locations", 2, null),

            (collectableQuestIds, "Collectables", 3, "Collectables"),
            (customDeliveriesQuestIds, "Collectables", 3, "Custom Deliveries")
        };

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            foreach (var (identifier, folderName, groupBy, jsonName) in categoryFolders)
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
                        3 => jsonName ?? folderName, // Use JSON Name
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
