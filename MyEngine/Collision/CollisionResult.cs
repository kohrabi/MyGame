using Microsoft.Xna.Framework;
using MyEngine.Utils.MyMath;

namespace MyEngine.Collision;

public struct CollisionResult
{
    public readonly RectangleF a;
    public readonly Vector2 aVelocity;
    public readonly RectangleF b;
    public readonly Vector2 bVelocity;
    public readonly Vector2 normal;
    public readonly float time;
    
    public CollisionResult(RectangleF a, Vector2 aVelocity, RectangleF b, Vector2 bVelocity, Vector2 normal, float time)
    {
        this.a = a;
        this.aVelocity = aVelocity;
        this.b = b;
        this.bVelocity = bVelocity;
        this.normal = normal;
        this.time = time;
    }
}