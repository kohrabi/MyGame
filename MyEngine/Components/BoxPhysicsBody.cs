using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MyEngine.Collision;
using MyEngine.GameObjects;
using MyEngine.Utils.MyMath;

namespace MyEngine.Components;

public class BoxPhysicsBody : Component
{
    public Vector2 Velocity = Vector2.Zero;
    public bool IsTrigger = false;
    private RectangleF _rectangle;
    
    public Vector2 Size => _rectangle.Size;
    public RectangleF Rectangle => _rectangle;
    
    public BoxPhysicsBody(Vector2 size)
    {
        _rectangle = new RectangleF(Vector2.Zero, size);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _rectangle.Location = Transform.GlobalPosition;
    }

    public void HandleCollision()
    {
        var rectangle = CollisionHelper.GetSweptBroadPhaseBox(Rectangle, Velocity);
        foreach (GameObject gameObject in GameObject.Scene.GameObjects)
        {
            if (gameObject.TryGetComponent(out BoxPhysicsBody boxBody))
            {
                if (rectangle.Intersects(boxBody.Rectangle))
                {
                    CollisionResult result = CollisionHelper.SweptAABB(Rectangle, Velocity, boxBody.Rectangle, boxBody.Velocity);
                    if (result.time >= 0.0f && result.time <= 1.0f)
                    {
                        GameObject.OnCollision(result);
                    }
                }
            }
        }
    }
}