using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyEngine.Components;

public class MouseFollow : Component
{
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Transform.GlobalPosition = GameObject.Scene.MainCamera.ScreenToWorld(Mouse.GetState().Position);
    }
}