using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Collision;
using MyEngine.GameObjects;
using MyEngine.Interfaces;
using MyEngine.Utils;
using MyEngine.Utils.Attributes;

#nullable enable
namespace MyEngine.Components;

public abstract class Component : IMyUpdateable, IMyDrawable
{
    protected int _updateOrder;
    protected int _drawOrder;
    protected bool _active = true;
    protected bool _activeSelf = true;
    
    public GameObject GameObject { get; internal set; }
    public Transform Transform { get; internal set; }

    [SerializeField]
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
    
    // [SerializeField]
    public int DrawOrder
    {
        get => _drawOrder; 
        set { _drawOrder = value; DrawOrderChanged?.Invoke(this, EventArgs.Empty); }
    }

    // [SerializeField]
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
    
    public virtual void OnDestroy() {}
    
    public virtual void OnCollision(CollisionResult collision) {}
    
    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

}