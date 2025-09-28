using System;
using System.Collections.Generic;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.ContentProcessors.Aseprite;
using MyEngine.Debug.IMGUIComponents;
using Num = System.Numerics;

namespace MyEngine.Editor.SpriteEditor;

public class ImGuiSpriteAnimationViewer : ImGuiComponent
{
    
    private Dictionary<string, List<AnimationFrame>> _animations = new Dictionary<string, List<AnimationFrame>>();
    private string _currentAnimation = "";
    private int _frameIndex = 0;
    private float _frameTimer = 0.0f;
    private bool _paused = false;
    
    private Rectangle _sourceRect = new Rectangle();
    private nint _texture;
    private Num.Vector2 _imageSize;
    private ImFontPtr _fontIcon;

    public int CurrentFrame => _frameIndex;
    
    public ImGuiSpriteAnimationViewer(ImGuiRenderer renderer, Scene scene, int id, ImFontPtr fontIcon) : base(renderer, scene, id)
    {
        _fontIcon = fontIcon;
    }

    
    public void SetAnimation(AsepriteJson asepriteJson, string defaultAnimation)
    {
        if (_texture != 0)
            ImGuiRenderer.UnbindTexture(_texture);
        _texture = ImGuiRenderer.BindTexture(asepriteJson.Texture);
        _imageSize = new Num.Vector2(asepriteJson.Texture.Width, asepriteJson.Texture.Height);
        _animations = asepriteJson.Animations;
        _currentAnimation = defaultAnimation;
    }

    public void PlayAnimation(string animation)
    {
        _currentAnimation = animation;
        _frameIndex = 0;
        _frameTimer = 0.0f;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (_animations.Count == 0)
            return;
        
        
        if (_currentAnimation != "" && _animations.TryGetValue(_currentAnimation, out List<AnimationFrame> frames))
        {
            if (!_paused)
            {
                if (_frameTimer < frames[_frameIndex].Duration)
                {
                    _frameTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    _frameTimer -= frames[_frameIndex].Duration;
                    _frameIndex = (_frameIndex + 1) % frames.Count;
                    _sourceRect = frames[_frameIndex].Rectangle;
                }
            }
            else
            {
                _sourceRect = frames[_frameIndex].Rectangle;
            }
        }
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;
    }

    public void SetCurrentFrame(int frame)
    {
        _frameIndex = frame;
        SetPaused(true);
    }
    
    public void SetSkipFrame(int frameSkip)
    {
        SetPaused(true);
        if (_currentAnimation != "" && _animations.TryGetValue(_currentAnimation, out List<AnimationFrame> frames))
        {
            if (frameSkip > 0)
                _frameIndex = (_frameIndex + frameSkip) % frames.Count;
            else
            {
                frameSkip %= frames.Count;
                while (_frameIndex + frameSkip < 0)
                {
                    _frameIndex = frames.Count - 1 + frameSkip;
                    frameSkip = Math.Min(frameSkip + frames.Count, 0);
                }
                _frameIndex += frameSkip;
            }
        }
    }

    public void SetToStart()
    {
        SetPaused(true);
        _frameIndex = 0;
    }

    public void SetToEnd()
    {
        SetPaused(true);
        if (_currentAnimation != "" && _animations.TryGetValue(_currentAnimation, out List<AnimationFrame> frames))
            _frameIndex = frames.Count - 1;
    }

    public override void Draw()
    {
        if (_texture == IntPtr.Zero)
            return;
        ImGui.PushFont(_fontIcon);
        
        if (ImGui.Begin("AnimationViewer"))
        {
            Num.Vector2 buttonSize = Num.Vector2.One * 24f;
            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Num.Vector2((ImGui.GetContentRegionAvail().X - buttonSize.X * 6) / 2, 5.0f) );
            if (ImGui.Button(IconFonts.FontAwesome4.Backward, buttonSize))
            {
                SetToStart();
            }
            ImGui.SameLine();
            if (ImGui.Button(IconFonts.FontAwesome4.FastBackward, buttonSize))
            {
                SetSkipFrame(-1);
            }
            ImGui.SameLine();
            if (ImGui.Button(_paused ? IconFonts.FontAwesome4.Pause : IconFonts.FontAwesome4.Play, buttonSize))
            {
                SetPaused(!_paused);
            }
            ImGui.SameLine();
            if (ImGui.Button(IconFonts.FontAwesome4.FastForward, buttonSize))
            {
                SetSkipFrame(1);
            }
            ImGui.SameLine();
            if (ImGui.Button(IconFonts.FontAwesome4.Forward, buttonSize))
            {
                SetToEnd();
            }
            
            
            Num.Vector2 pos = _sourceRect.Location.ToVector2().ToNumerics();
            Num.Vector2 size = _sourceRect.Size.ToVector2().ToNumerics();
            Num.Vector2 dividor = _imageSize;
            
            Num.Vector2 windowSize = ImGui.GetContentRegionAvail();
            float spriteAspectRatio = size.X / size.Y;
            float windowAspectRatio = windowSize.X / windowSize.Y;
            float scale = (windowAspectRatio > spriteAspectRatio) ? windowSize.Y / size.Y : windowSize.X / size.X;
            ImGui.SetCursorPos(ImGui.GetCursorPos() + (windowSize - (size * scale)) / 2.0f);
            ImGui.Image(_texture, scale * size, pos / dividor, (pos + size) / dividor);
        }
        ImGui.End();
        ImGui.PopFont();
    }
}