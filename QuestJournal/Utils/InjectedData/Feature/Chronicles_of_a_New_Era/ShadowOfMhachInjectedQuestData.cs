using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class ShadowOfMhachInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "To Rule the Skies"
        injections.AddInstance(67741, 0, "The Void Ark", 5);

        // "The Weeping City"
        injections.AddInstance(67821, 0, "The Weeping City of Mhach", 5);

        // "Where Shadows Reign"
        injections.AddInstance(67909, 0, "Dun Scaith", 5);
    }
}
