using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Tween;

public class TweenSequence
{
    private List<TweenParallel> _tweens = new List<TweenParallel>();
    private int _currentIndex = 0;

    public bool Parallel { get; set; } = true;
    public bool Paused { get; set; }
    public bool IsCompleted { get; private set; }
    public Action SequenceCompleteCallback = () => { };

    public void AddTweener(ITweener tweener)
    {
        if (Parallel)
        {
            if (_tweens.Count <= 0)
                _tweens.Add(new TweenParallel());
            _tweens.Last().AddTweener(tweener);
        }
        else
        {
            TweenParallel tweenParallel = new TweenParallel();
            tweenParallel.AddTweener(tweener);
            _tweens.Add(tweenParallel);
        }
    }
    
    public void AddTween(TweenParallel tweenParallel)
    {
        _tweens.Add(tweenParallel);
    }

    public void RemoveTween(TweenParallel tweenParallel)
    {
        _tweens.Remove(tweenParallel);
    }

    public void Update(GameTime gameTime)
    {
        if (Paused || IsCompleted)
            return;
        _tweens[_currentIndex].Update(gameTime);
        if (_tweens[_currentIndex].IsCompleted)
            _currentIndex++;
        if (_currentIndex >= _tweens.Count)
        {
            IsCompleted = true;
            SequenceCompleteCallback.Invoke();
        }
    }
}