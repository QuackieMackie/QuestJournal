using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class MateriaInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Marvelously Mutable Materia"
        injections.AddGeneralAction(66999, 14, "Materia Transmutation");
    }
}
