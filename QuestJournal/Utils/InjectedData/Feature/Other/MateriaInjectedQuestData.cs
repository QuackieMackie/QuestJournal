using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class MateriaInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Marvelously Mutable Materia"
            {
                66999, quest =>
                {
                    quest.Rewards?.GeneralActions?.Add(new GeneralActionReward
                    {
                        Id = 14,
                        Name = "Materia Transmutation"
                    });
                }
            }
        };
    }
}
