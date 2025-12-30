using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class LocationInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Where the Heart is (The Goblet)"
        injections.AddLocation(66749, "The Goblet", 83, 341);

        // "Where the Heart is (Mist)"
        injections.AddLocation(66750, "Mist", 72, 339);

        // "Where the Heart is (The Lavender Beds)"
        injections.AddLocation(66748, "The Lavender Beds", 82, 340);

        // "It Could Happen to You"
        injections.AddLocation(65970, "The Gold Saucer", 196, 144);

        // "Broadening Horizons"
        injections.AddLocation(66338, "White Wolf Gate", 2, 132);

        // "I Dream of Shirogane"
        injections.AddLocation(68167, "Shirogane", 364, 641);

        // "Ascending to Empyreum"
        injections.AddLocation(69708, "Empyreum", 679, 979);
    }
}
