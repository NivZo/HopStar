using System;
using Godot;

public static class Vector2Extensions
{
    public static Vector2 MapCords(this Vector2 vector, Func<float, float> mapper)
    {
        vector.X = mapper(vector.X);
        vector.Y = mapper(vector.Y);

        return vector;
    }

    public static Vector2 Mult(this Vector2 vector, float a) =>  new(vector.X * a, vector.Y * a);
}