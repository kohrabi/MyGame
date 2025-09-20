using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils.DebugDrawPrimitives;

public interface IDebugDrawPrimitive
{
    void Draw(SpriteBatch spriteBatch);
}