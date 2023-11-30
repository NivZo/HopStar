using Godot;

public static class Sprited2DExtensions
{
    public static void SetShaderParameter(this Sprite2D sprite, string parameter, Variant value)
    {
        (sprite.Material as ShaderMaterial).SetShaderParameter(parameter, value);
    }

    public static Variant GetShaderParameter(this Sprite2D sprite, string parameter) =>
        (sprite.Material as ShaderMaterial).GetShaderParameter(parameter);
}