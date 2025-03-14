using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Handlers;
using QuestJournal.Models;
using QuestJournal.UI;
using QuestJournal.Utils;

namespace QuestJournal;

public sealed class QuestJournal : IDalamudPlugin
{
    public readonly string DataDirectory;

    public readonly WindowSystem WindowSystem = new("QuestJournal");
    private int? cachedQuestCount;

    public QuestJournal()
    {
        DataDirectory = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "QuestJournal");
        if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        QuestDataFetcher = new QuestDataFetcher(DataManager, Log);
        QuestDataHandler = new QuestDataHandler(Configuration, Log);
        CommandHandler = new CommandHandler(CommandManager, QuestDataFetcher, Log, PluginInterface, Configuration);
        CacheQuestData();

        MainWindow = new MainWindow(this, CachedQuests, Log, Configuration);

        WindowSystem.AddWindow(MainWindow);

        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi += () => OpenMainWindow();
        PluginInterface.UiBuilder.OpenMainUi += () => OpenMainWindow();
    }

    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IDataManager DataManager { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog Log { get; private set; } = null!;

    public Configuration Configuration { get; init; }
    private MainWindow MainWindow { get; init; }

    private QuestDataFetcher QuestDataFetcher { get; init; }
    private QuestDataHandler QuestDataHandler { get; init; }
    private CommandHandler CommandHandler { get; init; }
    private List<IQuestInfo> CachedQuests { get; set; } = new();

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();

        CommandHandler.Dispose();
    }

    private void DrawUi()
    {
        WindowSystem.Draw();
    }

    public void OpenMainWindow()
    {
        MainWindow.Toggle();
    }

    private void CacheQuestData()
    {
        var questDataPath = Path.Combine(DataDirectory, "QuestData.json");

        if (!File.Exists(questDataPath))
        {
            Log.Error($"Quest data file not found at '{questDataPath}'.");
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(questDataPath);
            var questData = JsonSerializer.Deserialize<List<IQuestInfo>>(jsonContent);

            if (questData == null || !questData.Any())
            {
                Log.Error($"Quest data file is empty or invalid at '{questDataPath}'.");
                return;
            }

            CachedQuests = questData.Where(q => q.QuestId != 0).ToList();
            cachedQuestCount = CachedQuests.Count;
            Log.Info($"Cached quest count: {cachedQuestCount}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error reading or parsing QuestData.json: {ex.Message}");
        }
    }
}
