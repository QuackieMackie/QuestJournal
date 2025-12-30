using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Duties;

public class DungeonInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "An Auspicious Encounter"
        injections.AddInstance(68551, 284, "Hells' Lid", 2);
        injections.AddInstance(68551, 290, "The Jade Stoa", 4);

        // "Tortoise in Time"
        injections.AddInstance(68552, 536, "The Swallow's Compass", 2);

        // "Blood for Stone"
        injections.AddInstance(67061, 25, "The Stone Vigil (Hard)", 2);

        // "King of the Hull"
        injections.AddInstance(67062, 23, "Hullbreaker Isle", 2);

        // "One More Night in Amdapor"
        injections.AddInstance(67818, 140, "The Lost City of Amdapor (Hard)", 2);

        // "It's Defiantly Pirates"
        injections.AddInstance(65630, 28, "Sastasha (Hard)", 2);

        // "The Wrath of Qarn"
        injections.AddInstance(65632, 26, "The Sunken Temple of Qarn (Hard)", 2);

        // "For Keep's Sake"
        injections.AddInstance(65966, 29, "Amdapor Keep (Hard)", 2);

        // "Not Easy Being Green"
        injections.AddInstance(65967, 30, "The Wanderer's Palace (Hard)", 2);

        // "Things Are Getting Sirius"
        injections.AddInstance(67737, 40, "Pharos Sirius (Hard)", 2);

        // "Storming the Hull"
        injections.AddInstance(67784, 172, "Hullbreaker Isle (Hard)", 2);

        // "It Belongs in a Museum"
        injections.AddInstance(70549, 834, "Tender Valley", 2);

        // "The Palace of Lost Souls"
        injections.AddInstance(68168, 235, "Shisui of the Violet Tides", 2);

        // "An Overgrown Ambition"
        injections.AddInstance(67738, 41, "Saint Mocianne's Arboretum", 2);

        // "Let Me Gubal That for You"
        injections.AddInstance(67922, 196, "The Great Gubal Library (Hard)", 2);

        // "The Fires of Sohm Al"
        injections.AddInstance(67938, 221, "Sohm Al (Hard)", 2);

        // "Something Stray in the Neighborhood"
        injections.AddInstance(70550, 981, "The Strayborough Deadwalk", 2);

        // "Cutting the Cheese"
        injections.AddInstance(69703, 794, "Smileton", 2);

        // "Where No Loporrit Has Gone Before"
        injections.AddInstance(69704, 784, "The Stigma Dreamscape", 2);

        // "To Kill a Coeurl"
        injections.AddInstance(68169, 236, "The Temple of the Fist", 2);

        // "An Unwanted Truth"
        injections.AddInstance(68613, 285, "The Fractal Continuum (Hard)", 2);

        // "Secret of the Ooze"
        injections.AddInstance(68678, 584, "Saint Mocianne's Arboretum (Hard)", 2);

        // "King of the Castle"
        injections.AddInstance(68170, 262, "Kugane Castle", 2);

        // "For All the Nights to Come"
        injections.AddInstance(67647, 36, "The Dusk Vigil", 2);

        // "Reap What You Sow"
        injections.AddInstance(67648, 33, "Neverreap", 2);

        // "Do It for Gilly"
        injections.AddInstance(67649, 35, "The Fractal Continuum", 2);

        // "By the Time You Hear This"
        injections.AddInstance(69131, 655, "The Twinning", 2);

        // "Akadaemia Anyder"
        injections.AddInstance(69132, 661, "Akadaemia Anyder", 2);
    }
}
