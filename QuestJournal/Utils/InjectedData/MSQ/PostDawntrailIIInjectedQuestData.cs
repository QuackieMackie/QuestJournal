using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.MSQ;

public class PostDawntrailIIInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
         // "Beyond the Mountains"
         {
             70964, quest =>
             {
                 quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                 {
                     InstanceId = 103,
                     InstanceName = "Mistwake",
                     ContentType = 2
                 });
             }
         },
         // Where We Call Home
         {
             70969, quest =>
             {
                 quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                 {
                     InstanceId = 20106,
                     InstanceName = "Hell on Rails",
                     ContentType = 4
                 });
             }
         },
        };
    }
}
