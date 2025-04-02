using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

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
            List<QuestModel>? questsToSave = null;
            Dictionary<string, List<QuestModel>>? categorizedQuests = null;
            Dictionary<string, Dictionary<string, List<QuestModel>>>? groupedQuests = null;

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
                    parentFolderName = "Fetched-QuestJournal-Data/MSQ";
                    break;

                case "Job":
                    categorizedQuests = questDataFetcher.GetJobQuestsByCategory();
                    baseFileName = JobDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data/JOB";
                    break;

                case "Feature":
                    groupedQuests = questDataFetcher.GetFeatureQuestsByCategory();
                    baseFileName = FeatureDataFileName;
                    parentFolderName = "Fetched-QuestJournal-Data/FEATURE";
                    break;

                default:
                    log.Error($"Invalid fetch type: {fetchType}");
                    return;
            }

            if (questsToSave != null)
            {
                var filePath = Path.Combine(GetOutputPath(parentFolderName), baseFileName);
                SaveQuestDataToJson(questsToSave, filePath);
                log.Info($"Fetched and saved {questsToSave.Count} total quests to {filePath}.");
                return;
            }

            if (categorizedQuests != null)
            {
                var categoryDirectory = GetOutputPath(parentFolderName);

                foreach (var categoryEntry in categorizedQuests)
                {
                    var sanitizedFileName = $"{SanitizeFileName(categoryEntry.Key)}.json";
                    var filePath = Path.Combine(categoryDirectory, sanitizedFileName);
                    SaveQuestDataToJson(categoryEntry.Value, filePath, categoryEntry.Key);
                }

                log.Info($"Fetched and saved categorized {fetchType} data in the `{categoryDirectory}` directory.");
                return;
            }

            if (groupedQuests != null)
            {
                var featureDirectory = GetOutputPath(parentFolderName);

                foreach (var folderEntry in groupedQuests)
                {
                    var folderPath = Path.Combine(featureDirectory, folderEntry.Key.Replace(" ", "_"));
                    DoesDirectoryExists(folderPath);

                    foreach (var category in folderEntry.Value)
                    {
                        var filePath = Path.Combine(folderPath, $"{SanitizeFileName(category.Key)}.json");
                        SaveQuestDataToJson(category.Value, filePath, category.Key);
                    }
                }

                log.Info($"Fetched and saved grouped {fetchType} data in the `{featureDirectory}` directory.");
            }
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
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            
            var jsonString = JsonSerializer.Serialize(questData, serializerOptions);
            File.WriteAllText(filePath, jsonString);

            log.Info(category != null
                         ? $"Saved {questData.Count} quests for category '{category}' to file path '{filePath}'."
                         : $"Saved {questData.Count} quests to file path '{filePath}'.");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to save the file at '{filePath}': {ex.Message}");
        }
    }

    private string SanitizeFileName(string name)
    {
        return string.Concat(name.Replace(" ", "_").Split(Path.GetInvalidFileNameChars()));
    }

    private string GetOutputPath(string folderName)
    {
        var outputBasePath = pluginInterface.AssemblyLocation.Directory?.FullName ?? "";
        var fullPath = Path.Combine(outputBasePath, folderName);
        DoesDirectoryExists(fullPath);
        return fullPath;
    }

    private void DoesDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
