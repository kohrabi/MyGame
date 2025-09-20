using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils.DebugDrawPrimitives;

public class DebugDrawString : IDebugDrawPrimitive
{
    private string _text;
    private Vector2 _position;
    private Color _color;
    private Vector2 _size;
    
    public DebugDrawString(string text, Vector2 position, Color color, Vector2 size)
    {
        _text = text;
        _position = position;
        _color = color;
        _size = size;
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 textSize = DebugDraw._font.MeasureString(_text);
        spriteBatch.DrawString(
            DebugDraw._font,
            _text,
            _position,
            _color,
            0f,
            textSize / 2.0f,
            _size,
            SpriteEffects.None,
            0f
        );
    }
}