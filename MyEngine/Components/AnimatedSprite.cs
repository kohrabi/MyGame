using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Utils;
using MyEngine.Utils.Attributes;

namespace MyEngine.Components;

public class AnimationFrame
{
    public int FrameNumber { get; set; }
    public float Duration { get; set; }
    public Rectangle Rectangle { get; set; }
}

public class AnimatedSprite : Sprite
{
    private Dictionary<string, List<AnimationFrame>> _animations = new Dictionary<string, List<AnimationFrame>>();
    private string _currentAnimation = "";

    private int _frameIndex = 0;
    private float _frameTimer = 0.0f;
    
    public bool Paused = false;

    public Dictionary<string, List<AnimationFrame>> Animations
    {
        set => _animations = value;
    }
    
    public override void Initialize(ContentManager content)
    {
        base.Initialize(content);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Paused)
            return;
        if (_currentAnimation != "" && _animations.TryGetValue(_currentAnimation, out List<AnimationFrame> frames))
        {
            if (_frameTimer < frames[_frameIndex].Duration)
            {
                _frameTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                _frameTimer -= frames[_frameIndex].Duration;
                _frameIndex = (_frameIndex + 1) % frames.Count;
                SourceRectangle = frames[_frameIndex].Rectangle;
            }
        }
    }

    public void PlayAnimation(string animation)
    {
        _currentAnimation = animation;
        _frameIndex = 0;
        _frameTimer = 0.0f;
    }
}