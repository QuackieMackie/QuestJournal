using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class ReturnToIvaliceInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "A City Fallen"
        injections.AddInstance(68540, 0, "The Royal City of Rabanastre", 5);

        // "Annihilation"
        injections.AddInstance(68628, 0, "The Ridorana Lighthouse", 5);

        // "The City of Lost Angels"
        injections.AddInstance(68725, 0, "The Orbonne Monastery", 5);
    }
}
