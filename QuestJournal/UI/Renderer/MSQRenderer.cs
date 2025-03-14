using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using QuestJournal.Handlers;
using QuestJournal.Models;

namespace QuestJournal.UI.Renderer;

public class MsqRenderer(MsqHandler msqHandler, IPluginLog log)
{
    private string searchQuery = string.Empty;

    public void DrawMsqTab()
    {
        if (ImGui.BeginCombo("Select Journal Genre", msqHandler.GetSelectedCategory()))
        {
            var categories = msqHandler.GetCategories();
            foreach (var category in categories)
            {
                var isSelected = category == msqHandler.GetSelectedCategory();
                if (ImGui.Selectable(category, isSelected))
                {
                    if (!isSelected)
                        msqHandler.SetSelectedCategory(category);
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }


        var previousSearchQuery = searchQuery;
        if (ImGui.InputText("Search for a quest name##SearchBar", ref searchQuery, 256) &&
            !searchQuery.Equals(previousSearchQuery, StringComparison.OrdinalIgnoreCase))
            log.Info($"Updated search query: {searchQuery}");

        ImGui.Text(
            $"Loaded {msqHandler.FilteredQuests.Count} quests for journal genre: {msqHandler.GetSelectedCategory()}.");
        ImGui.Separator();

        DrawSelectedQuestDetails(msqHandler.GetSelectedQuest());

        var cachedFilteredQuests = msqHandler.GetCachedFilteredQuests();
        DrawQuestWidgets(cachedFilteredQuests);
    }

    private void DrawQuestWidgets(List<IQuestInfo> quests)
    {
        if (quests == null || quests.Count == 0)
        {
            ImGui.Text("No quests found.");
            return;
        }

        var childHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("QuestWidgetRegion", new Vector2(0, childHeight), true, ImGuiWindowFlags.HorizontalScrollbar);

        foreach (var quest in quests) DrawQuestWidget(quest);

        ImGui.EndChild();
    }

    private void DrawQuestWidget(IQuestInfo quest)
    {
        var availableWidth = ImGui.GetContentRegionAvail().X;
        var selectedQuest = msqHandler.GetSelectedQuest();

        var isCompleted = QuestManager.IsQuestComplete(quest.QuestId);

        var isSelected = selectedQuest != null && selectedQuest.QuestId == quest.QuestId;
        var isMatch = !string.IsNullOrEmpty(searchQuery) &&
                      quest.QuestTitle != null &&
                      quest.QuestTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);

        var styleCount = 0;

        if (isCompleted)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));          // Gray text
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 1f));        // Gray background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.4f, 0.4f, 0.4f, 1f)); // Slightly brighter hover
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0.5f, 0.5f, 1f));  // Muted gray when active
            styleCount += 4;
        }

        if (isSelected && isMatch)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.2f, 0.6f, 1f)); // Purple background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.3f, 0.7f, 1f)); // Purple hover
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.4f, 0.8f, 1f)); // Bright purple when active
            styleCount += 3;
        }
        else if (isSelected)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.3f, 0.6f, 1f));        // Blue background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.4f, 0.7f, 1f)); // Slightly lighter blue
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.4f, 0.8f, 1f));  // Bright blue when active
            styleCount += 3;
        }
        else if (isMatch)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.4f, 0.1f, 1f)); // Green background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered,
                                 new Vector4(0.1f, 0.5f, 0.1f, 1f)); // Slightly brighter green hover
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.1f, 0.6f, 0.1f, 1f)); // Bright green when active
            styleCount += 3;
        }

        if (ImGui.Button($"{(isCompleted ? "✓ " : string.Empty)}{quest.QuestTitle ?? "Unknown Quest"}",
                         new Vector2(availableWidth, 0)))
        {
            msqHandler.SetSelectedQuest(quest);
            log.Info($"Selected Quest: {quest.QuestTitle} (ID: {quest.QuestId})");
        }

        if (styleCount > 0) ImGui.PopStyleColor(styleCount);
    }

    private void DrawSelectedQuestDetails(IQuestInfo? questInfo)
    {
        if (questInfo == null)
        {
            ImGui.Text("Select a quest to view details.");
            return;
        }

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 1f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 6));

        ImGui.BeginChild("QuestDetails", new Vector2(0, 260), true);

        var firstQuest = msqHandler.GetFirstQuestInChain(questInfo);
        var nextQuest = msqHandler.GetNextQuestInChain(questInfo);

        ImGui.TextColored(new Vector4(0.9f, 0.7f, 0.2f, 1f), questInfo.QuestTitle);
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize($"ID: {questInfo.QuestId}").X);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), $"ID: {questInfo.QuestId}");

        ImGui.Text($"Expansion: {questInfo.Expansion}");
        ImGui.SameLine();
        var journalGenreX = ImGui.GetContentRegionMax().X -
                            ImGui.CalcTextSize(
                                $"Journal Genre: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}").X;
        ImGui.SetCursorPosX(journalGenreX);
        ImGui.Text($"Journal Genre: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}");

        var childWidth = ImGui.GetContentRegionAvail().X / 2f;
        float childHeight = 200;

        ImGui.BeginChild("LeftSection", new Vector2(childWidth, childHeight), true);

        ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.4f, 1f), "Chain Details");
        ImGui.Separator();

        if (ImGui.BeginTable("ChainTable", 2, ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableNextColumn();
            ImGui.Text("First quest:");
            ImGui.TableNextColumn();
            ImGui.Text(firstQuest?.QuestTitle ?? "None");

            ImGui.TableNextColumn();
            ImGui.Text("Previous quest:");
            ImGui.TableNextColumn();
            ImGui.Text(questInfo.PreviousQuestTitles != null && questInfo.PreviousQuestTitles.Count > 0
                           ? string.Join(", ", questInfo.PreviousQuestTitles)
                           : "None");

            ImGui.TableNextColumn();
            ImGui.Text("Next quest:");
            ImGui.TableNextColumn();
            ImGui.Text(nextQuest?.QuestTitle ?? "None");

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
