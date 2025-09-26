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
    
    private Vector2 _origin = Vector2.One / 2.0f;
    private Vector2 _rectSize = Vector2.One;
    private RectangleF _rectangle = RectangleF.Empty;

    // Range from 0-1
    public Vector2 Origin
    {
        get => _origin;
        set => _origin = value;
    }

    public Vector2 RectSize
    {
        get => _rectSize;
        set => _rectSize = value;
    }
    
    public RectangleF Rectangle => _rectangle;

    public Vector2 GlobalSize => RectSize * Transform.GlobalScale;
    public Vector2 Offset => GlobalSize * -Origin;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _rectangle.Location = Transform.GlobalPosition + Offset;
        _rectangle.Size = GlobalSize;
    }

    public void HandleCollision()
    {
        var rectangle = CollisionHelper.GetSweptBroadPhaseBox(Rectangle, Velocity);
        foreach (GameObject gameObject in GameObject.Scene.GameObjects)
        {
            if (!gameObject.Active || gameObject.IsDebugGameObject)
                continue;
            if (gameObject.TryGetComponent(out BoxPhysicsBody boxBody))
            {
                if (rectangle.Intersects(boxBody.Rectangle))
                {
                    if (boxBody.Rectangle.Intersects(Rectangle))
                    {
                        GameObject.OnCollision(new CollisionResult(Rectangle, Velocity, boxBody.Rectangle, boxBody.Velocity, Vector2.Zero, 0.0f));
                        continue;
                    }
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