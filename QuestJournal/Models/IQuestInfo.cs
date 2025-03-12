using System.Collections.Generic;

namespace QuestJournal.Models;

public class IQuestInfo
{
    public uint QuestId { get; init; }
    public string? QuestTitle { get; init; }

    public List<uint>? PreviousQuestIds { get; init; }
    public List<string>? PreviousQuestTitles { get; init; }

    public string? StarterNpc { get; init; }
    public string? FinishNpc { get; init; }

    public bool IsRepeatable { get; init; }

    public string? Expansion { get; init; }

    public JournalGenreDetails? JournalGenre { get; init; }

    public override string ToString()
    {
        return
            $"QuestId: {QuestId}, QuestTitle: {QuestTitle}, PreviousQuests: [{string.Join(", ", PreviousQuestIds ?? new List<uint>())}], " +
            $"StarterNpc: {StarterNpc}, FinishNpc: {FinishNpc}, IsRepeatable: {IsRepeatable}, " +
            $"Expansion: {Expansion}, JournalGenre: {JournalGenre}";
    }
}

public class JournalGenreDetails
{
    public uint Id { get; init; }
    public int Icon { get; init; }
    public JournalCategoryDetails? JournalCategory { get; init; }
    public string? Name { get; init; }
}

public class JournalCategoryDetails
{
    public uint Id { get; init; }
    public string? Name { get; init; }
}
