using System.Collections.Generic;
using ImGuiNET;

namespace QuestJournal.UI.Renderer;

public class SettingsRenderer(Configuration configuration, MsqRenderer msqRenderer)
{
    public void DrawSettings()
    {
        ImGui.Text("Settings");
        ImGui.Separator();
        ImGui.Text("Select which area you started in:");
        var startAreaOptions = new List<string> { "Gridania", "Limsa Lominsa", "Ul'dah" };
        var startAreaCurrentSelection = configuration.StartArea ?? string.Empty;

        if (ImGui.BeginCombo("##startAreaDropdown", startAreaCurrentSelection))
        {
            foreach (var option in startAreaOptions)
            {
                var isSelected = option == startAreaCurrentSelection;
                if (ImGui.Selectable(option, isSelected))
                {
                    configuration.StartArea = option;
                    configuration.Save();
                    msqRenderer.ReloadQuests();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        ImGui.Text("Select your desired/current Grand Company:");
        var options = new List<string> { "Immortal Flames", "Maelstorm", "Twin Adder" };
        var currentSelection = configuration.GrandCompany ?? string.Empty;

        if (ImGui.BeginCombo("##GrandCompanyDropdown", currentSelection))
        {
            foreach (var option in options)
            {
                var isSelected = option == currentSelection;
                if (ImGui.Selectable(option, isSelected))
                {
                    configuration.GrandCompany = option;
                    configuration.Save();
                    msqRenderer.ReloadQuests();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
        
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.Text("Developer Settings");
        ImGui.Separator();
        var devMode = configuration.DeveloperMode;
        ImGui.Text("Developer Mode");
        if (ImGui.Checkbox("This enables you to fetch quest data from the Lumina sheets don't use\nunless you know what you're doing.", ref devMode))
        {
            configuration.DeveloperMode = devMode;
            configuration.Save();
        }
    }
}
