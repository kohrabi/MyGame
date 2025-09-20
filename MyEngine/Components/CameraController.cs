using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyEngine.Components;

public class CameraController : Component
{
    private const float SPEED = 500.0f;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Vector2 direction = Vector2.Zero;
        if (Keyboard.GetState().IsKeyDown(Keys.W))
            direction += Vector2.UnitY;
        if (Keyboard.GetState().IsKeyDown(Keys.S))
            direction -= Vector2.UnitY;    
        
        if (Keyboard.GetState().IsKeyDown(Keys.A))
            direction += Vector2.UnitX;
        if  (Keyboard.GetState().IsKeyDown(Keys.D))
            direction -= Vector2.UnitX;
        
        if (direction != Vector2.Zero)
            direction.Normalize();
        
        Transform.Position += direction * SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}