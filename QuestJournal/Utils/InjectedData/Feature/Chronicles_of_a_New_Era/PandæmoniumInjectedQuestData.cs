using System;
using System.Collections.Generic;
using QuestJournal.Models;

namespace QuestJournal.Utils.InjectedData.Feature.Chronicles_of_a_New_Era;

public class PandæmoniumInjectedQuestData : IInjectedQuestData
{
    public void RegisterInjections(Dictionary<uint, Action<QuestModel>> injections)
    {
        // "Where Familiars Dare"
        injections.AddInstance(70012, 0, "Asphodelos: The First Circle", 5);

        // "Under the Surface"
        injections.AddInstance(70013, 0, "Asphodelos: The Second Circle", 5);

        // "The Fires of Creation"
        injections.AddInstance(70014, 0, "Asphodelos: The Third Circle", 5);

        // "Who Wards the Warders?"
        injections.AddInstance(70015, 0, "Asphodelos: The Fourth Circle", 5);

        // "Masks of the Father"
        injections.AddInstance(70172, 0, "Abyssos: The Fifth Circle", 5);

        // "A Keyward's Gaol"
        injections.AddInstance(70173, 0, "Abyssos: The Sixth Circle", 5);

        // "Servant of Violence"
        injections.AddInstance(70174, 0, "Abyssos: The Seventh Circle", 5);

        // "One Final Wish"
        injections.AddInstance(70175, 0, "Abyssos: The Eighth Circle", 5);

        // "Eater of Souls"
        injections.AddInstance(70292, 0, "Anabaseios: The Ninth Circle", 5);

        // "Pandæmonium Awakens"
        injections.AddInstance(70293, 0, "Anabaseios: The Tenth Circle", 5);

        // "The Emissary's Judgment"
        injections.AddInstance(70294, 0, "Anabaseios: The Eleventh Circle", 5);

        // "A Mother's Touch"
        injections.AddInstance(70295, 0, "Anabaseios: The Twelfth Circle", 5);
    }
}
