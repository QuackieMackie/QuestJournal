using System;
using System.Collections.Generic;
using QuestJournal.Models;
using QuestJournal.Utils.InjectedData;

namespace QuestJournal.Utils;

public class QuestDataInjector
{
    private readonly Dictionary<uint, Action<QuestModel>> dataInjections;
    
    public QuestDataInjector()
    {
        dataInjections = new();

        foreach (var keyValuePair in DungeonInjectedQuestData.GetData())
        {
            dataInjections[keyValuePair.Key] = keyValuePair.Value;
        }
        
        foreach (var keyValuePair in TrailInjectedQuestData.GetData())
        {
            dataInjections[keyValuePair.Key] = keyValuePair.Value;
        }
    }
    
    public void InjectMissingData(IEnumerable<QuestModel> quests)
    {
        foreach (var quest in quests)
        {
            if (dataInjections.TryGetValue(quest.QuestId, out var inject))
            {
                inject(quest);
            }
        }
    }
}
