using System;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Tween;

public enum LoopMode
{
	None,
	Restart,
	PingPong
}

public class Tweener<T> : ITweener where T : struct
{
    private T _start;
    private T _end;
    private float _duration = 0.0f;
    private Action<T> _setter;
    private Func<T, T, float, T> _lerpFunction;
    
    private EasingType _easingType = EasingType.Linear;
    private Func<float, float> _easingFunction;
    private LoopMode _loopMode = LoopMode.None;
    private int _loopCount = -1;

    private bool _isCompleted = false;
    public bool IsCompleted => _isCompleted;
    private double _timer = 0.0f;
    private double _elapsedTime = 0.0;
    
    public double TimeElapsed => _elapsedTime;
    public float Duration => _duration + _delay;
    
    private float _delay = 0.0f;
    private double _timeScale = 1.0f;
    private int _loopCounter = 0;
    
    private bool _pong = false;
    
    // Getter return 0-1
    // Setter return current value * 0-1
    public Tweener(T start, T end, float duration, Action<T> setter, Func<T, T, float, T> lerp)
    {
        _start = start;
        _end = end;
        _duration = duration;
        _setter = setter;
        _lerpFunction = lerp;
        _timer = duration;
        
        SetEasingType(EasingType.Linear);
    }

    public Tweener<T> SetEasingType(EasingType easingType)
    {
        _easingType = easingType;
        _easingFunction = Easings.GetEasingFunction(_easingType);
        return this;
    }

    public Tweener<T> SetLoopMode(LoopMode mode, int loop = -1)
    {
        _loopMode = mode;
        _loopCount = loop;
        return this;
    }

    public Tweener<T> SetTimeScale(float timeScale)
    {
        _timeScale = timeScale;
        return this;
    }

    public Tweener<T> SetDelay(float delay)
    {
        _delay = delay;
        return this;
    }

    public void Update(GameTime gameTime)
    {
        if (_delay > 0.0f)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            _delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return;
        }
        if (_loopCount != -1 && _loopCounter > _loopCount)
            _isCompleted = true;
        if (_isCompleted || _duration * _timeScale <= 0.0f)
            return;

        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (_loopMode == LoopMode.PingPong)
        {
            if (_pong)
                _timer -= gameTime.ElapsedGameTime.TotalSeconds;
            else
                _timer += gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
            _timer += gameTime.ElapsedGameTime.TotalSeconds;
        
        
        float range = _easingFunction.Invoke((float)Math.Clamp(_timer / (_duration * _timeScale), 0.0f, 1.0f));
        _setter.Invoke(_lerpFunction.Invoke(_start, _end, range));
        if (_loopMode == LoopMode.PingPong)
        {
            if (_timer <= 0.0f)
            {
                _loopCounter++;
                _pong = false;
                _timer = 0.0f;
            }
            else if (_timer >= _duration)
            {
                _pong = true;
                _timer = _duration;
            }
        }
        else if (_loopMode == LoopMode.Restart)
        {
            if (_timer >= _duration)
            {
                _loopCounter++;
                _timer = 0.0f;
            }
        }
    }
}