using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Components;

public class Camera : Component
{
    private RenderTarget2D? _renderTarget2D;

    public RenderTarget2D RenderTarget2D
    {
        get => _renderTarget2D;
        private set => _renderTarget2D = value;
    }
    
    public void SetRenderTarget(int width, int height)
    {
        RenderTarget2D = new RenderTarget2D(Core.GraphicsDevice, width, height);
    }
    
    // Can i get the view matrix from Transform.LocalMatrix?
    public Matrix GetViewMatrix()
    {
        return Matrix.CreateTranslation(new Vector3(-Transform.Position, 0)) *
               Matrix.CreateRotationZ(MathHelper.ToRadians(-Transform.Rotation)) *
               Matrix.CreateScale(new Vector3(Transform.Scale, 1));
    }

    public void Begin(SpriteBatch spriteBatch)
    {
        spriteBatch.GraphicsDevice.SetRenderTarget(RenderTarget2D);
        spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default,
            RasterizerState.CullNone,
            null,
            transformMatrix: Transform.WorldMatrix
            );
    }

    public void End(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
    }
        
    
    public override void Update(GameTime gameTime) { }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) {}
}