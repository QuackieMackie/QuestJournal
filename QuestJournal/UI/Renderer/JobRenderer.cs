using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using ImGuiNET;
using QuestJournal.Models;
using QuestJournal.UI.Handler;
using QuestJournal.Utils;

namespace QuestJournal.UI.Renderer;

public class JobRenderer(JobHandler jobHandler, RendererUtils rendererUtils, IPluginLog log)
{
    private bool isInitialized = false;
    
    private string selectedDropDownCategory = string.Empty;
    private List<string> dropDownCategories = new List<string>();
    private Dictionary<string, string> dropDownCategoryMap = new();

    private int questCount;
    private List<QuestModel> questList = new List<QuestModel>();
    private QuestModel? selectedQuest = null;

    private string searchQuery = string.Empty;

    public void DrawJobs()
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
            dropDownCategoryMap = jobHandler.GetJobFileNames();
            dropDownCategories = dropDownCategoryMap.Keys.ToList();
            
            selectedDropDownCategory = dropDownCategories.FirstOrDefault() ?? "Error";

            if (selectedDropDownCategory != "Error")
            {
                UpdateQuestList(selectedDropDownCategory);
            }
            else
            {
                log.Warning("No items found in job file names list.");
            }
        }
    }

    private void UpdateQuestList(string category)
    {
        if (dropDownCategoryMap.TryGetValue(category, out var originalFileName))
        {
            var fetchedQuests = jobHandler.FetchQuestData(originalFileName);

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
