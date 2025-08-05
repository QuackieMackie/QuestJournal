using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.UI.Handler;
using QuestJournal.Utils;

namespace QuestJournal.UI.Renderer;

public class FeatureRenderer(FeatureHandler featureHandler, RendererUtils rendererUtils, IPluginLog log)
{
    private List<string> dropDownCategories = new();
    private Dictionary<string, string> dropDownCategoryMap = new();
    private bool isInitialized;

    private int questCount;
    private List<QuestModel> questList = new();

    private string searchQuery = string.Empty;
    private string selectedDropDownCategory = string.Empty;
    private QuestModel? selectedQuest;

    private string selectedSubDir = string.Empty;
    private List<string> subDirList = new();

    public void DrawFeatures()
    {
        if (!isInitialized)
        {
            InitializeSubDirDropDown();
            InitializeDropDown();
            isInitialized = true;
        }

        rendererUtils.DrawDropDown("Select Subdirectory", subDirList, ref selectedSubDir, OnSubDirSelected);
        rendererUtils.DrawDropDown("Select Journal Genre", dropDownCategories, ref selectedDropDownCategory, UpdateQuestList);
        var highlightedQuestCount = questList.Count(quest =>
                                                        !string.IsNullOrWhiteSpace(searchQuery) &&
                                                        quest.QuestTitle != null &&
                                                        quest.QuestTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

        rendererUtils.DrawSearchBar(ref searchQuery, highlightedQuestCount);
        ImGui.Text($"Loaded {questCount} quests for journal genre category: {selectedDropDownCategory}.");
        rendererUtils.DrawSelectedQuestDetails(selectedQuest, ref questList, false);
        rendererUtils.DrawQuestWidgets(questList, ref searchQuery, ref selectedQuest, false);
    }

    public void ReloadQuests()
    {
        InitializeSubDirDropDown();
        InitializeDropDown();
        UpdateQuestList(selectedDropDownCategory);
    }

    private void InitializeSubDirDropDown()
    {
        subDirList.Clear();

        subDirList = featureHandler.GetFeatureSubDirs()
                                   ?.Where(d => !string.IsNullOrEmpty(d))
                                   .Select(TransformSubDirName)
                                   .ToList() ?? new List<string>();

        selectedSubDir = subDirList.FirstOrDefault() ?? string.Empty;

        if (!string.IsNullOrEmpty(selectedSubDir))
            OnSubDirSelected(selectedSubDir);
        else
            log.Warning("No valid subdirectories found in the FEATURE directory.");
    }

    private void OnSubDirSelected(string displayName)
    {
        var actualSubDir = ReverseTransformSubDirName(displayName);

        if (string.IsNullOrEmpty(actualSubDir))
        {
            log.Warning($"Invalid display name or subdirectory: {displayName}");
            return;
        }

        dropDownCategoryMap = featureHandler.GetJournalGenresInSubDir(actualSubDir) ?? new Dictionary<string, string>();
        dropDownCategories = dropDownCategoryMap.Keys.ToList();

        selectedDropDownCategory = dropDownCategories.FirstOrDefault() ?? string.Empty;

        if (!string.IsNullOrEmpty(selectedDropDownCategory))
            UpdateQuestList(selectedDropDownCategory);
        else
            log.Warning($"No journal genres found in subdirectory: {actualSubDir}");
    }

    private void InitializeDropDown()
    {
        if (dropDownCategoryMap.Count == 0) OnSubDirSelected(selectedSubDir);
    }

    private void UpdateQuestList(string category)
    {
        if (dropDownCategoryMap.TryGetValue(category, out var originalFileName))
        {
            var actualSubDir = ReverseTransformSubDirName(selectedSubDir);
            var fetchedQuests = featureHandler.FetchQuestDataFromSubDir(actualSubDir, originalFileName);

            if (fetchedQuests != null && fetchedQuests.Count > 0)
            {
                questList = fetchedQuests;
                questCount = questList.Count;
                log.Info($"Loaded {questCount} quests for category: {category} in subdirectory: {actualSubDir}");
            }
            else
            {
                log.Warning($"No quests found for category: {category} in subdirectory: {actualSubDir}");
                questList = new List<QuestModel>();
                questCount = 0;
            }
        }
    }

    private string TransformSubDirName(string subDir)
    {
        return subDir.Replace("_", " ");
    }

    private string ReverseTransformSubDirName(string displayName)
    {
        return displayName.Replace(" ", "_");
    }
}
