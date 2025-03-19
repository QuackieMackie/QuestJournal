using System.Collections.Generic;
using System.IO;
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

    public QuestJournal()
    {
        DataDirectory = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "QuestJournal");
        if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        QuestDataFetcher = new QuestDataFetcher(DataManager, Log);
        CommandHandler = new CommandHandler(CommandManager, QuestDataFetcher, Log, PluginInterface, Configuration);

        MainWindow = new MainWindow(this, Log, Configuration, PluginInterface);
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
    private CommandHandler CommandHandler { get; init; }

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
}
