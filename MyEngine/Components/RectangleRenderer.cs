using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Utils;
using MyEngine.Utils.MyMath;

namespace MyEngine.Components;

public class RectangleRenderer : Component
{
    public Vector2 Size;
    public Color Color;
    public bool IsOutlined;
    public Vector2 Origin;
    public float OutlineThickness;

    protected override void LoadContent(ContentManager content)
    {
        base.LoadContent(content);

        if (DebugDraw.WhiteRectangle == null)
        {
            DebugDraw.WhiteRectangle = new Texture2D(Core.GraphicsDevice, 1, 1);
            DebugDraw.WhiteRectangle.SetData([Color.White]);
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);
        
        if (IsOutlined)
        {   
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                Transform.GlobalPosition + Size * Origin - Vector2.UnitY * Size.Y / 2,
                null,
                Color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(Size.X, OutlineThickness),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                Transform.GlobalPosition + Size * Origin + Vector2.UnitY * Size.Y / 2,
                null,
                Color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(Size.X, OutlineThickness),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                Transform.GlobalPosition + Size * Origin + Vector2.UnitX * Size.X / 2,
                null,
                Color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(OutlineThickness, Size.Y),
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                Transform.GlobalPosition + Size * Origin - Vector2.UnitX * Size.X / 2,
                null,
                Color,
                0f,
                new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
                new Vector2(OutlineThickness, Size.Y),
                SpriteEffects.None,
                0f
            );
        }
        else
        {
            spriteBatch.Draw(
                DebugDraw.WhiteRectangle,
                Transform.GlobalPosition + Size * Origin,
                null,
                Color,
                0f,
                Origin,
                Size,
                SpriteEffects.None,
                0f
            );
        }
    }
}