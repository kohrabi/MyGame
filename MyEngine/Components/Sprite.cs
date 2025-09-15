using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Components;

public class Sprite : Component
{
    private Texture2D _texture = null;
    private Rectangle? _sourceRectangle = null;
    private Vector2 _origin = Vector2.One / 2.0f;
    
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
    
    
    public override void Update(GameTime gameTime) { }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!Visible || Texture == null)
            return;
        spriteBatch.Draw(
            Texture, 
            Transform.GlobalPosition, 
            SourceRectangle, 
            Color.White, 
            Transform.GlobalRotation,
            new Vector2(Texture.Width, Texture.Height) * Origin, 
            Transform.GlobalScale,
            SpriteEffects.None, 
            0.0f);
    }
}