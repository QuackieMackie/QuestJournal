using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using QuestJournal.Models;
using Level = QuestJournal.Models.Level;

namespace QuestJournal.Utils;

public class QuestFetcherUtils(IDataManager dataManager, IPluginLog log)
{
    private readonly ExcelSheet<ExVersion>? exVersionSheet = dataManager.GetExcelSheet<ExVersion>();
    private readonly ExcelSheet<JournalGenre>? journalGenreSheet = dataManager.GetExcelSheet<JournalGenre>();
    private readonly ExcelSheet<Quest>? questSheet = dataManager.GetExcelSheet<Quest>();
    private readonly ExcelSheet<Item>? itemSheet = dataManager.GetExcelSheet<Item>();
    private readonly ExcelSheet<Stain>? stainSheet = dataManager.GetExcelSheet<Stain>();
    private readonly ExcelSheet<Emote>? emoteSheet = dataManager.GetExcelSheet<Emote>();
    private readonly ExcelSheet<Lumina.Excel.Sheets.Action>? actionSheet = dataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>();
    private readonly ExcelSheet<GeneralAction>? generalActionSheet = dataManager.GetExcelSheet<GeneralAction>();
    private readonly ExcelSheet<QuestRewardOther>? questRewardOtherSheet = dataManager.GetExcelSheet<QuestRewardOther>();
    private readonly ExcelSheet<ContentFinderCondition>? contentFinderConditionSheet = dataManager.GetExcelSheet<ContentFinderCondition>();
    private readonly ExcelSheet<ContentType>? contentTypeSheet = dataManager.GetExcelSheet<ContentType>();
    private readonly ExcelSheet<EventIconType>? eventIconTypeSheet = dataManager.GetExcelSheet<EventIconType>();

    public ExcelSheet<ExVersion>? ExVersionSheet => exVersionSheet;
    public ExcelSheet<JournalGenre>? JournalGenreSheet => journalGenreSheet;
    public ExcelSheet<Quest>? QuestSheet => questSheet;
    public ExcelSheet<Item>? ItemSheet => itemSheet;
    public ExcelSheet<Stain>? StainSheet => stainSheet;
    public ExcelSheet<Emote>? EmoteSheet => emoteSheet;
    public ExcelSheet<Lumina.Excel.Sheets.Action>? ActionSheet => actionSheet;
    public ExcelSheet<GeneralAction>? GeneralActionSheet => generalActionSheet;
    public ExcelSheet<QuestRewardOther>? QuestRewardOtherSheet => questRewardOtherSheet;
    public ExcelSheet<ContentFinderCondition>? ContentFinderConditionSheet => contentFinderConditionSheet;
    public ExcelSheet<ContentType>? ContentTypeSheet => contentTypeSheet;
    public ExcelSheet<EventIconType>? EventIconTypes => eventIconTypeSheet;

    public QuestModel? BuildQuestInfo(Quest questData)
    {
        try
        {
            var questId = questData.RowId;

            var questDetails = new QuestModel
            {
                QuestId = questId,
                QuestTitle = questData.Name.ExtractText(),
                PreviousQuestIds = GetPrerequisiteQuestIds(questData.PreviousQuest),
                PreviousQuestTitles = GetPrerequisiteQuestTitles(questData.PreviousQuest),
                NextQuestIds = new List<uint>(),
                NextQuestTitles = new List<string>(),
                StarterNpc = ResolveNpcName(questData.IssuerStart),
                StarterNpcLocation = ResolveNpcLocation(questData),
                FinishNpc = ResolveNpcName(questData.TargetEnd),
                Expansion = GetExpansionName(questData.Expansion, questData.Id),
                JournalGenre = GetJournalGenreDetails(questData.JournalGenre, questData.Id),
                SortKey = questData.SortKey,
                EventIcon = questData.EventIconType.Value.RowId,
                Icon = questData.Icon,
                IconSpecial = questData.IconSpecial,
                Rewards = GetRewards(questId, questData)
            };

            return questDetails;
        }
        catch { return null; }
    }

    private Reward GetRewards(uint questId, Quest quest)
    {
        var level = quest.LevelMax != 0 ? quest.LevelMax : quest.ClassJobLevel.FirstOrDefault();
        var paramGrow = dataManager.GetExcelSheet<ParamGrow>().GetRow(level);
        var exp = paramGrow.ScaledQuestXP * paramGrow.QuestExpModifier * quest.ExpFactor / 100;
        
        return new Reward()
        {
            Exp = exp,
            Gil = quest.GilReward,
            Currency = GetCurrencyReward(quest),
            Catalysts = GetCatalysts(quest),
            Items = GetItemReward(quest),
            OptionalItems = GetOptionalItemReward(quest),
            Emote = GetEmoteReward(quest),
            Action = GetActionReward(quest),
            GeneralActions = GetGeneralActionRewards(quest),
            OtherReward = GetOtherReward(quest),
            InstanceContentUnlock = GetInstanceContentUnlockReward(questId)
        };
    }
    
    public List<ItemsReward> GetItemReward(Quest quest)
    {
        var itemRewards = quest.Reward;

        if (itemRewards.Count == 0)
            return new List<ItemsReward>();

        var rewards = new List<ItemsReward>();

        for (var i = 0; i < itemRewards.Count; i++)
        {
            var itemRef = itemRewards[i];

            if (itemRef.RowId == 0)
                continue;

            var item = ItemSheet?.GetRow(itemRef.RowId);
            string? stainName = null;

            if (i < quest.RewardStain.Count && quest.RewardStain[i].RowId != 0)
            {
                var stain = StainSheet?.GetRow(quest.RewardStain[i].RowId);
                if (stain != null)
                {
                    stainName = stain.Value.Name.ExtractText();
                }
            }

            if (item != null)
            {
                rewards.Add(new ItemsReward
                {
                    ItemId = itemRef.RowId,
                    ItemName = item.Value.Name.ExtractText(),
                    Count = (byte)(quest.ItemCountReward.Count > i ? quest.ItemCountReward[i] : 0),
                    Stain = stainName
                });
            }
        }

        return rewards;
    }
    
    public List<OptionalItemsReward> GetOptionalItemReward(Quest quest)
    {
        var optionalItemRewards = quest.OptionalItemReward;

        if (optionalItemRewards.Count == 0)
            return new List<OptionalItemsReward>();

        var rewards = new List<OptionalItemsReward>();

        for (var i = 0; i < optionalItemRewards.Count; i++)
        {
            var itemRef = optionalItemRewards[i];

            if (itemRef.RowId == 0)
                continue;

            var item = ItemSheet?.GetRow(itemRef.RowId);
            string? stainName = null;
            bool isHq = false;

            // Stain information
            if (i < quest.OptionalItemStainReward.Count && quest.OptionalItemStainReward[i].RowId != 0)
            {
                var stain = StainSheet?.GetRow(quest.OptionalItemStainReward[i].RowId);
                if (stain != null)
                {
                    stainName = stain.Value.Name.ExtractText();
                }
            }

            // Is HQ
            if (i < quest.OptionalItemIsHQReward.Count)
            {
                isHq = quest.OptionalItemIsHQReward[i];
            }

            if (item != null)
            {
                rewards.Add(new OptionalItemsReward
                {
                    ItemId = itemRef.RowId,
                    ItemName = item.Value.Name.ExtractText(),
                    Count = (byte)(quest.OptionalItemCountReward.Count > i ? quest.OptionalItemCountReward[i] : 0),
                    Stain = stainName,
                    IsHq = isHq
                });
            }
        }

        return rewards;
    }
    
    private CurrencyReward? GetCurrencyReward(Quest quest)
    {
        if (quest.CurrencyReward.RowId == 0)
            return null;

        var currencyRow = ItemSheet?.GetRow(quest.CurrencyReward.RowId);
        if (currencyRow == null)
            return null;

        return new CurrencyReward
        {
            CurrencyId = quest.CurrencyReward.RowId,
            CurrencyName = currencyRow.Value.Name.ExtractText(),
            Count = quest.CurrencyRewardCount
        };
    }
    
    private List<CatalystReward> GetCatalysts(Quest quest)
    {
        var catalysts = new List<CatalystReward>();

        for (int i = 0; i < quest.ItemCatalyst.Count; i++)
        {
            var itemRef = quest.ItemCatalyst[i];
            if (itemRef.RowId == 0)
                continue;

            var itemCount = quest.ItemCountCatalyst.ElementAtOrDefault(i);
            var currentItemName = ItemSheet?.GetRow(itemRef.RowId).Name.ExtractText();

            if (currentItemName == null)
                continue;

            var matchedItem = ItemSheet?.FirstOrDefault(item => item.Name.ExtractText() == currentItemName);
            var matchedItemId = matchedItem?.RowId ?? 0;

            catalysts.Add(new CatalystReward
            {
                ItemId = matchedItemId,
                ItemName = currentItemName,
                Count = itemCount,
            });
        }

        return catalysts;
    }

    private EmoteReward? GetEmoteReward(Quest quest)
    {
        if (quest.EmoteReward.RowId == 0) return null;

        var emote = EmoteSheet?.GetRow(quest.EmoteReward.RowId);
        if (emote == null) return null;

        return new EmoteReward
        {
            Id = quest.EmoteReward.RowId,
            EmoteName = emote.Value.Name.ExtractText()
        };
    }
    
    private ActionReward? GetActionReward(Quest quest)
    {
        if (quest.ActionReward.RowId == 0) return null;

        var action = ActionSheet?.GetRow(quest.ActionReward.RowId);
        if (action == null) return null;

        return new ActionReward
        {
            Id = quest.ActionReward.RowId,
            ActionName = action.Value.Name.ExtractText(),
        };
    }
    
    private List<GeneralActionReward> GetGeneralActionRewards(Quest quest)
    {
        var generalActionRewards = quest.GeneralActionReward;

        if (generalActionRewards.Count == 0)
            return new List<GeneralActionReward>();

        var rewards = new List<GeneralActionReward>();

        foreach (var generalActionRef in generalActionRewards)
        {
            if (generalActionRef.RowId == 0)
                continue;

            var generalAction = GeneralActionSheet?.GetRow(generalActionRef.RowId);
            if (generalAction != null)
            {
                rewards.Add(new GeneralActionReward
                {
                    Id = generalAction.Value.RowId,
                    Name = generalAction.Value.Name.ExtractText()
                });
            }
        }

        return rewards;
    }
    
    private OtherReward? GetOtherReward(Quest quest)
    {
        var otherRewardRef = quest.OtherReward;

        if (otherRewardRef.RowId == 0)
            return null;

        var otherReward = QuestRewardOtherSheet?.GetRow(otherRewardRef.RowId);
        if (otherReward != null)
        {
            return new OtherReward
            {
                Id = otherReward.Value.RowId,
                Name = otherReward.Value.Name.ExtractText()
            };
        }

        return null;
    }
    
    private List<InstanceContentUnlockReward>? GetInstanceContentUnlockReward(uint questId)
    {
        if (ContentFinderConditionSheet == null || ContentTypeSheet == null)
            return null;

        uint ResolveContentType(RowRef<ContentType> contentTypeRef)
        {
            var contentType = ContentTypeSheet.GetRow(contentTypeRef.RowId);
            return contentType.RowId;
        }

        var instanceContentRewards = ContentFinderConditionSheet
                                     .Where(c => c.UnlockQuest.RowId == questId)
                                     .Select(content => new InstanceContentUnlockReward
                                     {
                                         InstanceId = content.RowId,
                                         InstanceName = content.Name.ExtractText(),
                                         ContentType = ResolveContentType(content.ContentType)
                                     })
                                     .ToList();

        var quest = QuestSheet?.GetRow(questId);
        if (quest != null)
        {
            var instanceContentUnlockRowId = quest.Value.InstanceContentUnlock.RowId;
            if (instanceContentUnlockRowId != 0)
            {
                var matchingContent = ContentFinderConditionSheet.GetRow(instanceContentUnlockRowId);
                if (matchingContent.RowId != 0)
                {
                    instanceContentRewards.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = matchingContent.RowId,
                        InstanceName = matchingContent.Name.ExtractText(),
                        ContentType = ResolveContentType(matchingContent.ContentType)
                    });
                }
            }
        }

        return instanceContentRewards.Count > 0 ? instanceContentRewards : null;
    }

    private JournalGenreDetails? GetJournalGenreDetails(RowRef<JournalGenre> journalGenreRef, ReadOnlySeString questId)
    {
        if (JournalGenreSheet == null || journalGenreRef.RowId <= 0) return null;

        try
        {
            var journalGenre = journalGenreRef.Value;

            JournalCategoryDetails? journalCategoryDetails = null;
            if (journalGenre.JournalCategory.RowId > 0)
            {
                var journalCategory = journalGenre.JournalCategory.Value;
                journalCategoryDetails = new JournalCategoryDetails
                {
                    Id = journalCategory.RowId,
                    Name = journalCategory.Name.ExtractText()
                };
            }

            return new JournalGenreDetails
            {
                Id = journalGenre.RowId,
                Name = journalGenre.Name.ExtractText(),
                JournalCategory = journalCategoryDetails
            };
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve JournalGenre for Quest {questId}: {ex.Message}");
            return null;
        }
    }

    public List<uint> GetPrerequisiteQuestIds(Collection<RowRef<Quest>> previousQuests)
    {
        var ids = new List<uint>();
        foreach (var preQuestRef in previousQuests)
        {
            if (preQuestRef.RowId <= 0) continue;

            try
            {
                ids.Add(preQuestRef.RowId);
            }
            catch (Exception ex)
            {
                log.Error($"Failed to resolve PreviousQuestId for RowId {preQuestRef.RowId}: {ex.Message}");
            }
        }

        return ids;
    }

    private List<string> GetPrerequisiteQuestTitles(Collection<RowRef<Quest>> previousQuests)
    {
        var titles = new List<string>();
        foreach (var preQuestRef in previousQuests)
        {
            if (preQuestRef.RowId <= 0) continue;

            try
            {
                var preQuest = preQuestRef.Value;
                titles.Add(preQuest.Name.ExtractText());
            }
            catch (Exception ex)
            {
                log.Error($"Failed to resolve PreviousQuestTitle for RowId {preQuestRef.RowId}: {ex.Message}");
            }
        }

        return titles;
    }

    private string GetExpansionName(RowRef<ExVersion> expansionRef, ReadOnlySeString questId)
    {
        if (expansionRef.RowId <= 0 || ExVersionSheet == null) return "";

        try
        {
            var expansionRow = ExVersionSheet.GetRow(expansionRef.RowId);
            return expansionRow.Name.ExtractText();
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve Expansion for Quest {questId}: {ex.Message}");
            return "";
        }
    }

    private string? ResolveNpcName(RowRef npcRef)
    {
        if (npcRef.RowId == 0) return null;

        try
        {
            var enpcSheet = dataManager.GetExcelSheet<ENpcResident>();
            try
            {
                var npc = enpcSheet.GetRow(npcRef.RowId);
                return npc.Singular.ExtractText();
            }
            catch (ArgumentOutOfRangeException) { return null; }
        }
        catch (Exception ex)
        {
            log.Error($"Error resolving NPC name for RowId {npcRef.RowId}: {ex.Message}");
            return null;
        }
    }
    
    private Level? ResolveNpcLocation(Quest questData)
    {
        try
        {
            var levelSheet = dataManager.GetExcelSheet<Lumina.Excel.Sheets.Level>();
            var npcLocationRef = questData.IssuerLocation;

            if (npcLocationRef.RowId == 0) return null;

            var levelRow = levelSheet.GetRow(npcLocationRef.RowId);
            if (levelRow.RowId == 0)
            {
                log.Error($"No Level data found for RowId {npcLocationRef.RowId} (QuestId: {questData.RowId}).");
                return null;
            }

            return new Level
            {
                RowId = levelRow.RowId,
                X = levelRow.X,
                Y = levelRow.Y,
                Z = levelRow.Z,
                Yaw = levelRow.Yaw,
                Radius = levelRow.Radius,
                MapId = levelRow.Map.RowId,
                TerritoryId = levelRow.Territory.RowId
            };
        }
        catch (ArgumentOutOfRangeException ex)
        {
            log.Error($"RowId out of range for QuestId: {questData.RowId}. Actual value: {ex.ActualValue}");
            return null;
        }
        catch (Exception ex)
        {
            log.Error($"Unexpected error resolving NPC location for QuestId: {questData.RowId}: {ex.Message}");
            return null;
        }
    }
}
