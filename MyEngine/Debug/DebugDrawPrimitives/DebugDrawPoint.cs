using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils.DebugDrawPrimitives;

public class DebugDrawPoint : IDebugDrawPrimitive
{ 
    private Vector2 _position;
    private Color _color;
    private float _size;
    
    public DebugDrawPoint(Vector2 position, Color color, float size)
    {
        _position = position;
        _color = color;
        _size = size;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            DebugDraw._whiteRectangle,
            _position,
            null,
            _color,
            0f,
            new Vector2(DebugDraw._whiteRectangle.Width, DebugDraw._whiteRectangle.Height) / 2.0f,
            _size,
            SpriteEffects.None,
            0f
        );
    }
}