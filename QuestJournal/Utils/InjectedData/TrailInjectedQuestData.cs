using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData;

public class TrailInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new()
        {
            // "The New King on the Block"
            { 67090, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 475,
                        InstanceName = "The Great Hunt",
                        ContentType = 4,
                    });
                }
            },
        };
    }
}
