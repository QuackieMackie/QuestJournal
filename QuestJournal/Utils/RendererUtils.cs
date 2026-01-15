using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using QuestJournal.Models;
using Action = Lumina.Excel.Sheets.Action;

namespace QuestJournal.Utils;

public class RendererUtils
{
    private readonly Lazy<List<QuestModel>> fullQuestList = new(
        () => JsonConvert.DeserializeObject<List<QuestModel>>(EmbeddedResourceLoader.LoadJson("QuestData")) 
              ?? new List<QuestModel>());
    
    private readonly Lazy<ExcelSheet<Action>> actionSheet;
    private readonly Lazy<ExcelSheet<ContentType>> contentTypeSheet;
    private readonly Lazy<ExcelSheet<Emote>> emoteSheet;
    private readonly Lazy<ExcelSheet<EventIconType>> eventIconSheet;
    private readonly Lazy<ExcelSheet<GeneralAction>> generalActionSheet;
    private readonly Lazy<ExcelSheet<QuestRewardOther>> otherRewardSheet;
    private readonly Lazy<ExcelSheet<Item>> itemSheet;
    
    private readonly QuestJournal questJournal;
    private readonly IPluginLog log;

    public RendererUtils(IPluginLog log, QuestJournal questJournal)
    {
        this.questJournal = questJournal;
        this.log = log;

        itemSheet = new Lazy<ExcelSheet<Item>>(() => Service.DataManager.GetExcelSheet<Item>());
        emoteSheet = new Lazy<ExcelSheet<Emote>>(() => Service.DataManager.GetExcelSheet<Emote>());
        actionSheet = new Lazy<ExcelSheet<Action>>(() => Service.DataManager.GetExcelSheet<Action>());
        generalActionSheet = new Lazy<ExcelSheet<GeneralAction>>(() => Service.DataManager.GetExcelSheet<GeneralAction>());
        otherRewardSheet = new Lazy<ExcelSheet<QuestRewardOther>>(() => Service.DataManager.GetExcelSheet<QuestRewardOther>());
        contentTypeSheet = new Lazy<ExcelSheet<ContentType>>(() => Service.DataManager.GetExcelSheet<ContentType>());
        eventIconSheet = new Lazy<ExcelSheet<EventIconType>>(() => Service.DataManager.GetExcelSheet<EventIconType>());
    }

    private ExcelSheet<Item> ItemSheet => itemSheet.Value;
    private ExcelSheet<Emote> EmoteSheet => emoteSheet.Value;
    private ExcelSheet<Action> ActionSheet => actionSheet.Value;
    private ExcelSheet<GeneralAction> GeneralActionSheet => generalActionSheet.Value;
    private ExcelSheet<QuestRewardOther> OtherRewardSheet => otherRewardSheet.Value;
    private ExcelSheet<ContentType> ContentTypeSheet => contentTypeSheet.Value;
    private ExcelSheet<EventIconType> EventIconSheet => eventIconSheet.Value;

    public void DrawDropDown(
        string label, List<string> items, ref string selectedItem, Action<string>? onSelectionChanged = null)
    {
        if (ImGui.BeginCombo(label, selectedItem))
        {
            foreach (var item in items)
            {
                var isSelected = item == selectedItem;
                if (ImGui.Selectable(item, isSelected))
                {
                    if (!isSelected)
                    {
                        selectedItem = item;
                        onSelectionChanged?.Invoke(item);
                    }
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
    }

    public void DrawSearchBar(ref string searchQuery, int highlightedQuestCount)
    {
        var previousSearchQuery = searchQuery;

        if (ImGui.InputText("Search for a quest name##SearchBar", ref searchQuery, 256) &&
            !searchQuery.Equals(previousSearchQuery, StringComparison.OrdinalIgnoreCase)) { }
        
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            ImGui.SameLine();
            ImGui.Text($"[Found: {highlightedQuestCount}]");
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

    public void DrawQuestWidgets(List<QuestModel> quests, ref string searchQuery, ref QuestModel? selectedQuest, bool censorStarterLocations, bool markCompletedRepeatableQuests)
    {
        var childHeight = ImGui.GetContentRegionAvail().Y;
        ImGui.BeginChild("QuestWidgetRegion", new Vector2(0, childHeight), false);

        if (ImGui.BeginTable("QuestTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY,
                             new Vector2(-1, 0)))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 25);
            ImGui.TableSetupColumn("Quest Name", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Start Location", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupScrollFreeze(4, 1);

            ImGui.TableHeadersRow();

            for (var i = 0; i < quests.Count; i++)
            {
                ImGui.TableNextRow();
                bool isComplete;
                bool isAccepted;
                unsafe
                {
                    isAccepted = QuestManager.Instance()->IsQuestAccepted(quests[i].QuestId);
    
                    isComplete = (quests[i].IsRepeatable && !markCompletedRepeatableQuests)
                                     ? QuestManager.Instance()->IsDailyQuestCompleted((ushort)quests[i].QuestId)
                                     : QuestManager.IsQuestComplete(quests[i].QuestId);
                }

                var questStatus = isComplete ? "\u2713" : isAccepted ? "\u2192" : "\u00d7";
                var hoverText = isComplete ? "Complete" : isAccepted ? "In Progress" : "Not Started";

                var isMatch = !string.IsNullOrEmpty(searchQuery) &&
                              (quests[i].QuestTitle?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ??
                               false);
                var isSelected = selectedQuest != null && selectedQuest.QuestId == quests[i].QuestId;

                var rowColor = DetermineQuestColor(isComplete, isMatch, isSelected);

                ImGui.PushStyleColor(ImGuiCol.Text, rowColor);

                ImGui.TableNextColumn();
                ImGui.Text($"  {questStatus}  ");
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(hoverText);
                }

                ImGui.TableNextColumn();
                GetQuestIcon(quests[i]);

                ImGui.TableNextColumn();
                if (ImGui.Selectable($"{quests[i].QuestTitle ?? "Unknown Quest"}##Quest{i}", isSelected))
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
                if (censorStarterLocations)
                {
                    if (!isComplete && !isAccepted) 
                    {
                        if (ImGui.Selectable($"{quests[i].StarterNpc?.Replace(quests[i].StarterNpc ?? "??????", "??????")}##StarterNpc{i}"))
                        {
                            log.Info($"Selected quest starter location: {quests[i].StarterNpc} for Quest ID: {quests[i].QuestId}");
                            OtherUtils.OpenStarterLocation(quests[i], log);
                        }
                    }
                    else if (isAccepted || isComplete) 
                    {
                        if (ImGui.Selectable($"{quests[i].StarterNpc ?? "Unknown Location"}##StarterNpc{i}"))
                        {
                            log.Info($"Selected quest starter location: {quests[i].StarterNpc} for Quest ID: {quests[i].QuestId}");
                            OtherUtils.OpenStarterLocation(quests[i], log);
                        }
                    }
                }
                else
                {
                    if (ImGui.Selectable($"{quests[i].StarterNpc ?? "Unknown Location"}##StarterNpc{i}"))
                    {
                        log.Info($"Selected quest starter location: {quests[i].StarterNpc} for Quest ID: {quests[i].QuestId}");
                        OtherUtils.OpenStarterLocation(quests[i], log);
                    } 
                }
                
                ImGui.PopStyleColor();
            }

            ImGui.EndTable();
        }

        ImGui.EndChild();
    }

    public void DrawSelectedQuestDetails(QuestModel? quest, ref List<QuestModel> questList, bool censorStarterLocations, bool markCompletedRepeatableQuests)
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
            ImGui.BeginChild("QuestDetails", new Vector2(0, 270), true);

            var iconId = quest.Icon;
            if (iconId != 0 && Service.TextureProvider.TryGetFromGameIcon(iconId, out var imageTex)
                            && imageTex.TryGetWrap(out var image, out _))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);

                try
                {
                    var windowPos = ImGui.GetWindowPos();
                    var windowSize = ImGui.GetContentRegionAvail();
                    var aspectRatio = image.Size.Y / image.Size.X;
                    var newWidth = windowSize.X;
                    var newHeight = newWidth * aspectRatio;
                    var alpha = ImGui.GetStyle().Alpha;

                    if (newHeight < windowSize.Y)
                    {
                        newHeight = windowSize.Y;
                        newWidth = newHeight / aspectRatio;
                    }

                    var centeredX = windowPos.X + ((windowSize.X - newWidth) / 2f);
                    var centeredY = windowPos.Y + ((windowSize.Y - newHeight) / 2f);

                    ImGui.GetWindowDrawList().AddImage(
                        image.Handle,
                        new Vector2(centeredX, centeredY),
                        new Vector2(centeredX + newWidth, centeredY + newHeight),
                        Vector2.Zero,
                        Vector2.One,
                        ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, alpha))
                    );

                    var overlayColor = new Vector4(0f, 0f, 0f, 0.75f * alpha);
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

            ImGui.BeginGroup();
            GetQuestIcon(quest, 26);
            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.7f, 0.2f, 1f));
            if (ImGui.Selectable(quest.QuestTitle))
            {
                questJournal.OpenQuestWindow(quest, questList);
            }
            ImGui.PopStyleColor();
            
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize($"ID: {quest.QuestId}").X);
            ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), $"ID: {quest.QuestId}");
            ImGui.EndGroup();

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
                    var currentQuestList = questList;
                    var firstQuest = currentQuestList.FirstOrDefault(q =>
                                         q.PreviousQuestIds == null ||
                                         !q.PreviousQuestIds.Any(id => currentQuestList.Any(ql => ql.QuestId == id)));
                    if (firstQuest != null)
                    {
                        var firstQuestStatusInfo = OtherUtils.GetQuestStatus(firstQuest, markCompletedRepeatableQuests);
                        if (ImGui.Selectable($"{firstQuestStatusInfo.StatusSymbol} {firstQuest.QuestTitle ?? "None"}##{firstQuest.QuestId}"))
                        {
                            questJournal.OpenQuestWindow(firstQuest, questList);
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(firstQuestStatusInfo.HoverText);
                            ImGui.EndTooltip();
                        }
                    }


                    ImGui.TableNextColumn();
                    ImGui.Text("Previous quest:");
                    ImGui.TableNextColumn();
                    if (quest.PreviousQuestIds == null || quest.PreviousQuestIds.Count == 0) ImGui.Text("None");
                    else {
                        for (int i = 0; i < quest.PreviousQuestIds.Count; i++)
                        {
                            var previousQuestId = quest.PreviousQuestIds[i];
                            var previousQuest = fullQuestList.Value.FirstOrDefault(q => q.QuestId == previousQuestId);

                            if (previousQuest != null)
                            {
                                var previousQuestStatusInfo = OtherUtils.GetQuestStatus(previousQuest, markCompletedRepeatableQuests);
                                if (ImGui.Selectable($"{previousQuestStatusInfo.StatusSymbol} {previousQuest.QuestTitle ?? "None"}##{previousQuestId}"))
                                {
                                    questJournal.OpenQuestWindow(previousQuest, questList);
                                }
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text(previousQuestStatusInfo.HoverText);
                                    ImGui.EndTooltip();
                                }
                            }
                        }
                    }
                    
                    ImGui.TableNextColumn();
                    ImGui.Text("Next quest:");
                    ImGui.TableNextColumn();
                    if (quest.NextQuestIds == null || quest.NextQuestIds.Count == 0) ImGui.Text("None");
                    else {
                        for (int i = 0; i < quest.NextQuestIds.Count; i++)
                        {
                            var nextQuestId = quest.NextQuestIds[i];
                            var nextQuest = fullQuestList.Value.FirstOrDefault(q => q.QuestId == nextQuestId);

                            if (nextQuest != null)
                            {
                                var nextQuestStatusInfo = OtherUtils.GetQuestStatus(nextQuest, markCompletedRepeatableQuests);
                                if (ImGui.Selectable($"{nextQuestStatusInfo.StatusSymbol} {nextQuest.QuestTitle ?? "None"}##{nextQuestId}"))
                                {
                                    questJournal.OpenQuestWindow(nextQuest, questList);
                                }
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text(nextQuestStatusInfo.HoverText);
                                    ImGui.EndTooltip();
                                }
                            }
                        }
                    }
                    
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Text("Starter NPC:");
                    ImGui.TableNextColumn();
                    bool isComplete;
                    bool isAccepted;
                    unsafe
                    {
                        isAccepted = QuestManager.Instance()->IsQuestAccepted(quest.QuestId);
    
                        isComplete = (quest.IsRepeatable && !markCompletedRepeatableQuests)
                                         ? QuestManager.Instance()->IsDailyQuestCompleted((ushort)quest.QuestId)
                                         : QuestManager.IsQuestComplete(quest.QuestId);
                    }
                    
                    if (censorStarterLocations)
                    {
                        if (!isComplete && !isAccepted) 
                        {
                            if (ImGui.Selectable($"{quest.StarterNpc?.Replace(quest.StarterNpc ?? "??????", "??????")}##DetailsStarterNpc{quest.QuestId}"))
                            {
                                log.Info($"Selected quest starter location: {quest.StarterNpc} for Quest ID: {quest.QuestId}");
                                OtherUtils.OpenStarterLocation(quest, log);
                            }
                        }
                        else if (isAccepted || isComplete) 
                        {
                            if (ImGui.Selectable($"{quest.StarterNpc ?? "Unknown Location"}##DetailsStarterNpc{quest.QuestId}"))
                            {
                                log.Info($"Selected quest starter location: {quest.StarterNpc} for Quest ID: {quest.QuestId}");
                                OtherUtils.OpenStarterLocation(quest, log);
                            }
                        }
                    }
                    else
                    {
                        if (ImGui.Selectable($"{quest.StarterNpc ?? "Unknown Location"}##DetailsStarterNpc{quest.QuestId}")) {
                            log.Info($"Opening starter location for NPC: {quest.StarterNpc}");
                            OtherUtils.OpenStarterLocation(quest, log);
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

                ImGui.BeginChild("RightSection", new Vector2(childWidth - (ImGui.GetStyle().ScrollbarSize / 2), childHeight), true);

                if (ImGui.BeginTabBar("QuestDetailsTabBar", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Requirements"))
                    {
                        if (ImGui.BeginTable("RequirementsTable", 2, ImGuiTableFlags.BordersInnerV))
                        {
                            ImGui.TableSetupColumn("LabelColumn", ImGuiTableColumnFlags.WidthFixed, 100);
                            ImGui.TableSetupColumn("ValueColumn", ImGuiTableColumnFlags.WidthStretch);

                            ImGui.TableNextColumn();
                            ImGui.Text("Level:");
                            ImGui.TableNextColumn();
                            ImGui.PushTextWrapPos();
                            ImGui.Text(quest.JobLevel.ToString() ?? "Unknown");
                            ImGui.PopTextWrapPos();

                            ImGui.TableNextColumn();
                            ImGui.Text("Class:");
                            ImGui.TableNextColumn();
                            ImGui.PushTextWrapPos();
                            ImGui.Text(quest.ClassJobCategory ?? "Unknown");
                            ImGui.PopTextWrapPos();

                            if (!string.IsNullOrWhiteSpace(quest.BeastTribeRequirements?.BeastTribeName))
                            {
                                ImGui.TableNextColumn();
                                ImGui.Text("Beast Tribe:");
                                ImGui.TableNextColumn();
                                ImGui.PushTextWrapPos();
                                ImGui.Text(
                                    $"{quest.BeastTribeRequirements.BeastTribeName} ({quest.BeastTribeRequirements?.BeastTribeRank})");
                                ImGui.PopTextWrapPos();
                            }

                            ImGui.EndTable();
                        }

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Rewards"))
                    {
                        if (ImGui.BeginTable("RewardsTable", 2,
                                             ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable))
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

                            if (quest.Rewards?.ReputationReward?.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.Text("Reputation:");

                                ImGui.TableNextColumn();
                                ImGui.Text(
                                    $"{quest.Rewards.ReputationReward.Count} ({quest.Rewards.ReputationReward.ReputationName})");
                            }

                            if (quest.Rewards?.Currency != null)
                            {
                                var currency = quest.Rewards.Currency;

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Currency:");

                                ImGui.TableNextColumn();
                                DrawItemIconWithLabel(currency.CurrencyId, currency.CurrencyName ?? "Unknown",
                                                      (byte)currency.Count);
                            }

                            if (quest.Rewards?.Catalysts != null && quest.Rewards.Catalysts.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Catalysts:");

                                ImGui.TableNextColumn();
                                for (var i = 0; i < quest.Rewards.Catalysts.Count; i++)
                                {
                                    var catalyst = quest.Rewards.Catalysts[i];

                                    DrawItemIconWithLabel(catalyst.ItemId, catalyst.ItemName ?? "Unknown",
                                                          catalyst.Count);

                                    if (i < quest.Rewards.Catalysts.Count - 1) ImGui.SameLine();
                                }
                            }

                            if (quest.Rewards?.Items != null && quest.Rewards.Items.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Items:");

                                ImGui.TableNextColumn();
                                for (var i = 0; i < quest.Rewards.Items.Count; i++)
                                {
                                    var item = quest.Rewards.Items[i];
                                    DrawItemIconWithLabel(item.ItemId, item.ItemName ?? "Unknown", item.Count,
                                                          item.Stain);

                                    if (i < quest.Rewards.Items.Count - 1) ImGui.SameLine();
                                }
                            }

                            if (quest.Rewards?.OptionalItems != null && quest.Rewards.OptionalItems.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Optional Items:");

                                ImGui.TableNextColumn();
                                for (var i = 0; i < quest.Rewards.OptionalItems.Count; i++)
                                {
                                    var optionalItem = quest.Rewards.OptionalItems[i];
                                    DrawItemIconWithLabel(
                                        optionalItem.ItemId,
                                        optionalItem.ItemName ?? "Unknown",
                                        optionalItem.Count,
                                        optionalItem.Stain,
                                        optionalItem.IsHq);

                                    if (i < quest.Rewards.OptionalItems.Count - 1) ImGui.SameLine();
                                }
                            }

                            if (quest.Rewards?.Emote != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Emote:");

                                ImGui.TableNextColumn();
                                var emote = quest.Rewards.Emote;
                                DrawIconWithLabel(EmoteSheet, emote.Id, emote.EmoteName ?? "Unknown");
                            }

                            if (quest.Rewards?.Action != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Action:");

                                ImGui.TableNextColumn();
                                var action = quest.Rewards.Action;
                                DrawIconWithLabel(ActionSheet, action.Id, action.ActionName ?? "Unknown");
                            }

                            if (quest.Rewards?.GeneralActions != null && quest.Rewards.GeneralActions.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("General Actions:");

                                ImGui.TableNextColumn();
                                for (var i = 0; i < quest.Rewards.GeneralActions.Count; i++)
                                {
                                    var generalAction = quest.Rewards.GeneralActions[i];
                                    DrawIconWithLabel(GeneralActionSheet, generalAction.Id,
                                                      generalAction.Name ?? "Unknown");

                                    if (i < quest.Rewards.GeneralActions.Count - 1) ImGui.SameLine();
                                }
                            }

                            if (quest.Rewards?.OtherReward != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Other Reward:");

                                ImGui.TableNextColumn();
                                var otherReward = quest.Rewards.OtherReward;
                                DrawIconWithLabel(OtherRewardSheet, otherReward.Id, otherReward.Name ?? "Unknown");
                            }

                            if (quest.Rewards?.InstanceContentUnlock != null && quest.Rewards?.InstanceContentUnlock.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Instance Content:");
                                
                                ImGui.TableNextColumn();
                                for (var i = 0; i < quest.Rewards.InstanceContentUnlock.Count; i++)
                                {
                                    var instanceContentUnlock = quest.Rewards.InstanceContentUnlock[i];
                                    if (instanceContentUnlock.ContentType == 999) DrawWithProvidedIconValueWithLabel(instanceContentUnlock.InstanceName ?? "Unknown", 61418);
                                    else DrawIconWithLabel(ContentTypeSheet, instanceContentUnlock.ContentType, instanceContentUnlock.InstanceName ?? "Unknown");
                                    
                                    if (i < quest.Rewards.InstanceContentUnlock.Count - 1) ImGui.SameLine();
                                }
                            }

                            if (quest.Rewards?.LocationReward?.PlaceName != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Location Unlock:");

                                ImGui.TableNextColumn();
                                if (ImGui.Selectable(quest.Rewards?.LocationReward?.PlaceName))
                                {
                                    log.Info(
                                        $"Opening location for area unlocked: {quest.Rewards?.LocationReward?.PlaceName}");
                                    OtherUtils.OpenMapAtLocation(quest, log);
                                }
                            }

                            if (quest.Rewards?.CommentSection?.GuiComment != null)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.AlignTextToFramePadding();
                                ImGui.Text("Comment:");

                                ImGui.TableNextColumn();
                                //TODO: Couldn't get the selectable label wrapping, need to find a way to do that.
                                if (ImGui.Button(quest.Rewards?.CommentSection?.GuiComment))
                                {
                                    if (quest.Rewards?.CommentSection?.ClickToCopy == true)
                                    {
                                        ImGui.SetClipboardText(quest.Rewards?.CommentSection?.GuiComment);
                                    }
                                }
                                
                                if (!string.IsNullOrEmpty(quest.Rewards?.CommentSection?.HoverTextComment) && ImGui.IsItemHovered())
                                {
                                    string tooltipText = quest.Rewards.CommentSection.HoverTextComment;

                                    if (quest.Rewards.CommentSection.ClickToCopy)
                                    {
                                        tooltipText += "\n(Click to copy)";
                                    }

                                    ImGui.TextWrapped(tooltipText);
                                }
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
            var item = ItemSheet.GetRow(itemId);

            var iconId = item.Icon;
            var lookup = new GameIconLookup(iconId);
            var sharedTexture = Service.TextureProvider.GetFromGameIcon(lookup);
            
            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.BeginGroup();

                ImGui.Image(textureWrap.Handle, new Vector2(size, size));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(itemName);
                    if (!string.IsNullOrEmpty(stainName)) ImGui.Text($"[{stainName}]");
                    if (isHq) ImGui.Text("[High Quality]");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                ImGui.AlignTextToFramePadding();
                ImGui.TextColored(new Vector4(1f, 1f, 1f, 1f), $"x{count}");

                ImGui.EndGroup();
            }
            else
                ImGui.Text("[Missing Texture Wrap]");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to render icon for itemId {itemId}: {ex.Message}");
        }
    }

    public void DrawIconWithLabel<T>(ExcelSheet<T>? sheet, uint entityId, string entityName, float size = 27f)
        where T : struct, IExcelRow<T>
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
            var sharedTexture = Service.TextureProvider.GetFromGameIcon(lookup);

            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.BeginGroup();

                ImGui.Image(textureWrap.Handle, new Vector2(size, size));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(entityName.FirstCharToUpper());
                    ImGui.EndTooltip();
                }

                ImGui.EndGroup();
            }
            else
                ImGui.Text("[Missing Texture Wrap]");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to render icon for entityId {entityId}: {ex.Message}");
        }
    }
    
    public void DrawWithProvidedIconValueWithLabel(string instanceName, int iconValue, float size = 27f)
    {
        try
        {
            var iconId = Convert.ToUInt32(iconValue);
            var lookup = new GameIconLookup(iconId);
            var sharedTexture = Service.TextureProvider.GetFromGameIcon(lookup);

            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.BeginGroup();

                ImGui.Image(textureWrap.Handle, new Vector2(size, size));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(instanceName.FirstCharToUpper());
                    ImGui.EndTooltip();
                }

                ImGui.EndGroup();
            }
            else
                ImGui.Text("[Missing Texture Wrap]");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to render icon for instance {instanceName}: {ex.Message}");
        }
    }

    private void GetQuestIcon(QuestModel quest, float size = 16f)
    {
        try
        {
            var eventIconRow = EventIconSheet.GetRow(quest.EventIcon);

            var baseIconId = eventIconRow.MapIconAvailable;

            var iconOffset = 1u; // Default offset

            if (quest.IsRepeatable) iconOffset = 2u; // Offset for repeatable quests

            var finalIconId = baseIconId + iconOffset;

            var lookup = new GameIconLookup(finalIconId);
            var sharedTexture = Service.TextureProvider.GetFromGameIcon(lookup);

            if (sharedTexture.TryGetWrap(out var textureWrap, out _))
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5);
                ImGui.Image(textureWrap.Handle, new Vector2(size, size));

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Image(textureWrap.Handle, new Vector2(size * 4, size * 4));
                    ImGui.EndTooltip();
                }
            }
            else ImGui.Text("[Missing Icon]");
        }
        catch
        {
            /* ignored */
        }
    }
}
