using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class EdenInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Deploy the Core"
            {
                68792, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Gate: Resurrection",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "One Fell Swoop"
            {
                68793, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Gate: Descent",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Nor Any Drop to Drink"
            {
                68794, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Gate: Inundation",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Super Seismic"
            {
                68795, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Gate: Sepulture",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Blood and Thunder"
            {
                69324, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Verse: Fulmination",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Into the Firestorm"
            {
                69325, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Verse: Furor",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Heart of Darkness"
            {
                69326, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Verse: Iconoclasm",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "On Thin Ice"
            {
                69327, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Verse: Refulgence",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Fear of the Dark"
            {
                69512, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Promise: Umbra",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Shadows of the Past"
            {
                69513, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Promise: Litany",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Voice of the Soul"
            {
                69514, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Promise: Anamorphosis",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Eden's Promise: Eternity",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
