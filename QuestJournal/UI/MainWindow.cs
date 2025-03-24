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
    private readonly RendererUtils rendererUtils;
    private readonly IPluginLog log;
    
    private readonly MsqHandler msqHandler;
    
    private readonly MsqRenderer msqRenderer;
    private readonly SettingsRenderer settingsRenderer;

    public MainWindow(IPluginLog log, Configuration configuration, IDalamudPluginInterface pluginInterface) 
        : base("QuestJournal###QuestJournal-QuackieMackie", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.log = log;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(480, 720),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        
        msqHandler = new MsqHandler(log, pluginInterface, configuration);
        
        rendererUtils = new RendererUtils(log);

        msqRenderer = new MsqRenderer(msqHandler, rendererUtils, log);
        settingsRenderer = new SettingsRenderer(configuration, msqRenderer);
    }

    public override void Draw()
    {
        if (ImGui.Button("Refresh##RefreshButton"))
        {
            msqRenderer.ReloadQuests();
            log.Info("Refreshed quest list.");
        }
        
        ImGui.Spacing();

        if (ImGui.BeginTabBar("MainTabBar"))
        {
            if (ImGui.BeginTabItem("MSQ"))
            {
                msqRenderer.DrawMSQ();
                ImGui.EndTabItem();
            }
            
            // if (ImGui.BeginTabItem("Class & Jobs"))
            // {
            //     ImGui.EndTabItem();
            // }
            //
            // if (ImGui.BeginTabItem("Feature"))
            // {
            //     ImGui.EndTabItem();
            // }
            //
            // if (ImGui.BeginTabItem("Leve"))
            // {
            //     ImGui.EndTabItem();
            // }
            //
            // if (ImGui.BeginTabItem("Information"))
            // {
            //     ImGui.EndTabItem();
            // }

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
