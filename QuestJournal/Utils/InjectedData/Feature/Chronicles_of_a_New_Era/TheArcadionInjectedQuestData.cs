using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheArcadionInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "The Claw in the Dark"
            {
                70497, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Light-heavyweight M1",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Sweet Poison"
            {
                70498, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Light-heavyweight M2",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Vile Heat"
            {
                70500, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Light-heavyweight M3",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Neoteric Witch"
            {
                70501, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Light-heavyweight M4",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Dancing King"
            {
                70826, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Cruiserweight M1",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Art at War"
            {
                70827, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Cruiserweight M2",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Twisted Vengeance"
            {
                70828, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Cruiserweight M3",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Lone Wolf"
            {
                70830, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "AAC Cruiserweight M4",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
