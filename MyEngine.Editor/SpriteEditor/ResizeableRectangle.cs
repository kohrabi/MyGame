using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = MyEngine.Utils.MyMath.RectangleF;

namespace MyEngine.Editor.SpriteEditor;

public class ResizeableRectangle : Component
{
    const float ScaleSize = 5.0f;
    public Vector2 Size
    {
        get => rectangle.Size;
        set => rectangle.Size = value;
    }
    public Vector2 Origin = Vector2.Zero / 2.0f;
    public float OutlineThickness = 1.0f;
    public int selectedScaler = -1;
    RectangleF rectangle = new RectangleF();
    RectangleF scalerTopLeft = new RectangleF();
    RectangleF scalerTopRight = new RectangleF();
    RectangleF scalerBottomLeft = new RectangleF();
    RectangleF scalerBottomRight = new RectangleF();
    
    private RectangleF orignalRectangle;
    private Vector2 prevMousePosition;

    public void SetRectangle()
    {
        rectangle.Location = Transform.GlobalPosition;
        rectangle.Offset(-Size * Origin);
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        scalerTopLeft.Size = Vector2.One * ScaleSize;
        scalerTopLeft.Location = Vector2.Round(new Vector2(rectangle.Left, rectangle.Top) - scalerTopLeft.Size / 2f);
        
        scalerTopRight.Size = Vector2.One * ScaleSize;
        scalerTopRight.Location = Vector2.Round(new Vector2(rectangle.Right, rectangle.Top) - scalerTopRight.Size / 2f);
        
        scalerBottomLeft.Size = Vector2.One * ScaleSize;
        scalerBottomLeft.Location = Vector2.Round(new Vector2(rectangle.Left, rectangle.Bottom) - scalerBottomLeft.Size / 2f);
        
        scalerBottomRight.Size = Vector2.One * ScaleSize;
        scalerBottomRight.Location = Vector2.Round(new Vector2(rectangle.Right, rectangle.Bottom) - scalerBottomRight.Size / 2f);

        var mouse = Mouse.GetState();
        Vector2 mouseWorldPosition = GameObject.Scene.MainCamera.ScreenToWorld(mouse.Position);
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            switch (selectedScaler)
            {
                case 1:
                    rectangle.Location = mouseWorldPosition;
                    rectangle.Size = new Vector2(orignalRectangle.Right - mouseWorldPosition.X, orignalRectangle.Bottom - mouseWorldPosition.Y);
                    break;
                
                case 2:
                    rectangle.Location = new Vector2(orignalRectangle.X, mouseWorldPosition.Y - orignalRectangle.Y);
                    rectangle.Size = new Vector2(mouseWorldPosition.X - rectangle.X, orignalRectangle.Bottom - rectangle.Y);
                    break;
                
                case 3:
                    rectangle.Location = new Vector2(mouseWorldPosition.X, orignalRectangle.Y);
                    rectangle.Size = new Vector2(orignalRectangle.Right - mouseWorldPosition.X, mouseWorldPosition.Y - orignalRectangle.Top);
                    break;
                
                case 4:
                    rectangle.Location = orignalRectangle.Location;
                    rectangle.Size = new Vector2(mouseWorldPosition.X - orignalRectangle.Left, mouseWorldPosition.Y - orignalRectangle.Top);
                    break;
            }
        }
        else
        {
            selectedScaler = -1;
            if (scalerTopLeft.Contains(mouseWorldPosition))
                selectedScaler = 1;
            else if (scalerTopRight.Contains(mouseWorldPosition))
                selectedScaler = 2;
            else if (scalerBottomLeft.Contains(mouseWorldPosition))
                selectedScaler = 3;
            else if (scalerBottomRight.Contains(mouseWorldPosition))
                selectedScaler = 4;
            prevMousePosition = mouseWorldPosition;
            orignalRectangle = rectangle;
            if (rectangle.Width < 0)
            {
                rectangle.X += rectangle.Width;
                rectangle.Width = Math.Abs(rectangle.Width);
            }

            if (rectangle.Height < 0)
            {
                rectangle.Y += rectangle.Height;
                rectangle.Height = Math.Abs(rectangle.Height);
            }
        }
        
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);


        Vector2 PositionTop = Vector2.Round(rectangle.Location + Size * (Vector2.One / 2.0f - Origin) - Vector2.UnitY * Size.Y / 2);
        Vector2 PositionBottom = Vector2.Round(rectangle.Location + Size * (Vector2.One / 2.0f - Origin) + Vector2.UnitY * Size.Y / 2);
        Vector2 PositionRight = Vector2.Round(rectangle.Location + Size * (Vector2.One / 2.0f - Origin) + Vector2.UnitX * Size.X / 2);
        Vector2 PositionLeft = Vector2.Round(rectangle.Location + Size * (Vector2.One / 2.0f - Origin) - Vector2.UnitX * Size.X / 2);
        
        // Draw Outlined Rectangle
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionTop,
            null,
            Color.White,
            0f,
            new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
            new Vector2(Size.X, OutlineThickness),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionBottom,
            null,
            Color.White,
            0f,
            new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
            new Vector2(Size.X, OutlineThickness),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionRight,
            null,
            Color.White,
            0f,
            new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
            new Vector2(OutlineThickness, Size.Y),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionLeft,
            null,
            Color.White,
            0f,
            new Vector2(DebugDraw.WhiteRectangle.Width, DebugDraw.WhiteRectangle.Height) / 2.0f,
            new Vector2(OutlineThickness, Size.Y),
            SpriteEffects.None,
            0.9f
        );
        
        // Draw TopLeft Scale
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            scalerTopLeft.Location,
            null,
            selectedScaler == 1 ? Color.DarkRed : Color.Red,
            0f,
            Vector2.Zero,
            scalerTopLeft.Size,
            SpriteEffects.None,
            1f
        );
        
        
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            scalerTopRight.Location,
            null,
            selectedScaler == 2 ? Color.DarkRed : Color.Red,
            0f,
            Vector2.Zero,
            scalerTopRight.Size,
            SpriteEffects.None,
            1f
        );
        
        
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            scalerBottomLeft.Location,
            null,
            selectedScaler == 3 ? Color.DarkRed : Color.Red,
            0f,
            Vector2.Zero, 
            scalerBottomLeft.Size,
            SpriteEffects.None,
            1f
        );
        
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            scalerBottomRight.Location,
            null,
            selectedScaler == 4 ? Color.DarkRed : Color.Red,
            0f,
            Vector2.Zero,
            scalerBottomRight.Size,
            SpriteEffects.None,
            1f
        );
    }
}