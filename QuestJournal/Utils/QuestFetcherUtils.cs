using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using QuestJournal.Models;
using Action = Lumina.Excel.Sheets.Action;
using Level = QuestJournal.Models.Level;

namespace QuestJournal.Utils;

public class QuestFetcherUtils(IDataManager dataManager, IPluginLog log)
{
    private readonly ExcelSheet<Action>? actionSheet = dataManager.GetExcelSheet<Action>();
    private readonly ExcelSheet<ContentFinderCondition>? contentFinderConditionSheet = dataManager.GetExcelSheet<ContentFinderCondition>();
    private readonly ExcelSheet<ContentType>? contentTypeSheet = dataManager.GetExcelSheet<ContentType>();
    private readonly ExcelSheet<Emote>? emoteSheet = dataManager.GetExcelSheet<Emote>();
    private readonly ExcelSheet<ExVersion>? exVersionSheet = dataManager.GetExcelSheet<ExVersion>();
    private readonly ExcelSheet<GeneralAction>? generalActionSheet = dataManager.GetExcelSheet<GeneralAction>();
    private readonly ExcelSheet<Item>? itemSheet = dataManager.GetExcelSheet<Item>();
    private readonly ExcelSheet<JournalGenre>? journalGenreSheet = dataManager.GetExcelSheet<JournalGenre>();
    private readonly ExcelSheet<Lumina.Excel.Sheets.Level>? levelSheet = dataManager.GetExcelSheet<Lumina.Excel.Sheets.Level>();
    private readonly ExcelSheet<ParamGrow>? paramGrowSheet = dataManager.GetExcelSheet<ParamGrow>();
    private readonly ExcelSheet<QuestRewardOther>? questRewardOtherSheet = dataManager.GetExcelSheet<QuestRewardOther>();
    private readonly ExcelSheet<Stain>? stainSheet = dataManager.GetExcelSheet<Stain>();

    public QuestModel? BuildQuestInfo(Quest questData)
    {
        if (questData.RowId == 65536) return null;

        try
        {
            var questDetails = new QuestModel
            {
                QuestId = questData.RowId,
                QuestTitle = questData.Name.ExtractText(),
                PreviousQuestIds = GetPrerequisiteQuestIds(questData.PreviousQuest),
                PreviousQuestTitles = GetPrerequisiteQuestTitles(questData.PreviousQuest),
                NextQuestIds = new List<uint>(),
                NextQuestTitles = new List<string>(),

                // Location
                StarterNpc = ResolveNpcName(questData.IssuerStart),
                StarterNpcLocation = ResolveNpcLocation(questData),
                FinishNpc = ResolveNpcName(questData.TargetEnd),

                // Organisation
                Expansion = GetExpansionName(questData.Expansion, questData.Id),
                JournalGenre = GetJournalGenreDetails(questData.JournalGenre, questData.Id),
                SortKey = questData.SortKey,

                // Other
                IsRepeatable = questData.IsRepeatable,

                // Icons
                EventIcon = GetEventIcon(questData),
                Icon = questData.Icon,

                // Requirements
                JobLevel = questData.ClassJobLevel.FirstOrDefault(),
                ClassJobCategory = questData.ClassJobCategory0.Value.Name.ExtractText(),
                BeastTribeRequirements = new BeastTribeRequirements
                {
                    BeastTribeName = questData.BeastTribe.Value.Name.ExtractText(),
                    BeastTribeRank = questData.BeastReputationRank.Value.Name.ExtractText()
                },

                // Rewards
                Rewards = GetRewards(questData)
            };

            return questDetails;
        }
        catch (Exception ex)
        {
            log.Error(
                $"Failed to build QuestInfo for QuestId {questData.RowId}: {ex.Message}\nStack Trace: {ex.StackTrace}");
            return null;
        }
    }

    private Reward? GetRewards(Quest quest)
    {
        var level = quest.LevelMax != 0 ? quest.LevelMax : quest.ClassJobLevel.FirstOrDefault();
        if (paramGrowSheet != null)
        {
            var paramGrow = paramGrowSheet.GetRow(level);
            var exp = paramGrow.ScaledQuestXP * paramGrow.QuestExpModifier * quest.ExpFactor / 100;

            return new Reward
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
                InstanceContentUnlock = GetAllInstanceContentUnlockRewards(quest),
                ReputationReward = GetReputationReward(quest)
            };
        }

        return null;
    }

    private ReputationReward GetReputationReward(Quest quest)
    {
        return new ReputationReward
        {
            ReputationId = quest.BeastTribe.Value.RowId,
            ReputationName = quest.BeastTribe.Value.Name.ExtractText(),
            Count = quest.ReputationReward
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

            var item = itemSheet?.GetRow(itemRef.RowId);
            string? stainName = null;

            if (i < quest.RewardStain.Count && quest.RewardStain[i].RowId != 0)
            {
                var stain = stainSheet?.GetRow(quest.RewardStain[i].RowId);
                if (stain != null) stainName = stain.Value.Name.ExtractText();
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

    private uint GetEventIcon(Quest quest)
    {
        try
        {
            if (quest.EventIconType.RowId != 0)
            {
                var resolvedEventIcon = quest.EventIconType.Value;
                return resolvedEventIcon.RowId;
            }
        }
        catch
        {
            /* ignored */
        }

        return 0;
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

            var item = itemSheet?.GetRow(itemRef.RowId);
            string? stainName = null;
            var isHq = false;

            // Stain information
            if (i < quest.OptionalItemStainReward.Count && quest.OptionalItemStainReward[i].RowId != 0)
            {
                var stain = stainSheet?.GetRow(quest.OptionalItemStainReward[i].RowId);
                if (stain != null) stainName = stain.Value.Name.ExtractText();
            }

            // Is HQ
            if (i < quest.OptionalItemIsHQReward.Count) isHq = quest.OptionalItemIsHQReward[i];

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

        var currencyRow = itemSheet?.GetRow(quest.CurrencyReward.RowId);
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

        for (var i = 0; i < quest.ItemCatalyst.Count; i++)
        {
            var itemRef = quest.ItemCatalyst[i];
            if (itemRef.RowId == 0)
                continue;

            var itemCount = quest.ItemCountCatalyst.ElementAtOrDefault(i);
            var currentItemName = itemSheet?.GetRow(itemRef.RowId).Name.ExtractText();

            if (currentItemName == null)
                continue;

            var matchedItem = itemSheet?.FirstOrDefault(item => item.Name.ExtractText() == currentItemName);
            var matchedItemId = matchedItem?.RowId ?? 0;

            catalysts.Add(new CatalystReward
            {
                ItemId = matchedItemId,
                ItemName = currentItemName,
                Count = itemCount
            });
        }

        return catalysts;
    }

    private EmoteReward? GetEmoteReward(Quest quest)
    {
        if (quest.EmoteReward.RowId == 0) return null;

        var emote = emoteSheet?.GetRow(quest.EmoteReward.RowId);
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

        var action = actionSheet?.GetRow(quest.ActionReward.RowId);
        if (action == null) return null;

        return new ActionReward
        {
            Id = quest.ActionReward.RowId,
            ActionName = action.Value.Name.ExtractText()
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

            var generalAction = generalActionSheet?.GetRow(generalActionRef.RowId);
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

        var otherReward = questRewardOtherSheet?.GetRow(otherRewardRef.RowId);
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

    private List<InstanceContentUnlockReward>? GetQuestInstanceContentUnlockReward(Quest quest)
    {
        try
        {
            var instanceRef = quest.InstanceContentUnlock;
            if (instanceRef.RowId == 0)
                return null;

            var instanceContent = instanceRef.Value;

            return new List<InstanceContentUnlockReward>
            {
                new()
                {
                    InstanceId = instanceContent.RowId,
                    InstanceName = instanceContent.ContentFinderCondition.Value.Name.ExtractText(),
                    ContentType = instanceContent.InstanceContentType.Value.RowId
                }
            };
        }
        catch (Exception ex)
        {
            log.Error(
                $"Failed to fetch InstanceContentUnlockReward for Quest: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return null;
        }
    }

    private List<InstanceContentUnlockReward> GetContentFinderConditionRewards(uint questId)
    {
        try
        {
            if (contentFinderConditionSheet == null || contentTypeSheet == null)
                return new List<InstanceContentUnlockReward>();

            uint ResolveContentType(RowRef<ContentType> contentTypeRef)
            {
                var contentType = contentTypeSheet.GetRow(contentTypeRef.RowId);
                return contentType.RowId;
            }

            var rewards = contentFinderConditionSheet
                          .Where(c =>
                          {
                              if (c.Unknown37 == 1)
                              {
                                  // Use Unknown31 as the quest ID (instead of UnlockQuest)
                                  return c.Unknown31 == questId;
                              }

                              return c.UnlockQuest.RowId == questId;
                          })
                          .Select(content =>
                          {
                              // Handle Unknown36 to determine unlock logic
                              // 0: No requirements, 1: Quest requirement, 2: Instance + quest, 3: Special case
                              if (content.Unknown36 == 2 && content.Unknown31 != 0)
                              {
                                  // Unreal unlocks: Requires both instance content and a quest (special logic)
                                  return new InstanceContentUnlockReward
                                  {
                                      InstanceId = content.RowId,
                                      InstanceName = content.Name.ExtractText(),
                                      ContentType = ResolveContentType(content.ContentType)
                                  };
                              }

                              if (content.Unknown36 == 3)
                              {
                                  // Special case for entries like "The Calamity Untold"
                                  return new InstanceContentUnlockReward
                                  {
                                      InstanceId = content.RowId,
                                      InstanceName = content.Name.ExtractText() + " (Calamity Untold)",
                                      ContentType = ResolveContentType(content.ContentType)
                                  };
                              }

                              // Default case for normal unlocks
                              return new InstanceContentUnlockReward
                              {
                                  InstanceId = content.RowId,
                                  InstanceName = content.Name.ExtractText(),
                                  ContentType = ResolveContentType(content.ContentType)
                              };
                          })
                          .ToList();

            return rewards;
        }
        catch (Exception ex)
        {
            log.Error(
                $"Failed to fetch rewards from ContentFinderConditionSheet: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return new List<InstanceContentUnlockReward>();
        }
    }

    private List<InstanceContentUnlockReward> GetAllInstanceContentUnlockRewards(Quest quest)
    {
        var questRewards = GetQuestInstanceContentUnlockReward(quest) ?? new List<InstanceContentUnlockReward>();

        var sheetRewards = GetContentFinderConditionRewards(quest.RowId);

        var allRewards = questRewards
                         .Concat(sheetRewards)
                         .GroupBy(r => r.InstanceId)
                         .Select(g => g.First())
                         .OrderBy(r => r.InstanceName)
                         .ToList();

        return allRewards;
    }

    private JournalGenreDetails? GetJournalGenreDetails(RowRef<JournalGenre> journalGenreRef, ReadOnlySeString questId)
    {
        if (journalGenreSheet == null || journalGenreRef.RowId <= 0) return null;

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
        if (expansionRef.RowId <= 0 || exVersionSheet == null) return "";

        try
        {
            var expansionRow = exVersionSheet.GetRow(expansionRef.RowId);
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
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
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
