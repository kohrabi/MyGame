using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Debug.IMGUIComponents;
using MyEngine.Managers;
using MyEngine.Utils;
using MyEngine.Utils.Tween;
using Num =  System.Numerics;

namespace MyEngine.Editor;

public class EditorScene1 : Scene
{
    private GameObject a;
    private GameObject b;
    private Texture2D test;

    private RenderTarget2D renderTarget;
    private bool show_test_window = false;
    private ImGuiManager imGuiManager;
    
    public EditorScene1()
    {
    }

    public override void Initialize()
    {
        // ImGui Setup
        MainCamera = Instantiate("MainCamera").AddComponent<Camera>();
        MainCamera.GameObject.AddComponent<CameraController>();
        
        imGuiManager = new ImGuiManager(this);
        string path = Path.GetFullPath(Content.RootDirectory + "/ImGuiIni/EditorScene1.ini");
        ImGui.LoadIniSettingsFromDisk(path);
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;
        imGuiManager.AddComponent<ImGuiViewportRender>();
        imGuiManager.AddDrawCommand(ImGuiLayout);
        var gameObjectWindow = imGuiManager.AddComponent<ImGuiGameObjectsWindow>();
        imGuiManager.AddComponent<ImGuiConsole>();
        
        // Before Load Content
        base.Initialize();
        // After Load Content
        
        // MainCamera.Transform.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        
        // ImGui.LoadIniSettingsFromDisk("imgui.ini");
        
        // a = Instantiate();
        // a.Transform.Position = new Vector2(0.0f, 0.0f);
        // a.Transform.Scale = Vector2.One;
        // a.AddComponent<Sprite>().Texture = test;
        
        b = Instantiate();
        b.Transform.Position = new Vector2(-100.0f, 300.0f);
        // a.Transform.AddChild(b.Transform);
        b.AddComponent<Sprite>().Texture = test;
        
        var gizmoObject = Instantiate();
        gizmoObject.AddComponent<Gizmo>();
        gizmoObject.Active = false;
        gameObjectWindow.OnSelectedGameObjectChanged += (selectedGameObject) =>
        {
            gizmoObject.Active = selectedGameObject != null;
            gizmoObject.Transform.Parent = selectedGameObject?.Transform;
        };

        b.Transform.TweenPosition(Vector2.One * 700f, 1.0f)
            .SetEasingType(EasingType.Linear)
            .SetLoopMode(LoopMode.Restart);
        b.Transform.TweenRotation(MathHelper.ToRadians(360f), 1.0f)
            .SetEasingType(EasingType.Linear)
            .SetLoopMode(LoopMode.Restart);
    }

    protected override void LoadContent()
    {
        test = Content.Load<Texture2D>("Sprites/test");
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // a.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));
        // b.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));
        
        imGuiManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        imGuiManager.Draw(gameTime);
    }

    private string _inputBuffer = "";
    private List<string> _log = new List<string>();
    private void ImGuiLayout()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Hello"))
            {
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
        
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport().ID);
        ImGui.ShowDemoWindow(ref show_test_window);

    }
}