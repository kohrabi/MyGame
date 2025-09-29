using System;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiSaveDialog : ImGuiComponent
{
    public string FileName;
    public Action PopupSaveFile;
    public Action PopupQuitFile;
    
    public ImGuiSaveDialog(ImGuiRenderer renderer, Scene scene, int id) : base(renderer, scene, id) { }
    public override void Update(GameTime gameTime) { }

    public void OnPopup(string fileName, Action saveFile, Action quitFile)
    {
        FileName = fileName;
        PopupSaveFile = saveFile;
        PopupQuitFile = quitFile;
        if (ImGui.IsPopupOpen("Save File?"))
            Console.WriteLine("Current Save File popup is open, discarding previous actions");
    }
    
    public override void Draw()
    {
        
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSizeConstraints(new Num.Vector2(300f, 100f), new Num.Vector2(800f, 600f));
        if (ImGui.BeginPopupModal("Save File?", ImGuiWindowFlags.NoSavedSettings))
        {
            ImGui.TextWrapped("Current document " + FileName + " hasn't been saved are you sure you want to quit?");
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            
            // Center it
            float spacing = 15.0f;
            Num.Vector2 buttonSize = new Num.Vector2(50f, ImGui.GetFrameHeight());

            float xPosition = (ImGui.GetContentRegionAvail().X - buttonSize.X * 3 - spacing * 2) / 2;
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + xPosition);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y - buttonSize.Y);
            if (ImGui.Button("Yes", buttonSize))
            {
                PopupSaveFile?.Invoke();
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine(0, spacing);
            if (ImGui.Button("No", buttonSize))
            {
                PopupQuitFile?.Invoke();
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine(0, spacing);
            if (ImGui.Button("Cancel", buttonSize))
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();    
        }
    }
}