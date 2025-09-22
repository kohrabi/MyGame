using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyEngine.Utils.Coroutines;

public class ObjectCoroutineManager
{
    
    private readonly List<RoutineHandle> _coroutines = new List<RoutineHandle>();

    public RoutineHandle StartCoroutine(IEnumerator coroutine)
    {
        RoutineHandle routineHandle = new RoutineHandle(coroutine);
        _coroutines.Add(routineHandle);
        return routineHandle;
    }
    
    // This might cause a problem if we remove a coroutine which other coroutine are waiting for
    public void StopCoroutine(RoutineHandle coroutine)
    {
        _coroutines.Remove(coroutine);
    }

    public void StopAllCoroutines()
    {
        _coroutines.Clear();
    }
    
    public void Update(GameTime gameTime)
    {
        foreach (var coroutine in _coroutines)
        {
            coroutine.Update(gameTime);
        }

        _coroutines.RemoveAll((value) => value.IsCompleted);
    }
}