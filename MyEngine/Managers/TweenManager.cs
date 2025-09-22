using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyEngine.Utils.Tween;

namespace MyEngine.Managers;

public class TweenManager : GlobalManager
{
    public static TweenManager Instance { get; private set; }
    
    private List<TweenSequence> _sequence = new List<TweenSequence>();

    public TweenManager()
    {
        System.Diagnostics.Debug.Assert(Instance == null, "Instance should be null");
        Instance = this;
    }
    
    public void AddSequence(TweenSequence tweenSequence)
    {
        _sequence.Add(tweenSequence);
    }

    public void RemoveSequence(TweenSequence tweenSequence)
    {
        _sequence.Remove(tweenSequence);
    }

    public TweenSequence CreateSequence()
    {
        var sequence = new TweenSequence();
        AddSequence(sequence);
        return sequence;
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var sequence in _sequence)
        {
            sequence.Update(gameTime);
        }
        _sequence.RemoveAll(sequence => sequence.IsCompleted);
    }
    
    public override void OnEnable() { }
    public override void OnDisable() { }
}