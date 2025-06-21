using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using QuestJournal.UI.Handler;
using QuestJournal.UI.Renderer;
using QuestJournal.Utils;

namespace QuestJournal.UI;

public class MainWindow : Window, IDisposable
{
    private readonly QuestJournal questJournal;
    private readonly FeatureHandler featureHandler;
    private readonly FeatureRenderer featureRenderer;
    private readonly JobHandler jobHandler;
    private readonly JobRenderer jobRenderer;
    private readonly IPluginLog log;

    private readonly MsqHandler msqHandler;

    private readonly MsqRenderer msqRenderer;
    private readonly RendererUtils rendererUtils;
    private readonly SettingsRenderer settingsRenderer;

    public MainWindow(IPluginLog log, Configuration configuration, QuestJournal questJournal)
        : base("QuestJournal###QuestJournal-QuackieMackie",
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.questJournal = questJournal;
        this.log = log;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(480, 720),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        msqHandler = new MsqHandler(log, configuration);
        jobHandler = new JobHandler(log, configuration);
        featureHandler = new FeatureHandler(log, configuration);

        rendererUtils = new RendererUtils(log, questJournal);

        msqRenderer = new MsqRenderer(msqHandler, rendererUtils, configuration, log);
        jobRenderer = new JobRenderer(jobHandler, rendererUtils, log);
        featureRenderer = new FeatureRenderer(featureHandler, rendererUtils, log);
        settingsRenderer = new SettingsRenderer(configuration, this);
    }

    public void Dispose()
    {
        msqHandler.Dispose();
        featureHandler.Dispose();
        jobHandler.Dispose();
    }

    public override void Draw()
    {
        if (ImGui.Button("Refresh##RefreshButton"))
        {
            msqRenderer.ReloadQuests();
            jobRenderer.ReloadQuests();
            featureRenderer.ReloadQuests();
            log.Info("Refreshed quest list.");
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button("Close All Quest Detail Windows##ClearPopOutWindowsButton\n"))
        {
            questJournal.CloseAllQuestWindows();
        }

        ImGui.Spacing();

        if (ImGui.BeginTabBar("MainTabBar"))
        {
            if (ImGui.BeginTabItem("MSQ"))
            {
                msqRenderer.DrawMSQ();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Class & Jobs"))
            {
                jobRenderer.DrawJobs();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Feature"))
            {
                featureRenderer.DrawFeatures();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Settings"))
            {
                settingsRenderer.DrawSettings();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    public void RefreshQuests()
    {
        msqRenderer.ReloadQuests();
        jobRenderer.ReloadQuests();
        featureRenderer.ReloadQuests();
    }
}
