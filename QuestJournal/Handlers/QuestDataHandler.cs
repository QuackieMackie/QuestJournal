using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace QuestJournal.Handlers;

public class QuestDataHandler
{
    private readonly Configuration configuration;

    public QuestDataHandler(Configuration configuration, IPluginLog log)
    {
        this.configuration = configuration;
        this.log = log;
    }

    public IPluginLog log { get; }

    public void FetchStartArea()
    {
        configuration.StartArea = QuestManager.IsQuestComplete(65575) ? "Gridania"
                                  : QuestManager.IsQuestComplete(65643) ? "Limsa Lominsa"
                                  : QuestManager.IsQuestComplete(66130) ? "Ul'dah"
                                  : "";

        log.Info($"Configuration start area set to: {configuration.StartArea}");
    }
}
