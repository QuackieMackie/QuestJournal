using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using QuestJournal.Models;
using QuestJournal.Utils;

namespace QuestJournal.UI;

public class QuestDetailWindow : Window, IDisposable
{
    private readonly QuestModel questModel;
    private List<QuestModel> questList;
    private readonly RendererUtils rendererUtils;
    
    public QuestDetailWindow(QuestModel questModel, List<QuestModel> questList, RendererUtils rendererUtils) 
        : base($"Quest Details###QuestDetailWindow-PopOut-{questModel.QuestId}", 
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.questModel = questModel;
        this.questList = questList;
        this.rendererUtils = rendererUtils;
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(700, 300),
            MaximumSize = new Vector2(700, 300),
        };
    }

    public override void Draw()
    {
        rendererUtils.DrawSelectedQuestDetails(questModel, ref questList);
    }

    public void Dispose()
    {
    }
}
