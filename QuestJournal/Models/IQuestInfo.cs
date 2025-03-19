using System.Collections.Generic;

namespace QuestJournal.Models;

public class QuestInfo
{
    public uint QuestId { get; init; }
    public string? QuestTitle { get; init; }

    public List<uint>? PreviousQuestIds { get; init; }
    public List<string>? PreviousQuestTitles { get; init; }

    public List<uint>? NextQuestIds { get; init; }
    public List<string>? NextQuestTitles { get; init; }

    public string? StarterNpc { get; init; }
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
            $"StarterNpc: {StarterNpc}, FinishNpc: {FinishNpc}, Expansion: {Expansion}, JournalGenre: {JournalGenre}";
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
