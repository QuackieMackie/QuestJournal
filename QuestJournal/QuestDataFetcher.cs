using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal;

public class QuestDataFetcher(IDataManager dataManager, IPluginLog log)
{
    private readonly QuestFetcherUtils questFetcherUtils = new(dataManager, log);
    private readonly Lazy<ExcelSheet<Quest>?> questSheet = new(() => dataManager.GetExcelSheet<Quest>());

    private ExcelSheet<Quest>? QuestSheet => questSheet.Value;

    public List<QuestModel> GetAllQuests()
    {
        var questInfoLookup = new Dictionary<uint, QuestModel>();

        if (QuestSheet != null)
        {
            foreach (var quest in QuestSheet)
                if (!questInfoLookup.ContainsKey(quest.RowId))
                {
                    var questInfo = questFetcherUtils.BuildQuestInfo(quest);
                    if (questInfo != null) questInfoLookup[quest.RowId] = questInfo;
                }

            foreach (var quest in QuestSheet)
                if (questInfoLookup.TryGetValue(quest.RowId, out _) && quest.PreviousQuest.Count > 0)
                {
                    var prevQuestIds = questFetcherUtils.GetPrerequisiteQuestIds(quest.PreviousQuest);

                    foreach (var prevQuestId in prevQuestIds)
                        if (questInfoLookup.TryGetValue(prevQuestId, out var prevQuestInfo))
                        {
                            prevQuestInfo?.NextQuestIds?.Add(quest.RowId);
                            prevQuestInfo?.NextQuestTitles?.Add(quest.Name.ExtractText());
                        }
                }

            var injector = new QuestDataInjector();
            injector.InjectMissingData(questInfoLookup.Values);
        }

        return questInfoLookup.Values.Where(quest => quest != null).ToList();
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

        foreach (var category in msqCategories) categorizedQuests[category] = new List<QuestModel>();

        foreach (var quest in allQuests)
        {
            var categoryName = quest.JournalGenre?.JournalCategory?.Name;
            if (categoryName != null && msqCategories.Contains(categoryName))
                categorizedQuests[categoryName].Add(quest);
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
            92  // "Disciple of the Magic Job Quests"
        };

        var categorizedQuests = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            if (category != null && jobCategoryIds.Contains((int)category.Id))
            {
                var journalGenreName = quest.JournalGenre?.Name;
                if (!string.IsNullOrEmpty(journalGenreName))
                {
                    if (!categorizedQuests.ContainsKey(journalGenreName))
                        categorizedQuests[journalGenreName] = new List<QuestModel>();

                    categorizedQuests[journalGenreName].Add(quest);
                }
            }
        }

        return categorizedQuests;
    }

    public Dictionary<string, Dictionary<string, List<QuestModel>>> GetFeatureQuestsByCategory()
    {
        var allQuests = GetAllQuests();

        var categorizedQuests =
            new Dictionary<string, Dictionary<string, List<QuestModel>>>(StringComparer.OrdinalIgnoreCase);

        List<int> glamourQuestIds =
        [
            66746, // "Beauty Is Only Scalp Deep"
            66235, // "Color Your World"
            67094, // "Simply to Dye For"
            68553, // "If I Had a Glamour"
            68554, // "Absolutely Glamourous"
            66957, // "A Self-improving Man"
            66958, // "Submission Impossible"
            70300, // "A Faerie Tale Come True"
            67896, // "An Egi by Any Other Name"
            70723  // "Bottled Fantasy"
        ];

        List<int> materiaQuestIds =
        [
            66174, // "Forging the Spirit"
            66175, // "Waking the Spirit"
            66999, // "Marvelously Mutable Materia"
            66176  // "Melding Materia Muchly"
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

        List<int> guildHestQuestIds =
        [
            65596, // "Simply the Hest (Gridania)"
            65595, // "Simply the Hest (Limsa Lominsa)"
            65594  // "Simply the Hest (Ul'dah)"
        ];

        // Duties
        List<int> dungeonQuestIds =
        [
            66233, // "Hallo Halatali"
            66300, // "Braving New Depths"
            66457, // "Dishonor Before Death"
            66515, // "Fort of Fear"
            66550, // "Going for Gold"
            66406, // "Trauma Queen"
            66671, // "Ghosts of Amdapor"
            66744, // "Sirius Business"
            66752, // "Out of Sight, Out of Mine"
            66751, // "Maniac Manor"
            66925, // "One Night in Amdapor"
            66946, // "This Time's for Fun"
            66947, // "Curds and Slay"
            67062, // "King of the Hull"
            67060, // "Corpse Groom"
            67061, // "Blood for Stone"
            65630, // "It's Definitely Pirates"
            65632, // "The Wrath of Qarn"
            65967, // "Not Easy Being Green"
            65966, // "For Keep's Sake"
            67647, // "For All the Nights to Come"
            67648, // "Reap What You Sow"
            67649, // "Do It for Gilly"
            67738, // "An Overgrown Ambition"
            67737, // "Things Are Getting Sirius"
            67818, // "One More Night in Amdapor"
            67784, // "Storming the Hull"
            67922, // "Let Me Gubal That for You"
            67938, // "The Fires of Sohm Al"
            68168, // "The Palace of Lost Souls"
            68170, // "King of the Castle"
            68169, // "To Kill a Coeurl"
            68551, // "An Auspicious Encounter"
            68613, // "An Unwanted Truth"
            68552, // "Tortoise in Time"
            68678, // "Secret of the Ooze"
            69131, // "By the Time You Hear This"
            69132, // "Akadaemia Anyder"
            69703, // "Cutting the Cheese"
            69704, // "Where No Loporrit Has Gone Before"
            70549, // "It Belongs in a Museum"
            70550  // "Something Stray in the Neighborhood"
        ];

        List<int> trailsQuestIds =
        [
            67090, // "The New King on the Block"
            67091  // "The Newer King on the Block"
        ];

        List<int> stoneSkySeaQuestIds =
        [
            67654, // "A Striking Opportunity"
            68476, // "Another Striking Opportunity"
            69137, // "Yet Another Striking Opportunity"
            69709, // "A Place to Train"
            70541  // "Trial by Spire"
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

            (glamourQuestIds, "Other", 3, "Glamour and Customization"),
            (materiaQuestIds, "Other", 3, "Materia"),
            (guildHestQuestIds, "Other", 3, "Guild Hests"),
            (locationsQuestIds, "Other", 3, "Residential Areas"),

            (collectableQuestIds, "Collectables", 3, "Collectables"),
            (customDeliveriesQuestIds, "Collectables", 3, "Custom Deliveries"),

            (dungeonQuestIds, "Duties", 3, "Dungeons"),
            (trailsQuestIds, "Duties", 3, "Trails"),
            (stoneSkySeaQuestIds, "Duties", 3, "Stone Sky Sea")
        };

        foreach (var quest in allQuests)
        {
            var category = quest.JournalGenre?.JournalCategory;

            foreach (var (identifier, folderName, groupBy, jsonName) in categoryFolders)
            {
                var matches = false;

                if (identifier is int categoryId)
                    matches = category != null && category.Id == categoryId;
                else if (identifier is List<int> questIds) matches = questIds.Contains((int)quest.QuestId);

                if (matches)
                {
                    var subFolderName = groupBy switch
                    {
                        0 => quest.JournalGenre?.Name ?? "Unknown Genre", // Group by JournalGenre.Name
                        1 => quest.JournalGenre?.JournalCategory?.Name ??
                             "Unknown Category",     // Group by JournalCategory.Name
                        3 => jsonName ?? folderName, // Use JSON Name
                        _ => folderName              // Use FolderName directly
                    };

                    if (!categorizedQuests.ContainsKey(folderName))
                        categorizedQuests[folderName] =
                            new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);

                    if (!categorizedQuests[folderName].ContainsKey(subFolderName))
                        categorizedQuests[folderName][subFolderName] = new List<QuestModel>();

                    categorizedQuests[folderName][subFolderName].Add(quest);
                    break;
                }
            }
        }

        return categorizedQuests;
    }
}
