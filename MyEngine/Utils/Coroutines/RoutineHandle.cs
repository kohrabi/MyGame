using System.Collections;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Coroutines;

public class RoutineHandle
{
    
    private readonly IEnumerator _coroutine;
    private bool waiting = false;
    
    public bool IsCompleted { get; private set; }

    public RoutineHandle(IEnumerator coroutine, bool shouldMoveNext = true)
    {
        IsCompleted = false;
        
        _coroutine = coroutine;
        _coroutine.MoveNext();
        if (_coroutine.Current is IYieldInstruction)
            waiting = true;
    }

    public void Update(GameTime gameTime)
    {
        if (IsCompleted)
            return;
        if (!waiting)
        {
            if (!_coroutine.MoveNext())
            {
                IsCompleted = true;
                return;
            }
        }

        if (_coroutine.Current is IYieldInstruction instruction)
        {
            waiting = true;
            if (instruction.Update(gameTime))
                waiting = false;
        }
        else if (_coroutine.Current != null)
        {
            DebugLog.Warning("The returned routine handle isn't handled");
        }
    }
}