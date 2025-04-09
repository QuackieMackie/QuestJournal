using System.Reflection;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Commands;
using QuestJournal.UI;

namespace QuestJournal;

public sealed class QuestJournal : IDalamudPlugin
{
    public readonly WindowSystem WindowSystem = new("QuestJournal");

    public QuestJournal()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        QuestDataFetcher = new QuestDataFetcher(DataManager, Log);
        CommandHandler = new CommandHandler(this, CommandManager, QuestDataFetcher, Log, PluginInterface, Configuration);

        MainWindow = new MainWindow(Log, Configuration, PluginInterface);
        WindowSystem.AddWindow(MainWindow);

        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;

        if (Configuration.DeveloperMode)
        {
            var resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (var resource in resources)
            {
                Log.Info(resource);
            }
        }
    }

    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

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
