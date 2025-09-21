using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;

namespace MyEngine.Utils;

public class Gizmo : Component
{
    private Texture2D _gizmoTexture;
    

    protected override void LoadContent(ContentManager content)
    {
        base.LoadContent(content);
        GameObject.IsDebugGameObject = true;     
        _gizmoTexture = content.Load<Texture2D>("Engine/gizmo");
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);

        if (Transform.Parent.GameObject.HasComponent<Camera>())
            return;
        spriteBatch.Draw(
            _gizmoTexture, 
            Transform.GlobalPosition, 
            new Rectangle(0, 0, 256, 256), 
            Color.White, 
            Transform.GlobalRotation, 
            Vector2.One * 256f / 2f, 
            Transform.GlobalScale, 
            SpriteEffects.None, 
            1f);
    }
}