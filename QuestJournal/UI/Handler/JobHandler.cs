using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.UI.Handler;

public class JobHandler : IDisposable
{
    private readonly IPluginLog log;
    private readonly Configuration configuration;

    public JobHandler(IPluginLog log, Configuration configuration)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.configuration = configuration;
    }
        
    public Dictionary<string, (uint StartQuestId, uint UnlockQuestId)> StarterClassQuestMapping()
    {
        return new Dictionary<string, (uint, uint)>
        {
            { "Gladiator",   (65789, 65821) },
            { "Marauder",    (65847, 65846) },
            { "Pugilist",    (66069, 66068) },
            { "Lancer",      (65559, 65668) },
            { "Archer",      (65557, 65667) },
            { "Thaumaturge", (65881, 65880) },
            { "Arcanist",    (65989, 65988) },
            { "Conjurer",    (65558, 65669) },
        };
    }
    
    public void Dispose() { }

    public List<QuestModel>? FetchQuestData(string fileName)
    {
        try
        {
            var resourcePath = $"{fileName}";
            var fileContent = EmbeddedResourceLoader.LoadJson(resourcePath, "JOB");

            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);
            if (quests == null)
            {
                log.Error($"Failed to deserialize quests from resource '{resourcePath}'. The content might be invalid.");
                return null;
            }
            
            var filteredQuests = PerformQuestFiltering(quests);

            log.Info($"Loaded {filteredQuests.Count} quests from resource '{resourcePath}'.");

            return filteredQuests.Where(q => q != null).OrderBy(q => q.SortKey).ToList();
        }
        catch (FileNotFoundException e)
        {
            log.Error($"Resource not found: {e.Message}");
            return null;
        }
        catch (Exception ex)
        {
            log.Error($"Error loading resource '{fileName}': {ex.Message}");
            return null;
        }
    }

    public Dictionary<string, string> GetJobFileNames()
    {
        var allResources = GetAllJobResources();
        var result = new Dictionary<string, string>();

        foreach (var resource in allResources.Where(res => res.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
        {
            var fileName = resource.Replace("QuestJournal.Data.JOB.", "").Replace(".json", "");
            var displayName = fileName.StartsWith("JOB-", StringComparison.OrdinalIgnoreCase)
                                  ? fileName.Substring(4).Replace("_", " ")
                                  : fileName.Replace("_", " ");

            var uniqueDisplayName = displayName;
            var counter = 1;
            while (result.ContainsKey(uniqueDisplayName))
            {
                uniqueDisplayName = $"{displayName} ({counter++})";
            }

            result[uniqueDisplayName] = fileName;
        }

        if (result.Count == 0)
        {
            log.Warning("No embedded JOB quest files found.");
        }

        return result;
    }
    
    private List<string> GetAllJobResources()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetManifestResourceNames())
                        .Where(res => res.StartsWith("QuestJournal.Data.JOB.", StringComparison.OrdinalIgnoreCase))
                        .Distinct()
                        .ToList();
    }

    private List<QuestModel> PerformQuestFiltering(List<QuestModel> quests)
    {
        var playerStarterClass = configuration.StarterClass;
        var classMapping = StarterClassQuestMapping();

        if (string.IsNullOrWhiteSpace(playerStarterClass) || !classMapping.ContainsKey(playerStarterClass))
            return quests.OrderBy(q => q.SortKey).ToList();

        var (startQuestId, _) = classMapping[playerStarterClass];
        var unlockQuestIds = classMapping
                             .Where(kvp => kvp.Key != playerStarterClass)
                             .Select(kvp => kvp.Value.UnlockQuestId)
                             .ToList();

        var mappedQuestIds = classMapping.SelectMany(kvp => new[] { kvp.Value.StartQuestId, kvp.Value.UnlockQuestId }).ToHashSet();

        return quests.Where(q =>
                                !mappedQuestIds.Contains(q.QuestId) || // Allow quests not in the mapping
                                q.QuestId == startQuestId ||           // Include StartQuestId for selected class
                                unlockQuestIds.Contains(q.QuestId)     // Include UnlockQuestIds for other classes
        ).OrderBy(q => q.SortKey).ToList();
    }
}
