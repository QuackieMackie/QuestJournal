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

    public string? StarterNpc { get; init; }
    public Level? StarterNpcLocation { get; init; }
    
    public string? FinishNpc { get; init; }

    public string? Expansion { get; init; }

    public JournalGenreDetails? JournalGenre { get; init; }
    
    public ushort SortKey { get; init; }
    
    public uint Icon { get; init; }
    public uint IconSpecial { get; init; }
    
    public override string ToString()
    {
        return
            $"QuestId: {QuestId}, QuestTitle: {QuestTitle}, PreviousQuests: [{string.Join(", ", PreviousQuestIds ?? new List<uint>())}], " +
            $"NextQuests: [{string.Join(", ", NextQuestIds ?? new List<uint>())}] ({string.Join(", ", NextQuestTitles ?? new List<string>())}), " +
            $"StarterNpc: {StarterNpc}, StarterNpcLocation: {StarterNpcLocation}, FinishNpc: {FinishNpc}, Expansion: {Expansion}, JournalGenre: {JournalGenre}";
    }

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

