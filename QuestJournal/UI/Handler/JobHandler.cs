using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.UI.Handler;

public class JobHandler : IDisposable
{
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;

    public JobHandler(IPluginLog log, IDalamudPluginInterface pluginInterface)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.pluginInterface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface));
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

            var orderedQuests = quests.Where(q => q != null).OrderBy(q => q.SortKey).ToList();
            log.Info($"Loaded and organized {orderedQuests.Count} quests from resource: '{resourcePath}'.");

            return orderedQuests;
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
                        .ToList();
    }
}
