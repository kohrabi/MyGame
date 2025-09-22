using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Tween;

public class TweenParallel
{
    private List<ITweener> _tweeners = new List<ITweener>();
    public bool IsCompleted { get; private set; }
    public bool Paused { get; set; }
    public Action TweenCompleteCallback = () => { };
    
    public void AddTweener(ITweener tweener)
    {
        _tweeners.Add(tweener);
    }

    public void RemoveTweener(ITweener tweener)
    {
        _tweeners.Remove(tweener);
    }
    
    public void Update(GameTime gameTime)
    {
        if (Paused)
            return;
        bool isCompleted = true;
        foreach (var tweener in _tweeners)
        {
            tweener.Update(gameTime);
            if (!tweener.IsCompleted)
                isCompleted = false;
        }
        IsCompleted = isCompleted;
        if (IsCompleted)
            TweenCompleteCallback.Invoke();
    }
}