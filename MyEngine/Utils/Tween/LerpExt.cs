namespace MyEngine.Utils.Tween;

public static class LerpExt
{
    public static double Lerp(double current, double to, float t)
    {
        return current + (to - current) * t;
    }
    
    public static int Lerp(int current, int to, float t)
    {
        return (int)(current + (to - current) * t);
    }
}