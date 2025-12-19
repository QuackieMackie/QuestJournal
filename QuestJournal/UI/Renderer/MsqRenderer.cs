using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.UI.Handler;
using QuestJournal.Utils;

namespace QuestJournal.UI.Renderer;

public class MsqRenderer(MsqHandler msqHandler, RendererUtils rendererUtils, Configuration configuration, IPluginLog log)
{
    private List<string> dropDownCategories = new();
    private Dictionary<string, string> dropDownCategoryMap = new();
    private bool isInitialized;

    private int questCount;
    private List<QuestModel> questList = new();

    private string searchQuery = string.Empty;

    private string selectedDropDownCategory = string.Empty;
    private QuestModel? selectedQuest;

    public void DrawMSQ()
    {
        if (!isInitialized)
        {
            InitializeDropDown();
            isInitialized = true;
        }

        rendererUtils.DrawDropDown("Select Journal Genre", dropDownCategories, ref selectedDropDownCategory, UpdateQuestList);
        var highlightedQuestCount = questList.Count(quest =>
                                                        !string.IsNullOrWhiteSpace(searchQuery) &&
                                                        quest.QuestTitle != null &&
                                                        quest.QuestTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

        rendererUtils.DrawSearchBar(ref searchQuery, highlightedQuestCount);
        rendererUtils.DrawSelectedQuestDetails(selectedQuest, ref questList, configuration.CensorStarterLocations);
        rendererUtils.DrawQuestWidgets(questList, ref searchQuery, ref selectedQuest, configuration.CensorStarterLocations);
    }

    public void ReloadQuests()
    {
        InitializeDropDown();
        UpdateQuestList(selectedDropDownCategory);
    }

    private void InitializeDropDown()
    {
        if (dropDownCategoryMap.Count == 0)
        {
            dropDownCategoryMap = msqHandler.GetMsqFileNames();
            dropDownCategories = dropDownCategoryMap.Keys.ToList();

            var msqCategories = new List<string>
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
                "Post-Dawntrail Main Scenario Quests",
                "Post-Dawntrail Main Scenario Quests II"
            };

            dropDownCategories = msqCategories
                                 .Where(category => dropDownCategories.Contains(category))
                                 .ToList();

            selectedDropDownCategory = dropDownCategories.FirstOrDefault() ?? "Error";

            if (selectedDropDownCategory != "Error")
                UpdateQuestList(selectedDropDownCategory);
            else
                log.Warning("No items found in msqFileNames list.");
        }
    }

    private void UpdateQuestList(string category)
    {
        if (dropDownCategoryMap.TryGetValue(category, out var originalFileName))
        {
            var fetchedQuests = msqHandler.FetchQuestData(originalFileName);

            if (fetchedQuests != null && fetchedQuests.Count > 0)
            {
                questList = fetchedQuests;
                questCount = questList.Count;
                log.Info($"Loaded {questCount} quests for category: {category}");
            }
            else
            {
                log.Warning($"No quests found for category: {category}");
                questList = new List<QuestModel>();
                questCount = 0;
            }
        }
    }
}
