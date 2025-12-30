using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class PrimalsInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Gale-force Warning"
        injections.AddInstance(66731, 0, "The Howling Eye (Extreme)", 4);

        // "Quake Me Up Before You O'Ghomoro"
        injections.AddInstance(66732, 0, "The Navel (Extreme)", 4);

        // "Ifrit Ain't Broke"
        injections.AddInstance(66733, 0, "The Bowl of Embers (Extreme)", 4);

        // "Judgment Bolts and Lightning"
        injections.AddInstance(67066, 0, "The Striking Tree (Extreme)", 4);

        // "Drop Dead Shiva"
        injections.AddInstance(65626, 0, "The Akh Afah Amphitheatre (Extreme)", 4);

        // "Fear and Odin in the Shroud"
        injections.AddInstance(65969, 0, "Urth's Fount", 4);
    }
}

