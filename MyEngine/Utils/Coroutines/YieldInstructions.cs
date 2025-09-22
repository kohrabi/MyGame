using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Coroutines;

public interface IYieldInstruction
{
    bool Update(GameTime gameTime);
}

public class WaitForSeconds : IYieldInstruction
{
    private double _timeLeft;

    public WaitForSeconds(double seconds)
    {
        _timeLeft = seconds;
    }

    public bool Update(GameTime gameTime)
    {
        _timeLeft -= gameTime.ElapsedGameTime.TotalSeconds;
        return _timeLeft <= 0; // return true = done
    }
}

public class WaitUntil : IYieldInstruction
{
    private readonly Func<bool> _condition;

    public WaitUntil(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool Update(GameTime gameTime)
    {
        return _condition();
    }
}

/// <summary>
/// Wait for other IEnumerator or Routine
/// </summary>
public class WaitFor : IYieldInstruction
{
    private readonly RoutineHandle _routine;

    public WaitFor(IEnumerator enumerator)
    {
        _routine = new RoutineHandle(enumerator, false);
    }

    
    public bool Update(GameTime gameTime)
    {
        _routine.Update(gameTime);
        return _routine.IsCompleted;
    }
}
