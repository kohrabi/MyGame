﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.GameObjects;
using MyEngine.Interfaces;

#nullable enable
namespace MyEngine.Components;

public abstract class Component : IUpdateable, IMyDrawable
{
    private int _updateOrder;
    private int _drawOrder;
    private bool _enabled = true;
    private bool _visible = true;
    private bool _active = true;
    private bool _activeSelf = true;
    
    public GameObject GameObject { get; internal set; }
    public Transform Transform { get; internal set; }

    public bool Active
    {
        get => _active;
        set => _active = value;
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
    
    public int DrawOrder
    {
        get => _drawOrder; 
        set { _drawOrder = value; DrawOrderChanged?.Invoke(this, EventArgs.Empty); }
    }

    public bool Visible
    {
        get => _visible; 
        set { _visible = value; VisibleChanged?.Invoke(this, EventArgs.Empty); }
    }

    public bool Enabled
    {
        get => _enabled; 
        set { _enabled = value; EnabledChanged?.Invoke(this, EventArgs.Empty); }
    }

    public int UpdateOrder
    {
        get => _updateOrder; 
        set { _updateOrder = value; UpdateOrderChanged?.Invoke(this, EventArgs.Empty); }
    }
    
    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;

    public event EventHandler<EventArgs> EnabledChanged;
    public event EventHandler<EventArgs> UpdateOrderChanged;
    
    // Callback order
    // Constructor -> Initialize -> Load Content
    // Unload content will be handled by content Manager / Scene
    public virtual void Initialize(ContentManager content)
    {
        LoadContent(content);
    }

    protected virtual void LoadContent(ContentManager content) {}
    
    public virtual void Update(GameTime gameTime)
    {
        
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
    }

}