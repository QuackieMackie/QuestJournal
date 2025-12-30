using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheWarringTriadInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "When the Bough Wakes"
        injections.AddInstance(67766, 0, "Containment Bay S1T7", 4);

        // "Balance unto All"
        injections.AddInstance(67868, 0, "Containment Bay P1T6", 4);

        // "The Last Pillar to Fall"
        injections.AddInstance(67930, 0, "Containment Bay Z1T9", 4);

        // "The Diabolical Bismarck"
        injections.AddInstance(67651, 0, "The Limitless Blue (Extreme)", 4);

        // "Thok Around the Clock"
        injections.AddInstance(67652, 0, "Thok ast Thok (Extreme)", 4);
    }
}
