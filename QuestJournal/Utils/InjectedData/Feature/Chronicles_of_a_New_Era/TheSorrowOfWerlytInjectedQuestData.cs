using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheSorrowOfWerlytInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Ruby Doomsday"
        injections.AddInstance(69319, 0, "Cinder Drift", 4);

        // "Blood of Emerald"
        injections.AddInstance(69516, 0, "Castrum Marinum", 4);

        // "Duty in the Sky with Diamond"
        injections.AddInstance(69567, 0, "The Cloud Deck", 4);

        // "Weapon of Choice"
        injections.SetSortKey(69378, 61);
        injections.SetCategory(69378, 26, "Chronicles of a New Era - The Sorrow of Werlyt");
        injections.AddInstance(69378, 0, "Cinder Drift (Extreme)", 4);
        injections.AddInstance(69378, 0, "Castrum Marinum (Extreme)", 4);
        injections.AddInstance(69378, 0, "The Cloud Deck (Extreme)", 4);
    }
}
