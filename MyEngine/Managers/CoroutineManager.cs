using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyEngine.Utils.Coroutines;

namespace MyEngine.Managers;

public class CoroutineManager : GlobalManager
{
    private readonly List<RoutineHandle> _coroutines = new List<RoutineHandle>();

    public void StartCoroutine(IEnumerator coroutine)
    {
        _coroutines.Add(new RoutineHandle(coroutine));
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var coroutine in _coroutines)
        {
            coroutine.Update(gameTime);
        }

        _coroutines.RemoveAll((value) => value.IsCompleted);
    }

    public override void OnEnable()
    { }

    public override void OnDisable()
    { }
}