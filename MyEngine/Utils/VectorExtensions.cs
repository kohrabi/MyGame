using Microsoft.Xna.Framework;

namespace MyEngine.Utils;

using Num = System.Numerics;

public static class VectorExtensions
{
    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2(vector3.X, vector3.Y);
    }
    
    public static Vector3 ToVector3(this Vector2 vector2, float z = 0f)
    {
        return new Vector3(vector2.X, vector2.Y, z);
    }

    public static Num.Vector2 ToNumerics(this Vector2 vector2)
    {
        return new Num.Vector2(vector2.X, vector2.Y);
    }
}