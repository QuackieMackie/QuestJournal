using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class GlamourAndCustomizationInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "A Faerie Tale Come True"
        injections.AddComment(70300, "/egiglamour \"egi name\" \"glamour\"", "This command lets you alter the appearance of your egi. You must resummon the egi for the glamour to take effect.", true);

        // "An Egi by Any Other Name"
        injections.AddComment(67896, "/egiglamour \"egi name\" \"glamour\"", "This command lets you alter the appearance of your egi. You must resummon the egi for the glamour to take effect.", false);
        
        // "A Self-improving Man"
        injections.AddComment(66957, "This quest and `If I Had a Glamour` are mutually exclusive. You can only do one.");
        
        // "Submission Impossible"
        injections.AddComment(66958, "Master Recipes: Glamours Set\nThis quest and `Absolutely Glamourous` are mutually exclusive. You can only do one.");
        
        // "If I Had a Glamour"
        injections.AddComment(68553, "This quest and `A Self-improving Man` are mutually exclusive. You can only do one.");
        
        // "Absolutely Glamourous"
        injections.AddComment(68554, "Master Recipes: Glamours Set\nThis quest and `Submission Impossible` are mutually exclusive. You can only do one.");
    }
}
