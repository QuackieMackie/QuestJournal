using System.Collections.Generic;
using System.Linq;
using QuestJournal.Models;

namespace QuestJournal.Utils;

public static class QuestSorter
{
    /// <summary>
    /// Performs a topological sort on a collection of quests based on their prerequisites.
    /// Uses SortKey as a tie-breaker for quests at the same dependency level.
    /// </summary>
    public static List<QuestModel> TopologicalSort(IEnumerable<QuestModel> quests)
    {
        var questList = quests.ToList();
        if (questList.Count <= 1) return questList;

        var questMap = questList.ToDictionary(q => q.QuestId);
        var inDegree = questList.ToDictionary(q => q.QuestId, _ => 0);
        var adjacencyList = questList.ToDictionary(q => q.QuestId, _ => new List<uint>());

        foreach (var quest in questList)
        {
            if (quest.PreviousQuestIds == null) continue;

            foreach (var prevId in quest.PreviousQuestIds)
            {
                if (questMap.ContainsKey(prevId))
                {
                    adjacencyList[prevId].Add(quest.QuestId);
                    inDegree[quest.QuestId]++;
                }
            }
        }

        var priorityQueue = new PriorityQueue<QuestModel, ushort>();
        foreach (var quest in questList.Where(q => inDegree[q.QuestId] == 0))
        {
            priorityQueue.Enqueue(quest, quest.SortKey);
        }

        var sortedList = new List<QuestModel>();

        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Dequeue();
            sortedList.Add(current);

            if (adjacencyList.TryGetValue(current.QuestId, out var dependents))
            {
                foreach (var dependentId in dependents)
                {
                    inDegree[dependentId]--;
                    if (inDegree[dependentId] == 0)
                    {
                        priorityQueue.Enqueue(questMap[dependentId], questMap[dependentId].SortKey);
                    }
                }
            }
        }

        if (sortedList.Count < questList.Count)
        {
            var remaining = questList.Except(sortedList).OrderBy(q => q.SortKey);
            sortedList.AddRange(remaining);
        }

        return sortedList;
    }
}
