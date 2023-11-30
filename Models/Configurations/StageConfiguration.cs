using Godot;

public record StageConfiguration(
    Vector2 Size,
    StageConstants.StartPosition StartPosition,
    EnemyConfiguration[] EnemyConfigurations,
    PlanetConfiguration[] PlanetConfigurations,
    PortalConfiguration[] PortalConfigurations);