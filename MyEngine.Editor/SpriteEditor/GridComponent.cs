using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.Utils;

namespace MyEngine.Editor.SpriteEditor;

public class GridComponent : Component
{
    public Vector2 Size;
    public Vector2 GridSize;
    public Color Color;

    public void Initialize(Vector2 size, Vector2 gridSize, Color color)
    {
        Size = size;
        GridSize = Vector2.Max(gridSize, Vector2.One * 16.0f);
        Color = color;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);

        bool filled = true;
        for (int j = 0; j < Size.Y / Math.Max(GridSize.Y, 1.0f); j++)
        {
            for (int i = 0; i < Size.X / Math.Max(GridSize.X, 1.0f); i++)
            {
                if (filled)
                {
                    DebugDraw.DrawRectangleInline(spriteBatch, 
                        Transform.GlobalPosition + new Vector2(i * GridSize.X, j * GridSize.Y), 
                        GridSize, 
                        Color, 
                        Vector2.Zero);

                }

                filled = !filled;
            }
        }
    }
}