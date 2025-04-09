using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Other;

public class LocationInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "Where the Heart is (The Goblet)"
            {
                66749, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "The Goblet",
                        MapId = 83,
                        TerritoryId = 341
                    };
                }
            },

            // "Where the Heart is (Mist)"
            {
                66750, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "Mist",
                        MapId = 72,
                        TerritoryId = 339
                    };
                }
            },

            // "Where the Heart is (The Lavender Beds)"
            {
                66748, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "The Lavender Beds",
                        MapId = 82,
                        TerritoryId = 340
                    };
                }
            },

            // "It Could Happen to You"
            {
                65970, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "The Gold Saucer",
                        MapId = 196,
                        TerritoryId = 144
                    };
                }
            },

            // "Broadening Horizons"
            {
                66338, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "White Wolf Gate",
                        MapId = 2,
                        TerritoryId = 132
                    };
                }
            },

            // "I Dream of Shirogane"
            {
                68167, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "Shirogane",
                        MapId = 364,
                        TerritoryId = 641
                    };
                }
            },

            // "Ascending to Empyreum"
            {
                69708, quest =>
                {
                    quest.Rewards ??= new Reward();
                    quest.Rewards.LocationReward = new LevelReward
                    {
                        PlaceName = "Empyreum",
                        MapId = 679,
                        TerritoryId = 979
                    };
                }
            }
        };
    }
}
