using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using QuestJournal.Models;
using QuestJournal.UI.Handler;

namespace QuestJournal.UI.Renderer;

public class MsqRenderer(MsqHandler msqHandler, IPluginLog log)
{
    private string selectedDropDownCategory = string.Empty;
    private List<string> dropDownCategories = new List<string>();
    private Dictionary<string, string> dropDownCategoryMap = new();
    
    private int questCount;
    private List<QuestInfo> questList = new List<QuestInfo>();
    private QuestInfo? selectedQuest = null;
    
    private string searchQuery = string.Empty;
    
    public void DrawMSQ()
    {
        InitializeDropDown();

        DrawDropDown();

        DrawSearchBar();

        ImGui.Text($"Loaded {questCount} quests for journal genre category: {selectedDropDownCategory}.");
        ImGui.Separator();

        DrawSelectedQuestDetails(selectedQuest);

        DrawQuestWidgets(questList);
    }

    private void DrawQuestWidgets(List<QuestInfo> quests)
    {
        var childHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("QuestWidgetRegion", new Vector2(0, childHeight), true, ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar);

        if (ImGui.BeginTable("QuestTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, new Vector2(-1, 0)))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("Quest Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Location", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();

            for (int i = 0; i < quests.Count; i++)
            {
                ImGui.TableNextRow();

                bool isComplete = QuestManager.IsQuestComplete(quests[i].QuestId);
                bool isMatch = !string.IsNullOrEmpty(searchQuery) && 
                               (quests[i].QuestTitle?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false);
                bool isSelected = selectedQuest != null && selectedQuest.QuestId == quests[i].QuestId;

                Vector4 rowColor = DetermineQuestColor(isComplete, isMatch, isSelected);

                ImGui.PushStyleColor(ImGuiCol.Text, rowColor);

                ImGui.TableNextColumn();
                ImGui.Text(isComplete ? " ✓ " : " x ");

                ImGui.TableNextColumn();
                RenderSelectableRow(i, quests[i], isSelected);

                ImGui.TableNextColumn();
                ImGui.Text(quests[i].StarterNpc ?? "Unknown Location");

                ImGui.PopStyleColor();
            }

            ImGui.EndTable();
        }

        ImGui.EndChild();
    }

    private void RenderSelectableRow(int rowIndex, QuestInfo quest, bool isSelected)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);

        if (ImGui.Selectable("##Selectable" + rowIndex, isSelected, ImGuiSelectableFlags.SpanAllColumns))
        {
            if (selectedQuest?.QuestId == quest.QuestId)
            {
                selectedQuest = null;
                log.Info("Deselected the currently selected quest.");
            }
            else
            {
                selectedQuest = quest;
                log.Info($"Quest selected: {quest.QuestTitle} (ID: {quest.QuestId})");
            }
        }
        ImGui.PopStyleColor(3);

        ImGui.SameLine();
        ImGui.TextColored(ImGui.GetStyle().Colors[(int)ImGuiCol.Text], quest.QuestTitle ?? "Unknown Quest");
    }

    private Vector4 DetermineQuestColor(bool isComplete, bool isMatch, bool isSelected)
    {
        if (isSelected && isMatch) 
            return new Vector4(0.8f, 0.5f, 1f, 1f); // Purple
        if (isSelected)
            return new Vector4(0.4f, 0.6f, 1f, 1f); // Blue
        if (isMatch) 
            return new Vector4(0.3f, 1f, 0.3f, 1f); // Green
        if (isComplete)
            return new Vector4(0.5f, 0.5f, 0.5f, 1f); // Gray for completed

        return new Vector4(1f, 1f, 1f, 1f); // Default white
    }
    
    private void InitializeDropDown()
    {
        if (dropDownCategoryMap.Count == 0)
        {
            dropDownCategoryMap = msqHandler.GetMsqFileNames();
            dropDownCategories = dropDownCategoryMap.Keys.ToList();

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
    
    private void DrawDropDown()
    {
        if (ImGui.BeginCombo("Select Journal Genre", selectedDropDownCategory))
        {
            foreach (var displayedName in dropDownCategories)
            {
                var isSelected = selectedDropDownCategory == displayedName;

                if (ImGui.Selectable(displayedName, isSelected))
                {
                    selectedDropDownCategory = displayedName;
                    log.Info($"Selected dropdown category: {selectedDropDownCategory}");
                    UpdateQuestList(selectedDropDownCategory);
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
    }
    
    private void DrawSearchBar()
    {
        var previousSearchQuery = searchQuery;

        if (ImGui.InputText("Search for a quest name##SearchBar", ref searchQuery, 256) &&
            !searchQuery.Equals(previousSearchQuery, StringComparison.OrdinalIgnoreCase))
        {
            log.Info($"Updated search query: {searchQuery}");
        }
    }

    private void DrawSelectedQuestDetails(QuestInfo? questInfo)
    {
        if (questInfo == null)
        {
            ImGui.Text("Select a quest to view details.");
            return;
        }

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 1f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 6));

        ImGui.BeginChild("QuestDetails", new Vector2(0, 260), true);

        ImGui.TextColored(new Vector4(0.9f, 0.7f, 0.2f, 1f), questInfo.QuestTitle);
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize($"ID: {questInfo.QuestId}").X);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), $"ID: {questInfo.QuestId}");

        ImGui.Text($"Expansion: {questInfo.Expansion}");
        ImGui.SameLine();
        var journalGenreX = ImGui.GetContentRegionMax().X -
                            ImGui.CalcTextSize(
                                $"Journal Genre Category: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}").X;
        ImGui.SetCursorPosX(journalGenreX);
        ImGui.Text($"Journal Genre Category: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}");

        var childWidth = ImGui.GetContentRegionAvail().X / 2f;
        float childHeight = 200;

        ImGui.BeginChild("LeftSection", new Vector2(childWidth, childHeight), true);

        ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.4f, 1f), "Quest Details");
        ImGui.Separator();

        if (ImGui.BeginTable("ChainTable", 2, ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("First quest:");
            ImGui.TableNextColumn();
            ImGui.Text(questList.FirstOrDefault()?.QuestTitle ?? "None");

            ImGui.TableNextColumn();
            ImGui.Text("Previous quest:");
            ImGui.TableNextColumn();
            if (questInfo.PreviousQuestTitles != null && questInfo.PreviousQuestTitles.Any())
            {
                ImGui.Text(string.Join(", ", questInfo.PreviousQuestTitles));
            }
            else
            {
                ImGui.Text("None");
            }

            ImGui.TableNextColumn();
            ImGui.Text("Next quest:");
            ImGui.TableNextColumn();
            if (questInfo.NextQuestTitles != null && questInfo.NextQuestTitles.Any())
            {
                ImGui.Text(string.Join(", ", questInfo.NextQuestTitles));
            }
            else
            {
                ImGui.Text("None");
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();

            ImGui.TableNextColumn();
            ImGui.Text("Starter NPC:");
            ImGui.TableNextColumn();
            ImGui.Text(questInfo.StarterNpc ?? "None");

            ImGui.TableNextColumn();
            ImGui.Text("Finish NPC:");
            ImGui.TableNextColumn();
            ImGui.Text(questInfo.FinishNpc ?? "None");

            ImGui.EndTable();
        }

        ImGui.EndChild();
        ImGui.SameLine();

        ImGui.BeginChild("RightSection", new Vector2(childWidth, childHeight), true);

        ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.4f, 1f), "Requirements");
        ImGui.Separator();

        if (ImGui.BeginTable("RequirementsTable", 2, ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("Requirement 1:");
            ImGui.TableNextColumn();
            ImGui.Text("TBD");

            ImGui.TableNextColumn();
            ImGui.Text("Requirement 2:");
            ImGui.TableNextColumn();
            ImGui.Text("TBD");

            ImGui.EndTable();
        }

        ImGui.EndChild();

        ImGui.EndChild();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }
}

