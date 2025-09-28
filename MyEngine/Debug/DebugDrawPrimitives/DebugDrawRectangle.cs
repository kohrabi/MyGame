using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils.DebugDrawPrimitives;

public class DebugDrawRectangle : IDebugDrawPrimitive
{
    private Vector2 _position;
    private Color _color;
    private Vector2 _size;
    private bool _isOutlined;
    private float _outlineThickness;
    
    public DebugDrawRectangle(Vector2 position, Color color, Vector2 size, bool isOutlined = false, float outlineThickness = 1.0f)
    {
        _position = position;
        _color = color;
        _size = size;
        _isOutlined = false;
        _outlineThickness = outlineThickness;
    }
    

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_isOutlined)
        {   
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                _position - Vector2.UnitY * _size.Y / 2,
                null,
                _color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(_size.X, _outlineThickness),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                _position + Vector2.UnitY * _size.Y / 2,
                null,
                _color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(_size.X, _outlineThickness),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                _position + Vector2.UnitX * _size.X / 2,
                null,
                _color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(_outlineThickness, _size.Y),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                _position - Vector2.UnitX * _size.X / 2,
                null,
                _color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(_outlineThickness, _size.Y),
                SpriteEffects.None,
                0f
            );
        }
        else
        {
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                _position,
                null,
                _color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                _size,
                SpriteEffects.None,
                0f
            );
        }
    }

}