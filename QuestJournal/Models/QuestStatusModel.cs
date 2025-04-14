namespace QuestJournal.Models;

public struct QuestStatusModel
{
    public string StatusSymbol { get; set; } // ✓, →, ×
    public string HoverText { get; set; }    // "Complete", "In Progress", "Not Started".
}
