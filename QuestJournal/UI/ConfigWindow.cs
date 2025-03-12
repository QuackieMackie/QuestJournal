using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace QuestJournal.UI;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    public ConfigWindow(QuestJournal questJournal) : base("QuestJournal Settings###QuestJournalSettings-QuackieMackie")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 150);
        SizeCondition = ImGuiCond.Always;

        configuration = questJournal.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var devMode = configuration.DeveloperMode;
        if (ImGui.Checkbox("Developer Mode", ref devMode))
        {
            configuration.DeveloperMode = devMode;
            configuration.Save();
        }

        ImGui.Separator();

        ImGui.Text("Debug Custom Start Area:");

        var options = new List<string> { "Gridania", "Limsa Lominsa", "Ul'dah" };
        var currentSelection = configuration.StartArea ?? string.Empty;

        if (ImGui.BeginCombo("##startAreaDropdown", currentSelection))
        {
            foreach (var option in options)
            {
                var isSelected = option == currentSelection;
                if (ImGui.Selectable(option, isSelected))
                {
                    configuration.StartArea = option;
                    configuration.Save();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
    }
}
