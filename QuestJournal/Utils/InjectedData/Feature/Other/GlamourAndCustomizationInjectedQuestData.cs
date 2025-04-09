using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class GlamourAndCustomizationInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "A Faerie Tale Come True"
            {
                70300, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "/egiglamour \"egi name\" \"glamour\"",
                        HoverTextComment = "This command lets you alter the appearance of your egi. You must resummon the egi for the glamour to take effect.",
                        ClickToCopy = true,
                    };
                }
            },

            // "An Egi by Any Other Name"
            {
                67896, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "/egiglamour \"egi name\" \"glamour\"",
                        HoverTextComment = "This command lets you alter the appearance of your egi. You must resummon the egi for the glamour to take effect.",
                        ClickToCopy = false,
                    };
                }
            },
            
            // "A Self-improving Man"
            {
                66957, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "This quest and `If I Had a Glamour` are mutually exclusive. You can only do one.",
                        ClickToCopy = false,
                    };
                }
            },
            
            // "Submission Impossible"
            {
                66958, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "Master Recipes: Glamours Set\nThis quest and `Absolutely Glamourous` are mutually exclusive. You can only do one.",
                        ClickToCopy = false,
                    };
                }
            },
            
            // "If I Had a Glamour"
            {
                68553, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "This quest and `A Self-improving Man` are mutually exclusive. You can only do one.",
                        ClickToCopy = false,
                    };
                }
            },
            
            // "Absolutely Glamourous"
            {
                68554, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "Master Recipes: Glamours Set\nThis quest and `Submission Impossible` are mutually exclusive. You can only do one.",
                        ClickToCopy = false,
                    };
                }
            },
        };
    }
}
