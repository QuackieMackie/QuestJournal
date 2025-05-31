using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheFourLordsInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "The Fire-bird Down Below"
            {
                68688, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Hells' Kier",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Surpassing the Samurai"
            {
                68701, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Wreath of Snakes",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
