using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using QuestJournal.Handlers;
using QuestJournal.Models;
using QuestJournal.UI.Renderer;

namespace QuestJournal.UI;

public class MainWindow : Window, IDisposable
{
    private readonly QuestJournal plugin;
    private readonly IPluginLog log;
    private readonly MsqHandler msqHandler;
    private readonly MsqRenderer msqRenderer;
    private readonly InformationRenderer informationRenderer;
    private readonly SettingsRenderer settingsRenderer;

    public MainWindow(QuestJournal plugin, List<IQuestInfo> questInfo, IPluginLog log, Configuration configuration) 
        : base("QuestJournal###QuestJournal-QuackieMackie", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.plugin = plugin;
        this.log = log;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(480, 720),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        msqHandler = new MsqHandler(questInfo, log, configuration);
        
        msqRenderer = new MsqRenderer(msqHandler, log);
        informationRenderer = new InformationRenderer();
        settingsRenderer = new SettingsRenderer(configuration);
        
        msqHandler.ReloadFilteredQuests();
    }

    public override void Draw()
    {
        if (ImGui.Button("Refresh##RefreshButton"))
        {
            msqHandler.ReloadFilteredQuests();
            log.Info("Refreshed quest list.");
        }
        
        ImGui.Spacing();

        if (ImGui.BeginTabBar("MainTabBar"))
        {
            if (ImGui.BeginTabItem("MSQ"))
            {
                msqRenderer.DrawMsqTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Information"))
            {
                informationRenderer.DrawInformation();
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
    
    public void Dispose()
    {
        msqHandler.Dispose();
    }
}
