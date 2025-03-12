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
    private readonly IPluginLog log;
    private readonly MsqHandler msqHandler;
    private readonly MsqRenderer msqRenderer;

    public MainWindow(List<IQuestInfo> questInfo, IPluginLog log, Configuration configuration)
        : base("QuestJournal###QuestJournal-QuackieMackie",
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.log = log;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(480, 720),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        msqHandler = new MsqHandler(questInfo, log, configuration);
        msqRenderer = new MsqRenderer(msqHandler, log);

        msqHandler.ReloadFilteredQuests();
    }

    public void Dispose()
    {
        msqHandler.Dispose();
    }

    public override void Draw()
    {
        var questCount = msqHandler.GetQuestCount();
        ImGui.Text($"Loaded {questCount} quests.");

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - ImGui.CalcTextSize("Refresh").X -
                            ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.Button("Refresh##RefreshButton"))
        {
            msqHandler.ReloadFilteredQuests();
            log.Info("Refreshed quest list.");
        }

        if (ImGui.BeginTabBar("MainTabBar"))
        {
            if (ImGui.BeginTabItem("MSQ"))
            {
                msqRenderer.DrawMsqTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Information"))
            {
                ImGui.Text("Information tab coming soon!");
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }
}
