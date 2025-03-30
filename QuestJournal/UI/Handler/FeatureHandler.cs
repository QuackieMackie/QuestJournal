using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

namespace QuestJournal.UI.Handler;

public class FeatureHandler : IDisposable
{
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;

    public FeatureHandler(IPluginLog log, IDalamudPluginInterface pluginInterface)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.pluginInterface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface));
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
            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);

            if (quests == null)
            {
                log.Error($"Failed to deserialize quests from file {fileName}.json.");
                return null;
            }

            return quests.OrderBy(q => q.SortKey).ToList();
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

    public void Dispose() { }
}
