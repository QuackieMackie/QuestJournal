using System;
using System.Collections.Generic;
using System.Linq;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class BahamutInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Primal Awakening"
            {
                66695, quest =>
                {
                    var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30002); // "the Binding Coil of Bahamut - Turn 1"
                    if (instance != null)
                    {
                        instance.ContentType = 5;
                        instance.InstanceName = "The Binding Coil of Bahamut - Turn 1";
                    }

                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Binding Coil of Bahamut - Turn 2",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Binding Coil of Bahamut - Turn 3",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Binding Coil of Bahamut - Turn 4",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Binding Coil of Bahamut - Turn 5",
                        ContentType = 5
                    });
                    
                    quest.Rewards ??= new Reward();
                }
            },
            // "Another Turn in the Coil"
            {
                66849, quest =>
                {
                    var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30007); // "the Second Coil of Bahamut - Turn 1"
                    if (instance != null)
                    {
                        instance.ContentType = 5;
                        instance.InstanceName = "The Second Coil of Bahamut - Turn 1";
                    }

                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Second Coil of Bahamut - Turn 2",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Second Coil of Bahamut - Turn 3",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Second Coil of Bahamut - Turn 4",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Second Coil of Bahamut - Turn 5",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Fragments of Truth"
            {
                65579, quest =>
                {
                    var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30016); // "the Final Coil of Bahamut - Turn 1"
                    if (instance != null)
                    {
                        instance.ContentType = 5;
                        instance.InstanceName = "The Final Coil of Bahamut - Turn 1";
                    }

                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Final Coil of Bahamut - Turn 2",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Final Coil of Bahamut - Turn 3",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Final Coil of Bahamut - Turn 4",
                        ContentType = 5
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Final Coil of Bahamut - Turn 5",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
