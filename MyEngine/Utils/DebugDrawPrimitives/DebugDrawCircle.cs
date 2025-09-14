using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils.DebugDrawPrimitives;

public class DebugDrawCircle : IDebugDrawPrimitive
{ 
    private Vector2 _position;
    private Color _color;
    private float _radius;
    private bool _isOutlined;
    
    public DebugDrawCircle(Vector2 position, Color color, float radius, bool isOutlined = false)
    {
        _position = position;
        _color = color;
        _radius = radius;
        _isOutlined = isOutlined;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            DebugDraw._whiteCircle,
            _position,
            null,
            _color,
            0f,
            Vector2.One / 0.5f,
            _radius,
            SpriteEffects.None,
            0f
        );
    }
}