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
    private const string FetchQuestCommandName = "/fetch-all";
    private const string FetchMsqCommandName = "/fetch-msq";
    private const string FetchJobCommandName = "/fetch-job";
    private const string FetchFeatureCommandName = "/fetch-feature";
    
    private const string QuestDataFileName = "QuestData.json";
    private const string MsqDataFileName = "MsqData.json";
    private const string JobDataFileName = "JobData.json";
    private const string FeatureDataFileName = "FeatureData.json";

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
        commandManager.RemoveHandler(FetchFeatureCommandName);
        commandManager.RemoveHandler(OpenJournalCommandFull);
        commandManager.RemoveHandler(OpenJournalCommandShort);
    }

    private void AddHandlers()
    {
        commandManager.AddHandler(OpenJournalCommandShort, new CommandInfo(OnOpenCommand)
        {
            HelpMessage = "Open the Quest Journal.",
            ShowInHelp = false
        });

        commandManager.AddHandler(OpenJournalCommandFull, new CommandInfo(OnOpenCommand)
        {
            HelpMessage = "Open the Quest Journal.",
            ShowInHelp = true
        });
        
        commandManager.AddHandler(FetchQuestCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch all quest data from the Lumina sheets and save it to the plugin's output directory.",
            ShowInHelp = false
        });
        
        commandManager.AddHandler(FetchMsqCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch msq data from the Lumina sheets and save it to the plugin's output directory.",
            ShowInHelp = false
        });
        
        commandManager.AddHandler(FetchJobCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch job data from the Lumina sheets and save it to the plugin's output directory.",
            ShowInHelp = false
        });
        
        commandManager.AddHandler(FetchFeatureCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = "[Developer Mode] Fetch feature data from the Lumina sheets and save it to the plugin's output directory.",
            ShowInHelp = false
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

        if (command == FetchQuestCommandName) FetchData("Quest");
        if (command == FetchMsqCommandName) FetchData("MSQ");
        if (command == FetchJobCommandName) FetchData("Job");
        if (command == FetchFeatureCommandName) FetchData("Feature");
    }

    private void OpenJournal()
    {
        questJournal.OpenMainWindow();
    }
    
    private void FetchData(string fetchType)
    {
        try
        {
            List<QuestModel> questsToSave = new();
            Dictionary<string, List<QuestModel>> categorizedQuests = new();
            Dictionary<string, Dictionary<string, List<QuestModel>>> groupedCategorizedQuests = new();
            string baseFileName;
            string parentFolderName;

            switch (fetchType)
            {
                case "Quest":
                    questsToSave = questDataFetcher.GetAllQuests();
                    baseFileName = QuestDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data";
                    break;

                case "MSQ":
                    categorizedQuests = questDataFetcher.GetMainScenarioQuestsByCategory();
                    baseFileName = MsqDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data\\MSQ";
                    break;

                case "Job":
                    categorizedQuests = questDataFetcher.GetJobQuestsByCategory();
                    baseFileName = JobDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data\\JOB";
                    break;

                case "Feature":
                    groupedCategorizedQuests = questDataFetcher.GetFeatureQuestsByCategory();
                    baseFileName = FeatureDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data\\FEATURE";
                    break;

                default:
                    log.Error($"Invalid fetch type: {fetchType}");
                    return;
            }

            if (fetchType == "Quest")
            {
                var baseFilePath = GetOutputFilePath("");
                var parentDirectory = Path.Combine(baseFilePath, parentFolderName);

                if (!Directory.Exists(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }

                var filePath = Path.Combine(parentDirectory, baseFileName);
                SaveQuestDataToJson(questsToSave, filePath);
                log.Info($"Fetched and saved {questsToSave.Count} total quests to {filePath}.");
                return;
            }

            if (fetchType == "Feature")
            {
                var outputBasePath = GetOutputFilePath("");
                var featureDirectory = Path.Combine(outputBasePath, parentFolderName);

                if (!Directory.Exists(featureDirectory))
                {
                    Directory.CreateDirectory(featureDirectory);
                }

                foreach (var folderEntry in groupedCategorizedQuests)
                {
                    var folderName = folderEntry.Key;
                    var categories = folderEntry.Value;
                    
                    var folderPath = Path.Combine(featureDirectory, folderName.Replace(" ", "_"));
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    foreach (var categoryEntry in categories)
                    {
                        var categoryName = categoryEntry.Key;
                        var quests = categoryEntry.Value;
                        
                        var sanitizedFileName = $"{string.Concat(categoryName.Replace(" ", "_").Split(Path.GetInvalidFileNameChars()))}.json";
                        var filePath = Path.Combine(folderPath, sanitizedFileName);

                        SaveQuestDataToJson(quests, filePath, categoryName);
                    }
                }

                log.Info($"Fetched and saved categorized {fetchType} data in the `{featureDirectory}` directory.");
                return;
            }
            
            var outputPath = GetOutputFilePath("");
            var categoryDirectory = Path.Combine(outputPath, parentFolderName);

            if (!string.IsNullOrEmpty(parentFolderName) && !Directory.Exists(categoryDirectory))
            {
                Directory.CreateDirectory(categoryDirectory);
            }

            foreach (var categoryEntry in categorizedQuests)
            {
                var categoryName = categoryEntry.Key;
                var quests = categoryEntry.Value;

                var sanitizedFileName = $"{string.Concat(categoryName.Replace(" ", "_").Split(Path.GetInvalidFileNameChars()))}.json";
                var filePath = Path.Combine(categoryDirectory, sanitizedFileName);

                SaveQuestDataToJson(quests, filePath, categoryName);
            }

            log.Info($"Fetched and saved categorized {fetchType} data in the `{categoryDirectory}` directory.");
        }
        catch (Exception ex)
        {
            log.Error($"Error fetching or saving {fetchType} data: {ex.Message}");
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
