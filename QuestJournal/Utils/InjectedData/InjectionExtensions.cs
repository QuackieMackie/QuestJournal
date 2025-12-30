using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData;

public static class InjectionExtensions
{
    public static void AddInstance(this Dictionary<uint, Action<QuestModel>> injections, uint questId, uint instanceId, string name, uint type)
    {
        injections.AddAction(questId, quest =>
        {
            quest.Rewards ??= new Reward();
            quest.Rewards.InstanceContentUnlock ??= new List<InstanceContentUnlockReward>();
            quest.Rewards.InstanceContentUnlock.Add(new InstanceContentUnlockReward
            {
                InstanceId = instanceId,
                InstanceName = name,
                ContentType = type
            });
        });
    }

    public static void AddLocation(this Dictionary<uint, Action<QuestModel>> injections, uint questId, string placeName, uint mapId, uint territoryId)
    {
        injections.AddAction(questId, quest =>
        {
            quest.Rewards ??= new Reward();
            quest.Rewards.LocationReward = new LevelReward
            {
                PlaceName = placeName,
                MapId = mapId,
                TerritoryId = territoryId
            };
        });
    }

    public static void AddGeneralAction(this Dictionary<uint, Action<QuestModel>> injections, uint questId, uint actionId, string name)
    {
        injections.AddAction(questId, quest =>
        {
            quest.Rewards ??= new Reward();
            quest.Rewards.GeneralActions ??= new List<GeneralActionReward>();
            quest.Rewards.GeneralActions.Add(new GeneralActionReward
            {
                Id = actionId,
                Name = name
            });
        });
    }

    public static void AddComment(this Dictionary<uint, Action<QuestModel>> injections, uint questId, string comment, string? hover = null, bool copy = false)
    {
        injections.AddAction(questId, quest =>
        {
            quest.Rewards ??= new Reward();
            quest.Rewards.CommentSection = new CommentSection
            {
                GuiComment = comment,
                HoverTextComment = hover,
                ClickToCopy = copy
            };
        });
    }

    public static void SetSortKey(this Dictionary<uint, Action<QuestModel>> injections, uint questId, ushort sortKey)
    {
        injections.AddAction(questId, quest =>
        {
            quest.SortKey = sortKey;
        });
    }

    public static void SetCategory(this Dictionary<uint, Action<QuestModel>> injections, uint questId, uint categoryId, string categoryName)
    {
        injections.AddAction(questId, quest =>
        {
            quest.JournalGenre ??= new JournalGenreDetails();
            quest.JournalGenre.JournalCategory ??= new JournalCategoryDetails
            {
                Id = categoryId,
                Name = categoryName
            };
        });
    }

    public static void AddCustom(this Dictionary<uint, Action<QuestModel>> injections, uint questId, Action<QuestModel> action)
    {
        injections.AddAction(questId, action);
    }

    private static void AddAction(this Dictionary<uint, Action<QuestModel>> injections, uint questId, Action<QuestModel> action)
    {
        if (injections.TryGetValue(questId, out var existing))
        {
            injections[questId] = q =>
            {
                existing(q);
                action(q);
            };
        }
        else
        {
            injections[questId] = action;
        }
    }
}
