using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class EdenInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Deploy the Core"
        injections.AddInstance(68792, 0, "Eden's Gate: Resurrection", 5);

        // "One Fell Swoop"
        injections.AddInstance(68793, 0, "Eden's Gate: Descent", 5);

        // "Nor Any Drop to Drink"
        injections.AddInstance(68794, 0, "Eden's Gate: Inundation", 5);

        // "Super Seismic"
        injections.AddInstance(68795, 0, "Eden's Gate: Sepulture", 5);

        // "Blood and Thunder"
        injections.AddInstance(69324, 0, "Eden's Verse: Fulmination", 5);

        // "Into the Firestorm"
        injections.AddInstance(69325, 0, "Eden's Verse: Furor", 5);

        // "Heart of Darkness"
        injections.AddInstance(69326, 0, "Eden's Verse: Iconoclasm", 5);

        // "On Thin Ice"
        injections.AddInstance(69327, 0, "Eden's Verse: Refulgence", 5);

        // "Fear of the Dark"
        injections.AddInstance(69512, 0, "Eden's Promise: Umbra", 5);

        // "Shadows of the Past"
        injections.AddInstance(69513, 0, "Eden's Promise: Litany", 5);

        // "Voice of the Soul"
        injections.AddInstance(69514, 0, "Eden's Promise: Anamorphosis", 5);
        injections.AddInstance(69514, 0, "Eden's Promise: Eternity", 5);
    }
}
