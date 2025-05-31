using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class ShadowOfMhachInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "To Rule the Skies"
            {
                67741, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Void Ark",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Weeping City"
            {
                67821, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Weeping City of Mhach",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Where Shadows Reign"
            {
                67909, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Dun Scaith",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
