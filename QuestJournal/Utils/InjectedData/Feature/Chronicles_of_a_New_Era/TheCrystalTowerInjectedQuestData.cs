using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheCrystalTowerInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Labyrinth of the Ancients"
        injections.AddInstance(66738, 0, "The Labyrinth of the Ancients", 5);

        // "Syrcus Tower"
        injections.AddInstance(67010, 0, "Syrcus Tower", 5);

        // "The World of Darkness"
        injections.AddInstance(66030, 0, "The World of Darkness", 5);
    }
}
