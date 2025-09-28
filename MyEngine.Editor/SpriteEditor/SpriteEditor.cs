using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using IconFonts;
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
    private const float CreateModeDelayTime = 0.2f;
    
    enum EditorMode
    {
        Edit, Create
    }
    
    EditorMode mode = EditorMode.Edit;
    
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
    private List<ResizeableRectangle> framesRectangle = new List<ResizeableRectangle>();
    private nint texturePointer = IntPtr.Zero;
    string currentAnimation = "";
    private int currentFrameSelected = 0;
    private ImGuiViewportRender gameView;

    private float createModeDelayTimer = 0.0f;
    private bool isCreating = false;
    private ResizeableRectangle currentCreateRectangle;
    
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
        SetResizableRectanglesActive(true);

        gameView.ComponentExtension += OnGameViewComponentExtension;
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
            .AddComponent<GridComponent>((g) =>
            {
                g.Initialize(asepriteJson.Texture.Bounds.Size.ToVector2(), asepriteJson.FrameSize, Color.Gray);
            })
            .GetGameObject(o =>
            {
                MainCamera.CenterPosition(o.Transform.Position);
                sprite = o;
            });

        PrefabBuilder.Instatiate()
            .AddComponent<ResizeableRectangle>(r =>
            {
                currentCreateRectangle = r;
                currentCreateRectangle.GameObject.Active = false;
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

    private void SetResizableRectanglesActive(bool active)
    {
        foreach (var frameRectangle in framesRectangle)
        {
            frameRectangle.IsResizeable = active;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        imGuiManager.Update(gameTime);

        for (int i = 0; i < framesRectangle.Count; i++) 
        {
            var rect = framesRectangle[i];
            rect.IsChosen = false;
            if (animationViewer.CurrentFrame == i)
                rect.IsChosen = true;
        }
        
        var mouse = Mouse.GetState();
        int scrollValue = mouse.ScrollWheelValue;
        if (gameView.IsWindowFocused)
        {
            // Zoom in
            if (Math.Abs(scrollValue - prevScrollValue) > 0)
            {
                MainCamera.Zoom += -(scrollValue - prevScrollValue) / 1000.0f * MainCamera.Zoom;
            }

            // Move Camera
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
                    MainCamera.Transform.GlobalPosition =
                        prevCameraPosition - (mouse.Position - downMousePosition).ToVector2();
                }
            }
            else
            {
                dragging = false;
                downMousePosition = Point.Zero;
                prevCameraPosition = Vector2.Zero;
            }

            if (createModeDelayTimer > 0)
                createModeDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                if (mode == EditorMode.Create && currentAnimation != "")
                {
                    if (mouse.LeftButton == ButtonState.Pressed && !isCreating)
                    {
                        isCreating = true;
                        currentCreateRectangle.Initialize(MainCamera.ScreenToWorld(mouse.Position), Vector2.Zero);
                        currentCreateRectangle.SetResizeIndex(4);
                        currentCreateRectangle.GameObject.Active = true;
                    }
                    else if (mouse.LeftButton == ButtonState.Released && isCreating)
                    {
                        isCreating = false;
                        currentCreateRectangle.GameObject.Active = false;
                        AnimationFrame frame = new AnimationFrame();
                        frame.Duration = 100.0f;
                        frame.FrameNumber = asepriteJson.Animations[currentAnimation].Last().FrameNumber + 1;
                        frame.Rectangle = currentCreateRectangle.Rectangle.ToRectangle();
                        asepriteJson.Animations[currentAnimation].Add(frame);
                        int index = asepriteJson.Animations[currentAnimation].Count - 1;
                        PrefabBuilder.Instatiate()
                            .AddComponent<ResizeableRectangle>(c =>
                            {
                                c.Initialize(frame.Rectangle.Location.ToVector2(), frame.Rectangle.Size.ToVector2());
                                c.OnResized += f =>
                                {
                                    asepriteJson.Animations[currentAnimation][index].Rectangle = f;
                                };
                                framesRectangle.Add(c);
                            });
                    }
                }
            }

            // Clamp View
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
    
    

    private void ChooseAnimation(string name)
    {
        animationViewer.PlayAnimation(name);
        foreach (var frame in framesRectangle)
        {
            RemoveGameObject(frame.GameObject);
        }
        framesRectangle.Clear();
        currentAnimation = name;
        var frames = asepriteJson.Animations[name];
        for (int i = 0; i < frames.Count; i++)
        {
            var frame = frames[i];
            PrefabBuilder.Instatiate()
                .AddComponent<ResizeableRectangle>(c =>
                {
                    c.Initialize(frame.Rectangle.Location.ToVector2(), frame.Rectangle.Size.ToVector2());
                    int i1 = i;
                    c.OnResized += f =>
                    {
                        asepriteJson.Animations[name][i1].Rectangle = f;
                    };
                    framesRectangle.Add(c);
                });
        }
        if (mode == EditorMode.Create)
            SetResizableRectanglesActive(false);
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
            ImGui.Separator();
            
            ImGui.NewLine();
            ImGui.Text("Current File: " + asepriteJson.FilePath); 
            // ImGui.SameLine();
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

        ImGuiFramesWindow();
    }

    private void ImGuiFramesWindow()
    {
        
        if (ImGui.Begin("Frames", ImGuiWindowFlags.HorizontalScrollbar))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 12.0f); // corner radius in px
            if (currentAnimation != "")
            {
                Num.Vector2 SelectableSize = new Num.Vector2(81, 42) * 2.0f;
                
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
                    Num.Vector2 selectableSize = SelectableSize;
                    
                    Num.Vector2 prevCursorPos = ImGui.GetCursorPos();
                    if (ImGui.Selectable("Frame " + i, animationViewer.CurrentFrame == i, ImGuiSelectableFlags.None, selectableSize))
                    {
                        currentFrameSelected = i;
                        animationViewer.SetCurrentFrame(i);
                    }
                    ImGui.SameLine();
                    ImGui.SetNextItemAllowOverlap(); // allow image inside selectable
                    ImGui.SetCursorPos(prevCursorPos);
                    ImGui.Image(texturePointer, selectableSize, pos / dividor, (pos + size) / dividor);
                    
                    ImGui.PopID();
                    ImGui.SameLine();
                    x += selectableSize.X + spacing;
                    i++;
                }
                
                ImGui.NewLine();
                x = 0;
                foreach (var animationFrame in asepriteJson.Animations[currentAnimation])
                {
                    ImGui.PushID("AnimationFrame " + i);
                    Num.Vector2 size = animationFrame.Rectangle.Size.ToVector2().ToNumerics();
                    Num.Vector2 selectableSize = SelectableSize;
                    
                    Num.Vector2 cursorPos = new Num.Vector2(x, ImGui.GetCursorPos().Y) + new Num.Vector2(selectableSize.X / 2.0f - 130.0f / 2f, 0.0f);
                    ImGui.SetCursorPos(cursorPos);
                    ImGui.SetNextItemWidth(40.0f);
                    ImGui.Text("Duration");
                    ImGui.SameLine();
                    ImGui.SetNextItemWidth(80.0f);
                    int duration = (int)animationFrame.Duration;
                    if (ImGui.InputInt("##" + animationFrame.GetHashCode(), ref duration))
                        animationFrame.Duration = duration;
                    
                    ImGui.PopID();
                    ImGui.SameLine();
                    x += selectableSize.X + spacing;
                    i++;
                }
                
                ImGui.NewLine();
                x = 0;
                foreach (var animationFrame in asepriteJson.Animations[currentAnimation])
                {
                    ImGui.PushID("AnimationFrame " + i);
                    Num.Vector2 size = animationFrame.Rectangle.Size.ToVector2().ToNumerics();
                    Num.Vector2 selectableSize = SelectableSize;
                    
                    Num.Vector2 cursorPos = new Num.Vector2(x, ImGui.GetCursorPos().Y) + new Num.Vector2(selectableSize.X / 2.0f - 80f / 2f, 0.0f);
                    ImGui.SetCursorPos(cursorPos);
                    ImGui.SetNextItemWidth(40.0f);
                    ImGui.Text("Delete");
                    ImGui.SameLine();
                    int duration = (int)animationFrame.Duration;
                    if (ImGui.Button(FontAwesome4.Trash + "##" + animationFrame.GetHashCode(), Num.Vector2.One * 20.0f))
                        Console.WriteLine("Delete");
                    
                    ImGui.PopID();
                    ImGui.SameLine();
                    x += selectableSize.X + spacing;
                    i++;
                }
            }
            ImGui.PopStyleVar();
        }
        ImGui.End();
    }
    
    private void OnGameViewComponentExtension()
    {
        ImGui.SetNextItemAllowOverlap();
        Num.Vector2 buttonSize = new Num.Vector2(25.0f);
        Num.Vector2 cursorPos = ImGui.GetWindowSize() / 2.0f;
        cursorPos.X -= buttonSize.X * 1;
        cursorPos.Y = buttonSize.Y * 0.5f + 15.0f;
        ImGui.SetCursorPos(cursorPos);

        EditorMode currentMode = mode;
        Num.Vector4 activedColor;
        unsafe
        {
            activedColor = *ImGui.GetStyleColorVec4(ImGuiCol.ButtonActive);
        }

        if (currentMode == EditorMode.Edit) ImGui.PushStyleColor(ImGuiCol.Button, activedColor);
        if (ImGui.Button(FontAwesome4.MousePointer, buttonSize))
        {
            mode = EditorMode.Edit;
            SetResizableRectanglesActive(true);
        }
        if (currentMode == EditorMode.Edit) ImGui.PopStyleColor();
        if (ImGui.IsItemHovered())
            createModeDelayTimer = CreateModeDelayTime;
        
        ImGui.SameLine();

        if (currentMode == EditorMode.Create) ImGui.PushStyleColor(ImGuiCol.Button, activedColor);
        if (ImGui.Button(FontAwesome4.PlusSquare, buttonSize))
        {
            mode = EditorMode.Create;
            createModeDelayTimer = CreateModeDelayTime;
            SetResizableRectanglesActive(false);
        }
        if (currentMode == EditorMode.Create) ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
            createModeDelayTimer = CreateModeDelayTime;

    }
}