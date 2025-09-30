#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using MyEngine.Components;
using MyEngine.Debug.IMGUIComponents;

namespace MyEngine.Managers;

public class ImGuiManager : GlobalManager
{
    private ImGuiRenderer  _imGuiRenderer;
    private List<ImGuiComponent> _components;
    private List<Action> _drawCommands;
    private int _idCounter = 0;
    public bool EnableDocking = true;
    
    public ImGuiRenderer ImGuiRenderer => _imGuiRenderer;
    public Scene Scene => SceneManager.Instance.CurrentScene;
    
    public ImGuiManager()
    {
        _components = new List<ImGuiComponent>();
        _drawCommands = new List<Action>();
        _imGuiRenderer = new ImGuiRenderer(Core.Instance);
        _imGuiRenderer.RebuildFontAtlas();
        // This will create a new imgui renderer every time the scene change
        SceneManager.Instance.OnSceneChanged += scene1 =>
        {
            _components.Clear();
            _drawCommands.Clear();
            _imGuiRenderer = new ImGuiRenderer(Core.Instance);
            _imGuiRenderer.RebuildFontAtlas();
        };
    }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update(GameTime gameTime)
    {
        foreach (var component in _components)
        {
            component.Update(gameTime);
        }
    }
    
    public override void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeforeLayout(gameTime);
        
        if (EnableDocking)
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport().ID);

        foreach (var drawCommand in _drawCommands)
        {
            drawCommand();
        }
        
        foreach (var component in _components)
        {
            component.Draw();
        }
        
        _imGuiRenderer.AfterLayout();
    }
    
    
    public T AddComponent<T>(params object[] varargs) where T : ImGuiComponent
    {
        T? returnComponent = (T?)_components.Find(c => c is T);
        if (returnComponent != null)
        {
            Console.WriteLine("Component type " + typeof(T).FullName + " already exists in ImGuiManager. ");
            return returnComponent;
        }
        var args = new object[] { _imGuiRenderer, Scene, ++_idCounter }
            .Concat(varargs)
            .ToArray();
        T component = Activator.CreateInstance(typeof(T), args) as T;
        _components.Add(component);
        return component;
    }

    public void RemoveComponent<T>() where T : ImGuiComponent
    {
        var removeComponent = _components.FirstOrDefault(c => c.GetType() == typeof(T));
        if (removeComponent != null)
            _components.Remove(removeComponent);
        else
            Console.WriteLine("No component of type " + typeof(T).ToString());
    }

    public void AddDrawCommand(Action drawCommand)
    {
        _drawCommands.Add(drawCommand);
    }

    public void RemoveDrawCommand(Action drawCommand)
    {
        _drawCommands.Remove(drawCommand);
    }
}