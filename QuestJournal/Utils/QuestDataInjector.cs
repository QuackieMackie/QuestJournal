using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QuestJournal.Models;
using QuestJournal.Utils.InjectedData;

namespace QuestJournal.Utils;

public class QuestDataInjector
{
    private static readonly Dictionary<uint, Action<QuestModel>> CachedInjections;

    static QuestDataInjector()
    {
        CachedInjections = new Dictionary<uint, Action<QuestModel>>();
        
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IInjectedQuestData).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in types)
        {
            var provider = (IInjectedQuestData)Activator.CreateInstance(type)!;
            provider.RegisterInjections(CachedInjections);
        }
    }

    public void InjectMissingData(IEnumerable<QuestModel> quests)
    {
        foreach (var quest in quests)
        {
            if (CachedInjections.TryGetValue(quest.QuestId, out var inject))
            {
                inject(quest);
            }
        }
    }
}
