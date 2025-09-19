using System;
using System.Collections.Generic;
using System.Globalization;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Utils;
using Num =  System.Numerics;

namespace MyEngine.Editor;

public class EditorScene1 : Scene
{
    private GameObject a;
    private GameObject b;
    private Texture2D test;

    private ImGuiRenderer imGuiRenderer;
    private RenderTarget2D renderTarget;
    
    readonly Vector2 DEFAULT_RENDER_TARGET_SIZE = new Vector2(1280, 720);
    readonly Vector2 PADDING = new Vector2(-20, -30);
    private nint renderTargetPointer;
    private Texture2D renderTargetTexture;
    Color[] renderTargetData;
    private bool resizeWindow = false;
    private Vector2 windowSize;
    
    public EditorScene1()
    {
    }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the 
    /// initial screens to the ScreenManager.
    /// </summary>
    public override void Initialize()
    {
        
        // ImGui Setup
        imGuiRenderer = new ImGuiRenderer(Core.Instance);
        imGuiRenderer.RebuildFontAtlas();
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;
        
        GameObject gameObject = new GameObject("Hello");
        MainCamera = gameObject.AddComponent<Camera>();
        // MainCamera.Transform.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
  
        a = new GameObject();
        a.Transform.Position = new Vector2(0.0f, 0.0f);
        a.Transform.Scale = Vector2.One;
        Sprite asprite = a.AddComponent<Sprite>();
        asprite.Texture = test;
        
        b = new GameObject();
        b.Transform.Position = new Vector2(-100.0f, 300.0f);
        a.Transform.AddChild(b.Transform);
        var bsprite = b.AddComponent<Sprite>();
        bsprite.Texture = test;
        
        // ImGui.LoadIniSettingsFromDisk("imgui.ini");
        CreateRenderTarget(DEFAULT_RENDER_TARGET_SIZE);
    }

    public override void LoadContent()
    {
        test = Content.Load<Texture2D>("Sprites/test");
    }

    public override void UnloadContent()
    {
    }

    private bool show_test_window = false;
    public override void Update(GameTime gameTime)
    {
        a.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));
        b.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));

        if (resizeWindow)
        {
            resizeWindow = false;
            CreateRenderTarget(windowSize);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            MainCamera.Begin(SpriteBatch);

            // SpriteBatch.Begin();
            a.DrawComponents(SpriteBatch, gameTime);
            b.DrawComponents(SpriteBatch, gameTime);
            // SpriteBatch.End();
            
            MainCamera.End(SpriteBatch);
        }
        GraphicsDevice.SetRenderTarget(null);

        
        
        imGuiRenderer.BeforeLayout(gameTime);
        
        ImGuiLayout();
        
        // Call AfterLayout now to finish up and draw all the things
        imGuiRenderer.AfterLayout();
        
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

        ImGui.Begin("GameWindow");
        
        // Get Render data
        renderTarget.GetData(renderTargetData);
        renderTargetTexture.SetData(renderTargetData);
        
        // Set center
        Num.Vector2 size = new Num.Vector2(renderTarget.Width, renderTarget.Height);
        Num.Vector2 avail = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(ImGui.GetCursorPos() + (avail - size) / 2.0f);
        
        // Render
        ImGui.Image(renderTargetPointer, size);
        
        // If it is resized, queue and recreate 
        windowSize = avail;
        if (!windowSize.Equals(new Num.Vector2(renderTarget.Width, renderTarget.Height)))
            resizeWindow = true;
        
        ImGui.End();
        
    }

    private void CreateRenderTarget(Vector2 size)
    {
        if (renderTarget != null)
            renderTarget.Dispose();
        renderTarget = new RenderTarget2D(GraphicsDevice, (int)size.X, (int)size.Y);
        
        renderTargetData = new Color[renderTarget.Width * renderTarget.Height];
        if (renderTargetTexture != null)
            renderTargetTexture.Dispose();
        renderTargetTexture = new Texture2D(GraphicsDevice, renderTarget.Width, renderTarget.Height);
        
        if (renderTargetPointer != 0)
            imGuiRenderer.UnbindTexture(renderTargetPointer);
        renderTargetPointer = imGuiRenderer.BindTexture(renderTargetTexture);
    }
}