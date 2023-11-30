using Godot;

public record PlanetConfiguration(
    Vector2 GlobalPosition,
    PlanetConstants.PlanetTexture PlanetTexture,
    float Gravity,
    float RadiusScale,
    float GravityRadiusScale);