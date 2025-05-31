using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheSorrowOfWerlytInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Ruby Doomsday"
            {
                69319, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Cinder Drift",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Blood of Emerald"
            {
                69516, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Castrum Marinum",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            // "Duty in the Sky with Diamond"
            {
                69567, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Cloud Deck",
                        ContentType = 4
                    });

                    quest.Rewards ??= new Reward();
                }
            },
            
            //TODO: Add quest `Weapon of Choice`
            // - This is for some reason does not have a JourneyCategory set, so I'll have to manually inject the entire quest.
            //  - Maybe a method that would achieve this in case it pops up again?
        };
    }
}
