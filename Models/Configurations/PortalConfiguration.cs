using Godot;

public record PortalConfiguration(
    Vector2 GlobalPosition,
    PlanetConstants.PortalTexture PortalTexture,
    float Gravity,
    float RadiusScale,
    float GravityRadiusScale);