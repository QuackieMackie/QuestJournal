using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.MSQ;

public class PostDawntrailInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "In Search of the Past"
        injections.AddInstance(70782, 100, "Yuweyawata Field Station", 2);

        // "Descent to the Foundation"
        injections.AddInstance(70840, 101, "The Underkeep", 2);
        injections.AddInstance(70840, 20100, "Recollection", 4);

        // "A Terminal Invitation"
        injections.AddInstance(70907, 102, "The Meso Terminal", 2);
        injections.AddInstance(70907, 20104, "The Ageless Necropolis", 4);
    }
}
