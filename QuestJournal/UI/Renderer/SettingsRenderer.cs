using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using Serilog;

namespace QuestJournal.UI.Renderer;

public class SettingsRenderer(Configuration configuration, MainWindow mainsWindow)
{
    public void DrawSettings()
    {
        ImGui.Text("Settings");
        ImGui.SameLine(ImGui.GetContentRegionAvail().X - 90);
        if (ImGui.Button("Reset Settings", new System.Numerics.Vector2(100, 0)))
        {
            configuration.Reset();
            configuration.Save();
        }

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
                    mainsWindow.RefreshQuests();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
        
        ImGui.SameLine();
        if (ImGui.Button("No clue!##startArea"))
        {
            configuration.StartArea = GetStartArea();
            configuration.Save();
            mainsWindow.RefreshQuests();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("If you don't know what to select, click the button to automatically fill it in the correct value.");
            ImGui.EndTooltip();
        }

        ImGui.Text("Select your desired/current Grand Company:");
        var grandCompanyOptions = new List<string> { "Immortal Flames", "Maelstorm", "Twin Adder" };
        var grandCompanyCurrentSelection = configuration.GrandCompany ?? string.Empty;

        if (ImGui.BeginCombo("##GrandCompanyDropdown", grandCompanyCurrentSelection))
        {
            foreach (var option in grandCompanyOptions)
            {
                var isSelected = option == grandCompanyCurrentSelection;
                if (ImGui.Selectable(option, isSelected))
                {
                    configuration.GrandCompany = option;
                    configuration.Save();
                    mainsWindow.RefreshQuests();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
        
        ImGui.SameLine();
        if (ImGui.Button("No clue!##GrandCompany"))
        {
            configuration.GrandCompany = GetGrandCompany();
            configuration.Save();
            mainsWindow.RefreshQuests();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("If you don't know what to select, click the button to automatically fill it in the correct value.");
            ImGui.EndTooltip();
        }
        
        ImGui.Text("Select your starter class:");
        var starterClassOptions = new List<string> { "Gladiator", "Marauder", "Pugilist", "Lancer", "Archer", "Thaumaturge", "Arcanist", "Conjurer" };
        var starterClassCurrentSelection = configuration.StarterClass ?? string.Empty;
        
        if (ImGui.BeginCombo("##StarterClassDropdown", starterClassCurrentSelection))
        {
            foreach (var option in starterClassOptions)
            {
                var isSelected = option == starterClassCurrentSelection;
                if (ImGui.Selectable(option, isSelected))
                {
                    configuration.StarterClass = option;
                    configuration.Save();
                    mainsWindow.RefreshQuests();
                }

                if (isSelected) ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }
        
        ImGui.SameLine();
        if (ImGui.Button("No clue!##StarterClass"))
        {
            configuration.StarterClass = GetStarterClass();
            configuration.Save();
            mainsWindow.RefreshQuests();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("If you don't know what to select, click the button to automatically fill it in the correct value.");
            ImGui.EndTooltip();
        }
        
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Spacing();
        
        ImGui.Separator();
        ImGui.Text("Censor starter locations for MSQ");
        var censorStarterLocations = configuration.CensorStarterLocations;
        if (ImGui.Checkbox(
                "This replaces starter locations for uncompleted quests with '????' instead of showing their names.",
                ref censorStarterLocations))
        {
            configuration.CensorStarterLocations = censorStarterLocations;
            configuration.Save();
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
    
    private string GetStartArea()
    {
        if (QuestManager.IsQuestComplete(65621) || QuestManager.IsQuestComplete(65659) || QuestManager.IsQuestComplete(65660)) return "Gridania";
        if (QuestManager.IsQuestComplete(65644) || QuestManager.IsQuestComplete(65645)) return "Limsa Lominsa";
        if (QuestManager.IsQuestComplete(66104) || QuestManager.IsQuestComplete(66105) || QuestManager.IsQuestComplete(66106)) return "Ul'dah";
        return "Gridania";
    }
    
    private string GetGrandCompany()
    {
        if (QuestManager.IsQuestComplete(66217)) return "Maelstorm";
        if (QuestManager.IsQuestComplete(66216)) return "Twin Adder";
        if (QuestManager.IsQuestComplete(66218)) return "Immortal Flames";
        return "Maelstorm";
    }
    
    private string GetStarterClass()
    {
        if (QuestManager.IsQuestComplete(65789)) return "Gladiator";
        if (QuestManager.IsQuestComplete(65847)) return "Marauder";
        if (QuestManager.IsQuestComplete(66069)) return "Pugilist";
        if (QuestManager.IsQuestComplete(65559)) return "Lancer";
        if (QuestManager.IsQuestComplete(65557)) return "Archer";
        if (QuestManager.IsQuestComplete(65881)) return "Thaumaturge";
        if (QuestManager.IsQuestComplete(65989)) return "Arcanist";
        if (QuestManager.IsQuestComplete(65558)) return "Conjurer";
        return "Gladiator";
    }
}
