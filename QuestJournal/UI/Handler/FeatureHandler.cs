using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using QuestJournal.Models;

namespace QuestJournal.UI.Handler;

public class FeatureHandler : IDisposable
{
    private readonly Configuration configuration;
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;

    public FeatureHandler(IPluginLog log, IDalamudPluginInterface pluginInterface, Configuration configuration)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.pluginInterface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface));
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
        var featureDirectoryPath = GetFeatureDirectory();

        if (string.IsNullOrEmpty(featureDirectoryPath) || !Directory.Exists(featureDirectoryPath))
        {
            log.Warning($"FEATURE directory does not exist: {featureDirectoryPath}");
            return new List<string>();
        }

        return Directory.GetDirectories(featureDirectoryPath)
                        .Select(subDir => Path.GetFileName(subDir))
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();
    }

    public Dictionary<string, string> GetJournalGenresInSubDir(string subDir)
    {
        var featureDirectoryPath = GetFeatureDirectory();

        if (string.IsNullOrEmpty(featureDirectoryPath))
        {
            log.Warning("Feature directory path is null or empty.");
            return new Dictionary<string, string>();
        }

        var subDirPath = Path.Combine(featureDirectoryPath, subDir);

        if (!Directory.Exists(subDirPath))
        {
            log.Warning($"Subdirectory does not exist: {subDirPath}");
            return new Dictionary<string, string>();
        }

        return Directory.GetFiles(subDirPath, "*.json", SearchOption.TopDirectoryOnly)
                        .Where(file => !string.IsNullOrEmpty(file))
                        .ToDictionary(
                            file =>
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                return fileName!.Replace("_", " ");
                            },
                            file => Path.GetFileNameWithoutExtension(file));
    }

    public List<QuestModel>? FetchQuestDataFromSubDir(string subDir, string fileName)
    {
        var questStartAreaMapping = StartAreaQuestMapping();
        var featureDirectoryPath = GetFeatureDirectory();

        if (string.IsNullOrEmpty(featureDirectoryPath))
        {
            log.Error("Feature directory path is null or empty.");
            return null;
        }

        var subDirPath = Path.Combine(featureDirectoryPath, subDir);

        if (string.IsNullOrEmpty(subDirPath) || !Directory.Exists(subDirPath))
        {
            log.Warning($"Subdirectory does not exist: {subDirPath}");
            return null;
        }

        var filePath = Path.Combine(subDirPath, fileName + ".json");

        if (!File.Exists(filePath))
        {
            log.Error($"File not found: {filePath}");
            return null;
        }

        try
        {
            var fileContent = File.ReadAllText(filePath);
            var playerStartArea = configuration.StartArea;
            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);

            if (quests == null)
            {
                log.Error($"Failed to deserialize quests from file {fileName}.json.");
                return null;
            }

            var filteredQuests = new List<QuestModel>();

            // Start Area Filtering
            if (!string.IsNullOrWhiteSpace(playerStartArea))
            {
                filteredQuests = quests
                                 .Where(q => q != null)
                                 .GroupBy(q => q.QuestTitle)
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
                                 .ToList();
            }
            
            // Swappable Quest Filtering
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

            return filteredQuests.OrderBy(q => q.SortKey).ToList();
        }
        catch (Exception ex)
        {
            log.Error($"Error loading quests from file {fileName}: {ex.Message}");
            return null;
        }
    }

    private string? GetFeatureDirectory()
    {
        var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName;

        if (string.IsNullOrEmpty(outputDirectory))
        {
            log.Error("Output directory is null or empty.");
            return null;
        }

        var featureDirectoryPath = Path.Combine(outputDirectory, "QuestJournal", "FEATURE");

        return Directory.Exists(featureDirectoryPath) ? featureDirectoryPath : null;
    }
}
