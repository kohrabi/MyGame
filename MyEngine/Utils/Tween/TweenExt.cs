using System;
using Microsoft.Xna.Framework;
using MyEngine.Components;
using MyEngine.Managers;

namespace MyEngine.Utils.Tween;

public static class TweenExt
{
    public static Tweener<T> Tween<T>(Func<T> getter, Action<T> setter, T to, float duration, Func<T, T, float, T> lerp) where T : struct
    {
        var sequence = TweenManager.Instance.CreateSequence();
        Tweener<T> tweener = new Tweener<T>(getter.Invoke(), to, duration, setter, lerp);
        sequence.AddTweener(tweener);
        return tweener;
    }
    
    public static Tweener<float> Tween(Func<float> getter, Action<float> setter, float to, float duration)
        => Tween(getter, setter, to, duration, MathHelper.Lerp);
    
    public static Tweener<double> Tween(Func<double> getter, Action<double> setter, float to, float duration)
        => Tween(getter, setter, to, duration, LerpExt.Lerp);
    
    public static Tweener<Vector2> Tween(Func<Vector2> getter, Action<Vector2> setter, Vector2 to, float duration)
        => Tween(getter, setter, to, duration, Vector2.Lerp);
    
    public static Tweener<Vector3> Tween(Func<Vector3> getter, Action<Vector3> setter, Vector3 to, float duration)
        => Tween(getter, setter, to, duration, Vector3.Lerp);
    
    public static Tweener<Vector4> Tween(Func<Vector4> getter, Action<Vector4> setter, Vector4 to, float duration)
        => Tween(getter, setter, to, duration, Vector4.Lerp);
    
    public static Tweener<Color> Tween(Func<Color> getter, Action<Color> setter, Color to, float duration)
        => Tween(getter, setter, to, duration, Color.Lerp);

    public static Tweener<Vector2> TweenPosition(this Transform transform, Vector2 to, float duration)
        => Tween(() => transform.Position, f => transform.Position = f, to, duration);
    
    public static Tweener<float> TweenRotation(this Transform transform, float to, float duration)
        =>  Tween(() => transform.Rotation, f => transform.Rotation = f, to, duration);
    
}