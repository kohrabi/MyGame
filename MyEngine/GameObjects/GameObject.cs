using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Collision;
using MyEngine.Components;

namespace MyEngine.GameObjects;

#nullable enable

// Use Composition not Inheritance. Thank you!
public sealed class GameObject
{
    private bool _active = true;
    private bool _activeSelf = true;
    private List<Component> _unitializedComponents = new List<Component>();
    private List<Component> _components = new List<Component>();
    private Scene _scene;

    public bool IsDebugGameObject = false;
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
        Transform = new Transform();
        Transform.GameObject = this;
        Transform.Transform = Transform;
        _components.Add(Transform);
    }
    
    public T AddComponent<T>(params object[] varargs) where T : Component
    {
        T? returnComponent = (T?)_components.Find(c => c is T);
        if (returnComponent != null)
        {
            Console.WriteLine("Component type " + typeof(T).FullName + " already exists in GameObject " + Name);
            return returnComponent;
        }
        T component = Activator.CreateInstance(typeof(T), varargs) as T;
        component.GameObject = this;
        component.Transform = Transform;
        _unitializedComponents.Add(component);
        return component;
    }
    
    public T? GetComponent<T>() where T : Component
    {
        return (T?)_components.Find(c => c is T);
    }
    
    public bool HasComponent<T>() where T : Component
    {
        return _components.Find(c => c is T) != null;
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = (T)_components.Find(c => c is T);
        if (component == null)
            component = (T)_unitializedComponents.Find(c => c is T);
        return component != null;
    }

    public void Initialize(ContentManager content)
    {
        foreach (var component in _unitializedComponents)
        {
            component.Initialize(Scene.Content);
            _components.Add(component);
        }
        _unitializedComponents.Clear();
        
    }

    public void Destroy()
    {
        foreach (var component in _components)
        {
            component.OnDestroy();
        }
        _components.Clear();
        Scene.RemoveGameObject(this);
    }
    
    public void UpdateComponents(GameTime gameTime)
    {
        if (!Active) return;

        // In case it isn't initialized
        foreach (var component in _unitializedComponents)
        {
            component.Initialize(Scene.Content);
            _components.Add(component);
        }
        _unitializedComponents.Clear();
        
        foreach (var component in _components)
        {
            if (component.Active)
                component.Update(gameTime);
        }
    }

    public void OnCollision(CollisionResult collision)
    {
        foreach (var component in _components)
        {
            component.OnCollision(collision);
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