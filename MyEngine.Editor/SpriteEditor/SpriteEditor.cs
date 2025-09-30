#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
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
using MyEngine.Utils.Serialization;
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

    private ImGuiManager imGuiManager;
    private CameraController cameraController;

    private GameObject sprite = null;
    private AsepriteJson? asepriteJson = null;
    private int selectedAnimation = 0;
    private string[] animations = [""];
    private ImGuiSpriteAnimationViewer animationViewer;
    private List<ResizeableRectangle> framesRectangle = new List<ResizeableRectangle>();
    private nint texturePointer = IntPtr.Zero;
    string currentAnimation = "";
    private ImGuiViewportRender textureViewer;

    private float createModeDelayTimer = 0.0f;
    private bool isCreating = false;
    private ResizeableRectangle? currentCreateRectangle;
    private ImGuiSaveDialog imguiSaveDialog;
    private ImGuiFilePicker imguiFilePicker;
    private GridComponent gridComponent;
    private Num.Vector2 gridCount = Num.Vector2.One;
    private List<string> recentOpeneds = new List<string>();
    
    public SpriteEditor()
    {
        Core.Instance.Exiting += OnInstanceOnExiting;
        imGuiManager = (Core.RegisterOrGetManager(new ImGuiManager()) as ImGuiManager)!;
    }


    public override void Initialize()
    {
        // ImGui Setup
        MainCamera = Instantiate("MainCamera").AddComponent<Camera>();
        cameraController = MainCamera.GameObject.AddComponent<CameraController>();
        cameraController.UseDragControl = true;
        
        // ImGui Setup
        imGuiManager.ImGuiRenderer.AddFontFromFileTTF(
            Helpers.GetContentPath(Content, "Engine/ImGuiFonts/fontawesome-webfont.ttf"),
            13.0f,
            IconFonts.FontAwesome4.IconMin,
            IconFonts.FontAwesome4.IconMax);
        string path = Path.GetFullPath(Content.RootDirectory + "/ImGuiIni/SpriteEditor.ini");
        ImGui.LoadIniSettingsFromDisk(path);
        
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigDockingWithShift = true;
        ImGuiColor.CherryTheme();
        
        textureViewer = imGuiManager.AddComponent<ImGuiViewportRender>(MainCamera);
        textureViewer.ComponentExtension += OnGameViewComponentExtension;
        
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
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        
        Core.Instance.Exiting -= OnInstanceOnExiting;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        for (int i = 0; i < framesRectangle.Count; i++) 
        {
            var rect = framesRectangle[i];
            rect.IsChosen = false;
            if (animationViewer.CurrentFrame == i)
                rect.IsChosen = true;
        }
        
        var mouse = Mouse.GetState();
        int scrollValue = mouse.ScrollWheelValue;
        cameraController.Active = false;
        if (textureViewer.IsWindowFocused && asepriteJson != null)
        {
            cameraController.Active = true;
            cameraController.ClampBound = asepriteJson.Texture.Bounds;
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                MainCamera.CenterPosition(sprite.Transform.GlobalPosition);
            
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
            
        }
        
    }
    
    private void SetResizableRectanglesActive(bool active)
    {
        foreach (var frameRectangle in framesRectangle)
        {
            frameRectangle.IsResizeable = active;
        }
    }
    
    private void OnInstanceOnExiting(object? sender, ExitingEventArgs args)
    {
    }
    
    private void InitializeAnimations()
    {
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
                gridComponent = g;
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
        
        gridComponent.GridSize = Vector2.Max(gridComponent.GridSize, Num.Vector2.One);
        gridCount = (gridComponent.Size / gridComponent.GridSize).ToNumerics();
        
        animationViewer.SetAnimation(asepriteJson, asepriteJson.Animations.Keys.First());
    }
    
    private void LoadAsepriteJson(string path)
    {
        asepriteJson = AsepriteJson.FromFile(Content, path);
        InitializeAnimations();
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
                if (ImGui.MenuItem(FontAwesome4.FileO + " New", "Ctrl + N"))
                {
                    
                }
                ImGui.Separator();
                if (ImGui.MenuItem(FontAwesome4.FolderOpen + " Open", "Ctrl + O"))
                {
                    imguiFilePicker.ConfirmButtonName = "Open";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowUncreatedFile = false;
                    imguiFilePicker.AllowedExtensions = [".maf"];
                    imguiFilePicker.OnItemConfirmed = file =>
                    {
                        AsepriteJson temp = JsonSerializer.Deserialize<AsepriteJson>(File.ReadAllText(file), SerializeSettings.Default)!;
                        temp.LoadTexture();
                        asepriteJson = temp;
                        InitializeAnimations();
                    };
                    imguiFilePicker.OpenPopup("Open File");   
                }
                ImGui.Separator();
                
                if (ImGui.BeginMenu(FontAwesome4.FolderOpenO + " Recent Open"))
                {
                    if (recentOpeneds.Count > 0)
                    {
                        ImGui.MenuItem("None");
                    }
                    else
                    {
                        foreach (var recentOpen in recentOpeneds)
                        {
                            if (ImGui.MenuItem(Path.GetFileName(recentOpen)))
                            {
                                
                            }
                        }
                    }
                    ImGui.EndMenu();
                }

                
                ImGui.Separator();
                if (ImGui.MenuItem(FontAwesome4.FloppyO + " Save", "Ctrl + S"))
                {
                    imguiFilePicker.ConfirmButtonName = "Save";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowedExtensions = [".maf"];
                    imguiFilePicker.OnItemConfirmed = file =>
                    {
                        string jsonString = JsonSerializer.Serialize(asepriteJson, SerializeSettings.Default);
                        {
                            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(file));
                            File.WriteAllText(tempFile, jsonString);
                            File.Copy(tempFile, file, true);
                            File.Delete(tempFile);
                        }
                        // Console.WriteLine(jsonString);
                    };
                    imguiFilePicker.OpenPopup("Save File");   
                }
                ImGui.Separator();
                if (ImGui.BeginMenu(FontAwesome4.LevelDown + " Import"))
                {
                    if (ImGui.MenuItem("AsepriteJson"))
                    {
                        imguiFilePicker.ConfirmButtonName = "Import";
                        imguiFilePicker.OnlyAllowFolders = false;
                        imguiFilePicker.AllowedExtensions = [".json"];
                        imguiFilePicker.OnItemConfirmed = file =>
                        {
                            LoadAsepriteJson(file);
                        };
                        imguiFilePicker.OpenPopup("Import File");   
                    }
                    ImGui.EndMenu();
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

        if (ImGui.Begin("Settings"))
        {
            
            ImGui.NewLine();
            ImGui.Separator();
            
            ImGui.NewLine();
            ImGui.Text("Current File: " + asepriteJson?.FilePath); 
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            if (ImGui.ListBox("Animations", ref selectedAnimation, animations, animations.Length))
            {
                ChooseAnimation(animations[selectedAnimation]);                        
            }
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            ImGui.Text("Current Texture File: " + asepriteJson?.TextureFilePath);
            if (ImGui.Button("Change File##Texture"))
            {
                imguiFilePicker.ConfirmButtonName = "Open";
                imguiFilePicker.OnlyAllowFolders = false;
                imguiFilePicker.AllowedExtensions = [".xnb", ".png"];
                imguiFilePicker.OnItemConfirmed = file =>
                {
                    if (asepriteJson != null)
                    {
                        asepriteJson.TextureFilePath = file;
                        asepriteJson.LoadTexture();
                    }
                };
                imguiFilePicker.OpenPopup("Open File");
            }
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            Num.Vector2 frameSize = asepriteJson?.FrameSize.ToNumerics() ?? Num.Vector2.One;
            if (ImGui.DragFloat2("Frame Size", ref frameSize, 1.0f, 1.0f, 500f))
            {
                if (asepriteJson != null)
                {
                    gridComponent.GridSize = Vector2.Max(frameSize, Num.Vector2.One);
                    gridCount = (gridComponent.Size / gridComponent.GridSize).ToNumerics();
                    asepriteJson.FrameSize = gridComponent.GridSize;
                }
            }
            if (ImGui.DragFloat2("Grid Count", ref gridCount, 1.0f, 1.0f, 500f))
            {
                if (asepriteJson != null)
                {
                    gridComponent.GridSize = gridComponent.Size / gridCount;
                    asepriteJson.FrameSize = gridComponent.GridSize;
                }
            }
        }
        ImGui.End();
        
        ImGuiFramesWindow();

        if (!testPopup)
        {
            testPopup = true;
        }
        ImGui.OpenPopup("Hello");
        
        if (ImGui.Begin("Hello",
                ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.Popup))
        {
            ImGui.TextWrapped("Helloooooo");
        }
        ImGui.End();
    }

    private bool testPopup = false;

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