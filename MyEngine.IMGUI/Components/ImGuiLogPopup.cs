using System;
using System.Collections.Generic;
using IconFonts;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MyEngine.Managers;
using MyEngine.Utils.Tween;
using Num = System.Numerics;

namespace MyEngine.IMGUI.Components;

public enum ImGuiLogPopupType
{
    None,
    Info,
    Warning,
    Error,
}

public class ImGuiLogPopupItem
{
    public ImGuiLogPopupType Type = ImGuiLogPopupType.Info;
    public string Message = String.Empty;
    public float TimeLeft = 0.0f;
    public float Time = 0.0f;
    public int CurrentIndex = 0;
    public Num.Vector2 PrevPosition = Num.Vector2.Zero;

    public ImGuiLogPopupItem(ImGuiLogPopupType type, string message)
    {
        Type = type;
        Message = message;
    }
}

public class ImGuiLogPopup : ImGuiObject
{
    private List<ImGuiLogPopupItem> _items = new List<ImGuiLogPopupItem>();
    private Queue<int> _indexQueue = new Queue<int>();
    private int indexSize = 5;
    
    public ImGuiLogPopup(ImGuiManager manager, Scene scene, int id) : base(manager, scene, id)
    {
        for (int i = 0; i < indexSize; i++)
           _indexQueue.Enqueue(i);
    }

    public void Show(ImGuiLogPopupItem item)
    {
        item.Time = 5.0f;
        item.TimeLeft = item.Time; // Time until discard
        if (_indexQueue.Count <= 0)
        {
            for (int i = indexSize; i < indexSize + 5; i++)
                _indexQueue.Enqueue(i);
            indexSize += 5;
        }
        Console.WriteLine(indexSize);
        item.CurrentIndex = _indexQueue.Dequeue();
        _items.Add(item);
    }
    
    public override void Update(GameTime gameTime)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].TimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_items[i].TimeLeft <= 0)
                _indexQueue.Enqueue(_items[i].CurrentIndex);
        }
        _items.RemoveAll((item) => item.TimeLeft <= 0);
    }

    public override void Draw()
    {

        float nextYPosition = 0;
        int i = 0;
        foreach (var item in _items)
        {
            Num.Vector2 windowSize = new Num.Vector2(350, 0); // width, height (height=0 for auto)
            Num.Vector2 displaySize = ImGui.GetIO().DisplaySize;
            Num.Vector2 pos = new Num.Vector2(displaySize.X - windowSize.X - 20, 10); // 20px from right, 60px from bottom
            if (nextYPosition > 0)
                pos.Y += nextYPosition;
            
            if (item.PrevPosition == Num.Vector2.Zero)
                _items[i].PrevPosition = pos;
            
            const float LeaveTime = 0.5f;
            if (item.TimeLeft <= LeaveTime)
                pos.X += MathHelper.Lerp(0f, windowSize.X + 20, Easings.EaseOutExpo(1.0f - item.TimeLeft / LeaveTime));
            else
                pos = Num.Vector2.Lerp(item.PrevPosition, pos, 0.3f);
            
            _items[i].PrevPosition = pos;
            ImGui.SetNextWindowPos(pos, ImGuiCond.Always);
            ImGui.SetNextWindowSize(windowSize);
            // ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 2.0f * item.TimeLeft / Math.Max(item.Time, 1f));
            if (ImGui.Begin("Error##" + item.CurrentIndex,
                    ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing))
            {
                Color color = Color.Yellow;
                string symbol = FontAwesome4.ExclamationCircle;

                
                switch (item.Type)
                {
                    case ImGuiLogPopupType.None:
                        color = Color.White;
                        symbol = FontAwesome4.Exclamation;
                        break;
                    case ImGuiLogPopupType.Info: 
                        color = Color.SkyBlue;
                        symbol = FontAwesome4.Info; 
                        break;
                    case ImGuiLogPopupType.Warning: 
                        color = Color.Yellow;
                        symbol = FontAwesome4.ExclamationCircle;
                        break;
                    case ImGuiLogPopupType.Error: 
                        color = Color.PaleVioletRed;
                        symbol = FontAwesome4.Xing;
                        break;
                }
                
                Num.Vector2 prevPos = ImGui.GetCursorPos();
                ImGui.PushFont(ImGuiManager.BigIconFont);
                Num.Vector2 textSize = ImGui.CalcTextSize(symbol);
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (32.0f - textSize.X) / 2.0f);
                ImGui.TextColored(color.ToVector4().ToNumerics(), symbol);
                ImGui.PopFont();
                
                ImGui.SameLine(0, 0);
                ImGui.SetCursorPosX(prevPos.X + 42.0f);
                ImGui.TextWrapped(item.Message);
                nextYPosition = pos.Y + ImGui.GetWindowSize().Y;
            }
            ImGui.End();
            i++;
            // ImGui.PopStyleVar();
        }
    }
}