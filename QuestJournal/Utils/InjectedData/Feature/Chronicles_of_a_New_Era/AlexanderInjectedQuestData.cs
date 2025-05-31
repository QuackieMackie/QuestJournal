using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class AlexanderInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Disarmed"
            {
                67626, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Fist of the Father",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Steel and Steam"
            {
                67627, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Cuff of the Father",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Tinker, Seeker, Soldier, Spy"
            {
                67628, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Arm of the Father",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Pulsing Heart"
            {
                67629, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Burden of the Father",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Rearmed"
            {
                67785, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Fist of the Son",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Folly of Youth"
            {
                67786, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Cuff of the Son",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Toppling the Tyrant"
            {
                67787, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Arm of the Son",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "One Step Behind"
            {
                67788, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Burden of the Son",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Coeurl and the Colossus"
            {
                67871, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Eyes of the Creator",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Biggs and Wedge's Excellent Adventure"
            {
                67872, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Breath of the Creator",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Thus Spake Quickthinx"
            {
                67873, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Heart of the Creator",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Judgment Day"
            {
                67874, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alexander - The Soul of the Creator",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
