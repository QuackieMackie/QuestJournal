using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class ReturnToIvaliceInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "A City Fallen"
            {
                68540, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Royal City of Rabanastre",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Annihilation"
            {
                68628, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Ridorana Lighthouse",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The City of Lost Angels"
            {
                68725, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Orbonne Monastery",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
