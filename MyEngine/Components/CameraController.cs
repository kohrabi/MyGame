using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MyEngine.GameObjects;
using MyEngine.Utils.MyMath;

namespace MyEngine.Components;

public class CameraController : Component
{
    private const float SPEED = 500.0f;
    
    // Components
    private Camera camera;
    
    public bool UseDragControl = false;
    
    // Drag Control
    public Rectangle ClampBound;
    public float BoundInflateSize = 0.2f;
    
    private int prevScrollValue = 0;
    private Vector2 prevCameraPosition = Vector2.Zero;
    private bool dragging = false;
    private Point downMousePosition = Point.Zero;

    public override void Initialize(ContentManager content)
    {
        base.Initialize(content);
        
        camera = GameObject.GetComponent<Camera>();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var mouse = Mouse.GetState();
        int scrollValue = mouse.ScrollWheelValue;
        // Zoom in
        if (Math.Abs(scrollValue - prevScrollValue) > 0)
        {
            camera.Zoom += -(scrollValue - prevScrollValue) / 1000.0f * camera.Zoom;
        }
        
        if (UseDragControl)
        { 
            // Move Camera
            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (!dragging)
                {
                    downMousePosition = mouse.Position;
                    dragging = true;
                    prevCameraPosition = camera.Transform.GlobalPosition;
                }
                else
                {
                    camera.Transform.GlobalPosition = prevCameraPosition - (mouse.Position - downMousePosition).ToVector2();
                }
            }
            else
            {
                dragging = false;
                downMousePosition = Point.Zero;
                prevCameraPosition = Vector2.Zero;
            }

            RectangleF cameraBound = new RectangleF(
                camera.Transform.GlobalPosition.X, 
                camera.Transform.GlobalPosition.Y, 
                camera.RenderTarget2D?.Width ?? Core.GraphicsDevice.Viewport.Width, 
                camera.RenderTarget2D?.Height ?? Core.GraphicsDevice.Viewport.Height);
            cameraBound.Width = (int)(cameraBound.Width * camera.Zoom);
            cameraBound.Height = (int)(cameraBound.Height * camera.Zoom);
            Rectangle clampBound = ClampBound;
            clampBound.Inflate(clampBound.Width * BoundInflateSize, clampBound.Height * BoundInflateSize);
            
            if (clampBound.Left < clampBound.Right - cameraBound.Width)
                cameraBound.X = Math.Clamp(cameraBound.X, clampBound.Left, clampBound.Right - cameraBound.Width);
            else
                cameraBound.X = clampBound.Left - (cameraBound.Width - clampBound.Width) / 2.0f;
            
            if (clampBound.Top < clampBound.Bottom - cameraBound.Height)
                cameraBound.Y = Math.Clamp(cameraBound.Y, clampBound.Top, clampBound.Bottom - cameraBound.Height);
            else
                cameraBound.Y = clampBound.Top - (cameraBound.Height - clampBound.Height) / 2.0f;
            
            camera.Transform.GlobalPosition = cameraBound.Location;
        }
        else
        {
            Vector2 direction = Vector2.Zero;
            
            // What the fuck why is this shit flipped?
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                direction -= Vector2.UnitY;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                direction += Vector2.UnitY;    
            
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                direction -= Vector2.UnitX;
            if  (Keyboard.GetState().IsKeyDown(Keys.D))
                direction += Vector2.UnitX;
            
            if (direction != Vector2.Zero)
                direction.Normalize();
            
            Transform.Position += direction * SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        prevScrollValue = scrollValue;
    }
}