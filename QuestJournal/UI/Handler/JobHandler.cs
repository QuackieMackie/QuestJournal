using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

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
            var jobDirectoryPath = GetJobDirectory();

            if (jobDirectoryPath == null) return null;

            var filePath = Path.Combine(jobDirectoryPath, fileName + ".json");

            if (!File.Exists(filePath))
            {
                log.Error($"File not found: {filePath}");
                return null;
            }

            var fileContent = File.ReadAllText(filePath);
            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);

            if (quests == null)
            {
                log.Error($"Failed to deserialize quests from file {fileName}. The content might be invalid.");
                return null;
            }

            var orderedQuests = quests.Where(q => q != null).OrderBy(q => q.SortKey).ToList();

            log.Info($"Filtered and organized a total of {orderedQuests.Count} quests for file: \"{filePath}\".");
            return orderedQuests;
        }
        catch (Exception ex)
        {
            log.Error($"Error loading file {fileName}: {ex.Message}");
            return null;
        }
    }

    public Dictionary<string, string> GetJobFileNames()
    {
        var jobDirectoryPath = GetJobDirectory();

        if (!Directory.Exists(jobDirectoryPath))
        {
            log.Warning($"JOB directory does not exist: {jobDirectoryPath}");
            return new Dictionary<string, string>();
        }

        log.Info($"Searching JOB files in: {jobDirectoryPath}");

        return Directory.GetFiles(jobDirectoryPath, "*.json", SearchOption.TopDirectoryOnly)
                        .ToDictionary(
                            file =>
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                return fileName.StartsWith("JOB-")
                                           ? fileName.Substring(4).Replace("_", " ")
                                           : fileName.Replace("_", " ");
                            },
                            file => Path.GetFileNameWithoutExtension(file)
                        );
    }

    private string? GetJobDirectory()
    {
        var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName ?? string.Empty;
        var jobDirectoryPath = Path.Combine(outputDirectory, "QuestJournal", "JOB");

        return Directory.Exists(jobDirectoryPath) ? jobDirectoryPath : null;
    }
}
