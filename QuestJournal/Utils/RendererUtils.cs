using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public class RendererUtils(IPluginLog log)
{
    public void DrawDropDown(string label, List<string> items, ref string selectedItem, Action<string>? onSelectionChanged = null)
    {
        if (ImGui.BeginCombo(label, selectedItem))
        {
            foreach (var item in items)
            {
                if (ImGui.Selectable(item, item == selectedItem))
                {
                    if (item != selectedItem)
                    {
                        selectedItem = item;
                        onSelectionChanged?.Invoke(item);
                    }
                }
            }
            ImGui.EndCombo();
        }
    }
    
    public void DrawSearchBar(ref string searchQuery)
    {
        var previousSearchQuery = searchQuery;

        if (ImGui.InputText("Search for a quest name##SearchBar", ref searchQuery, 256) &&
            !searchQuery.Equals(previousSearchQuery, StringComparison.OrdinalIgnoreCase))
        {
            log.Info($"Updated search query: {searchQuery}");
        }
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

    public void DrawQuestWidgets(List<QuestModel> quests, ref string searchQuery, ref QuestModel? selectedQuest)
    {
        var childHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("QuestWidgetRegion", new Vector2(0, childHeight), false);

        if (ImGui.BeginTable("QuestTable", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY,
                             new Vector2(-1, 0)))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("Quest Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Start Location", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupScrollFreeze(3, 1);
            
            ImGui.TableHeadersRow();

            for (int i = 0; i < quests.Count; i++)
            {
                ImGui.TableNextRow();
                bool isComplete = QuestManager.IsQuestComplete(quests[i].QuestId);
                bool isMatch = !string.IsNullOrEmpty(searchQuery) &&
                               (quests[i].QuestTitle?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ??
                                false);
                bool isSelected = selectedQuest != null && selectedQuest.QuestId == quests[i].QuestId;

                Vector4 rowColor = DetermineQuestColor(isComplete, isMatch, isSelected);

                ImGui.PushStyleColor(ImGuiCol.Text, rowColor);

                ImGui.TableNextColumn();
                ImGui.Text(isComplete ? " ✓ " : " x ");

                ImGui.TableNextColumn();
                if (ImGui.Selectable(quests[i].QuestTitle ?? "Unknown Quest", isSelected))
                {
                    if (selectedQuest?.QuestId == quests[i].QuestId)
                    {
                        selectedQuest = null;
                        log.Info("Deselected the currently selected quest.");
                    }
                    else
                    {
                        selectedQuest = quests[i];
                        log.Info($"Quest selected: {quests[i].QuestTitle} (ID: {quests[i].QuestId})");
                    }
                }

                ImGui.TableNextColumn();
                if (ImGui.Selectable($"{quests[i].StarterNpc ?? "Unknown Location"}##StarterNpc{i}"))
                {
                    log.Info(
                        $"Selected quest starter location: {quests[i].StarterNpc} for Quest ID: {quests[i].QuestId}");
                    QuestHandler.OpenStarterLocation(quests[i], log);
                }

                ImGui.PopStyleColor();
            }

            ImGui.EndTable();
        }

        ImGui.EndChild();
    }

    public void DrawSelectedQuestDetails(QuestModel? questInfo, ref List<QuestModel> questList)
    {
        if (questInfo == null)
        {
            ImGui.Text("Select a quest to view details.");
            return;
        }

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 1f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 6));

        try
        {
            ImGui.BeginChild("QuestDetails", new Vector2(0, 260), true);

            var iconId = questInfo.Icon;
            if (iconId != 0 && QuestJournal.TextureProvider.TryGetFromGameIcon(iconId, out var imageTex)
                            && imageTex.TryGetWrap(out var image, out _))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);

                try
                {
                    Vector2 windowPos = ImGui.GetWindowPos();
                    Vector2 windowSize = ImGui.GetContentRegionAvail();
                    float aspectRatio = image.Size.Y / image.Size.X;
                    float newWidth = windowSize.X;
                    float newHeight = newWidth * aspectRatio;

                    if (newHeight < windowSize.Y)
                    {
                        newHeight = windowSize.Y;
                        newWidth = newHeight / aspectRatio;
                    }

                    float centeredX = windowPos.X + (windowSize.X - newWidth) / 2f;
                    float centeredY = windowPos.Y + (windowSize.Y - newHeight) / 2f;

                    ImGui.GetWindowDrawList().AddImage(
                        image.ImGuiHandle,
                        new Vector2(centeredX, centeredY),
                        new Vector2(centeredX + newWidth, centeredY + newHeight)
                    );

                    Vector4 overlayColor = new Vector4(0f, 0f, 0f, 0.75f);
                    ImGui.GetWindowDrawList().AddRectFilled(
                        new Vector2(centeredX, centeredY),
                        new Vector2(centeredX + newWidth, centeredY + newHeight),
                        ImGui.GetColorU32(overlayColor)
                    );
                } finally
                {
                    ImGui.PopStyleVar(2);
                }
            }

            ImGui.TextColored(new Vector4(0.9f, 0.7f, 0.2f, 1f), questInfo.QuestTitle);
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize($"ID: {questInfo.QuestId}").X);
            ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), $"ID: {questInfo.QuestId}");

            ImGui.Text($"Expansion: {questInfo.Expansion}");
            ImGui.SameLine();
            var journalGenreX = ImGui.GetContentRegionMax().X -
                                ImGui.CalcTextSize(
                                         $"Journal Genre Category: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}")
                                     .X;
            ImGui.SetCursorPosX(journalGenreX);
            ImGui.Text($"Journal Genre Category: {questInfo.JournalGenre?.JournalCategory?.Name ?? "None"}");

            var childWidth = ImGui.GetContentRegionAvail().X / 2f;
            const float childHeight = 200;

            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0));
            try
            {
                ImGui.BeginChild("LeftSection", new Vector2(childWidth, childHeight), true);
                ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.4f, 1f), "Quest Details");
                ImGui.Separator();

                if (ImGui.BeginTable("ChainTable", 2, ImGuiTableFlags.BordersInnerV))
                {
                    ImGui.TableSetupColumn("LabelColumn", ImGuiTableColumnFlags.WidthFixed, 95);
                    ImGui.TableSetupColumn("ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                    ImGui.TableNextColumn();
                    ImGui.Text("First quest:");
                    ImGui.TableNextColumn();
                    ImGui.Text(questList.FirstOrDefault()?.QuestTitle ?? "None");

                    ImGui.TableNextColumn();
                    ImGui.Text("Previous quest:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text(questInfo.PreviousQuestTitles?.Any() == true
                                   ? string.Join(", ", questInfo.PreviousQuestTitles)
                                   : "None");
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.Text("Next quest:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text(questInfo.NextQuestTitles?.Any() == true
                                   ? string.Join(", ", questInfo.NextQuestTitles)
                                   : "None");
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.Text("Starter NPC:");
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable(questInfo.StarterNpc ?? "None"))
                    {
                        if (questInfo.StarterNpcLocation != null)
                        {
                            log.Info($"Opening starter location for NPC: {questInfo.StarterNpc}");
                            QuestHandler.OpenStarterLocation(questInfo, log);
                        }
                        else
                        {
                            log.Warning($"No starter location available for NPC: {questInfo.StarterNpc}");
                        }
                    }
                    
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
                    ImGui.TableSetupColumn("LabelColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                    ImGui.TableNextColumn();
                    ImGui.Text("Requirement 1:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text("TBD");
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.Text("Requirement 2:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text("TBD");
                    ImGui.PopTextWrapPos();

                    ImGui.EndTable();
                }

                ImGui.EndChild();
            } finally
            {
                ImGui.PopStyleColor();
            }

            ImGui.EndChild();
        } finally
        {
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
        }
    }
}
