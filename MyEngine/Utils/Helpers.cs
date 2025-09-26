using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyEngine.Utils;

public static class Helpers
{
    // public static Texture2D GetTexture2D(this RenderTarget2D renderTarget, GraphicsDevice graphicsDevice)
    // {
    //     renderTarget.GetData(renderTargetData);
    //     var renderTargetTexture = new Texture2D(graphicsDevice, renderTarget.Width, renderTarget.Height);
    //     renderTargetTexture.SetData(renderTargetData);
    //     return renderTargetTexture;
    // }
    
    public static Rectangle ClampRectangle(Rectangle inner, Rectangle outer)
    {
        int clampedX = Math.Clamp(inner.X, outer.Left, outer.Right - inner.Width);
        int clampedY = Math.Clamp(inner.Y, outer.Top,  outer.Bottom - inner.Height);

        return new Rectangle(clampedX, clampedY, inner.Width, inner.Height);
    }

    public static string GetContentPath(ContentManager content, string path)
    {
        return Path.GetFullPath(content.RootDirectory + "/" + path);
    }
}