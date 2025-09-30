﻿using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using MyEngine.Managers;

namespace MyEngine.IMGUI.Components;

public abstract class ImGuiObject
{
    protected readonly int _id = 0;
    protected Scene _scene;
    protected ImGuiManager _imGuiManager;
    public int ID => _id;
    protected Scene Scene => _scene;
    protected ImGuiManager ImGuiManager => _imGuiManager;
    protected ImGuiRenderer ImGuiRenderer => ImGuiManager.ImGuiRenderer;
    public ImGuiObject(ImGuiManager imGuiManager, Scene scene, int id)
    {
        _imGuiManager = imGuiManager;
        _scene = scene;
        _id = id;
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}