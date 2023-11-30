
using System;
using Godot;

public static class MathUtils
{
    public static float RandomBetween(float from, float to) => (float)new Random().NextDouble() * (to - from) + from;
    public static float Wrap(float num, float min, float max)
    {
        var length = Math.Abs(max) + Math.Abs(min);
        while (num > max)
        {
            num -= length;
        }
        
        while (num < min)
        {
            num += length;
        }

        return num;
    }

    public static float WrapAroundZero(float num, float radius)
    {
        if (radius <= 0)
        {
            return num;
        }

        return Wrap(num, -radius, radius);
    }

    public static float Clamp(float num, float min, float max)
    {
        if (num < min)
        {
            return min;
        }

        if (num > max)
        {
            return max;
        }

        return num;
    }

    public static Vector2 ClampBox(Vector2 vector, float min, float max) => 
        vector.MapCords((float crd) => Clamp(crd, min, max));
    
    public static Vector2 ClampCircle(Vector2 vector, float radius)
    {
        if (vector.Length() > radius)
        {
            var normalized = vector.Normalized();
            normalized.X *= radius;
            normalized.Y *= radius;
            return normalized;
        }

        return vector;
    }
}