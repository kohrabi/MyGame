using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Components;

public class Camera : Component
{
    private RenderTarget2D? _renderTarget2D;

    public RenderTarget2D RenderTarget2D
    {
        get => _renderTarget2D;
        set => _renderTarget2D = value;
    }
    
    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        
    }
}