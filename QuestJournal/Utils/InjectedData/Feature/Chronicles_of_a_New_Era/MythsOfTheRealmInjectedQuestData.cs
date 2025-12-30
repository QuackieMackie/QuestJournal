using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class MythsOfTheRealmInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "The Realm of the Gods"
        injections.AddInstance(70073, 0, "Aglaia", 5);

        // "Return to the Phantom Realm"
        injections.AddInstance(70202, 0, "Euphrosyne", 5);

        // "The Heart of the Myth"
        injections.AddInstance(70327, 0, "Thaleia", 5);
    }
}
