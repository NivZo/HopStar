using System.Collections;
using System.Collections.Generic;
using Godot;

public record CombatantConfiguration(
    int MaxHealth,
    float Speed,
    float StartingRotation,
    float RotationSpeedPerDelta,
    WeaponConstants.WeaponTarget TargetableType
);