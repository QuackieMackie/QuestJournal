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
    
    private const string QuestDataFileName = "QuestData.json";
    private const string MsqDataFileName = "MsqData.json";

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

            LogQuests(allQuests);

            var filePath = GetOutputFilePath(QuestDataFileName);
            questDataFetcher.SaveQuestDataToJson(allQuests, filePath);

            log.Information($"Quest data successfully fetched and saved to {filePath}.");
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

    private void SaveQuestDataToJson(List<QuestModel> questData, string filePath, string msqCategory)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(questData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);

            log.Info($"Saved {questData.Count} quests for MSQ category '{msqCategory}; to file path: '{filePath}'");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to save the file at '{filePath}': {ex.Message}");
        }
    }

    private void LogQuests(List<QuestModel?> quests)
    {
        var count = 0;

        foreach (var quest in quests)
        {
            if (quest != null)
            {
                log.Information(
                    $"Quest ID: {quest.QuestId}, " +
                    $"Title: {quest.QuestTitle}, " +
                    $"Prerequisite Quest IDs: [{string.Join(", ", quest.PreviousQuestIds ?? new List<uint>())}], " +
                    $"Prerequisite Quest Titles: [{string.Join(", ", quest.PreviousQuestTitles ?? new List<string>())}], " +
                    $"Starter NPC: {quest.StarterNpc}, " +
                    $"Finish NPC: {quest.FinishNpc}, " +
                    $"Expansion: {quest.Expansion}, " +
                    $"Journal Genre: {quest.JournalGenre}, ");
            }

            count++;
        }

        log.Information($"Total quests fetched: {count}");
    }

    private string GetOutputFilePath(string fileName)
    {
        var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName ?? "";
        return Path.Combine(outputDirectory, fileName);
    }
}
