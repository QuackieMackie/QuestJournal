using System.Collections.Generic;

namespace QuestJournal.Models;

public class QuestModel
{
    public uint QuestId { get; init; }
    public string? QuestTitle { get; init; }
    
    public List<uint>? PreviousQuestIds { get; init; }
    public List<string>? PreviousQuestTitles { get; init; }

    public List<uint>? NextQuestIds { get; init; }
    public List<string>? NextQuestTitles { get; init; }

    // Locations
    public string? StarterNpc { get; init; }
    public Level? StarterNpcLocation { get; init; }
    public string? FinishNpc { get; init; }
    
    // Organisation
    public string? Expansion { get; init; }
    public JournalGenreDetails? JournalGenre { get; init; }
    public ushort SortKey { get; init; }
    
    // Icons
    public uint EventIcon { get; init; }
    public uint Icon { get; init; }
    
    // Requirements
    public uint? JobLevel { get; init; }
    public string? ClassJobCategory { get; init; }
    public BeastTribeRequirements? BeastTribeRequirements { get; init; }
    
    // Rewards
    public Reward? Rewards { get; init; }
    
    public override string ToString()
    {
        return
            $"QuestId: {QuestId}, QuestTitle: {QuestTitle}, PreviousQuests: [{string.Join(", ", PreviousQuestIds ?? new List<uint>())}], " +
            $"NextQuests: [{string.Join(", ", NextQuestIds ?? new List<uint>())}] ({string.Join(", ", NextQuestTitles ?? new List<string>())}), " +
            $"StarterNpc: {StarterNpc}, StarterNpcLocation: {StarterNpcLocation}, FinishNpc: {FinishNpc}, Expansion: {Expansion}, JournalGenre: {JournalGenre}";
    }

}

public class BeastTribeRequirements
{
    public string? BeastTribeName  { get; init; }
    public string? BeastTribeRank { get; init; }
}

public class JournalGenreDetails
{
    public uint Id { get; init; }
    public JournalCategoryDetails? JournalCategory { get; init; }
    public string? Name { get; init; }
}

public class JournalCategoryDetails
{
    public uint Id { get; init; }
    public string? Name { get; init; }
}

public class Level
{
    public uint RowId { get; init; }
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public float Yaw { get; init; }
    public float Radius { get; init; }
    public uint MapId { get; init; }
    public uint TerritoryId { get; init; }

    public override string ToString()
    {
        return $"RowId: {RowId}, Coordinates: (X: {X:F2}, Y: {Y:F2}, Z: {Z:F2}), " +
               $"Yaw: {Yaw:F2}, Radius: {Radius:F2}, " +
               $"MapId: {MapId}, TerritoryId: {TerritoryId}";
    }
}

public class Reward
{
    public int Exp { get; init; }
    public uint Gil { get; init; }
    public CurrencyReward? Currency { get; init; }
    public List<CatalystReward>? Catalysts { get; init; }
    public List<ItemsReward>? Items { get; init; }
    public List<OptionalItemsReward>? OptionalItems { get; init; }
    public EmoteReward? Emote { get; init; }
    public ActionReward? Action { get; init; }
    public List<GeneralActionReward>? GeneralActions { get; init; }
    public OtherReward? OtherReward { get; init; }
    public List<InstanceContentUnlockReward>? InstanceContentUnlock { get; init; }
}

public class CurrencyReward
{
    public uint CurrencyId { get; init; }
    public string? CurrencyName { get; init; }
    public uint Count { get; init; }
}

public class CatalystReward
{
    public uint ItemId { get; init; }
    public string? ItemName { get; init; }
    public byte Count { get; init; }
}

public class ItemsReward
{
    public uint ItemId { get; init; }
    public string? ItemName { get; init; }
    public byte Count { get; init; }
    public string? Stain { get; init; }
}

public class OptionalItemsReward
{
    public uint ItemId { get; init; }
    public string? ItemName { get; init; }
    public byte Count { get; init; }
    public string? Stain { get; init; }
    public bool IsHq { get; init; }
}

public class EmoteReward
{
    public uint Id { get; init; }
    public string? EmoteName { get; init; }
}

public class ActionReward
{
    public uint Id { get; init; }
    public string? ActionName { get; init; }
}

public class GeneralActionReward
{
    public uint Id { get; init; }
    public string? Name { get; init; }
}

public class OtherReward
{
    public uint Id { get; init; }
    public string? Name { get; init; }
}

public class InstanceContentUnlockReward
{
    public uint InstanceId { get; init; }
    public string? InstanceName { get; init; }
    public uint ContentType { get; init; }
}
