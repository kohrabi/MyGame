using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;

namespace MyEngine.GameObjects;

#nullable enable

// Use Composition not Inheritance. Thank you!
public sealed class GameObject
{
    private bool _active = true;
    private bool _activeSelf = true;
    private bool _isInitialized = false;
    private List<Component> _components = new List<Component>();
    private Scene _scene;

    public bool IsDebugGameObject = false;
    
    public bool IsInitialized => _isInitialized;
    public string Name { get; set; }
    public Transform Transform { get; set; }
    public Scene Scene => _scene;
    public List<Component> Components => _components; 
    
    // Set Active Recursively
    // This might be bad
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            foreach (Component component in _components) {
                component.Active = value;
            }

            foreach (Transform child in Transform.Children)
            {
                child.GameObject.Active = value;
            }
        }
    }
    
    public bool ActiveSelf
    {
        get => _activeSelf;
        set
        {
            _activeSelf = value;
            Active = value;
        }
    }
    
    public GameObject(Scene scene, string name = "GameObject")
    {
        _scene = scene;
        Name = name;
        Transform = new Transform(this);
    }
    
    public T AddComponent<T>() where T : Component
    {
        T? returnComponent = (T?)_components.Find(c => c is T);
        if (returnComponent != null)
        {
            Console.WriteLine("Component type " + typeof(T).FullName + " already exists in GameObject " + Name);
            return returnComponent;
        }
        T component = Activator.CreateInstance<T>();
        component.GameObject = this;
        component.Transform = Transform;
        _components.Add(component);
        return component;
    }
    
    public T? GetComponent<T>() where T : Component
    {
        return _components.Find(c => c is T) as T;
    }

    public void Initialize(ContentManager content)
    {
        _isInitialized = true;
        foreach (var component in _components)
        {
            component.Initialize(content);
        }
    }
    
    public void UpdateComponents(GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Active)
                component.Update(gameTime);
        }
    }
    
    public void DrawComponents(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Active)
                component.Draw(spriteBatch, gameTime);
        }
    }
}