using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.UI.Handler;

public class FeatureHandler : IDisposable
{
    private readonly Configuration configuration;
    private readonly IPluginLog log;

    public FeatureHandler(IPluginLog log, Configuration configuration)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.configuration = configuration;
    }

    public void Dispose() { }

    public Dictionary<string, Dictionary<string, uint>> StartAreaQuestMapping()
    {
        return new Dictionary<string, (uint Gridania, uint Limsa, uint Uldah)>
            {
                //Guild Hests
                { "Simply the Hest", (65596, 65595, 65594) }
            }
            .ToDictionary(
                pair => pair.Key,
                pair => new Dictionary<string, uint>
                {
                    { "Gridania", pair.Value.Gridania },
                    { "Limsa Lominsa", pair.Value.Limsa },
                    { "Ul'dah", pair.Value.Uldah }
                });
    }
    
    public Dictionary<string, List<(uint, uint)>> SwappableQuestMapping()
    {
        return new Dictionary<string, List<(uint, uint)>>
        {
            { "MasterRecipes-GlamoursSet", new List<(uint, uint)>
                {
                    // If I had a Glamour || A Self-Improving Man
                    (68553, 66957),
                    // Absolutely Glamourous || A Submission Impossible
                    (68554, 66958)
                }
            }
        };
    }

    public List<string> GetFeatureSubDirs()
    {
        var allResources = GetAllFeatureResources();
        
        var subDirs = allResources
                      .Select(res => res.Replace("QuestJournal.Data.FEATURE.", "").Split('.')[0])
                      .Distinct()
                      .ToList();

        if (subDirs.Count == 0)
        {
            log.Warning("No FEATURE subdirectories found in embedded resources.");
        }

        return subDirs;
    }

    public Dictionary<string, string> GetJournalGenresInSubDir(string subDir)
    {
        var subDirPrefix = $"QuestJournal.Data.FEATURE.{subDir}.";

        var allResources = GetAllFeatureResources();

        var filesInSubDir = allResources
                            .Where(res => res.StartsWith(subDirPrefix, StringComparison.OrdinalIgnoreCase) && res.EndsWith(".json"))
                            .ToDictionary(
                                res =>
                                {
                                    var fileName = res.Replace(subDirPrefix, "").Replace(".json", "");
                                    return fileName.Replace("_", " ");
                                },
                                res => res.Replace(subDirPrefix, "").Replace(".json", "")
                            );

        if (filesInSubDir.Count == 0)
        {
            log.Warning($"No JSON files found in subdirectory '{subDir}' within embedded resources.");
        }

        return filesInSubDir;
    }

    public List<QuestModel>? FetchQuestDataFromSubDir(string subDir, string fileName)
    {
        try
        {
            var resourcePath = $"{subDir}.{fileName}";
            var fileContent = EmbeddedResourceLoader.LoadJson(resourcePath, "FEATURE");

            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);
            if (quests == null)
            {
                log.Error($"Failed to deserialize quests for resource: {resourcePath}");
                return null;
            }

            var filteredQuests = PerformQuestFiltering(quests);

            log.Info($"Loaded and filtered {filteredQuests.Count} quests from: {resourcePath}");
            return filteredQuests;
        }
        catch (FileNotFoundException e)
        {
            log.Error($"Resource not found: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            log.Error($"Error loading resource '{fileName}' from subdirectory '{subDir}': {e.Message}");
            return null;
        }
    }

    private List<string> GetAllFeatureResources()
    {
        return Assembly.GetExecutingAssembly()
                       .GetManifestResourceNames()
                       .Where(res => res.StartsWith("QuestJournal.Data.FEATURE.", StringComparison.OrdinalIgnoreCase))
                       .ToList();
    }

    private List<QuestModel> PerformQuestFiltering(List<QuestModel> quests)
    {
        // Filter for start area quests
        var playerStartArea = configuration.StartArea;
        var questStartAreaMapping = StartAreaQuestMapping();

        var filteredQuests = !string.IsNullOrWhiteSpace(playerStartArea)
            ? quests.GroupBy(q => q.QuestTitle)
                    .Select(group =>
                    {
                        if (group.Key != null &&
                            questStartAreaMapping.TryGetValue(group.Key, out var areaMapping) &&
                            areaMapping.TryGetValue(playerStartArea, out var mappedQuestId))
                            return group.FirstOrDefault(q => q.QuestId == mappedQuestId);
                        
                        return group.FirstOrDefault();
                    })
                    .Where(q => q != null)
                    .Select(q => q!)
                    .ToList()
            : quests;

        // Filter for swappable quests
        var swappableGroups = SwappableQuestMapping();
        foreach (var group in swappableGroups.Values)
        {
            foreach (var (a, b) in group)
            {
                var aCompleted = QuestManager.IsQuestComplete(a);
                var bCompleted = QuestManager.IsQuestComplete(b);

                if (aCompleted && !bCompleted)
                {
                    filteredQuests.RemoveAll(q => q.QuestId == b);
                }
                else if (bCompleted && !aCompleted)
                {
                    filteredQuests.RemoveAll(q => q.QuestId == a);
                }
            }
        }

        return filteredQuests.OrderBy(q => q.SortKey).ToList();;
    }
}
