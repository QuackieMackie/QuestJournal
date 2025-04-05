using System.Collections.Generic;

namespace QuestJournal.Models;

public class QuestModel
{
    public uint QuestId { get; init; }
    public string? QuestTitle { get; set; }
    
    public List<uint>? PreviousQuestIds { get; set; }
    public List<string>? PreviousQuestTitles { get; set; }

    public List<uint>? NextQuestIds { get; set; }
    public List<string>? NextQuestTitles { get; set; }

    // Locations
    public string? StarterNpc { get; set; }
    public Level? StarterNpcLocation { get; set; }
    public string? FinishNpc { get; set; }
    
    // Organisation
    public string? Expansion { get; set; }
    public JournalGenreDetails? JournalGenre { get; set; }
    public ushort SortKey { get; set; }
    
    // Other
    public bool IsRepeatable { get; set; }
    
    // Icons
    public uint EventIcon { get; set; }
    public uint Icon { get; set; }
    
    // Requirements
    public uint? JobLevel { get; set; }
    public string? ClassJobCategory { get; set; }
    public BeastTribeRequirements? BeastTribeRequirements { get; set; }
    
    // Rewards
    public Reward? Rewards { get; set; }
    
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
    public string? BeastTribeName  { get; set; }
    public string? BeastTribeRank { get; set; }
}

public class JournalGenreDetails
{
    public uint Id { get; set; }
    public JournalCategoryDetails? JournalCategory { get; set; }
    public string? Name { get; set; }
}

public class JournalCategoryDetails
{
    public uint Id { get; set; }
    public string? Name { get; set; }
}

public class Level
{
    public uint RowId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float Yaw { get; set; }
    public float Radius { get; set; }
    public uint MapId { get; set; }
    public uint TerritoryId { get; set; }

    public override string ToString()
    {
        return $"RowId: {RowId}, Coordinates: (X: {X:F2}, Y: {Y:F2}, Z: {Z:F2}), " +
               $"Yaw: {Yaw:F2}, Radius: {Radius:F2}, " +
               $"MapId: {MapId}, TerritoryId: {TerritoryId}";
    }
}

public class Reward
{
    public int Exp { get; set; }
    public uint Gil { get; set; }
    public CurrencyReward? Currency { get; set; }
    public List<CatalystReward>? Catalysts { get; set; }
    public List<ItemsReward>? Items { get; set; }
    public List<OptionalItemsReward>? OptionalItems { get; set; }
    public EmoteReward? Emote { get; set; }
    public ActionReward? Action { get; set; }
    public List<GeneralActionReward>? GeneralActions { get; set; }
    public OtherReward? OtherReward { get; set; }
    public List<InstanceContentUnlockReward>? InstanceContentUnlock { get; set; }
}

public class CurrencyReward
{
    public uint CurrencyId { get; set; }
    public string? CurrencyName { get; set; }
    public uint Count { get; set; }
}

public class CatalystReward
{
    public uint ItemId { get; set; }
    public string? ItemName { get; set; }
    public byte Count { get; set; }
}

public class ItemsReward
{
    public uint ItemId { get; set; }
    public string? ItemName { get; set; }
    public byte Count { get; set; }
    public string? Stain { get; set; }
}

public class OptionalItemsReward
{
    public uint ItemId { get; set; }
    public string? ItemName { get; set; }
    public byte Count { get; set; }
    public string? Stain { get; set; }
    public bool IsHq { get; set; }
}

public class EmoteReward
{
    public uint Id { get; set; }
    public string? EmoteName { get; set; }
}

public class ActionReward
{
    public uint Id { get; set; }
    public string? ActionName { get; set; }
}

public class GeneralActionReward
{
    public uint Id { get; set; }
    public string? Name { get; set; }
}

public class OtherReward
{
    public uint Id { get; set; }
    public string? Name { get; set; }
}

public class InstanceContentUnlockReward
{
    public uint InstanceId { get; set; }
    public string? InstanceName { get; set; }
    public uint ContentType { get; set; }
}
