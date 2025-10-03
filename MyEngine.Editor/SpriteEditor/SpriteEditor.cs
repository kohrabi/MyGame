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
using MyEngine.IMGUI.Components;
using MyEngine.GameObjects;
using MyEngine.Graphics;
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

    #region GameView Properties

    private CameraController cameraController;
    
    private GameObject sprite = null;
    private GridComponent gridComponent;
    private List<ResizeableRectangle> framesRectangle = new List<ResizeableRectangle>();

    #endregion
    
    #region ImGui Properties
    
    private ImGuiManager imGuiManager;
    private ImGuiSpriteAnimationViewer animationViewer;
    private nint texturePointer = IntPtr.Zero;
    private ImGuiViewportRender textureViewer;
    
    private ResizeableRectangle? currentCreateRectangle;
    private ImGuiSaveDialog imguiSaveDialog;
    private ImGuiFilePicker imguiFilePicker;
    private ImGuiLogPopup logPopup;
    private ImFontPtr bigIconFont;
    
    private List<string> recentOpeneds = new List<string>();
    #endregion
    
    private SpriteAnimation? spriteAnimation = null;

    private int selectedAnimation = 0;
    private string[] animations = [""];
    
    string currentAnimation = "";

    private float createModeDelayTimer = 0.0f;
    private bool isCreating = false;
    
    private Num.Vector2 gridCount = Num.Vector2.One;
    
    public SpriteEditor()
    {
        imGuiManager = (Core.RegisterOrGetManager(new ImGuiManager()) as ImGuiManager)!;
        BackgroundColor = new Color(15, 15, 15);
    }


    public override void Initialize()
    {
        // ImGui Setup
        MainCamera = Instantiate("MainCamera").AddComponent<Camera>();
        cameraController = MainCamera.GameObject.AddComponent<CameraController>();
        cameraController.UseDragControl = true;
        
        // ImGui Setup
        string path = Path.GetFullPath(Content.RootDirectory + "/ImGuiIni/SpriteEditor.ini");
        ImGui.LoadIniSettingsFromDisk(path);
        
        textureViewer = imGuiManager.AddComponent<ImGuiViewportRender>(MainCamera);
        textureViewer.ComponentExtension += OnGameViewComponentExtension;
        
        imguiFilePicker = imGuiManager.AddComponent<ImGuiFilePicker>();
        imGuiManager.AddDrawCommand(ImGuiLayout);
        imguiSaveDialog = imGuiManager.AddComponent<ImGuiSaveDialog>();
        // imGuiManager.AddComponent<ImGuiConsole>();
        animationViewer = imGuiManager.AddComponent<ImGuiSpriteAnimationViewer>();
        logPopup = imGuiManager.AddComponent<ImGuiLogPopup>();
        
        SetResizableRectanglesActive(true);
        // Before Load Content
        base.Initialize();
        // After Load Content
    } 
    
    protected override void LoadContent()
    {
    }

    private float saved = 0.0f;
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
        
        HotKeyHandles(gameTime);
        
        cameraController.Active = false;
        if (textureViewer.IsWindowFocused && spriteAnimation != null)
        {
            cameraController.Active = true;
            cameraController.ClampBound = spriteAnimation.Texture.Bounds;
            if (InputManager.IsKeyDown(Keys.Space))
                MainCamera.CenterPosition(sprite.Transform.GlobalPosition);
            
            if (createModeDelayTimer > 0)
                createModeDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                if (mode == EditorMode.Create && currentAnimation != "")
                {
                    if (InputManager.IsMouseDown(0) && !isCreating)
                    {
                        isCreating = true;
                        currentCreateRectangle.Initialize(MainCamera.ScreenToWorld(InputManager.GetMousePosition()), Vector2.Zero);
                        currentCreateRectangle.SetResizeIndex(4);
                        currentCreateRectangle.GameObject.Active = true;
                    }
                    else if (InputManager.IsMouseUp(0) && isCreating)
                    {
                        isCreating = false;
                        currentCreateRectangle.GameObject.Active = false;
                        AnimationFrame frame = new AnimationFrame();
                        frame.Duration = 100.0f;
                        frame.Rectangle = currentCreateRectangle.Rectangle.ToRectangle();
                        spriteAnimation.Animations[currentAnimation].Add(frame);
                        int index = spriteAnimation.Animations[currentAnimation].Count - 1;
                        PrefabBuilder.Instatiate()
                            .AddComponent<ResizeableRectangle>(c =>
                            {
                                c.Initialize(frame.Rectangle.Location.ToVector2(), frame.Rectangle.Size.ToVector2());
                                c.OnResized += f =>
                                {
                                    spriteAnimation.Animations[currentAnimation][index].Rectangle = f;
                                };
                                framesRectangle.Add(c);
                            });
                    }
                }
            }
            
        }
        
    }

    private void HotKeyHandles(GameTime gameTime)
    {
        if (saved > 0.0f)
            saved -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        else
        {
            if (spriteAnimation != null && InputManager.IsCombinationPressed(Keys.LeftControl, Keys.S))
            {
                saved = 0.5f;
                SaveFile(spriteAnimation.FilePath);
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
    
    private void InitializeAnimations()
    {
        animations = spriteAnimation.Animations.ToList().Select((pair => pair.Key)).ToArray();
        
        RemoveGameObject(sprite);
        PrefabBuilder.Instatiate()
            .AddComponent<Sprite>((sprite) =>
            {
                sprite.Texture = spriteAnimation.Texture;
                sprite.Origin = Vector2.Zero;
            })
            .AddComponent<GridComponent>((g) =>
            {
                gridComponent = g;
                g.Initialize(spriteAnimation.Texture.Bounds.Size.ToVector2(), spriteAnimation.FrameSize, Color.Gray);
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
        texturePointer = imGuiManager.ImGuiRenderer.BindTexture(spriteAnimation.Texture);
        
        gridComponent.GridSize = Vector2.Max(gridComponent.GridSize, Num.Vector2.One);
        gridCount = (gridComponent.Size / gridComponent.GridSize).ToNumerics();
        
        animationViewer.SetAnimation(spriteAnimation, spriteAnimation.Animations.Keys.First());
    }
    
    private void LoadAsepriteJson(string file)
    {
        spriteAnimation = AsepriteJson.FromFile(Content, file);
        InitializeAnimations();
        
        logPopup.Show(new ImGuiLogPopupItem(ImGuiLogPopupType.None, $"Imported {file} loaded successfully"));
    }

    private void LoadMafFile(string file)
    {
        SpriteAnimation temp = JsonSerializer.Deserialize<SpriteAnimation>(File.ReadAllText(file), SerializeSettings.Default)!;
        temp.LoadTexture();
        spriteAnimation = temp;
        InitializeAnimations();
        logPopup.Show(new ImGuiLogPopupItem(ImGuiLogPopupType.None, $"Opened {file} loaded successfully"));
    }
    
    private void SaveFile(string file)
    {
        if (Path.GetExtension(file) != ".maf")
            file = Path.GetFileNameWithoutExtension(file) + ".maf";
        string jsonString = JsonSerializer.Serialize(spriteAnimation, SerializeSettings.Default);
        string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(file));
        File.WriteAllText(tempFile, jsonString);
        File.Copy(tempFile, file, true);
        File.Delete(tempFile);
        
        logPopup.Show(new ImGuiLogPopupItem(ImGuiLogPopupType.Info, $"Saved {file} successfully"));
    }

    private void ChooseAnimation(string name)
    {
        if (spriteAnimation == null)
            return;
        
        animationViewer.PlayAnimation(name);
        foreach (var frame in framesRectangle)
        {
            RemoveGameObject(frame.GameObject);
        }
        framesRectangle.Clear();
        currentAnimation = name;
        var frames = spriteAnimation.Animations[name];
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
                        spriteAnimation.Animations[name][i1].Rectangle = f;
                    };
                    framesRectangle.Add(c);
                });
        }
        if (mode == EditorMode.Create)
            SetResizableRectanglesActive(false);
    }

    private bool test = false;
    private void ImGuiLayout()
    {
        ImGui.ShowDemoWindow(ref test);
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
                    imguiFilePicker.OnItemConfirmed = LoadMafFile;
                    imguiFilePicker.OpenPopup("Open File");   
                }
                ImGui.Separator();
                
                if (ImGui.BeginMenu(FontAwesome4.FolderOpenO + " Recent Open"))
                {
                    if (recentOpeneds.Count > 0)
                    {
                        ImGui.MenuItem("None");
                    }
                    
                    ImGui.EndMenu();
                }

                
                ImGui.Separator();
                if (ImGui.MenuItem(FontAwesome4.FloppyO + " Save", "Ctrl + S"))
                {
                    imguiFilePicker.ConfirmButtonName = "Save";
                    imguiFilePicker.OnlyAllowFolders = false;
                    imguiFilePicker.AllowedExtensions = [".maf"];
                    imguiFilePicker.OnItemConfirmed = SaveFile;
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
                        imguiFilePicker.OnItemConfirmed = LoadAsepriteJson;
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
            ImGui.Text("Current File: " + spriteAnimation?.FilePath); 
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
            ImGui.Text("Current Texture File: " + spriteAnimation?.TextureFilePath);
            if (ImGui.Button("Change File##Texture"))
            {
                imguiFilePicker.ConfirmButtonName = "Open";
                imguiFilePicker.OnlyAllowFolders = false;
                imguiFilePicker.AllowedExtensions = [".xnb", ".png"];
                imguiFilePicker.OnItemConfirmed = file =>
                {
                    if (spriteAnimation != null)
                    {
                        spriteAnimation.TextureFilePath = file;
                        spriteAnimation.LoadTexture();
                    }
                };
                imguiFilePicker.OpenPopup("Open File");
            }
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            Num.Vector2 frameSize = spriteAnimation?.FrameSize.ToNumerics() ?? Num.Vector2.One;
            if (ImGui.DragFloat2("Frame Size", ref frameSize, 1.0f, 1.0f, 500f))
            {
                if (spriteAnimation != null)
                {
                    gridComponent.GridSize = Vector2.Max(frameSize, Num.Vector2.One);
                    gridCount = (gridComponent.Size / gridComponent.GridSize).ToNumerics();
                    spriteAnimation.FrameSize = gridComponent.GridSize;
                }
            }
            if (ImGui.DragFloat2("Grid Count", ref gridCount, 1.0f, 1.0f, 500f))
            {
                if (spriteAnimation != null)
                {
                    gridComponent.GridSize = gridComponent.Size / gridCount;
                    spriteAnimation.FrameSize = gridComponent.GridSize;
                }
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
            if (currentAnimation != "" && spriteAnimation != null)
            {
                Num.Vector2 SelectableSize = new Num.Vector2(81, 42) * 2.0f;
                
                int i = 0;
                float spacing = ImGui.GetStyle().ItemSpacing.X;
                float avail = ImGui.GetContentRegionAvail().X;
                float x = 0;
                foreach (var animationFrame in spriteAnimation.Animations[currentAnimation])
                {
                    ImGui.PushID("AnimationFrame " + i);
                    Num.Vector2 pos = animationFrame.Rectangle.Location.ToVector2().ToNumerics();
                    Num.Vector2 size = animationFrame.Rectangle.Size.ToVector2().ToNumerics();
                    Num.Vector2 dividor = new Num.Vector2(spriteAnimation.Texture.Width, spriteAnimation.Texture.Height);
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
                foreach (var animationFrame in spriteAnimation.Animations[currentAnimation])
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
                foreach (var animationFrame in spriteAnimation.Animations[currentAnimation])
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