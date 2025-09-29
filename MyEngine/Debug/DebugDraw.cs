using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Utils.DebugDrawPrimitives;
using MyEngine.Utils.MyMath;

namespace MyEngine.Utils;

public class DebugDraw
{
    private static readonly DebugDraw _instance = new DebugDraw();  
    internal static DebugDraw Instance => _instance;
    private static List<IDebugDrawPrimitive> _drawCommands = new(); 
    
    public static Texture2D WhiteRectangle;
    public static Texture2D WhiteCircle;
    public static Texture2D WhiteCircleOutlined;
    
    internal static SpriteFont _font;

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content)
    {
        WhiteRectangle = new Texture2D(graphicsDevice, 1, 1);
        WhiteRectangle.SetData([Color.White]);
        
        WhiteCircle = content.Load<Texture2D>("Engine/whiteCircle");
        WhiteCircleOutlined = content.Load<Texture2D>("Engine/whiteCircleOutlined");
        _font = content.Load<SpriteFont>("Engine/Fonts/DebugFont");
    }

    public void Draw(SpriteBatch spriteBatch, Matrix? transformMatrix = null)
    {
        spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            depthStencilState: DepthStencilState.None,
            rasterizerState: RasterizerState.CullNone,
            samplerState: SamplerState.PointWrap,
            effect: null,
            transformMatrix: transformMatrix
        );
        foreach (var command in _drawCommands)
        {
            command.Draw(spriteBatch);
        }
        spriteBatch.End();
        _drawCommands.Clear();
    }

    public static void DrawRectangleInline(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color, Vector2 origin)
    {
        spriteBatch.Draw(
            WhiteRectangle,
            position,
            null,
            color,
            0f,
            new Vector2(WhiteRectangle.Width, WhiteRectangle.Height) * origin,
            size,
            SpriteEffects.None,
            0f
        );
    }

    public static void DrawPrimitive(IDebugDrawPrimitive primitive)
    {
        _drawCommands.Add(primitive);
    }
    
    public static void DrawPoint(Vector2 position, Color color, float size = 5.0f)
    {
        _drawCommands.Add(new DebugDrawPoint(position, color, size));
    }

    public static void DrawRectangle(Vector2 position, Color color, Vector2 size, bool isOutlined = false, float outlineThickness = 1.0f)
    {
        _drawCommands.Add(new DebugDrawRectangle(position, color, size, isOutlined, outlineThickness));
    }
    
    public static void DrawRectangle(RectangleF rect, Color color, bool isOutlined = false, float outlineThickness = 1.0f)
    {
        _drawCommands.Add(new DebugDrawRectangle(rect.Location, color, rect.Size, isOutlined, outlineThickness));
    }
    
    public static void DrawCircle(Vector2 position, Color color, float radius, bool isOutlined = false)
    {
        _drawCommands.Add(new DebugDrawCircle(position, color, radius, isOutlined));
    }
    
    public static void DrawString(string text, Vector2 position, Color color, Vector2 size)
    {
        _drawCommands.Add(new DebugDrawString(text, position, color, size));
    }
}