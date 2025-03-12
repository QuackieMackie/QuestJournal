using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.Handlers;

public class CommandHandler : IDisposable
{
    private const string FetchQuestCommandName = "/fetch-qd";
    private const string QuestDataFileName = "QuestData.json";

    private readonly ICommandManager commandManager;
    private readonly Configuration configuration;
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly QuestDataFetcher questDataFetcher;

    public CommandHandler(
        ICommandManager commandManager, QuestDataFetcher questDataFetcher, IPluginLog log,
        IDalamudPluginInterface pluginInterface, Configuration configuration)
    {
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
    }

    private void AddHandlers()
    {
        commandManager.AddHandler(FetchQuestCommandName, new CommandInfo(OnFetchCommand)
        {
            HelpMessage = $"Fetch quest data from the game with {FetchQuestCommandName}"
        });
    }

    private void OnFetchCommand(string command, string args)
    {
        if (!configuration.DeveloperMode)
        {
            log.Information("The fetch command is only available in Developer Mode.");
            return;
        }

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

    private void LogQuests(IEnumerable<IQuestInfo> quests)
    {
        var count = 0;

        foreach (var quest in quests)
        {
            log.Information(
                $"Quest ID: {quest.QuestId}, " +
                $"Title: {quest.QuestTitle}, " +
                $"Prerequisite Quest IDs: [{string.Join(", ", quest.PreviousQuestIds ?? new List<uint>())}], " +
                $"Prerequisite Quest Titles: [{string.Join(", ", quest.PreviousQuestTitles ?? new List<string>())}], " +
                $"Starter NPC: {quest.StarterNpc}, " +
                $"Finish NPC: {quest.FinishNpc}, " +
                $"Expansion: {quest.Expansion}, " +
                $"Journal Genre: {quest.JournalGenre}, " +
                $"Is Repeatable: {quest.IsRepeatable}");

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
