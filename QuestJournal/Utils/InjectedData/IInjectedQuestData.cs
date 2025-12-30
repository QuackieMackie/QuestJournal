using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData;

public interface IInjectedQuestData
{
    void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections);
}
