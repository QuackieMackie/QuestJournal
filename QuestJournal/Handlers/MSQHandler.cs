using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

namespace QuestJournal.Handlers;

public class MsqHandler : IDisposable
{
    private readonly List<IQuestInfo> cachedQuests;
    private readonly Configuration configuration;
    private readonly IPluginLog log;

    private int selectedDropDownIndex;
    private IQuestInfo? selectedQuest;

    public MsqHandler(List<IQuestInfo> cachedQuests, IPluginLog log, Configuration configuration)
    {
        this.cachedQuests = cachedQuests;
        this.log = log;
        this.configuration = configuration;

        DropDownList = new List<string>
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

        selectedDropDownIndex = 0;
    }

    public List<string> DropDownList { get; private set; }

    public List<IQuestInfo> FilteredQuests { get; private set; } = new();

    public void Dispose()
    {
        DropDownList.Clear();
        cachedQuests.Clear();
        FilteredQuests.Clear();
        selectedQuest = null;
    }

    //TODO:
    // - Add manual drop downs in the settings, letting users pick which grand company or start area quests that they would like to see instead of trying to
    //   automate it. Automating it is to much extra effort for the results it would provide. Plus it wouldn't work at lower msq quest chains otherwise the
    //   ui would look like it's suggesting you go to x gc.

    private Dictionary<string, Dictionary<string, uint>> BuildQuestAreaMapping()
    {
        return new Dictionary<string, (uint Gridania, uint Limsa, uint Uldah)>
            {
                //Removed Quests
                { "Close to Home", (0, 0, 0) },

                //Grindania
                { "To the Bannock", (65564, 0, 0) },
                { "Passing Muster", (65737, 0, 0) },
                { "Chasing Shadows", (65981, 0, 0) },
                { "Eggs over Queasy", (69390, 0, 0) },
                { "Surveying the Damage", (65711, 0, 0) },
                { "A Soldier's Breakfast", (69391, 0, 0) },
                { "Spirithold Broken", (65665, 0, 0) },
                { "On to Bentbranch", (65712, 0, 0) },
                { "You Shall Not Trespass", (65912, 0, 0) },
                { "Don't Look Down", (65913, 0, 0) },
                { "In the Grim Darkness of the Forest", (65915, 0, 0) },
                { "Threat Level Elevated", (65916, 0, 0) },
                { "Migrant Marauders", (65917, 0, 0) },
                { "A Hearer Is Often Late", (65920, 0, 0) },
                { "Salvaging the Scene", (65923, 0, 0) },
                { "Leia's Legacy", (65697, 0, 0) },
                { "Dread Is in the Air", (65982, 0, 0) },
                { "To Guard a Guardian", (65983, 0, 0) },
                { "Festive Endeavors", (65984, 0, 0) },
                { "Renewing the Covenant", (65985, 0, 0) },
                { "The Gridanian Envoy", (66043, 0, 0) },

                // Limsa
                { "On to Summerford", (0, 65998, 0) },
                { "Dressed to Call", (0, 65999, 0) },
                { "Lurkers in the Grotto", (0, 66079, 0) },
                { "Washed Up", (0, 66001, 0) },
                { "Double Dealing", (0, 66002, 0) },
                { "Loam Maintenance", (0, 66003, 0) },
                { "Plowshares to Swords", (0, 66004, 0) },
                { "Just Deserts", (0, 66005, 0) },
                { "Sky-high", (0, 65933, 0) },
                { "Thanks a Million", (0, 65938, 0) },
                { "Relighting the Torch", (0, 65939, 0) },
                { "On to the Drydocks", (0, 65942, 0) },
                { "Without a Doubt", (0, 65948, 0) },
                { "Righting the Shipwright", (0, 65951, 0) },
                { "Do Angry Pirates Dream", (0, 65949, 0) },
                { "Victory in Peril", (0, 65950, 0) },
                { "Men of the Blue Tattoos", (0, 66225, 0) },
                { "Feint and Strike", (0, 66080, 0) },
                { "High Society", (0, 66226, 0) },
                { "A Mizzenmast Repast", (0, 66081, 0) },
                { "The Lominsan Envoy", (0, 66082, 0) },

                // Uldah
                { "We Must Rebuild", (0, 0, 66131) },
                { "Nothing to See Here", (0, 0, 66207) },
                { "Underneath the Sultantree", (0, 0, 66086) },
                { "Step Nine", (0, 0, 65839) },
                { "Prudence at This Junction", (0, 0, 69388) },
                { "Out of House and Home", (0, 0, 65843) },
                { "Way Down in the Hole", (0, 0, 65856) },
                { "Takin' What They're Givin'", (0, 0, 66159) },
                { "Supply and Demands", (0, 0, 65864) },
                { "Give It to Me Raw", (0, 0, 66039) },
                { "The Perfect Swarm", (0, 0, 65865) },
                { "Last Letter to Lost Hope", (0, 0, 65866) },
                { "Heir Today, Gone Tomorrow", (0, 0, 69389) },
                { "Passing the Blade", (0, 0, 65868) },
                { "Following Footfalls", (0, 0, 65869) },
                { "Storms on the Horizon", (0, 0, 65870) },
                { "Oh Captain, My Captain", (0, 0, 65872) },
                { "Secrets and Lies", (0, 0, 66164) },
                { "Duty, Honor, Country", (0, 0, 66087) },
                { "A Matter of Tradition", (0, 0, 66177) },
                { "A Royal Reception", (0, 0, 66088) },
                { "The Ul'dahn Envoy", (0, 0, 66064) },

                { "Call of the Sea", (66210, 66210, 66209) }
            }
            .ToDictionary(
                pair => pair.Key,
                pair => new Dictionary<string, uint>
                {
                    { "Gridania", pair.Value.Gridania },
                    { "Limsa Lominsa", pair.Value.Limsa },
                    { "Ul'dah", pair.Value.Uldah }
                });
    }

    private Dictionary<string, Dictionary<string, uint>> BuildGrandCompanyMapping()
    {
        return new Dictionary<string, (uint TwinAdder, uint Maelstorm, uint ImmortalFlames)>
            {
                { "The Company You Keep (Immortal Flames)", (0, 0, 66218) },
                { "For Coin and Country", (0, 0, 66221) },
                { "The Company You Keep (Maelstrom)", (0, 66217, 0) },
                { "Till Sea Swallows All", (0, 66220, 0) },
                { "The Company You Keep (Twin Adder)", (66216, 0, 0) },
                { "Wood's Will Be Done", (66219, 0, 0) }
            }
            .ToDictionary(
                pair => pair.Key,
                pair => new Dictionary<string, uint>
                {
                    { "Twin Adder", pair.Value.TwinAdder },
                    { "Maelstorm", pair.Value.Maelstorm },
                    { "Immortal Flames", pair.Value.ImmortalFlames }
                });
    }

    public void ReloadFilteredQuests()
    {
        var questAreaMapping = BuildQuestAreaMapping();
        var grandCompanyMapping = BuildGrandCompanyMapping();
        var selectedCategory = DropDownList[selectedDropDownIndex];
        var startArea = configuration.StartArea;
        var grandCompany = configuration.GrandCompany;
        
        if (string.IsNullOrWhiteSpace(startArea) && string.IsNullOrWhiteSpace(grandCompany))
        {
            log.Warning("StartArea and GrandCompany not configured. Loading all matching quests.");
            FilteredQuests = cachedQuests
                             .Where(q =>
                                        string.Equals(
                                            q.JournalGenre?.JournalCategory?.Name,
                                            selectedCategory,
                                            StringComparison.OrdinalIgnoreCase
                                        ))
                             .ToList();
            return;
        }

        // Filter by category
        FilteredQuests = cachedQuests
                         .Where(q =>
                                    string.Equals(
                                        q.JournalGenre?.JournalCategory?.Name,
                                        selectedCategory,
                                        StringComparison.OrdinalIgnoreCase
                                    ))
                         .ToList();

        // Start Area Filtering
        if (!string.IsNullOrWhiteSpace(startArea))
        {
            FilteredQuests = FilteredQuests
                             .GroupBy(q => q.QuestTitle)
                             .Select(group =>
                             {
                                 if (questAreaMapping.TryGetValue(group.Key, out var areaMapping) &&
                                     areaMapping.TryGetValue(startArea, out var mappedQuestId))
                                     return group.FirstOrDefault(q => q.QuestId == mappedQuestId);
                                 return group.First();
                             })
                             .Where(q => q != null)
                             .ToList();
        }

        // Grand Company Filtering
        if (!string.IsNullOrWhiteSpace(grandCompany))
        {
            FilteredQuests = FilteredQuests
                             .GroupBy(q => q.QuestTitle)
                             .Select(group =>
                             {
                                 if (grandCompanyMapping.TryGetValue(group.Key, out var gcMapping) &&
                                     gcMapping.TryGetValue(grandCompany, out var mappedQuestId))
                                     return group.FirstOrDefault(q => q.QuestId == mappedQuestId);
                                 return group.First();
                             })
                             .Where(q => q != null)
                             .ToList();
        }

        log.Info($"Reloaded {FilteredQuests.Count} quests for category: \"{selectedCategory}\", start area: \"{startArea}\", and grand company: \"{grandCompany}\".");
        
        FilteredQuests = GetOrderedQuestList();
    }

    public List<IQuestInfo> GetOrderedQuestList()
    {
        List<IQuestInfo> orderedQuests = new();
        HashSet<uint> processedQuestIds = new();

        foreach (var quest in FilteredQuests)
        {
            if (processedQuestIds.Contains(quest.QuestId))
                continue;

            var currentQuest = GetFirstQuestInChain(quest);

            while (currentQuest != null && !processedQuestIds.Contains(currentQuest.QuestId))
            {
                orderedQuests.Add(currentQuest);
                processedQuestIds.Add(currentQuest.QuestId);

                currentQuest = GetNextQuestInChain(currentQuest);
            }
        }

        log.Info($"Ordered quest list generated. Quests organized: {orderedQuests.Count}");
        return orderedQuests;
    }

    public IQuestInfo? GetFirstQuestInChain(IQuestInfo quest)
    {
        var currentQuest = quest;

        while (currentQuest.PreviousQuestIds != null && currentQuest.PreviousQuestIds.Count > 0)
        {
            var previousQuest = FilteredQuests.FirstOrDefault(
                q => currentQuest.PreviousQuestIds.Contains(q.QuestId));

            if (previousQuest == null) break;

            currentQuest = previousQuest;
        }

        return currentQuest;
    }

    public IQuestInfo? GetNextQuestInChain(IQuestInfo quest)
    {
        return FilteredQuests.FirstOrDefault(q =>
                                                 q.PreviousQuestIds != null &&
                                                 q.PreviousQuestIds.Contains(quest.QuestId));
    }

    public List<IQuestInfo> GetCachedFilteredQuests()
    {
        return FilteredQuests;
    }

    public List<string> GetCategories()
    {
        return DropDownList;
    }

    public string GetSelectedCategory()
    {
        return DropDownList[selectedDropDownIndex];
    }

    public void SetSelectedCategory(string category)
    {
        var index = DropDownList.IndexOf(category);
        if (index == -1)
        {
            log.Warning($"Attempted to set invalid category: {category}");
            return;
        }

        selectedDropDownIndex = index;
        ReloadFilteredQuests();
        log.Info($"Selected category changed to: \"{DropDownList[selectedDropDownIndex]}\"");
    }

    public int GetQuestCount()
    {
        return cachedQuests.Count;
    }

    public IQuestInfo? GetSelectedQuest()
    {
        return selectedQuest;
    }

    public void SetSelectedQuest(IQuestInfo quest)
    {
        selectedQuest = quest;
        log.Info($"Selected Quest: \"{quest.QuestTitle}\" (ID: {quest.QuestId})");
    }
}
