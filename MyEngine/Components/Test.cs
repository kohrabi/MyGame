using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MyEngine.Collision;

namespace MyEngine.Components;

public class Test : Component
{
    
    private Sprite _sprite;
    private BoxPhysicsBody _boxPhysicsBody;
    
    public override void Initialize(ContentManager content)
    {
        base.Initialize(content);
        
        GameObject.TryGetComponent(out _sprite);
        GameObject.TryGetComponent(out _boxPhysicsBody);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _sprite.Color = Color.White;
        _boxPhysicsBody?.HandleCollision();
    }

    public override void OnCollision(CollisionResult collision)
    {
        base.OnCollision(collision);
        Console.WriteLine("Collided");
        if (_sprite != null)
        {
            _sprite.Color = Color.Green;
        }
    }
}