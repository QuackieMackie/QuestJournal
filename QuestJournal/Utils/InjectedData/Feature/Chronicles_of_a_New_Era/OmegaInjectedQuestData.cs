using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class OmegaInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Into the Deltascape"
        injections.AddInstance(68465, 0, "Deltascape V1.0", 5);

        // "A Catastrophe Waiting"
        injections.AddInstance(68466, 0, "Deltascape V2.0", 5);

        // "The Croak Queen"
        injections.AddInstance(68467, 0, "Deltascape V3.0", 5);

        // "A Void at All Costs"
        injections.AddInstance(68468, 0, "Deltascape V4.0", 5);

        // "No Slowing Down"
        injections.AddInstance(68568, 0, "Sigmascape V1.0", 5);

        // "An Unfinished Masterpiece"
        injections.AddInstance(68569, 0, "Sigmascape V2.0", 5);

        // "Won't Let You Pass"
        injections.AddInstance(68570, 0, "Sigmascape V3.0", 5);

        // "Test World of Ruin"
        injections.AddInstance(68571, 0, "Sigmascape V4.0", 5);

        // "In the Beginning, There Was Chaos"
        injections.AddInstance(68690, 0, "Alphascape V1.0", 5);

        // "And Like Fire Was His Mane"
        injections.AddInstance(68691, 0, "Alphascape V2.0", 5);

        // "In the End, There Is Omega"
        injections.AddInstance(68692, 0, "Alphascape V3.0", 5);
        injections.AddInstance(68692, 0, "Alphascape V4.0", 5);
    }
}
