using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.GameObjects;

namespace MyEngine.Components;

public class Transform : Component
{
    private Vector2 _position;
    private float _rotation;
    private Vector2 _scale;
    private Transform _parent;
    private Matrix _matrix;
    
    public Vector2 Position { get => _position; set => _position = value; }
    public float Rotation { get => _rotation; set => _rotation = value; }
    public Vector2 Scale { get => _scale; set => _scale = value; }
    public Transform Parent { get => _parent; set => _parent = value; }
    public Matrix TransformMatrix { get => _matrix; set => _matrix = value; }

    public Transform()
    {
        Position = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
        Parent = null;
    }

    public override void Update(GameTime gameTime) {}

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {}
}