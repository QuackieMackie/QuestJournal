using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class TheArcadionInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "The Claw in the Dark"
        injections.AddInstance(70497, 0, "AAC Light-heavyweight M1", 5);

        // "Sweet Poison"
        injections.AddInstance(70498, 0, "AAC Light-heavyweight M2", 5);

        // "Vile Heat"
        injections.AddInstance(70500, 0, "AAC Light-heavyweight M3", 5);

        // "The Neoteric Witch"
        injections.AddInstance(70501, 0, "AAC Light-heavyweight M4", 5);

        // "The Dancing King"
        injections.AddInstance(70826, 0, "AAC Cruiserweight M1", 5);

        // "Art at War"
        injections.AddInstance(70827, 0, "AAC Cruiserweight M2", 5);

        // "Twisted Vengeance"
        injections.AddInstance(70828, 0, "AAC Cruiserweight M3", 5);

        // "The Lone Wolf"
        injections.AddInstance(70830, 0, "AAC Cruiserweight M4", 5);
    }
}
