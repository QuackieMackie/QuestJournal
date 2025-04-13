using System;
using System.Collections.Generic;
using QuestJournal.Models;
using QuestJournal.Utils.InjectedData.Feature.Duties;
using QuestJournal.Utils.InjectedData.Feature.Other;

namespace QuestJournal.Utils;

public class QuestDataInjector
{
    private readonly Dictionary<uint, Action<QuestModel>> dataInjections;

    public QuestDataInjector()
    {
        dataInjections = new Dictionary<uint, Action<QuestModel>>();

        FeatureInjectedQuestData();
    }

    public void FeatureInjectedQuestData()
    {
        var allData = new[]
        {
            DungeonInjectedQuestData.GetData(),
            TrailInjectedQuestData.GetData(),
            MateriaInjectedQuestData.GetData(),
            LocationInjectedQuestData.GetData(),
            GlamourAndCustomizationInjectedQuestData.GetData(),
            StoneSkySeaInjectedQuestData.GetData()
        };

        foreach (var dataSet in allData)
        foreach (var keyValuePair in dataSet)
            dataInjections[keyValuePair.Key] = keyValuePair.Value;
    }

    public void InjectMissingData(IEnumerable<QuestModel> quests)
    {
        foreach (var quest in quests)
            if (dataInjections.TryGetValue(quest.QuestId, out var inject))
                inject(quest);
    }
}
