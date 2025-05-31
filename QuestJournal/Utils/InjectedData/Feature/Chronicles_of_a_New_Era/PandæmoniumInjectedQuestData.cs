using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class PandæmoniumInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Where Familiars Dare"
            {
                70012, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Asphodelos: The First Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Under the Surface"
            {
                70013, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Asphodelos: The Second Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Fires of Creation"
            {
                70014, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Asphodelos: The Third Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Who Wards the Warders?"
            {
                70015, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Asphodelos: The Fourth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Masks of the Father"
            {
                70172, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Abyssos: The Fifth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "A Keyward's Gaol"
            {
                70173, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Abyssos: The Sixth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Servant of Violence"
            {
                70174, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Abyssos: The Seventh Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "One Final Wish"
            {
                70175, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Abyssos: The Eighth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Eater of Souls"
            {
                70292, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Anabaseios: The Ninth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Pandæmonium Awakens"
            {
                70293, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Anabaseios: The Tenth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "The Emissary's Judgment"
            {
                70294, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Anabaseios: The Eleventh Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "A Mother's Touch"
            {
                70295, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Anabaseios: The Twelfth Circle",
                        ContentType = 5
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}
