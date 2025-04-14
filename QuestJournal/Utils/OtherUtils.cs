using System;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public class OtherUtils
{
    public static void OpenStarterLocation(QuestModel quest, IPluginLog log)
    {
        if (quest.StarterNpcLocation == null)
        {
            log.Warning("Starter NPC location is unavailable.");
            return;
        }

        var location = quest.StarterNpcLocation;
        if (location.TerritoryId == 0 || location.MapId == 0)
        {
            log.Warning($"Invalid location data for starter NPC: {quest.StarterNpc}.");
            return;
        }

        try
        {
            var mapLink = new MapLinkPayload(
                location.TerritoryId,
                location.MapId,
                (int)(location.X * 1_000f),
                (int)(location.Z * 1_000f)
            );

            Service.GameGui.OpenMapWithMapLink(mapLink);

            log.Info(
                $"Opened map for starter NPC: {quest.StarterNpc} at coordinates X: {location.X}, Z: {location.Z}. Territory: {location.TerritoryId}, Map: {location.MapId}");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to open map for starter NPC: {quest.StarterNpc}. Exception: {ex.Message}");
        }
    }

    public static void OpenMapAtLocation(QuestModel quest, IPluginLog log)
    {
        try
        {
            if (quest.Rewards?.LocationReward != null)
                Service.GameGui.OpenMapWithMapLink(
                    new MapLinkPayload(quest.Rewards.LocationReward.TerritoryId, quest.Rewards.LocationReward.MapId, 0,
                                       0));
        }
        catch (Exception ex)
        {
            log.Error($"Failed to open map {quest.Rewards?.LocationReward}. Exception: {ex.Message}");
        }
    }

    public static QuestStatusModel GetQuestStatus(QuestModel quest)
    {
        bool isComplete;
        bool isAccepted;
        unsafe
        {
            isAccepted = QuestManager.Instance()->IsQuestAccepted(quest.QuestId);

            isComplete = quest.IsRepeatable
                             ? QuestManager.Instance()->IsDailyQuestCompleted((ushort)quest.QuestId)
                             : QuestManager.IsQuestComplete(quest.QuestId);
        }

        return new QuestStatusModel
        {
            StatusSymbol = isComplete ? "\u2713" : isAccepted ? "\u2192" : "\u00d7",
            HoverText = isComplete ? "Complete" : isAccepted ? "In Progress" : "Not Started"
        };
    }
}
