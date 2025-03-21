using System;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public class QuestHandler
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

            QuestJournal.GameGui.OpenMapWithMapLink(mapLink);

            log.Info(
                $"Opened map for starter NPC: {quest.StarterNpc} at coordinates X: {location.X}, Z: {location.Z}. Territory: {location.TerritoryId}, Map: {location.MapId}");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to open map for starter NPC: {quest.StarterNpc}. Exception: {ex.Message}");
        }
    }
}
