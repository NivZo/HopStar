using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class IEnumerableExtensions
{
    public static T PickRandomElement<T>(this IEnumerable<T> values, IEnumerable<T> except = null)
        => values
            .Where(val => except == null || !except.Contains(val))
            .ElementAt(new Random().Next(0, values.Count() - (except?.Count() ?? 0)));
}