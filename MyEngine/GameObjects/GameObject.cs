﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;

namespace MyEngine.GameObjects;

#nullable enable

// Use Composition not Inheritance. Thank you!
public sealed class GameObject
{
    private bool _active = true;
    private bool _activeSelf = true;
    private List<Component> _components = new List<Component>();
    public string Name { get; set; }
    public Transform Transform { get; set; }

    // Set Active Recursively
    // This might be bad
    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Enabled = value;
            }

            for (int i = 0; i < Transform.ChildCount; i++)
            {
                Transform.GameObject.Active = value;
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
    
    public GameObject(string name = "GameObject")
    {
        Name = name;
        Transform = new Transform(this);
    }
    
    public T AddComponent<T>() where T : Component
    {
        T? returnComponent = (T?)_components.Find(c => c is T);
        if (returnComponent != null)
            return returnComponent;
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
    
    public void UpdateComponents(GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Update(gameTime);
            }
        }
    }
    
    public void DrawComponents(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Visible)
            {
                component.Draw(spriteBatch, gameTime);
            }
        }
    }
}