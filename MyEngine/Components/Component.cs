using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Interfaces;

namespace MyEngine.Components;

public abstract class Component : IUpdateable, IMyDrawable
{
    private int _updateOrder;
    private int _drawOrder;
    private bool _enabled = true;
    private bool _visible = true;
    
    public GameObject GameObject { get; internal set; }
    public Transform transform { get; internal set; }

    public void Draw(GameTime gameTime) {}

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

    public void Initialize() {}
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

}