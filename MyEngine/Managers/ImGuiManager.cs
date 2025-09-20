using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using MyEngine.Components;
using MyEngine.Debug.IMGUIComponents;

namespace MyEngine.Managers;

public class ImGuiManager
{
    private ImGuiRenderer  _imGuiRenderer;
    private List<ImGuiComponent> _components;
    private List<Action> _drawCommands;
    
    public ImGuiRenderer ImGuiRenderer => _imGuiRenderer;

    public ImGuiManager()
    {
        _components = new List<ImGuiComponent>();
        _drawCommands = new List<Action>();
        _imGuiRenderer = new ImGuiRenderer(Core.Instance);
        _imGuiRenderer.RebuildFontAtlas();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var component in _components)
        {
            component.Update(gameTime);
        }
    }
    
    public void Draw(GameTime gameTime)
    {
        _imGuiRenderer.BeforeLayout(gameTime);

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
    
    
    public void AddComponent<T>(params object[] varargs) where T : ImGuiComponent
    {
        var args = new object[] { _imGuiRenderer }
            .Concat(varargs)
            .ToArray();
        T component = Activator.CreateInstance(typeof(T), args) as T;
        _components.Add(component);
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