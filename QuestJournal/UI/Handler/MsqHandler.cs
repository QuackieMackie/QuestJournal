﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using QuestJournal.Models;

namespace QuestJournal.UI.Handler;

public class MsqHandler : IDisposable
{
    private readonly IPluginLog log;
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly Configuration configuration;

    public MsqHandler(IPluginLog log, IDalamudPluginInterface pluginInterface, Configuration configuration)
    {
        this.log = log ?? throw new ArgumentNullException(nameof(log));
        this.pluginInterface = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    
    public Dictionary<string, Dictionary<string, uint>> StartAreaQuestMapping()
    {
        return new Dictionary<string, (uint Gridania, uint Limsa, uint Uldah)>
            {
                //Removed Quests
                { "Close to Home", (0, 0, 0) },

                // Gridania
                { "To the Bannock", (65564, 0, 0) },
                { "Passing Muster", (65737, 0, 0) },
                { "Chasing Shadows", (65981, 0, 0) },
                { "Eggs over Queasy", (69390, 0, 0) },
                { "Surveying the Damage", (65711, 0, 0) },
                { "A Soldier's Breakfast", (69391, 0, 0) },
                { "Spirithold Broken", (65665, 0, 0) },
                { "On to Bentbranch", (65712, 0, 0) },
                { "You Shall Not Trespass", (65912, 0, 0) },
                { "Don't Look Down", (65913, 0, 0) },
                { "In the Grim Darkness of the Forest", (65915, 0, 0) },
                { "Threat Level Elevated", (65916, 0, 0) },
                { "Migrant Marauders", (65917, 0, 0) },
                { "A Hearer Is Often Late", (65920, 0, 0) },
                { "Salvaging the Scene", (65923, 0, 0) },
                { "Leia's Legacy", (65697, 0, 0) },
                { "Dread Is in the Air", (65982, 0, 0) },
                { "To Guard a Guardian", (65983, 0, 0) },
                { "Festive Endeavors", (65984, 0, 0) },
                { "Renewing the Covenant", (65985, 0, 0) },
                { "The Gridanian Envoy", (66043, 0, 0) },

                // Limsa
                { "On to Summerford", (0, 65998, 0) },
                { "Dressed to Call", (0, 65999, 0) },
                { "Lurkers in the Grotto", (0, 66079, 0) },
                { "Washed Up", (0, 66001, 0) },
                { "Double Dealing", (0, 66002, 0) },
                { "Loam Maintenance", (0, 66003, 0) },
                { "Plowshares to Swords", (0, 66004, 0) },
                { "Just Deserts", (0, 66005, 0) },
                { "Sky-high", (0, 65933, 0) },
                { "Thanks a Million", (0, 65938, 0) },
                { "Relighting the Torch", (0, 65939, 0) },
                { "On to the Drydocks", (0, 65942, 0) },
                { "Without a Doubt", (0, 65948, 0) },
                { "Righting the Shipwright", (0, 65951, 0) },
                { "Do Angry Pirates Dream", (0, 65949, 0) },
                { "Victory in Peril", (0, 65950, 0) },
                { "Men of the Blue Tattoos", (0, 66225, 0) },
                { "Feint and Strike", (0, 66080, 0) },
                { "High Society", (0, 66226, 0) },
                { "A Mizzenmast Repast", (0, 66081, 0) },
                { "The Lominsan Envoy", (0, 66082, 0) },

                // Uldah
                { "We Must Rebuild", (0, 0, 66131) },
                { "Nothing to See Here", (0, 0, 66207) },
                { "Underneath the Sultantree", (0, 0, 66086) },
                { "Step Nine", (0, 0, 65839) },
                { "Prudence at This Junction", (0, 0, 69388) },
                { "Out of House and Home", (0, 0, 65843) },
                { "Way Down in the Hole", (0, 0, 65856) },
                { "Takin' What They're Givin'", (0, 0, 66159) },
                { "Supply and Demands", (0, 0, 65864) },
                { "Give It to Me Raw", (0, 0, 66039) },
                { "The Perfect Swarm", (0, 0, 65865) },
                { "Last Letter to Lost Hope", (0, 0, 65866) },
                { "Heir Today, Gone Tomorrow", (0, 0, 69389) },
                { "Passing the Blade", (0, 0, 65868) },
                { "Following Footfalls", (0, 0, 65869) },
                { "Storms on the Horizon", (0, 0, 65870) },
                { "Oh Captain, My Captain", (0, 0, 65872) },
                { "Secrets and Lies", (0, 0, 66164) },
                { "Duty, Honor, Country", (0, 0, 66087) },
                { "A Matter of Tradition", (0, 0, 66177) },
                { "A Royal Reception", (0, 0, 66088) },
                { "The Ul'dahn Envoy", (0, 0, 66064) },
                { "Call of the Sea", (66210, 66210, 66209) }
            }
            .ToDictionary(
                pair => pair.Key,
                pair => new Dictionary<string, uint>
                {
                    { "Gridania", pair.Value.Gridania },
                    { "Limsa Lominsa", pair.Value.Limsa },
                    { "Ul'dah", pair.Value.Uldah }
                });
    }
    
    public Dictionary<string, Dictionary<string, uint>> GrandCompanyQuestMapping()
    {
        return new Dictionary<string, (uint TwinAdder, uint Maelstorm, uint ImmortalFlames)>
            {
                { "The Company You Keep (Immortal Flames)", (0, 0, 66218) },
                { "For Coin and Country", (0, 0, 66221) },
                { "The Company You Keep (Maelstrom)", (0, 66217, 0) },
                { "Till Sea Swallows All", (0, 66220, 0) },
                { "The Company You Keep (Twin Adder)", (66216, 0, 0) },
                { "Wood's Will Be Done", (66219, 0, 0) }
            }
            .ToDictionary(
                pair => pair.Key,
                pair => new Dictionary<string, uint>
                {
                    { "Twin Adder", pair.Value.TwinAdder },
                    { "Maelstorm", pair.Value.Maelstorm },
                    { "Immortal Flames", pair.Value.ImmortalFlames }
                });
    }

    public Dictionary<string, string> GetMsqFileNames()
    {
        var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName ?? "";
        var msqDirectoryPath = Path.Combine(outputDirectory, "QuestJournal", "MSQ");

        if (!Directory.Exists(msqDirectoryPath))
        {
            log.Warning($"MSQ directory does not exist: {msqDirectoryPath}");
            return new Dictionary<string, string>();
        }

        log.Info($"Searching MSQ files in: {msqDirectoryPath}");

        return Directory.GetFiles(msqDirectoryPath, "*.json", SearchOption.TopDirectoryOnly)
                        .ToDictionary(
                            file => 
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                return fileName.StartsWith("MSQ-") 
                                           ? fileName.Substring(4).Replace("_", " ") 
                                           : fileName.Replace("_", " ");
                            },
                            file => Path.GetFileNameWithoutExtension(file)
                        );
    }
    
    public List<QuestModel>? FetchQuestData(string fileName)
    {
        try
        {
            var questAreaMapping = StartAreaQuestMapping();
            var grandCompanyMapping = GrandCompanyQuestMapping();
            var playerStartArea = configuration.StartArea;
            var playerGrandCompany = configuration.GrandCompany;
            var outputDirectory = pluginInterface.AssemblyLocation.Directory?.FullName ?? string.Empty;
            var msqDirectoryPath = Path.Combine(outputDirectory, "QuestJournal", "MSQ");
            var filePath = Path.Combine(msqDirectoryPath, fileName + ".json");

            if (!File.Exists(filePath))
            {
                log.Error($"File not found: {filePath}");
                return null;
            }

            var fileContent = File.ReadAllText(filePath);
            var quests = JsonSerializer.Deserialize<List<QuestModel>>(fileContent);

            if (quests == null)
            {
                log.Error($"Failed to deserialize quests from file {fileName}. The content might be invalid.");
                return null;
            }

            var filteredQuests = new List<QuestModel>();

            // Start Area Filtering
            if (!string.IsNullOrWhiteSpace(playerStartArea))
            {
                filteredQuests = quests
                                 .Where(q => q != null)
                                 .GroupBy(q => q.QuestTitle)
                                 .Select(group =>
                                 {
                                     if (group.Key != null &&
                                         questAreaMapping.TryGetValue(group.Key, out var areaMapping) &&
                                         areaMapping.TryGetValue(playerStartArea, out var mappedQuestId))
                                         return group.FirstOrDefault(q => q.QuestId == mappedQuestId);
                                     return group.FirstOrDefault();
                                 })
                                 .Where(q => q != null)
                                 .Select(q => q!)
                                 .ToList();
            }

            // Grand Company Filtering
            if (!string.IsNullOrWhiteSpace(playerGrandCompany))
            {
                filteredQuests = filteredQuests
                                 .Where(q => q != null)
                                 .GroupBy(q => q.QuestTitle)
                                 .Select(group =>
                                 {
                                     if (group.Key != null &&
                                         grandCompanyMapping.TryGetValue(group.Key, out var gcMapping) &&
                                         gcMapping.TryGetValue(playerGrandCompany, out var mappedQuestId))
                                         return group.FirstOrDefault(q => q.QuestId == mappedQuestId);
                                     return group.FirstOrDefault();
                                 })
                                 .Where(q => q != null)
                                 .Select(q => q!)
                                 .ToList();
            }

            var orderedQuests = filteredQuests.Where(q => q != null).OrderBy(q => q.SortKey).ToList();
            
            log.Info($"Filtered and organized a total of {orderedQuests.Count} quests for file: \"{filePath}\".");
            return orderedQuests;
        }
        catch (Exception ex)
        {
            log.Error($"Error loading file {fileName}: {ex.Message}");
            return null;
        }
    }
    
    public void Dispose() { }
}
