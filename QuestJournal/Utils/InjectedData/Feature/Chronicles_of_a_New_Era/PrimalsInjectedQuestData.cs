using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class PrimalsInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Gale-force Warning"
            {
                66731, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Howling Eye (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Quake Me Up Before You O'Ghomoro"
            {
                66732, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Navel (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Ifrit Ain't Broke"
            {
                66733, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Bowl of Embers (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Judgment Bolts and Lightning"
            {
                67066, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Striking Tree (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Drop Dead Shiva"
            {
                65626, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Akh Afah Amphitheatre (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Fear and Odin in the Shroud"
            {
                65969, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Urth's Fount",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
        };
    }
}

