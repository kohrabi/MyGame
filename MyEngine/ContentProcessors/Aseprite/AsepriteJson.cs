using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;

namespace MyEngine.ContentProcessors.Aseprite;


public class AsepriteJson
{
    public string FilePath { get; private set; }
    public Texture2D Texture { get; private set; }
    public Vector2 FrameSize { get; private set; }
    public Dictionary<string, List<AnimationFrame> > Animations { get; private set; } = new();

    public static AsepriteJson FromFileRelative(ContentManager content, string path)
    {
        string filePath = Path.Combine(content.RootDirectory + "/" + path);
        return FromFile(content, Path.GetFullPath(filePath));
    }
    
    // Currently no rotated, trimmed
    public static AsepriteJson FromFile(ContentManager content, string path)
    {
        string jsonText = File.ReadAllText(path);
        AsepriteRawJsonData? jsonData = JsonSerializer.Deserialize<AsepriteRawJsonData>(jsonText);

        if (jsonData == null)
            throw new NullReferenceException("Yoo this Json data is null something went wrong");
        if (jsonData.frames.Count == 0)
            throw new ArgumentOutOfRangeException("Yoo there can't be zero frames");

        AsepriteJson asepriteJson = new AsepriteJson();
        string imageLoadPath = Path.GetRelativePath(content.RootDirectory, Path.GetDirectoryName(path) + "/" +
                                                                       Path.GetFileNameWithoutExtension(jsonData.meta.image));
        string imagePathXnb = Path.GetFullPath(Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(jsonData.meta.image) + ".xnb");
        string imagePathJson = Path.GetFullPath(Path.GetDirectoryName(path) + "/" + jsonData.meta.image);
        if (!File.Exists(imagePathXnb))
        {
            if (!File.Exists(imagePathJson))
                throw new ArgumentException("Cannot load because image file " + imagePathXnb + " doesn't exists in original format nor .xnb");
            else
                asepriteJson.Texture = Texture2D.FromFile(Core.GraphicsDevice, imagePathJson);
        }
        else
            asepriteJson.Texture = content.Load<Texture2D>(imageLoadPath);
        asepriteJson.FrameSize = new Vector2(jsonData.frames[0].sourceSize.w,  jsonData.frames[0].sourceSize.h);
        asepriteJson.FilePath = path;
        
        int endFrame = 0;
        string parentTagName = "";
        foreach (var frameTag in jsonData.meta.frameTags)
        {
            string name = frameTag.name;
            List<AnimationFrame> frames = new();
            if (frameTag.to > endFrame)
            {
               endFrame = frameTag.to;
               parentTagName = frameTag.name;
            }
            else
            {
                name = parentTagName + frameTag.name;
                asepriteJson.Animations[parentTagName]
                    .RemoveAll((match) => match.FrameNumber >= frameTag.from && match.FrameNumber <= frameTag.to);
                if (asepriteJson.Animations[parentTagName].Count == 0)
                    asepriteJson.Animations.Remove(parentTagName);
            }
            for (int i = frameTag.from; i <= frameTag.to; i++)
            {
                var dataFrame = jsonData.frames[i];
                
                AnimationFrame frame = new AnimationFrame();
                frame.Duration = dataFrame.duration;
                frame.FrameNumber = i;
                frame.Rectangle = new Rectangle(
                    dataFrame.frame.x, 
                    dataFrame.frame.y, 
                    dataFrame.frame.w, 
                    dataFrame.frame.h);
                frames.Add(frame);
            }
            asepriteJson.Animations.Add(name, frames);
        }
        
        return asepriteJson;
    }
    
    private class AsepriteRawJsonData
    {
        public class Frame
        {
            public string filename { get; set; }
            public Rect frame { get; set; }
            public bool rotated { get; set; }
            public bool trimmed { get; set; }
            public Rect spriteSourceSize { get; set; }
            public Size sourceSize { get; set; }
            public float duration { get; set; }
        }

        public class Meta
        {
            public class FrameTag
            {
                public string name { get; set; }
                public int from { get; set; }
                public int to { get; set; }
                public string direction { get; set; }
                public string color { get; set; }
            }

            public string app { get; set; }
            public string version { get; set; }
            public string image { get; set; }
            public string format { get; set; }
            public Size size { get; set; }
            public string scale { get; set; }
            public List<FrameTag> frameTags { get; set; }
        }

        public class Rect
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public class Size
        {
            public int w { get; set; }
            public int h { get; set; }
        }

        public List<Frame> frames { get; set; }
        public Meta meta { get; set; }
    }
}