using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Lumina.Excel;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public class RendererUtils
{
    private readonly IPluginLog log;
    private readonly ExcelSheet<Lumina.Excel.Sheets.Item>? ItemSheet;
    private readonly ExcelSheet<Lumina.Excel.Sheets.Emote>? EmoteSheet;
    private readonly ExcelSheet<Lumina.Excel.Sheets.Action>? ActionSheet;

    public RendererUtils(IPluginLog log)
    {
        this.log = log;

        ItemSheet = QuestJournal.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Item>();
        EmoteSheet = QuestJournal.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Emote>();
        ActionSheet = QuestJournal.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>();
    }
    
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

    public void DrawSelectedQuestDetails(QuestModel? quest, ref List<QuestModel> questList)
    {
        if (quest == null)
        {
            ImGui.Text("Select a quest to view details.");
            return;
        }

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 1f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 6));

        try
        {
            ImGui.BeginChild("QuestDetails", new Vector2(0, 260), true);

            var iconId = quest.Icon;
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

            ImGui.TextColored(new Vector4(0.9f, 0.7f, 0.2f, 1f), quest.QuestTitle);
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize($"ID: {quest.QuestId}").X);
            ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), $"ID: {quest.QuestId}");

            ImGui.Text($"Expansion: {quest.Expansion}");
            ImGui.SameLine();
            var journalGenreX = ImGui.GetContentRegionMax().X -
                                ImGui.CalcTextSize(
                                         $"Journal Genre Category: {quest.JournalGenre?.JournalCategory?.Name ?? "None"}")
                                     .X;
            ImGui.SetCursorPosX(journalGenreX);
            ImGui.Text($"Journal Genre Category: {quest.JournalGenre?.JournalCategory?.Name ?? "None"}");

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
                    ImGui.TableSetupColumn("LabelColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn("ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                    ImGui.TableNextColumn();
                    ImGui.Text("First quest:");
                    ImGui.TableNextColumn();
                    ImGui.Text(questList.FirstOrDefault()?.QuestTitle ?? "None");

                    ImGui.TableNextColumn();
                    ImGui.Text("Previous quest:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text(quest.PreviousQuestTitles?.Any() == true
                                   ? string.Join(", ", quest.PreviousQuestTitles)
                                   : "None");
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.Text("Next quest:");
                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos();
                    ImGui.Text(quest.NextQuestTitles?.Any() == true
                                   ? string.Join(", ", quest.NextQuestTitles)
                                   : "None");
                    ImGui.PopTextWrapPos();
                    
                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Text("Starter NPC:");
                    ImGui.TableNextColumn();
                    if (ImGui.Selectable(quest.StarterNpc ?? "None"))
                    {
                        if (quest.StarterNpcLocation != null)
                        {
                            log.Info($"Opening starter location for NPC: {quest.StarterNpc}");
                            QuestHandler.OpenStarterLocation(quest, log);
                        }
                        else
                        {
                            log.Warning($"No starter location available for NPC: {quest.StarterNpc}");
                        }
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Finish NPC:");
                    ImGui.TableNextColumn();
                    ImGui.Text(quest.FinishNpc ?? "None");

                    ImGui.EndTable();
                }

                ImGui.EndChild();
                ImGui.SameLine();

                ImGui.BeginChild("RightSection", new Vector2(childWidth, childHeight), true);

                if (ImGui.BeginTabBar("QuestDetailsTabBar", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Requirements"))
                    {
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

                        ImGui.EndTabItem();
                    }
                    
                    if (ImGui.BeginTabItem("Rewards"))
                    {
                        ImGui.TextColored(new Vector4(0.9f, 0.75f, 0.4f, 1f), "Rewards");
                        ImGui.Separator();

                        if (ImGui.BeginTable("RewardsTable", 2, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable))
                        {
                            ImGui.TableSetupColumn("LabelColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                            ImGui.TableSetupColumn("ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                            if (quest.Rewards?.Exp > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("EXP:");
                            
                                ImGui.TableNextColumn();
                                ImGui.Text(quest.Rewards.Exp.ToString());
                            }
                            
                            if (quest.Rewards?.Gil > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Gil:");

                                ImGui.TableNextColumn();
                                ImGui.Text(quest.Rewards.Gil.ToString());
                            }
                            
                            if (quest.Rewards?.Currency != null)
                            {
                                var currency = quest.Rewards.Currency;

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Currency:");

                                ImGui.TableNextColumn();
                                DrawItemIconWithLabel(currency.CurrencyId, currency.CurrencyName ?? "Unknown", (byte)currency.Count);
                            }
                            
                            if (quest.Rewards?.Catalysts != null && quest.Rewards.Catalysts.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Catalysts:");

                                ImGui.TableNextColumn();
                                for (int i = 0; i < quest.Rewards.Catalysts.Count; i++)
                                {
                                    var catalyst = quest.Rewards.Catalysts[i];

                                    DrawItemIconWithLabel(catalyst.ItemId, catalyst.ItemName ?? "Unknown", catalyst.Count);

                                    if (i < quest.Rewards.Catalysts.Count - 1)
                                    {
                                        ImGui.SameLine();
                                    }
                                }
                            }
                            
                            if (quest.Rewards?.Items != null && quest.Rewards.Items.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Items:");

                                ImGui.TableNextColumn();
                                for (int i = 0; i < quest.Rewards.Items.Count; i++)
                                {
                                    var item = quest.Rewards.Items[i];
                                    DrawItemIconWithLabel(item.ItemId, item.ItemName ?? "Unknown", item.Count, item.Stain);

                                    if (i < quest.Rewards.Items.Count - 1)
                                    {
                                        ImGui.SameLine();
                                    }
                                }
                            }
                            
                            if (quest.Rewards?.OptionalItems != null && quest.Rewards.OptionalItems.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Optional Items:");

                                ImGui.TableNextColumn();
                                for (int i = 0; i < quest.Rewards.OptionalItems.Count; i++)
                                {
                                    var optionalItem = quest.Rewards.OptionalItems[i];
                                    DrawItemIconWithLabel(
                                        optionalItem.ItemId, 
                                        optionalItem.ItemName ?? "Unknown", 
                                        optionalItem.Count, 
                                        optionalItem.Stain, 
                                        optionalItem.IsHq);

                                    if (i < quest.Rewards.OptionalItems.Count - 1)
                                    {
                                        ImGui.SameLine();
                                    }
                                }
                            }
                            
                            if (quest.Rewards?.Emote != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Emote:");

                                ImGui.TableNextColumn();
                                var emote = quest.Rewards.Emote;
                                DrawIconWithLabel(EmoteSheet, emote.Id, emote.EmoteName ?? "Unknown");
                            }
                            
                            if (quest.Rewards?.Action != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Action:");

                                ImGui.TableNextColumn();
                                var action = quest.Rewards.Action;
                                DrawIconWithLabel(ActionSheet, action.Id, action.ActionName ?? "Unknown");
                            }

                            ImGui.EndTable();
                        }

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
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
    
    public void DrawItemIconWithLabel(uint itemId, string itemName, byte count, string? stainName = null, bool isHq = false, float size = 27f)
    {
        try
        {
            var item = ItemSheet?.GetRow(itemId);
            if (item == null)
            {
                ImGui.Text("[Invalid Item]");
                return;
            }

            var iconId = item.Value.Icon;
            var lookup = new GameIconLookup(iconId);
            var sharedTexture = QuestJournal.TextureProvider.GetFromGameIcon(lookup);

            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.BeginGroup();

                ImGui.Image(textureWrap.ImGuiHandle, new Vector2(size, size));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(itemName);
                    if (!string.IsNullOrEmpty(stainName)) ImGui.Text($"[{stainName}]");
                    if (isHq) ImGui.Text("[High Quality]");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (size / 4));
                ImGui.TextColored(new Vector4(1f, 1f, 1f, 1f), $"x{count}");

                ImGui.EndGroup();
            }
            else
            {
                ImGui.Text("[Missing Texture Wrap]");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Failed to render icon for itemId {itemId}: {ex.Message}");
        }
    }
    
    public void DrawIconWithLabel<T>(ExcelSheet<T>? sheet, uint entityId, string entityName, float size = 27f) where T : struct, IExcelRow<T>
    {
        try
        {
            var row = sheet?.GetRow(entityId);
            if (row == null)
            {
                ImGui.Text("[Invalid Entity]");
                return;
            }

            var iconProperty = typeof(T).GetProperty("Icon");
            if (iconProperty == null)
            {
                ImGui.Text("[Icon Property Not Found]");
                return;
            }
            
            var iconValue = iconProperty.GetValue(row);
            if (iconValue == null)
            {
                ImGui.Text("[Icon Value Not Found]");
                return;
            }

            var iconId = Convert.ToUInt32(iconValue);
            var lookup = new GameIconLookup(iconId);
            var sharedTexture = QuestJournal.TextureProvider.GetFromGameIcon(lookup);

            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.BeginGroup();

                ImGui.Image(textureWrap.ImGuiHandle, new Vector2(size, size));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(entityName);
                    ImGui.EndTooltip();
                }

                ImGui.EndGroup();
            }
            else
            {
                ImGui.Text("[Missing Texture Wrap]");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Failed to render icon for entityId {entityId}: {ex.Message}");
        }
    }
}
