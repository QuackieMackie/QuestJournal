using System;
using System.Collections.Generic;
using System.Linq;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class BahamutInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Primal Awakening"
        injections.AddCustom(66695, quest =>
        {
            var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30002); // "the Binding Coil of Bahamut - Turn 1"
            if (instance != null)
            {
                instance.ContentType = 5;
                instance.InstanceName = "The Binding Coil of Bahamut - Turn 1";
            }
        });
        injections.AddInstance(66695, 0, "The Binding Coil of Bahamut - Turn 2", 5);
        injections.AddInstance(66695, 0, "The Binding Coil of Bahamut - Turn 3", 5);
        injections.AddInstance(66695, 0, "The Binding Coil of Bahamut - Turn 4", 5);
        injections.AddInstance(66695, 0, "The Binding Coil of Bahamut - Turn 5", 5);

        // "Another Turn in the Coil"
        injections.AddCustom(66849, quest =>
        {
            var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30007); // "the Second Coil of Bahamut - Turn 1"
            if (instance != null)
            {
                instance.ContentType = 5;
                instance.InstanceName = "The Second Coil of Bahamut - Turn 1";
            }
        });
        injections.AddInstance(66849, 0, "The Second Coil of Bahamut - Turn 2", 5);
        injections.AddInstance(66849, 0, "The Second Coil of Bahamut - Turn 3", 5);
        injections.AddInstance(66849, 0, "The Second Coil of Bahamut - Turn 4", 5);
        injections.AddInstance(66849, 0, "The Second Coil of Bahamut - Turn 5", 5);

        // "Fragments of Truth"
        injections.AddCustom(65579, quest =>
        {
            var instance = quest.Rewards?.InstanceContentUnlock?.FirstOrDefault(i => i.InstanceId == 30016); // "the Final Coil of Bahamut - Turn 1"
            if (instance != null)
            {
                instance.ContentType = 5;
                instance.InstanceName = "The Final Coil of Bahamut - Turn 1";
            }
        });
        injections.AddInstance(65579, 0, "The Final Coil of Bahamut - Turn 2", 5);
        injections.AddInstance(65579, 0, "The Final Coil of Bahamut - Turn 3", 5);
        injections.AddInstance(65579, 0, "The Final Coil of Bahamut - Turn 4", 5);
        injections.AddInstance(65579, 0, "The Final Coil of Bahamut - Turn 5", 5);
    }
}
