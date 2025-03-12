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

    private List<IQuestInfo> activeFilteredQuests = new();
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

    public List<IQuestInfo> FilteredQuests => cachedQuests
                                              .Where(q => string.Equals(
                                                         q.JournalGenre?.JournalCategory?.Name,
                                                         DropDownList[selectedDropDownIndex],
                                                         StringComparison.OrdinalIgnoreCase)
                                              )
                                              .ToList();

    public void Dispose()
    {
        DropDownList.Clear();
        cachedQuests.Clear();
        activeFilteredQuests.Clear();
        selectedQuest = null;
    }

    public void ReloadFilteredQuests()
    {
        var selectedCategory = DropDownList[selectedDropDownIndex];
        activeFilteredQuests = cachedQuests
                               .Where(q =>
                                          string.Equals(
                                              q.JournalGenre?.JournalCategory?.Name,
                                              selectedCategory,
                                              StringComparison.OrdinalIgnoreCase
                                          )
                               ).ToList();

        log.Info($"Reloaded {activeFilteredQuests.Count} quests for category: {selectedCategory}");
    }

    public List<IQuestInfo> GetCachedFilteredQuests()
    {
        return activeFilteredQuests;
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
        log.Info($"Selected category changed to: {DropDownList[selectedDropDownIndex]}");
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
        log.Info($"Selected Quest: {quest.QuestTitle} (ID: {quest.QuestId})");
    }

    public IQuestInfo? GetFirstQuestInChain(IQuestInfo selectedQuest)
    {
        var currentQuest = selectedQuest;
        while (currentQuest.PreviousQuestIds != null && currentQuest.PreviousQuestIds.Count > 0)
        {
            var previousQuest = cachedQuests.FirstOrDefault(
                q => currentQuest.PreviousQuestIds.Contains(q.QuestId));
            if (previousQuest == null) break;
            currentQuest = previousQuest;
        }

        return currentQuest;
    }

    public IQuestInfo? GetNextQuestInChain(IQuestInfo selectedQuest)
    {
        return cachedQuests.FirstOrDefault(q =>
                                               q.PreviousQuestIds != null &&
                                               q.PreviousQuestIds.Contains(selectedQuest.QuestId));
    }
}
