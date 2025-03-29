using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.Commands;

public class CommandHandler : IDisposable
{
    private const string OpenJournalCommandShort = "/qj";
    private const string OpenJournalCommandFull = "/questjournal";
    private const string FetchQuestCommandName = "/fetch-qd";
    private const string FetchMsqCommandName = "/fetch-msq";
    private const string FetchJobCommandName = "/fetch-job";
    
    private const string QuestDataFileName = "QuestData.json";
    private const string MsqDataFileName = "MsqData.json";
    private const string JobDataFileName = "JobData.json";

    private readonly QuestJournal questJournal;
    private readonly ICommandManager commandManager;
    private readonly Configuration configuration;
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly QuestDataFetcher questDataFetcher;

    public CommandHandler(
        QuestJournal questJournal, ICommandManager commandManager, QuestDataFetcher questDataFetcher, 
        IPluginLog log, IDalamudPluginInterface pluginInterface, Configuration configuration)
    {
        this.questJournal = questJournal;
        this.commandManager = commandManager;
        this.questDataFetcher = questDataFetcher;
        this.log = log;
        this.pluginInterface = pluginInterface;
        this.configuration = configuration;

        AddHandlers();
    }

    public void Dispose()
    {
        commandManager.RemoveHandler(FetchQuestCommandName);
        commandManager.RemoveHandler(FetchMsqCommandName);
        commandManager.RemoveHandler(FetchJobCommandName);
        commandManager.RemoveHandler(OpenJournalCommandFull);
        commandManager.RemoveHandler(OpenJournalCommandShort);
    }

    private void AddHandlers()
    {
        commandManager.AddHandler(OpenJournalCommandShort, new CommandInfo(OnOpenCommand)
        {
            HelpMessage = "Open the Quest Journal."
        });

        commandManager.AddHandler(OpenJournalCommandFull, new CommandInfo(OnOpenCommand)
        {
            HelpMessage = "Open the Quest Journal."
        });
        
        commandManager.AddHandler(FetchQuestCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch all quest data from the Lumina sheets and save it to the plugin's output directory."
        });
        
        commandManager.AddHandler(FetchMsqCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch msq data from the Lumina sheets and save it to the plugin's output directory."
        });
        
        commandManager.AddHandler(FetchJobCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch job data from the Lumina sheets and save it to the plugin's output directory."
        });
    }
    
    private void OnOpenCommand(string command, string args)
    {
        if (command == "/qj" || command == "/questjournal")
        {
            OpenJournal();
        }
    }
    
    private void OnFetchCommand(string command, string args)
    {
        if (!configuration.DeveloperMode)
        {
            log.Information("The fetch commands are only available in Developer Mode.");
            return;
        }

        if (command == FetchQuestCommandName) FetchQuestData();
        if (command == FetchMsqCommandName) FetchMsqData();
        if (command == FetchJobCommandName) FetchJobData();
    }

    private void OpenJournal()
    {
        questJournal.OpenMainWindow();
    }

    private void FetchQuestData()
    {
        try
        {
            var allQuests = questDataFetcher.GetAllQuests();

            var filePath = GetOutputFilePath(QuestDataFileName);
            SaveQuestDataToJson(allQuests, filePath);
        }
        catch (Exception ex)
        {
            log.Error($"Error fetching or saving quest data: {ex.Message}");
        }
    }

    public void FetchMsqData()
    {
        try
        {
            var categorizedMsqData = questDataFetcher.GetMainScenarioQuestsByCategory();

            var baseFilePath = GetOutputFilePath(MsqDataFileName);
            var baseDirectoryPath = Path.GetDirectoryName(baseFilePath);

            var msqDirectoryPath = Path.Combine(baseDirectoryPath ?? ".", "MSQ");
            if (!Directory.Exists(msqDirectoryPath))
            {
                Directory.CreateDirectory(msqDirectoryPath);
            }

            foreach (var categoryEntry in categorizedMsqData)
            {
                var categoryName = categoryEntry.Key;
                var quests = categoryEntry.Value;

                var sanitizedFileName = $"MSQ-{string.Concat(categoryName.Replace(" ", "_").Split(Path.GetInvalidFileNameChars()))}.json";
                var filePath = Path.Combine(msqDirectoryPath, sanitizedFileName);
                
                SaveQuestDataToJson(quests, filePath, categoryName);
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error fetching or saving MSQ data: {ex.Message}");
        }
    }
    
    public void FetchJobData()
    {
        try
        {
            var categorizedJobData = questDataFetcher.GetJobQuestsByCategory();

            var baseFilePath = GetOutputFilePath(JobDataFileName);
            var baseDirectoryPath = Path.GetDirectoryName(baseFilePath);

            var jobDirectoryPath = Path.Combine(baseDirectoryPath ?? ".", "JOB");
            if (!Directory.Exists(jobDirectoryPath))
            {
                Directory.CreateDirectory(jobDirectoryPath);
            }

            foreach (var categoryEntry in categorizedJobData)
            {
                var categoryName = categoryEntry.Key;
                var quests = categoryEntry.Value;

                var sanitizedFileName = $"Job-{string.Concat(categoryName.Replace(" ", "_").Split(Path.GetInvalidFileNameChars()))}.json";
                var filePath = Path.Combine(jobDirectoryPath, sanitizedFileName);

                SaveQuestDataToJson(quests, filePath, categoryName);
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error fetching or saving Job data: {ex.Message}");
        }
    }

    private void SaveQuestDataToJson(List<QuestModel> questData, string filePath, string? category = null)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(questData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);

            if (category != null) log.Info($"Saved {questData.Count} quests for category '{category}; to file path: '{filePath}'");
            if (category == null) log.Info($"Saved {questData.Count} quests to file path: '{filePath}'");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to save the file at '{filePath}': {ex.Message}");
        }
    }

    private string GetOutputFilePath(string fileName)
    {
        var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName ?? "";
        return Path.Combine(outputDirectory, fileName);
    }
}
