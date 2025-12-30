using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class EchoesOfVanadielInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "An Otherworldly Encounter"
        injections.AddInstance(70769, 0, "Jeuno: The First Walk", 5);
    }
}

