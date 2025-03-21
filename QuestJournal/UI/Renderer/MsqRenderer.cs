using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using QuestJournal.Models;
using QuestJournal.UI.Handler;
using QuestJournal.Utils;

namespace QuestJournal.UI.Renderer;

public class MsqRenderer(MsqHandler msqHandler, RendererUtils rendererUtils, IPluginLog log)
{
    private bool isInitialized = false;
    
    private string selectedDropDownCategory = string.Empty;
    private List<string> dropDownCategories = new List<string>();
    private Dictionary<string, string> dropDownCategoryMap = new();

    private int questCount;
    private List<QuestInfo> questList = new List<QuestInfo>();
    private QuestInfo? selectedQuest = null;

    private string searchQuery = string.Empty;

    public void DrawMSQ()
    {
        if (!isInitialized)
        {
            InitializeDropDown();
            isInitialized = true;
        }
        rendererUtils.DrawDropDown("Select Journal Genre", dropDownCategories, ref selectedDropDownCategory, UpdateQuestList);
        rendererUtils.DrawSearchBar(ref searchQuery);
        ImGui.Text($"Loaded {questCount} quests for journal genre category: {selectedDropDownCategory}.");
        rendererUtils.DrawSelectedQuestDetails(selectedQuest, ref questList);
        rendererUtils.DrawQuestWidgets(questList, ref searchQuery, ref selectedQuest);
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

            var msqCategories = new List<string>()
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
            
            dropDownCategories = msqCategories
                                 .Where(category => dropDownCategories.Contains(category))
                                 .ToList();
            
            selectedDropDownCategory = dropDownCategories.FirstOrDefault() ?? "Error";
            log.Info($"Populated msqFileNames list with {dropDownCategories.Count} items.");

            if (selectedDropDownCategory != "Error")
            {
                UpdateQuestList(selectedDropDownCategory);
            }
            else
            {
                log.Warning("No items found in msqFileNames list.");
            }
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
                questList = new List<QuestInfo>();
                questCount = 0;
            }
        }
    }
}
