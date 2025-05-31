using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class OmegaInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Into the Deltascape"
            {
                68465, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Deltascape V1.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "A Catastrophe Waiting"
            {
                68466, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Deltascape V2.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Croak Queen"
            {
                68467, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Deltascape V3.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "A Void at All Costs"
            {
                68468, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Deltascape V4.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "No Slowing Down"
            {
                68568, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Sigmascape V1.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "An Unfinished Masterpiece"
            {
                68569, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Sigmascape V2.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Won't Let You Pass"
            {
                68570, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Sigmascape V3.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Test World of Ruin"
            {
                68571, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Sigmascape V4.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "In the Beginning, There Was Chaos"
            {
                68690, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alphascape V1.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "And Like Fire Was His Mane"
            {
                68691, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alphascape V2.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "In the End, There Is Omega"
            {
                68692, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alphascape V3.0",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Alphascape V4.0",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
