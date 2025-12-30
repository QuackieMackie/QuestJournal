using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class YoRHaInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "On the Threshold"
        injections.AddInstance(69254, 0, "The Copied Factory", 5);

        // "Everything You Know Is Wrong"
        injections.AddInstance(69489, 0, "The Puppets' Bunker", 5);

        // "Brave New World"
        injections.AddInstance(69571, 0, "The Tower at Paradigm's Breach", 5);
    }
}
