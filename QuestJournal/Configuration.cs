using System;
using Dalamud.Configuration;

namespace QuestJournal;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public string StartArea { get; set; } = "Gridania";
    public string GrandCompany { get; set; } = "Immortal Flames";
    public string StarterClass { get; set; } = "Gladiator";
    
    public bool DeveloperMode { get; set; } = false;
    public int Version { get; set; } = 0;

    public void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }

    public void Reset()
    {
        StartArea = "Gridania";
        GrandCompany = "Immortal Flames";
        StarterClass = "Gladiator";
        DeveloperMode = false;
    }
}
