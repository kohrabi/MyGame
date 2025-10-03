using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;

namespace MyEngine.Graphics;


public class SpriteAnimation
{
    public string FilePath;
    public Texture2D Texture;
    public string TextureFilePath { get; set; }
    public Vector2 FrameSize { get; set; }
    public Dictionary<string, List<AnimationFrame> > Animations { get; set; } = new();

    // Make sure TextureFilePath already exists
    public void LoadTexture()
    {
        if (Path.GetExtension(TextureFilePath) == ".png")
            Texture = Texture2D.FromFile(Core.GraphicsDevice, TextureFilePath);
        else
        {
            string xnb = Path.GetRelativePath(Path.GetFullPath(Core.Content.RootDirectory), TextureFilePath);
            xnb = Path.Combine(Path.GetDirectoryName(xnb), Path.GetFileNameWithoutExtension(xnb));
            Texture = Core.Content.Load<Texture2D>(xnb);
        }
    }
}