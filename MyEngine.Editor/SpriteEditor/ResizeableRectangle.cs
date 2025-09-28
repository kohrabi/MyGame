using System;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.Utils;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
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

    public RectangleF Rectangle => rectangle;
    public Vector2 Origin = Vector2.Zero / 2.0f;
    public float OutlineThickness = 1.0f;
    public int selectedScaler = -1;
    public Action<Rectangle> OnResized;
    public bool IsChosen = false;
    RectangleF rectangle = new RectangleF();
    RectangleF scalerTopLeft = new RectangleF();
    RectangleF scalerTopRight = new RectangleF();
    RectangleF scalerBottomLeft = new RectangleF();
    RectangleF scalerBottomRight = new RectangleF();
    private Vector2 offset = Vector2.Zero;
    public bool IsResizeable { get; set; } = true;
    
    private RectangleF orignalRectangle;
    private Vector2 prevMousePosition;

    public void Initialize(Vector2 position, Vector2 size)
    {
        Transform.GlobalPosition = position;
        Console.WriteLine(Transform.GlobalPosition);
        Size = size;
        rectangle.Location = Transform.GlobalPosition;
        rectangle.Offset(-Size * Origin);
        orignalRectangle = rectangle;
    }
    
    public void SetResizeIndex(int index) => selectedScaler = index;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!IsResizeable)
            return;
        
        var mouse = Mouse.GetState();
        Vector2 mouseWorldPosition = GameObject.Scene.MainCamera.ScreenToWorld(mouse.Position);
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            // Offset
            Vector2 scalerOffset = Vector2.Zero;
            switch (selectedScaler)
            {
                case 1: scalerOffset = -Vector2.One * ScaleSize / 2.0f; break;
                case 2:  scalerOffset = -new Vector2(-1f, 1f) * ScaleSize / 2.0f; break;
                case 3: scalerOffset = -new Vector2(1.0f, -1f) * ScaleSize / 2.0f; break;
                case 4: scalerOffset = Vector2.One * ScaleSize / 2.0f; break;
            }
            mouseWorldPosition -= scalerOffset;
            switch (selectedScaler)
            {
                case 1:
                    rectangle.Location = mouseWorldPosition;
                    rectangle.Size = new Vector2(orignalRectangle.Right - mouseWorldPosition.X, orignalRectangle.Bottom - mouseWorldPosition.Y);
                    break;
                
                case 2:
                    rectangle.Location = new Vector2(orignalRectangle.X, mouseWorldPosition.Y);
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
            
            selectedScaler = -1;
            if (scalerTopLeft.Contains(mouseWorldPosition))
                selectedScaler = 1;
            else if (scalerTopRight.Contains(mouseWorldPosition))
                selectedScaler = 2;
            else if (scalerBottomLeft.Contains(mouseWorldPosition))
                selectedScaler = 3;
            else if (scalerBottomRight.Contains(mouseWorldPosition))
                selectedScaler = 4;
            offset = Vector2.Zero;
            prevMousePosition = mouseWorldPosition;
            orignalRectangle = rectangle;
        }
        
        scalerTopLeft.Size = Vector2.One * ScaleSize;
        scalerTopLeft.Location = Vector2.Round(new Vector2(rectangle.Left, rectangle.Top));
        
        scalerTopRight.Size = Vector2.One * ScaleSize;
        scalerTopRight.Location = Vector2.Round(new Vector2(rectangle.Right, rectangle.Top) - Vector2.UnitX * ScaleSize);
        
        scalerBottomLeft.Size = Vector2.One * ScaleSize;
        scalerBottomLeft.Location = Vector2.Round(new Vector2(rectangle.Left, rectangle.Bottom) - Vector2.UnitY * ScaleSize);
        
        scalerBottomRight.Size = Vector2.One * ScaleSize;
        scalerBottomRight.Location = Vector2.Round(new Vector2(rectangle.Right, rectangle.Bottom) - Vector2.One * ScaleSize);

        
        if (!rectangle.Equals(orignalRectangle))
        {
            OnResized?.Invoke(rectangle.ToRectangle());
        }
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);


        Vector2 PositionTop = Vector2.Round(new Vector2(rectangle.Left, rectangle.Top));
        Vector2 PositionBottom = Vector2.Round(new Vector2(rectangle.Left, rectangle.Bottom)) - Vector2.UnitY * OutlineThickness;
        Vector2 PositionLeft = Vector2.Round(new Vector2(rectangle.Left, rectangle.Top));
        Vector2 PositionRight = Vector2.Round(new Vector2(rectangle.Right, rectangle.Top)) - Vector2.UnitX * OutlineThickness;
        
        // Draw Outlined Rectangle
        Color outlineColor = IsChosen ? new Color(Color.Green, 0.5f) : new Color(Color.White, 0.5f);
        
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionTop,
            null,
            outlineColor,
            0f,
            Vector2.Zero,
            new Vector2((float)Math.Round(Size.X), OutlineThickness),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionBottom,
            null,
            outlineColor,
            0f,
            Vector2.Zero,
            new Vector2((float)Math.Round(Size.X), OutlineThickness),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionRight,
            null,
            outlineColor,
            0f,
            Vector2.Zero,
            new Vector2(OutlineThickness, (float)Math.Round(Size.Y)),
            SpriteEffects.None,
            0.9f
        );
        spriteBatch.Draw(
            DebugDraw.WhiteRectangle,
            PositionLeft,
            null,
            outlineColor,
            0f,
            Vector2.Zero,
            new Vector2(OutlineThickness, (float)Math.Round(Size.Y)),
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