using System;
using System.Threading.Tasks;
using Godot;

public record WeaponConfiguration(
    bool IsPlayerWeapon,
    float LengthOfSight,
    float AngleOfSight,
    float ProjectileSpeed,
    float ProjectileReloadSeconds,
    WeaponConstants.ProjectileType ProjectileType,
    WeaponConstants.WeaponTarget WeaponTarget,
    int Damage,
    float AccuracyAngle = Mathf.Pi / 30,
    int PierceCount = 0,
    int ProjectileCount = 1,
    float ProjectileGroupSpreadAngle = Mathf.Pi/24,
    bool IsLocking = false,
    bool IsAlwaysFire = false,
    bool IsAutoAim = false,
    bool IsControllerAim = false,
    bool IsSideAim = false,
    Func<Task> PrefireAction = null);