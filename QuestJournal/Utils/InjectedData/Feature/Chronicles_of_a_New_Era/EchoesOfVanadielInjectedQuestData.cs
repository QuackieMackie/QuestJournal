using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class EchoesOfVanadielInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "An Otherworldly Encounter"
            {
                70769, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Jeuno: The First Walk",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}

