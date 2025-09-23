using System;
using Microsoft.Xna.Framework;
using MyEngine.Components;
using MyEngine.Utils.MyMath;

namespace MyEngine.Collision;

public static class CollisionHelper
{
    public static CollisionResult SweptAABB(RectangleF a, Vector2 aVelocity, RectangleF b, Vector2 bVelocity)
    {
        Vector2 normal = Vector2.Zero;
        Vector2 invEntry = Vector2.Zero;
        Vector2 invExit = Vector2.Zero;
        if (aVelocity.X > 0)
        {
            invEntry.X = b.Left - a.Right;
            invExit.X = b.Right - a.Left;
        }
        else
        {
            invEntry.X = b.Right - a.Left;
            invExit.X = b.Left - a.Right;
        }

        if (aVelocity.Y > 0)
        {
            invEntry.Y = b.Top - a.Bottom;
            invExit.Y = b.Bottom - a.Top;
        }
        else
        {
            invEntry.Y = b.Bottom - a.Top;
            invExit.Y = b.Top - a.Bottom;
        }

        Vector2 entry = Vector2.Zero;
        Vector2 exit = Vector2.Zero;

        if (aVelocity.X == 0.0f)
        {
            entry.X = float.PositiveInfinity;
            exit.X = float.PositiveInfinity;
        }
        else
        {
            entry.X = invEntry.X / aVelocity.X;
            exit.X = invExit.X / aVelocity.X;
        }

        if (aVelocity.Y == 0.0f)
        {
            entry.Y = float.PositiveInfinity;
            exit.Y = float.PositiveInfinity;
        }
        else
        {
            entry.Y = invEntry.Y / aVelocity.Y;
            exit.Y = invExit.Y / aVelocity.Y;
        }

        float entryTime = Math.Max(entry.X, entry.Y);
        float exitTime = Math.Min(exit.X, exit.Y);

        if (entryTime > exitTime || entryTime < 0.0f || entryTime > 1.0f)
        {
            normal = Vector2.Zero;
            return new CollisionResult(a, aVelocity, b, bVelocity, normal, entryTime);
        }

        // calculate normal of collided surface
        if (entry.X > entry.Y)
        {
            if (invEntry.X < 0.0f)
                normal = Vector2.UnitX;
            else
                normal = -Vector2.UnitX;
        }
        else
        {
            if (invEntry.Y < 0.0f)
                normal = Vector2.UnitY;
            else
                normal = -Vector2.UnitY;
        }

        return new CollisionResult(a, aVelocity, b, bVelocity, normal, entryTime);
    }

    public static RectangleF GetSweptBroadPhaseBox(RectangleF rect, Vector2 velocity)
    {
        RectangleF result = new RectangleF();
        result.X = velocity.X > 0 ? result.X : result.X + velocity.X;
        result.Y = velocity.Y > 0 ? result.Y : result.Y + velocity.Y;
        result.Width = velocity.X > 0 ? result.Width + velocity.X : result.Width - velocity.X;
        result.Height = velocity.Y > 0 ? result.Height + velocity.Y : result.Height - velocity.Y;
        return result;
    }

}