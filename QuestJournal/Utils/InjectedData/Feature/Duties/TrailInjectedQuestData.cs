using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Duties;

public class TrailInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "The New King on the Block"
        injections.AddInstance(67090, 475, "The Great Hunt", 4);
    }
}
