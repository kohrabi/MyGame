#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Graphics;


namespace MyEngine.Components;

public class Camera : Component
{
    private MyRenderTarget _myRenderTarget = new MyRenderTarget();

    public float Zoom
    {
        get => Transform.Scale.X;
        set => Transform.Scale = Vector2.One * MathHelper.Clamp(value, 0.1f, 10.0f);
    }
    
    public RenderTarget2D? RenderTarget2D
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
        return Matrix.Invert(Transform.WorldMatrix);
    }

    public void CenterPosition(Vector2 position)
    {
        if (RenderTarget2D != null)
            Transform.GlobalPosition = position - new Vector2(RenderTarget2D.Width / 2.0f, RenderTarget2D.Height / 2.0f);
        else
            Transform.GlobalPosition = position - new Vector2(Core.GraphicsDevice.Viewport.Width / 2.0f, Core.GraphicsDevice.Viewport.Height / 2.0f);
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
            transformMatrix: GetViewMatrix()
            );
    }

    public void End(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
    }

    public Vector2 ScreenToWorld(Point point)
    {
        if (RenderTarget2D == null)
           return Vector2.Transform(new Vector2(point.X, point.Y), Transform.WorldMatrix);
        else
        {
            Vector2 screen = new Vector2(point.X, point.Y);
            // screen = Vector2.Clamp(screen, Vector2.Zero, new Vector2(RenderTarget2D.Width, RenderTarget2D.Height));
            screen -= MyRenderTarget.Position;
            return Vector2.Transform(screen, Transform.WorldMatrix);
        }
    }
}