using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Duties;

public class StoneSkySeaInjectedQuestData
{
    public static Dictionary<uint, Action<QuestModel>> GetData()
    {
        return new Dictionary<uint, Action<QuestModel>>
        {
            // "A Striking Opportunity"
            {
                67654, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "Stone, Sky, Sea",
                        ContentType = 999
                    });

                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "These practice duties are unlocked by completing this quest.\n- Stone, Sky, Sea - Level 60\n- Stone, Sky, Sea - Basic Training\n- Stone, Sky, Sea - The Limitless Blue (Extreme)\n- Stone, Sky, Sea - Alexander: Gordias\n- Stone, Sky, Sea - The Fist of the Father (Savage)\n- Stone, Sky, Sea - The Cuff of the Father (Savage)\n- Stone, Sky, Sea - The Arm of the Father (Savage)\n- Stone, Sky, Sea - The Burden of the Father (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Thordan's Reign\n- Stone, Sky, Sea - Containment Bay S1T7 (Extreme)\n- Stone, Sky, Sea - Alexander: Midas\n- Stone, Sky, Sea - The Fist of the Son (Savage)\n- Stone, Sky, Sea - The Cuff of the Son (Savage)\n- Stone, Sky, Sea - The Arm of the Son (Savage)\n- Stone, Sky, Sea - The Burden of the Son (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Nidhogg's Rage\n- Stone, Sky, Sea - Containment Bay P1T6 (Extreme)\n- Stone, Sky, Sea - Alexander: The Creator\n- Stone, Sky, Sea - The Eyes of the Creator (Savage)\n- Stone, Sky, Sea - The Breath of the Creator (Savage)\n- Stone, Sky, Sea - The Heart of the Creator (Savage)\n- Stone, Sky, Sea - The Soul of the Creator (Savage)\n- Stone, Sky, Sea - Containment Bay Z1T9 (Extreme)",
                        HoverTextComment = "",
                        ClickToCopy = false,
                    };
                }
            },
            // "Another Striking Opportunity"
            {
                68476, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Circles of Answering",
                        ContentType = 999
                    });
                    
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "These practice duties are unlocked by completing this quest.\n- Stone, Sky, Sea - Level 70\n- Stone, Sky, Sea - The Royal Menagerie\n- Stone, Sky, Sea - Emanation (Extreme)\n- Stone, Sky, Sea - The Pool of Tribute (Extreme)\n- Stone, Sky, Sea - Omega: Deltascape\n- Stone, Sky, Sea - Deltascape V1.0 (Savage)\n- Stone, Sky, Sea - Deltascape V2.0 (Savage)\n- Stone, Sky, Sea - Deltascape V3.0 (Savage)\n- Stone, Sky, Sea - Deltascape V4.0 (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Shinryu's Domain\n- Stone, Sky, Sea - The Jade Stoa (Extreme)\n- Stone, Sky, Sea - Omega: Sigmascape\n- Stone, Sky, Sea - Sigmascape V1.0 (Savage)\n- Stone, Sky, Sea - Sigmascape V2.0 (Savage)\n- Stone, Sky, Sea - Sigmascape V3.0 (Savage)\n- Stone, Sky, Sea - Sigmascape V4.0 (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Tsukuyomi's Pain\n- Stone, Sky, Sea - Hells' Kier (Extreme)\n- Stone, Sky, Sea - Omega: Alphascape\n- Stone, Sky, Sea - Alphascape V1.0 (Savage)\n- Stone, Sky, Sea - Alphascape V2.0 (Savage)\n- Stone, Sky, Sea - Alphascape V3.0 (Savage)\n- Stone, Sky, Sea - Alphascape V4.0 (Savage)\n- Stone, Sky, Sea - The Wreath of Snakes (Extreme)\n",
                        HoverTextComment = "",
                        ClickToCopy = false,
                    };
                }
            },
            // "Yet Another Striking Opportunity"
            {
                69137, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Lawns",
                        ContentType = 999
                    });
                    
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "These practice duties are unlocked by completing this quest.\n- Stone, Sky, Sea - Level 80\n- Stone, Sky, Sea - The Dying Gasp\n- Stone, Sky, Sea - The Dancing Plague (Extreme)\n- Stone, Sky, Sea - The Crown of the Immaculate (Extreme)\n- Stone, Sky, Sea - Eden's Gate\n- Stone, Sky, Sea - Eden's Gate: Resurrection (Savage)\n- Stone, Sky, Sea - Eden's Gate: Descent (Savage)\n- Stone, Sky, Sea - Eden's Gate: Inundation (Savage)\n- Stone, Sky, Sea - Eden's Gate: Sepulture (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Hades's Elegy\n- Stone, Sky, Sea - Cinder Drift (Extreme)\n- Stone, Sky, Sea - Eden's Verse\n- Stone, Sky, Sea - Eden's Verse: Fulmination (Savage)\n- Stone, Sky, Sea - Eden's Verse: Furor (Savage)\n- Stone, Sky, Sea - Eden's Verse: Iconoclasm (Savage)\n- Stone, Sky, Sea - Eden's Verse: Refulgence (Savage)\n- Stone, Sky, Sea - The Seat of Sacrifice (Extreme)\n- Stone, Sky, Sea - Castrum Marinum (Extreme)\n- Stone, Sky, Sea - Eden's Promise\n- Stone, Sky, Sea - Eden's Promise: Umbra (Savage)\n- Stone, Sky, Sea - Eden's Promise: Litany (Savage)\n- Stone, Sky, Sea - Eden's Promise: Anamorphosis (Savage)\n- Stone, Sky, Sea - Eden's Promise: Eternity (Savage)\n- Stone, Sky, Sea - The Cloud Deck (Extreme)\n",
                        HoverTextComment = "",
                        ClickToCopy = false,
                    };
                }
            },
            // "A Place to Train"
            {
                69709, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Burning Field",
                        ContentType = 999
                    });
                    
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "These practice duties are unlocked by completing this quest.\n- Stone, Sky, Sea - Level 90\n- Stone, Sky, Sea - The Final Day\n- Stone, Sky, Sea - The Minstrel's Ballad: Hydaelyn's Call\n- Stone, Sky, Sea - The Minstrel's Ballad: Zodiark's Fall\n- Stone, Sky, Sea - Pandæmonium: Asphodelos\n- Stone, Sky, Sea - Asphodelos: The First Circle (Savage)\n- Stone, Sky, Sea - Asphodelos: The Second Circle (Savage)\n- Stone, Sky, Sea - Asphodelos: The Third Circle (Savage)\n- Stone, Sky, Sea - Asphodelos: The Fourth Circle (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Endsinger's Aria\n- Stone, Sky, Sea - Storm's Crown (Extreme)\n- Stone, Sky, Sea - Pandæmonium: Abyssos\n- Stone, Sky, Sea - Abyssos: The Fifth Circle (Savage)\n- Stone, Sky, Sea - Abyssos: The Sixth Circle (Savage)\n- Stone, Sky, Sea - Abyssos: The Seventh Circle (Savage)\n- Stone, Sky, Sea - Abyssos: The Eighth Circle (Savage)\n- Stone, Sky, Sea - Mount Ordeals (Extreme)\n- Stone, Sky, Sea - The Voidcast Dais (Extreme)\n- Stone, Sky, Sea - Pandæmonium: Anabaseios\n- Stone, Sky, Sea - Anabaseios: The Ninth Circle (Savage)\n- Stone, Sky, Sea - Anabaseios: The Tenth Circle (Savage)\n- Stone, Sky, Sea - Anabaseios: The Eleventh Circle (Savage)\n- Stone, Sky, Sea - Anabaseios: The Twelfth Circle (Savage)\n- Stone, Sky, Sea - The Abyssal Fracture (Extreme)\n",
                        HoverTextComment = "",
                        ClickToCopy = false,
                    };
                }
            },
            // "Trial by Spire"
            {
                70541, quest =>
                {
                    quest.Rewards?.InstanceContentUnlock?.Add(new InstanceContentUnlockReward
                    {
                        InstanceId = 0,
                        InstanceName = "The Spire of Trial",
                        ContentType = 999
                    });
                    
                    quest.Rewards ??= new Reward();
                    quest.Rewards.CommentSection = new CommentSection
                    {
                        GuiComment = "These practice duties are unlocked by completing this quest.\n- Stone, Sky, Sea - Level 100\n- Stone, Sky, Sea - Queen Eternal\n- Stone, Sky, Sea - Everkeep (Extreme)\n- Stone, Sky, Sea - Worqor Lar Dor (Extreme)\n- Stone, Sky, Sea - AAC Light-heavyweight\n- Stone, Sky, Sea - AAC LHW M1 (Savage)\n- Stone, Sky, Sea - AAC LHW M2 (Savage)\n- Stone, Sky, Sea - AAC LHW M3 (Savage)\n- Stone, Sky, Sea - AAC LHW M4 (Savage)\n- Stone, Sky, Sea - The Minstrel's Ballad: Sphene's Burden\n- Stone, Sky, Sea - Cloud of Darkness (Chaotic)\n- Stone, Sky, Sea - Recollection (Extreme)\n- Stone, Sky, Sea - AAC Cruiserweight\n- Stone, Sky, Sea - AAC CW M1 (Savage)\n- Stone, Sky, Sea - AAC CW M2 (Savage)\n- Stone, Sky, Sea - AAC CW M3 (Savage)\n- Stone, Sky, Sea - AAC CW M4 (Savage)\n",
                        HoverTextComment = "",
                        ClickToCopy = false,
                    };
                }
            },
        };
    }
}
