using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData;

public class DungeonInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new()
        {
            // "An Auspicious Encounter"
            { 68551, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 284,
                        InstanceName = "Hells' Lid",
                        ContentType = 2,
                    });
                    
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 290,
                        InstanceName = "The Jade Stoa",
                        ContentType = 4,
                    });
                }
            },
            
            // "Tortoise in Time"
            { 68552, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 536,
                        InstanceName = "The Swallow's Compass",
                        ContentType = 2,
                    });
                }
            },

            // "Blood for Stone"
            { 67061, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 25,
                        InstanceName = "The Stone Vigil (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "King of the Hull"
            { 67062, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 23,
                        InstanceName = "Hullbreaker Isle",
                        ContentType = 2,
                    });
                }
            },
            
            // "One More Night in Amdapor"
            { 67818, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 140,
                        InstanceName = "The Lost City of Amdapor (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "It's Defiantly Pirates"
            { 65630, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 28,
                        InstanceName = "Sastasha (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "The Wrath of Qarn"
            { 65632, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 26,
                        InstanceName = "The Sunken Temple of Qarn (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "For Keep's Sake"
            { 65966, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 29,
                        InstanceName = "Amdapor Keep (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "Not Easy Being Green"
            { 65967, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 30,
                        InstanceName = "The Wanderer's Palace (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "Things Are Getting Sirius"
            { 67737, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 40,
                        InstanceName = "Pharos Sirius (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "Storming the Hull"
            { 67784, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 172,
                        InstanceName = "Hullbreaker Isle (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "It Belongs in a Museum"
            { 70549, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 834,
                        InstanceName = "Tender Valley",
                        ContentType = 2,
                    });
                }
            },
            
            // "The Palace of Lost Souls"
            { 68168, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 235,
                        InstanceName = "Shisui of the Violet Tides",
                        ContentType = 2,
                    });
                }
            },
            
            // "An Overgrown Ambition"
            { 67738, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 41,
                        InstanceName = "Saint Mocianne's Arboretum",
                        ContentType = 2,
                    });
                }
            },
            
            // "Let Me Gubal That for You"
            { 67922, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 196,
                        InstanceName = "The Great Gubal Library (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "The Fires of Sohm Al"
            { 67938, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 221,
                        InstanceName = "Sohm Al (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "Something Stray in the Neighborhood"
            { 70550, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 981,
                        InstanceName = "The Strayborough Deadwalk",
                        ContentType = 2,
                    });
                }
            },
            
            // "Cutting the Cheese"
            { 69703, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 794,
                        InstanceName = "Smileton",
                        ContentType = 2,
                    });
                }
            },
            
            // "Where No Loporrit Has Gone Before"
            { 69704, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 784,
                        InstanceName = "The Stigma Dreamscape",
                        ContentType = 2,
                    });
                }
            },
            
            // "To Kill a Coeurl"
            { 68169, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 236,
                        InstanceName = "The Temple of the Fist",
                        ContentType = 2,
                    });
                }
            },
            
            // "An Unwanted Truth"
            { 68613, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 285,
                        InstanceName = "The Fractal Continuum (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "Secret of the Ooze"
            { 68678, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 584,
                        InstanceName = "Saint Mocianne's Arboretum (Hard)",
                        ContentType = 2,
                    });
                }
            },
            
            // "King of the Castle"
            { 68170, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 262,
                        InstanceName = "Kugane Castle",
                        ContentType = 2,
                    });
                }
            },
            
            // "For All the Nights to Come"
            { 67647, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 36,
                        InstanceName = "The Dusk Vigil",
                        ContentType = 2,
                    });
                }
            },
            
            // "Reap What You Sow"
            { 67648, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 33,
                        InstanceName = "Neverreap",
                        ContentType = 2,
                    });
                }
            },
            
            // "Do It for Gilly"
            { 67649, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 35,
                        InstanceName = "The Fractal Continuum",
                        ContentType = 2,
                    });
                }
            },
            
            // "By the Time You Hear This"
            { 69131, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 655,
                        InstanceName = "The Twinning",
                        ContentType = 2,
                    });
                }
            },
            
            // "Akadaemia Anyder"
            { 69132, (quest) => {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 661,
                        InstanceName = "Akadaemia Anyder",
                        ContentType = 2,
                    });
                }
            },
        };
    }
}
