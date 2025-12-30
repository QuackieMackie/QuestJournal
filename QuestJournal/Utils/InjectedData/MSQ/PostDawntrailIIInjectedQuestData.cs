using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.MSQ;

public class PostDawntrailIIInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Beyond the Mountains"
        injections.AddInstance(70964, 103, "Mistwake", 2);

        // Where We Call Home
        injections.AddInstance(70969, 20106, "Hell on Rails", 4);
    }
}
