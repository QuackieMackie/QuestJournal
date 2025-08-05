using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.UI;

public class QuestDetailWindow : Window, IDisposable
{
    private readonly QuestModel questModel;
    private List<QuestModel> questList;
    private readonly RendererUtils rendererUtils;
    private readonly Configuration configuration;
    
    public QuestDetailWindow(QuestModel questModel, List<QuestModel> questList, RendererUtils rendererUtils, Configuration configuration) 
        : base($"Quest Details###QuestDetailWindow-PopOut-{questModel.QuestId}", 
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.questModel = questModel;
        this.questList = questList;
        this.rendererUtils = rendererUtils;
        this.configuration = configuration;
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 300),
            MaximumSize = new Vector2(900, 300),
        };
    }

    public override void Draw()
    {
        rendererUtils.DrawSelectedQuestDetails(questModel, ref questList, configuration.CensorStarterLocations);
    }

    public void Dispose()
    {
    }
}
