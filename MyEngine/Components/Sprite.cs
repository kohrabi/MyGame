using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Utils;
using MyEngine.Utils.Attributes;

namespace MyEngine.Components;

public class Sprite : Component
{
    protected Texture2D _texture = null;
    protected Rectangle? _sourceRectangle = null;
    protected Vector2 _origin = Vector2.One / 2.0f;

    [SerializeField] 
    public double blablabla = 1;
    public Texture2D Texture
    {
        get => _texture;
        set => _texture = value;
    }

    // Set to null to use the full texture
    public Rectangle? SourceRectangle
    {
        get => _sourceRectangle;
        set => _sourceRectangle = value;
    }
    
    // Normalized Origin (0 to 1)
    public Vector2 Origin
    {
        get => _origin;
        set => _origin = value;
    }
    
    public Color Color { get; set; } = Color.White;


    public override void Initialize(ContentManager content)
    {
        base.Initialize(content);
    }

    public override void Update(GameTime gameTime) { }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (Texture == null)
            return;
        spriteBatch.Draw(
            Texture, 
            Transform.GlobalPosition, 
            SourceRectangle, 
            Color, 
            Transform.GlobalRotation,
            (SourceRectangle.HasValue ? SourceRectangle.Value.Size.ToVector2() : new Vector2(Texture.Width, Texture.Height)) * Origin, 
            Transform.GlobalScale,
            SpriteEffects.None, 
            0.5f);
    }
}