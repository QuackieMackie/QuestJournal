using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheWarringTriadInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "When the Bough Wakes"
            {
                67766, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Containment Bay S1T7",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Balance unto All"
            {
                67868, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Containment Bay P1T6",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Last Pillar to Fall"
            {
                67930, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Containment Bay Z1T9",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Diabolical Bismarck"
            {
                67651, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Limitless Blue (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Thok Around the Clock"
            {
                67652, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Thok ast Thok (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
