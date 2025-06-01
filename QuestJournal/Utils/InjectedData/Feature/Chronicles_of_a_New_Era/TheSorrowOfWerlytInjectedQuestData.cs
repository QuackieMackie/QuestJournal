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

            // "Weapon of Choice"
            {
                69378, quest =>
                {
                    quest.SortKey = 61;

                    quest.JournalGenre ??= new JournalGenreDetails();
                    quest.JournalGenre.JournalCategory ??= new JournalCategoryDetails();
                    quest.JournalGenre.JournalCategory.Id = 26;
                    quest.JournalGenre.JournalCategory.Name = "Chronicles of a New Era - The Sorrow of Werlyt";

                    quest.Rewards ??= new Reward();
                    quest.Rewards.InstanceContentUnlock ??= new List<InstanceContentUnlockReward>();

                    quest.Rewards.InstanceContentUnlock.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Cinder Drift (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards.InstanceContentUnlock.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Castrum Marinum (Extreme)",
                        ContentType = 4
                    });

                    quest.Rewards.InstanceContentUnlock.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Cloud Deck (Extreme)",
                        ContentType = 4
                    });
                }
            },
        };
    }
}
