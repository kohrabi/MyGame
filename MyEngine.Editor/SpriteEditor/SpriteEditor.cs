#nullable enable
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
    private AsepriteJson? asepriteJson = null;
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
    private ResizeableRectangle? currentCreateRectangle;
    private ImGuiSaveDialog imguiSaveDialog;
    private ImGuiFilePicker imguiFilePicker;
    
    public SpriteEditor()
    {
    }

    public override void Initialize()
    {
        // ImGui Setup
        MainCamera = Instantiate("MainCamera").AddComponent<Camera>();
        
        // ImGui Setup
        imGuiManager = new ImGuiManager(this);
        imGuiManager.ImGuiRenderer.AddFontFromFileTTF(
            Helpers.GetContentPath(Content, "Engine/ImGuiFonts/fontawesome-webfont.ttf"),
            13.0f,
            IconFonts.FontAwesome4.IconMin,
            IconFonts.FontAwesome4.IconMax);
        // string path = Path.GetFullPath(Content.RootDirectory + "/ImGuiIni/SpriteEditor.ini");
        // ImGui.LoadIniSettingsFromDisk(path);
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;
        ImGuiColor.CherryTheme();
        
        gameView = imGuiManager.AddComponent<ImGuiViewportRender>(MainCamera);
        gameView.ComponentExtension += OnGameViewComponentExtension;
        
        imguiFilePicker = imGuiManager.AddComponent<ImGuiFilePicker>();
        imguiFilePicker.OnItemConfirmed += file => Console.WriteLine(file);
        imGuiManager.AddDrawCommand(ImGuiLayout);
        imguiSaveDialog = imGuiManager.AddComponent<ImGuiSaveDialog>();
        // imGuiManager.AddComponent<ImGuiConsole>();
        animationViewer = imGuiManager.AddComponent<ImGuiSpriteAnimationViewer>();
        
        BackgroundColor = new Color(15, 15, 15);
        SetResizableRectanglesActive(true);
        // Before Load Content
        base.Initialize();
        // After Load Content
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
        if (gameView.IsWindowFocused && asepriteJson != null)
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

    private void LoadAsepriteJson(string path)
    {
        asepriteJson = AsepriteJson.FromFile(Content, path);
        animations = asepriteJson.Animations.ToList().Select((pair => pair.Key)).ToArray();
        
        RemoveGameObject(sprite);
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

        RemoveGameObject(currentCreateRectangle?.GameObject);
        PrefabBuilder.Instatiate()
            .AddComponent<ResizeableRectangle>(r =>
            {
                currentCreateRectangle = r;
                currentCreateRectangle.GameObject.Active = false;
            });

        imGuiManager.ImGuiRenderer.UnbindTexture(texturePointer);
        texturePointer = imGuiManager.ImGuiRenderer.BindTexture(asepriteJson.Texture);
        
        animationViewer.SetAnimation(asepriteJson, "Idle");
    }

    private void ChooseAnimation(string name)
    {
        if (asepriteJson == null)
            return;
        
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
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Num.Vector4(0.502f, 0.075f, 0.256f, 1.00f));
            ImGui.BeginChild("TextBg", new Num.Vector2(130, ImGui.GetFrameHeight()), ImGuiChildFlags.NavFlattened);
            ImGui.Text(" " + FontAwesome4.CameraRetro + " Sprite Editor");
            ImGui.EndChild();
            ImGui.PopStyleColor();
            
            ImGui.Separator();
            if (ImGui.BeginMenu(FontAwesome4.File + " File"))
            {
                if (ImGui.MenuItem(FontAwesome4.FileO + " New"))
                {
                    
                }
                ImGui.Separator();
                if (ImGui.MenuItem(FontAwesome4.EnvelopeOpen + " Open"))
                {
                    imguiFilePicker.ConfirmButtonName = "Open";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowedExtensions = [".json"];
                    imguiFilePicker.OnItemConfirmed = file =>
                    {
                        LoadAsepriteJson(file);
                    };
                    imguiFilePicker.OpenPopup("Open File");   
                }
                ImGui.Separator();
                
                if (ImGui.MenuItem(FontAwesome4.FloppyO + " Save"))
                {
                    imguiFilePicker.ConfirmButtonName = "Save";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowedExtensions = [".json"];
                    imguiFilePicker.OnItemConfirmed = file =>
                    {
                        Console.WriteLine(file);
                    };
                    imguiFilePicker.OpenPopup("Save File");   
                }
                ImGui.Separator();
                if (ImGui.MenuItem(FontAwesome4.Download + " Export"))
                {
                    imguiFilePicker.ConfirmButtonName = "Export";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowedExtensions = [".json"];
                    imguiFilePicker.OnItemConfirmed = file =>
                    {
                        Console.WriteLine(file);
                    };
                    imguiFilePicker.OpenPopup("Export as");
                }
                ImGui.Separator();
                
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
            ImGui.Text("Current File: " + asepriteJson?.FilePath); 
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
            if (currentAnimation != "" && asepriteJson != null)
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