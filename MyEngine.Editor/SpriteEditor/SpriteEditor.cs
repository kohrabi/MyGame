using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.ContentProcessors.Aseprite;
using MyEngine.Debug.IMGUIComponents;
using MyEngine.GameObjects;
using MyEngine.Managers;
using MyEngine.Utils;
using MyEngine.Utils.MyMath;
using Num = System.Numerics;

namespace MyEngine.Editor.SpriteEditor;

public class SpriteEditor : Scene
{
    
    private Texture2D test;

    private RenderTarget2D renderTarget;
    private ImGuiManager imGuiManager;
    private int prevScrollValue = 0;
    private GameObject sprite = null;

    private Vector2 prevCameraPosition = Vector2.Zero;
    private bool dragging = false;
    private Point downMousePosition = Point.Zero;
    private AsepriteJson asepriteJson;
    private int selectedAnimation = 0;
    private string[] animations = [""];
    private ImGuiSpriteAnimationViewer animationViewer;
    private List<GameObject> framesRectangle = new List<GameObject>();
    private nint texturePointer = IntPtr.Zero;
    string currentAnimation = "";
    private int currentFrameSelected = 0;
    private ImGuiViewportRender gameView;
    
    public SpriteEditor()
    {
    }

    public override void Initialize()
    {
        // ImGui Setup
        MainCamera = Instantiate("MainCamera").AddComponent<Camera>();
        // MainCamera.GameObject.AddComponent<CameraController>();
        
        imGuiManager = new ImGuiManager(this);
        // string path = Path.GetFullPath(Content.RootDirectory + "/ImGuiIni/SpriteEditor.ini");
        // ImGui.LoadIniSettingsFromDisk(path);
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;
        gameView = imGuiManager.AddComponent<ImGuiViewportRender>(MainCamera);
        imGuiManager.AddDrawCommand(ImGuiLayout);
        // imGuiManager.AddComponent<ImGuiConsole>();
        BackgroundColor = new Color(15, 15, 15);
        ImGuiColor.CherryTheme();
        
        // Before Load Content
        base.Initialize();
        // After Load Content

        asepriteJson = AsepriteJson.FromFile(Content, "Sprites/ninja.json");
        animations = asepriteJson.Animations.ToList().Select((pair => pair.Key)).ToArray();

        PrefabBuilder.Instatiate()
            .AddComponent<Sprite>((sprite) =>
            {
                sprite.Texture = asepriteJson.Texture;
                sprite.Origin = Vector2.Zero;
            })
            .AddComponent<RectangleRenderer>((rect) =>
            {
                rect.Size = new Vector2(asepriteJson.Texture.Width, asepriteJson.Texture.Height);
                rect.Color = Color.Gray;
            })
            .GetGameObject(o =>
            {
                MainCamera.CenterPosition(o.Transform.Position);
                sprite = o;
            });
        
        texturePointer = imGuiManager.ImGuiRenderer.BindTexture(asepriteJson.Texture);

        var fontIcon =
            imGuiManager.ImGuiRenderer.AddFontFromFileTTF(
                Helpers.GetContentPath(Content, "ImGuiFonts/fontawesome-webfont.ttf"),
                13.0f,
                IconFonts.FontAwesome4.IconMin,
                IconFonts.FontAwesome4.IconMax);
        animationViewer = imGuiManager.AddComponent<ImGuiSpriteAnimationViewer>(fontIcon);
        animationViewer.SetAnimation(asepriteJson, "Idle");
    }

    protected override void LoadContent()
    {
        test = Content.Load<Texture2D>("Sprites/test");
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        imGuiManager.Update(gameTime);

        var mouse = Mouse.GetState();
        int scrollValue = mouse.ScrollWheelValue;
        if (gameView.IsWindowFocused)
        {
                
            if (Math.Abs(scrollValue - prevScrollValue) > 0)
            {
                MainCamera.Zoom += -(scrollValue - prevScrollValue) / 1000.0f * MainCamera.Zoom;
            }

            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (!dragging)
                {
                    downMousePosition = mouse.Position;
                    dragging = true;
                    prevCameraPosition = MainCamera.Transform.GlobalPosition;    
                }
                else
                {
                    MainCamera.Transform.GlobalPosition = prevCameraPosition - (mouse.Position - downMousePosition).ToVector2();
                }
            }
            else
            {
                dragging = false;
                downMousePosition = Point.Zero;
                prevCameraPosition = Vector2.Zero;
            }

            // Sprite Editor View
            RectangleF cameraBound = new RectangleF(
                MainCamera.Transform.GlobalPosition.X, 
                MainCamera.Transform.GlobalPosition.Y, 
                MainCamera.RenderTarget2D.Width, 
                MainCamera.RenderTarget2D.Height);
            cameraBound.Width = (int)(cameraBound.Width * MainCamera.Zoom);
            cameraBound.Height = (int)(cameraBound.Height * MainCamera.Zoom);
            Rectangle textureBound = asepriteJson.Texture.Bounds;
            textureBound.Inflate(textureBound.Width * 0.2f, textureBound.Height * 0.2f);
            
            if (textureBound.Left < textureBound.Right - cameraBound.Width)
                cameraBound.X = Math.Clamp(cameraBound.X, textureBound.Left, textureBound.Right - cameraBound.Width);
            else
                cameraBound.X = textureBound.Left - (cameraBound.Width - textureBound.Width) / 2.0f;
            
            if (textureBound.Top < textureBound.Bottom - cameraBound.Height)
                cameraBound.Y = Math.Clamp(cameraBound.Y, textureBound.Top, textureBound.Bottom - cameraBound.Height);
            else
                cameraBound.Y = textureBound.Top - (cameraBound.Height - textureBound.Height) / 2.0f;
            MainCamera.Transform.GlobalPosition = cameraBound.Location;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                MainCamera.CenterPosition(sprite.Transform.GlobalPosition);
        }
        prevScrollValue = scrollValue;
        
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        
        imGuiManager.Draw(gameTime);
    }

    private bool show_test_window = false;
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

        ImGui.ShowDemoWindow(ref show_test_window);
        if (ImGui.Begin("Settings"))
        {
            ImGui.NewLine();
            ImGui.Text("Current File: " + asepriteJson.FilePath); 
            ImGui.SameLine();
            ImGui.Button("Change File");
            ImGui.NewLine();

            ImGui.Separator();
            ImGui.NewLine();
            if (ImGui.ListBox("Animations", ref selectedAnimation, animations, animations.Length))
            {
                ChooseAnimation(animations[selectedAnimation]);                        
            }
        }
        ImGui.End();

        if (ImGui.Begin("Frames"))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 12.0f); // corner radius in px
            if (currentAnimation != "")
            {
                int i = 0;
                float spacing = ImGui.GetStyle().ItemSpacing.X;
                float avail = ImGui.GetContentRegionAvail().X;
                float x = 0;
                foreach (var animationFrame in asepriteJson.Animations[currentAnimation])
                {
                    ImGui.PushID("AnimationFrame " + i);
                    Num.Vector2 pos = animationFrame.Rectangle.Location.ToVector2().ToNumerics();
                    Num.Vector2 size = animationFrame.Rectangle.Size.ToVector2().ToNumerics();
                    Num.Vector2 dividor = new Num.Vector2(asepriteJson.Texture.Width, asepriteJson.Texture.Height);
                    Num.Vector2 selectableSize = size * 2.0f;

                    if (x + selectableSize.X > avail)
                    {
                        x = 0;
                        ImGui.NewLine();
                    }
                    
                    var prev = ImGui.GetCursorPos();
                    if (ImGui.Selectable("Frame " + i, currentFrameSelected == i, ImGuiSelectableFlags.None, selectableSize))
                    {
                        currentFrameSelected = i;
                    }
                    ImGui.SameLine();
                    ImGui.SetNextItemAllowOverlap(); // allow image inside selectable
                    ImGui.SetCursorPos(prev);
                    ImGui.Image(texturePointer, selectableSize, pos / dividor, (pos + size) / dividor);
                    
                    
                    // var drawList = ImGui.GetWindowDrawList();
                    // uint borderColor = ImGui.GetColorU32(ImGuiCol.Header);
                    // float thickness = 2.0f;
                    // drawList.AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), borderColor, 0.0f, ImDrawFlags.None, thickness);
                    
                    ImGui.PopID();
                    ImGui.SameLine(0, spacing);
                    x += selectableSize.X + spacing;
                    i++;
                }
            }
            ImGui.PopStyleVar();
        }
        ImGui.End();
        
    }

    private void ChooseAnimation(string name)
    {
        animationViewer.PlayAnimation(name);
        foreach (var frame in framesRectangle)
        {
            RemoveGameObject(frame);
        }
        framesRectangle.Clear();
        currentAnimation = name;
        var frames = asepriteJson.Animations[name];
        foreach (var frame in frames)
        {
            PrefabBuilder.Instatiate()
                .AddComponent<ResizeableRectangle>(c =>
                {
                    c.Size = frame.Rectangle.Size.ToVector2();
                    c.Transform.Position = frame.Rectangle.Location.ToVector2();
                    c.SetRectangle();
                })
                .GetGameObject(o => framesRectangle.Add(o));
        }
    }
}