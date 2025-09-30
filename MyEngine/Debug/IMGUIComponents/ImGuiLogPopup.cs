﻿using System;
using System.Collections.Generic;
using IconFonts;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using MyEngine.Managers;
using MyEngine.Utils.Tween;
using Color = System.Drawing.Color;
using Num = System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public enum ImGuiLogPopupType
{
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

    public ImGuiLogPopupItem(ImGuiLogPopupType type, string message)
    {
        Type = type;
        Message = message;
    }
}

public class ImGuiLogPopup : ImGuiComponent
{
    private List<ImGuiLogPopupItem> _items = new List<ImGuiLogPopupItem>();
    private Queue<int> _indexQueue = new Queue<int>();
    
    public ImGuiLogPopup(ImGuiManager manager, Scene scene, int id) : base(manager, scene, id)
    {
        for (int i = 0; i < 5; i++)
           _indexQueue.Enqueue(i);
    }

    public void AddItem(ImGuiLogPopupItem item)
    {
        item.Time = 5.0f;
        item.TimeLeft = item.Time; // Time until discard
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
        foreach (var item in _items)
        {
            Num.Vector2 windowSize = new Num.Vector2(350, 0); // width, height (height=0 for auto)
            Num.Vector2 displaySize = ImGui.GetIO().DisplaySize;
            Num.Vector2 pos = new Num.Vector2(displaySize.X - windowSize.X - 20, 10); // 20px from right, 60px from bottom
            if (nextYPosition > 0)
                pos.Y += nextYPosition + 0.0f;
            const float LeaveTime = 1.0f;
            if (item.TimeLeft <= LeaveTime)
                pos.X += MathHelper.Lerp(0f, windowSize.X + 20, Easings.EaseOutExpo(1.0f - item.TimeLeft / LeaveTime));
            
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
                    case ImGuiLogPopupType.Info: 
                        color = Color.AliceBlue;
                        symbol = FontAwesome4.Exclamation; 
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
                ImGui.TextColored(new Num.Vector4(color.R, color.G, color.B, color.A), symbol);
                ImGui.PopFont();
                
                ImGui.SameLine(0, 0);
                ImGui.SetCursorPosX(prevPos.X + 42.0f);
                ImGui.TextWrapped(item.Message);
                nextYPosition = pos.Y + ImGui.GetWindowSize().Y;
            }
            ImGui.End();
            // ImGui.PopStyleVar();
        }
    }
}