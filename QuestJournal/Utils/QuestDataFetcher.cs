using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using QuestJournal.Models;
using Level = QuestJournal.Models.Level;

namespace QuestJournal.Utils;

public class QuestDataFetcher
{
    private readonly IDataManager dataManager;
    private readonly IPluginLog log;

    public QuestDataFetcher(IDataManager dataManager, IPluginLog log)
    {
        this.dataManager = dataManager;
        this.log = log;
    }

    // Main Public Methods

    /// <summary>
    ///     Fetches all quests and their details.
    /// </summary>
    public List<QuestModel?> GetAllQuests()
    {
        var exVersionSheet = dataManager.GetExcelSheet<ExVersion>();
        var journalGenreSheet = dataManager.GetExcelSheet<JournalGenre>();
        var questSheet = dataManager.GetExcelSheet<Quest>();
        var itemSheet = dataManager.GetExcelSheet<Item>();
        var stainSheet = dataManager.GetExcelSheet<Stain>();

        var questInfoLookup = questSheet.ToDictionary(
            quest => quest.RowId,
            quest => BuildQuestInfo(quest, exVersionSheet, journalGenreSheet, itemSheet, stainSheet)
        );

        foreach (var quest in questSheet)
        {
            if (questInfoLookup.TryGetValue(quest.RowId, out var questInfo) && quest.PreviousQuest.Count > 0)
            {
                var prevQuestIds = GetPrerequisiteQuestIds(quest.PreviousQuest);

                foreach (var prevQuestId in prevQuestIds)
                {
                    if (questInfoLookup.TryGetValue(prevQuestId, out var prevQuestInfo))
                    {
                        prevQuestInfo?.NextQuestIds?.Add(quest.RowId);
                        prevQuestInfo?.NextQuestTitles?.Add(quest.Name.ToString());
                    }
                }
            }
        }
        return questInfoLookup.Values.ToList();
    }

    /// <summary>
    /// Groups main scenario quests by categories and fetches details.
    /// </summary>
    public Dictionary<string, List<QuestModel>> GetMainScenarioQuestsByCategory()
    {
        var allQuests = GetAllQuests();
        var msqCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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

        var categorizedQuests = new Dictionary<string, List<QuestModel>>(StringComparer.OrdinalIgnoreCase);

        foreach (var category in msqCategories)
        {
            categorizedQuests[category] = new List<QuestModel>();
        }

        foreach (var quest in allQuests)
        {
            if (quest == null) continue;

            var categoryName = quest.JournalGenre?.JournalCategory?.Name;
            if (categoryName != null && msqCategories.Contains(categoryName))
            {
                categorizedQuests[categoryName].Add(quest);
            }
        }

        return categorizedQuests;
    }

    /// <summary>
    ///     Saves a list of quests to a JSON file.
    /// </summary>
    public void SaveQuestDataToJson(List<QuestModel?> questData, string filePath)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(questData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            log.Error($"Failed to save quest data to JSON file at '{filePath}': {ex.Message}");
            throw;
        }
    }

    // IQuestInfo Builders

    /// <summary>
    ///     Builds a IQuestInfo object from raw quest data.
    /// </summary>
    private QuestModel? BuildQuestInfo(
        Quest questData, ExcelSheet<ExVersion>? exVersionSheet, ExcelSheet<JournalGenre>? journalGenreSheet, ExcelSheet<Item>? itemSheet, ExcelSheet<Stain>? stainSheet)
    {
        try
        {
            var previousQuestIds = GetPrerequisiteQuestIds(questData.PreviousQuest);
            var previousQuestTitles = GetPrerequisiteQuestTitles(questData.PreviousQuest);

            var expansionName = GetExpansionName(questData.Expansion, exVersionSheet, questData.Id);
            var journalGenreDetails = GetJournalGenreDetails(questData.JournalGenre, journalGenreSheet, questData.Id);
            var reward = GetRewards(questData, itemSheet, stainSheet);

            var questDetails = new QuestModel
            {
                QuestId = questData.RowId,
                QuestTitle = questData.Name.ToString(),
                PreviousQuestIds = previousQuestIds,
                PreviousQuestTitles = previousQuestTitles,
                NextQuestIds = new List<uint>(),
                NextQuestTitles = new List<string>(),
                StarterNpc = ResolveNpcName(questData.IssuerStart, dataManager),
                StarterNpcLocation = ResolveNpcLocation(questData, dataManager),
                FinishNpc = ResolveNpcName(questData.TargetEnd, dataManager),
                Expansion = expansionName,
                JournalGenre = journalGenreDetails,
                SortKey = questData.SortKey,
                Icon = questData.Icon,
                IconSpecial = questData.IconSpecial,
                Rewards = reward
            };
            
            return questDetails;
        }
        catch (Exception ex)
        {
            log.Error($"Failed to build IQuestInfo for QuestId {questData.RowId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Resolves rewards for the quest.
    /// </summary>
    private Reward? GetRewards(Quest quest, ExcelSheet<Item>? itemSheet, ExcelSheet<Stain>? stainSheet)
    {
        var level = quest.LevelMax != 0 ? quest.LevelMax : quest.ClassJobLevel.FirstOrDefault();
        var paramGrow = dataManager.GetExcelSheet<ParamGrow>().GetRow(level);
        var exp = paramGrow.ScaledQuestXP * paramGrow.QuestExpModifier * quest.ExpFactor / 100;
        
        var catalysts = GetCatalysts(quest, itemSheet);
        var currencyReward = GetCurrencyReward(quest, itemSheet);
        var itemReward = GetItemReward(quest, itemSheet, stainSheet);

        return new Reward()
        {
            Exp = exp,
            Gil = quest.GilReward,
            Currency = currencyReward,
            Catalysts = catalysts,
            Items = itemReward
        };
    }
    
    public List<ItemsReward> GetItemReward(Quest quest, ExcelSheet<Item>? itemSheet, ExcelSheet<Stain>? stainSheet)
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
                if (stain != null)
                {
                    stainName = stain.Value.Name.ToString();
                }
            }

            if (item != null)
            {
                rewards.Add(new ItemsReward
                {
                    ItemId = itemRef.RowId,
                    ItemName = item.Value.Name.ToString(),
                    Count = (byte)(quest.ItemCountReward.Count > i ? quest.ItemCountReward[i] : 0),
                    Stain = stainName
                });
            }
        }

        return rewards;
    }
    
    private CurrencyReward? GetCurrencyReward(Quest quest, ExcelSheet<Item>? itemSheet)
    {
        if (quest.CurrencyReward.RowId == 0)
            return null;

        var currencyRow = itemSheet?.GetRow(quest.CurrencyReward.RowId);
        if (currencyRow == null)
            return null;

        return new CurrencyReward
        {
            CurrencyId = quest.CurrencyReward.RowId,
            CurrencyName = currencyRow.Value.Name.ToString(),
            Count = quest.CurrencyRewardCount
        };
    }
    
    private List<CatalystReward> GetCatalysts(Quest quest, ExcelSheet<Item>? itemSheet)
    {
        var catalysts = new List<CatalystReward>();

        for (int i = 0; i < quest.ItemCatalyst.Count; i++)
        {
            var itemRef = quest.ItemCatalyst[i];
            if (itemRef.RowId == 0)
                continue;

            var itemCount = quest.ItemCountCatalyst.ElementAtOrDefault(i);
            var currentItemName = itemSheet?.GetRow(itemRef.RowId).Name.ToString();

            if (currentItemName == null)
                continue;

            var matchedItem = itemSheet?.FirstOrDefault(item => item.Name.ToString() == currentItemName);
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


    /// <summary>
    ///     Resolves JournalGenreDetails for the quest.
    /// </summary>
    private JournalGenreDetails? GetJournalGenreDetails(
        RowRef<JournalGenre> journalGenreRef, ExcelSheet<JournalGenre>? journalGenreSheet, ReadOnlySeString questId)
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
                    Name = journalCategory.Name.ToString()
                };
            }

            return new JournalGenreDetails
            {
                Id = journalGenre.RowId,
                Name = journalGenre.Name.ToString(),
                JournalCategory = journalCategoryDetails
            };
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve JournalGenre for Quest {questId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Resolves the IDs of prerequisite quests.
    /// </summary>
    private List<uint> GetPrerequisiteQuestIds(Collection<RowRef<Quest>> previousQuests)
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

    /// <summary>
    ///     Resolves the titles of prerequisite quests.
    /// </summary>
    private List<string> GetPrerequisiteQuestTitles(Collection<RowRef<Quest>> previousQuests)
    {
        var titles = new List<string>();
        foreach (var preQuestRef in previousQuests)
        {
            if (preQuestRef.RowId <= 0) continue;

            try
            {
                var preQuest = preQuestRef.Value;
                titles.Add(preQuest.Name.ToString());
            }
            catch (Exception ex)
            {
                log.Error($"Failed to resolve PreviousQuestTitle for RowId {preQuestRef.RowId}: {ex.Message}");
            }
        }

        return titles;
    }

    /// <summary>
    ///     Resolves the expansion name for the quest.
    /// </summary>
    private string GetExpansionName(
        RowRef<ExVersion> expansionRef, ExcelSheet<ExVersion>? exVersionSheet, ReadOnlySeString questId)
    {
        if (expansionRef.RowId <= 0 || exVersionSheet == null) return "";

        try
        {
            var expansionRow = exVersionSheet.GetRow(expansionRef.RowId);
            return expansionRow.Name.ToString() ?? "";
        }
        catch (Exception ex)
        {
            log.Error($"Failed to resolve Expansion for Quest {questId}: {ex.Message}");
            return "";
        }
    }

    /// <summary>
    ///     Resolves the NPC name for the given RowRef.
    /// </summary>
    private string? ResolveNpcName(RowRef npcRef, IDataManager dataManager)
    {
        if (npcRef.RowId == 0) return null;

        try
        {
            var enpcSheet = dataManager.GetExcelSheet<ENpcResident>();
            try
            {
                var npc = enpcSheet.GetRow(npcRef.RowId);
                return npc.Singular.ToString();
            }
            catch (ArgumentOutOfRangeException) { return null; }
        }
        catch (Exception ex)
        {
            log.Error($"Error resolving NPC name for RowId {npcRef.RowId}: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Resolves the NPC's location based on the issuer (NPC location).
    /// </summary>
    private Level? ResolveNpcLocation(Quest questData, IDataManager dataManager)
    {
        try
        {
            var levelSheet = dataManager.GetExcelSheet<Lumina.Excel.Sheets.Level>();
            var npcLocationRef = questData.IssuerLocation;

            if (npcLocationRef.RowId == 0)
            {
                log.Warning($"Invalid IssuerLocation RowId for QuestId: {questData.RowId}. Skipping.");
                return null;
            }

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
