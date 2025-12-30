using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class AlexanderInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Disarmed"
        injections.AddInstance(67626, 0, "Alexander - The Fist of the Father", 5);

        // "Steel and Steam"
        injections.AddInstance(67627, 0, "Alexander - The Cuff of the Father", 5);

        // "Tinker, Seeker, Soldier, Spy"
        injections.AddInstance(67628, 0, "Alexander - The Arm of the Father", 5);

        // "The Pulsing Heart"
        injections.AddInstance(67629, 0, "Alexander - The Burden of the Father", 5);

        // "Rearmed"
        injections.AddInstance(67785, 0, "Alexander - The Fist of the Son", 5);

        // "The Folly of Youth"
        injections.AddInstance(67786, 0, "Alexander - The Cuff of the Son", 5);

        // "Toppling the Tyrant"
        injections.AddInstance(67787, 0, "Alexander - The Arm of the Son", 5);

        // "One Step Behind"
        injections.AddInstance(67788, 0, "Alexander - The Burden of the Son", 5);

        // "The Coeurl and the Colossus"
        injections.AddInstance(67871, 0, "Alexander - The Eyes of the Creator", 5);

        // "Biggs and Wedge's Excellent Adventure"
        injections.AddInstance(67872, 0, "Alexander - The Breath of the Creator", 5);

        // "Thus Spake Quickthinx"
        injections.AddInstance(67873, 0, "Alexander - The Heart of the Creator", 5);

        // "Judgment Day"
        injections.AddInstance(67874, 0, "Alexander - The Soul of the Creator", 5);
    }
}
