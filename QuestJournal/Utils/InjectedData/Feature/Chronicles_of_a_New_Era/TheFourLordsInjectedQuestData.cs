using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheFourLordsInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "The Fire-bird Down Below"
        injections.AddInstance(68688, 0, "Hells' Kier", 4);

        // "Surpassing the Samurai"
        injections.AddInstance(68701, 0, "The Wreath of Snakes", 4);
    }
}
