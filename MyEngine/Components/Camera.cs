using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Graphics;


namespace MyEngine.Components;

public class Camera : Component
{
    private MyRenderTarget _myRenderTarget = new MyRenderTarget();

    public RenderTarget2D RenderTarget2D
    {
        get => _myRenderTarget.RenderTarget;
    }

    public MyRenderTarget MyRenderTarget
    {
        get => _myRenderTarget;
        set => _myRenderTarget = value;
    }
    
    public void SetRenderTarget(int width, int height)
    {
        _myRenderTarget.RenderTarget = new RenderTarget2D(Core.GraphicsDevice, width, height);
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
        if (RenderTarget2D != null)
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

    public Vector2 ScreenToWorld(Point point)
    {
        if (RenderTarget2D == null)
           return Vector2.Transform(new Vector2(point.X, point.Y), Matrix.Invert(Transform.WorldMatrix));
        else
        {
            Vector2 screen = new Vector2(point.X, point.Y);
            // screen = Vector2.Clamp(screen, Vector2.Zero, new Vector2(RenderTarget2D.Width, RenderTarget2D.Height));
            screen -= MyRenderTarget.Position;
            return Vector2.Transform(screen, Matrix.Invert(Transform.WorldMatrix));
        }
    }
}