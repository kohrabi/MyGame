namespace MyEngine.Utils.Tween;

public enum EasingType
{
    Linear,
    
    // Sine easing functions
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    
    // Quadratic easing functions
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    
    // Cubic easing functions
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    
    // Quartic easing functions
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    
    // Quintic easing functions
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    
    // Exponential easing functions
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    
    // Circular easing functions
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    
    // Back easing functions
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    
    // Elastic easing functions
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
    
    // Bounce easing functions
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
}

public static class Easings
{
    private const float Pi = (float)System.Math.PI;
    private const float HalfPi = Pi / 2.0f;
    private const float TwoPi = Pi * 2.0f;
    
    /// <summary>
    /// Gets the easing function for the specified easing type
    /// </summary>
    /// <param name="easingType">The type of easing to use</param>
    /// <returns>A function that takes a time parameter (0-1) and returns the eased value</returns>
    public static System.Func<float, float> GetEasingFunction(EasingType easingType)
    {
        return easingType switch
        {
            EasingType.Linear => Linear,
            EasingType.EaseInSine => EaseInSine,
            EasingType.EaseOutSine => EaseOutSine,
            EasingType.EaseInOutSine => EaseInOutSine,
            EasingType.EaseInQuad => EaseInQuad,
            EasingType.EaseOutQuad => EaseOutQuad,
            EasingType.EaseInOutQuad => EaseInOutQuad,
            EasingType.EaseInCubic => EaseInCubic,
            EasingType.EaseOutCubic => EaseOutCubic,
            EasingType.EaseInOutCubic => EaseInOutCubic,
            EasingType.EaseInQuart => EaseInQuart,
            EasingType.EaseOutQuart => EaseOutQuart,
            EasingType.EaseInOutQuart => EaseInOutQuart,
            EasingType.EaseInQuint => EaseInQuint,
            EasingType.EaseOutQuint => EaseOutQuint,
            EasingType.EaseInOutQuint => EaseInOutQuint,
            EasingType.EaseInExpo => EaseInExpo,
            EasingType.EaseOutExpo => EaseOutExpo,
            EasingType.EaseInOutExpo => EaseInOutExpo,
            EasingType.EaseInCirc => EaseInCirc,
            EasingType.EaseOutCirc => EaseOutCirc,
            EasingType.EaseInOutCirc => EaseInOutCirc,
            EasingType.EaseInBack => EaseInBack,
            EasingType.EaseOutBack => EaseOutBack,
            EasingType.EaseInOutBack => EaseInOutBack,
            EasingType.EaseInElastic => EaseInElastic,
            EasingType.EaseOutElastic => EaseOutElastic,
            EasingType.EaseInOutElastic => EaseInOutElastic,
            EasingType.EaseInBounce => EaseInBounce,
            EasingType.EaseOutBounce => EaseOutBounce,
            EasingType.EaseInOutBounce => EaseInOutBounce,
            _ => Linear
        };
    }
    
    // Linear
    public static float Linear(float t) => t;
    
    // Sine easing functions
    public static float EaseInSine(float t) => 1.0f - (float)System.Math.Cos(t * HalfPi);
    public static float EaseOutSine(float t) => (float)System.Math.Sin(t * HalfPi);
    public static float EaseInOutSine(float t) => -(float)System.Math.Cos(Pi * t) / 2.0f + 0.5f;
    
    // Quadratic easing functions
    public static float EaseInQuad(float t) => t * t;
    public static float EaseOutQuad(float t) => 1.0f - (1.0f - t) * (1.0f - t);
    public static float EaseInOutQuad(float t) => t < 0.5f ? 2.0f * t * t : 1.0f - (float)System.Math.Pow(-2.0f * t + 2.0f, 2.0f) / 2.0f;
    
    // Cubic easing functions
    public static float EaseInCubic(float t) => t * t * t;
    public static float EaseOutCubic(float t) => 1.0f - (float)System.Math.Pow(1.0f - t, 3.0f);
    public static float EaseInOutCubic(float t) => t < 0.5f ? 4.0f * t * t * t : 1.0f - (float)System.Math.Pow(-2.0f * t + 2.0f, 3.0f) / 2.0f;
    
    // Quartic easing functions
    public static float EaseInQuart(float t) => t * t * t * t;
    public static float EaseOutQuart(float t) => 1.0f - (float)System.Math.Pow(1.0f - t, 4.0f);
    public static float EaseInOutQuart(float t) => t < 0.5f ? 8.0f * t * t * t * t : 1.0f - (float)System.Math.Pow(-2.0f * t + 2.0f, 4.0f) / 2.0f;
    
    // Quintic easing functions
    public static float EaseInQuint(float t) => t * t * t * t * t;
    public static float EaseOutQuint(float t) => 1.0f - (float)System.Math.Pow(1.0f - t, 5.0f);
    public static float EaseInOutQuint(float t) => t < 0.5f ? 16.0f * t * t * t * t * t : 1.0f - (float)System.Math.Pow(-2.0f * t + 2.0f, 5.0f) / 2.0f;
    
    // Exponential easing functions
    public static float EaseInExpo(float t) => t == 0.0f ? 0.0f : (float)System.Math.Pow(2.0f, 10.0f * (t - 1.0f));
    public static float EaseOutExpo(float t) => System.Math.Abs(t - 1.0f) < float.Epsilon ? 1.0f : 1.0f - (float)System.Math.Pow(2.0f, -10.0f * t);
    public static float EaseInOutExpo(float t)
    {
        if (t == 0.0f) return 0.0f;
        if (System.Math.Abs(t - 1.0f) < float.Epsilon) return 1.0f;
        return t < 0.5f ? (float)System.Math.Pow(2.0f, 20.0f * t - 10.0f) / 2.0f : (2.0f - (float)System.Math.Pow(2.0f, -20.0f * t + 10.0f)) / 2.0f;
    }
    
    // Circular easing functions
    public static float EaseInCirc(float t) => 1.0f - (float)System.Math.Sqrt(1.0f - t * t);
    public static float EaseOutCirc(float t) => (float)System.Math.Sqrt(1.0f - (float)System.Math.Pow(t - 1.0f, 2.0f));
    public static float EaseInOutCirc(float t) => t < 0.5f ? (1.0f - (float)System.Math.Sqrt(1.0f - (float)System.Math.Pow(2.0f * t, 2.0f))) / 2.0f : ((float)System.Math.Sqrt(1.0f - (float)System.Math.Pow(-2.0f * t + 2.0f, 2.0f)) + 1.0f) / 2.0f;
    
    // Back easing functions
    private const float C1 = 1.70158f;
    private const float C2 = C1 * 1.525f;
    private const float C3 = C1 + 1.0f;
    
    public static float EaseInBack(float t) => C3 * t * t * t - C1 * t * t;
    public static float EaseOutBack(float t) => 1.0f + C3 * (float)System.Math.Pow(t - 1.0f, 3.0f) + C1 * (float)System.Math.Pow(t - 1.0f, 2.0f);
    public static float EaseInOutBack(float t) => t < 0.5f ? ((float)System.Math.Pow(2.0f * t, 2.0f) * ((C2 + 1.0f) * 2.0f * t - C2)) / 2.0f : ((float)System.Math.Pow(2.0f * t - 2.0f, 2.0f) * ((C2 + 1.0f) * (t * 2.0f - 2.0f) + C2) + 2.0f) / 2.0f;
    
    // Elastic easing functions
    private const float C4 = TwoPi / 3.0f;
    private const float C5 = TwoPi / 4.5f;
    
    public static float EaseInElastic(float t)
    {
        if (t == 0.0f) return 0.0f;
        if (System.Math.Abs(t - 1.0f) < float.Epsilon) return 1.0f;
        return -(float)System.Math.Pow(2.0f, 10.0f * (t - 1.0f)) * (float)System.Math.Sin((t - 1.1f) * C4);
    }
    
    public static float EaseOutElastic(float t)
    {
        if (t == 0.0f) return 0.0f;
        if (System.Math.Abs(t - 1.0f) < float.Epsilon) return 1.0f;
        return (float)System.Math.Pow(2.0f, -10.0f * t) * (float)System.Math.Sin((t - 0.1f) * C4) + 1.0f;
    }
    
    public static float EaseInOutElastic(float t)
    {
        if (t == 0.0f) return 0.0f;
        if (System.Math.Abs(t - 1.0f) < float.Epsilon) return 1.0f;
        return t < 0.5f ? 
            -((float)System.Math.Pow(2.0f, 20.0f * t - 10.0f) * (float)System.Math.Sin((20.0f * t - 11.125f) * C5)) / 2.0f :
            ((float)System.Math.Pow(2.0f, -20.0f * t + 10.0f) * (float)System.Math.Sin((20.0f * t - 11.125f) * C5)) / 2.0f + 1.0f;
    }
    
    // Bounce easing functions
    private const float N1 = 7.5625f;
    private const float D1 = 2.75f;
    
    public static float EaseOutBounce(float t)
    {
        if (t < 1.0f / D1)
            return N1 * t * t;
        else if (t < 2.0f / D1)
            return N1 * (t -= 1.5f / D1) * t + 0.75f;
        else if (t < 2.5f / D1)
            return N1 * (t -= 2.25f / D1) * t + 0.9375f;
        else
            return N1 * (t -= 2.625f / D1) * t + 0.984375f;
    }
    
    public static float EaseInBounce(float t) => 1.0f - EaseOutBounce(1.0f - t);
    
    public static float EaseInOutBounce(float t) => t < 0.5f ? (1.0f - EaseOutBounce(1.0f - 2.0f * t)) / 2.0f : (1.0f + EaseOutBounce(2.0f * t - 1.0f)) / 2.0f;
}