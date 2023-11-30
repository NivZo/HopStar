using System;
using Godot;

[AttributeUsage(AttributeTargets.Field)]
public class OnReadyAttribute : Attribute
{
    public string Name { get; set; }

    public bool IsAutoload { get; set; } = false;
}