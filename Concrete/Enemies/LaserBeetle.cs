using System.Collections.Generic;
using Godot;

public partial class LaserBeetle : Combatant
{
    public static string ScenePath = EditorConstants.CombatantsScenePathBase + "LaserBeetle.tscn";
    public override CombatantConfiguration Configuration
    {
        get => new(
            MaxHealth: 20,
            Speed: 200f,
            StartingRotation: 0f,
            RotationSpeedPerDelta: .4f,
            TargetableType: WeaponConstants.WeaponTarget.Enemy);
    }
    protected override IStateController Controller { get => new EnemyController(Body); }
    protected override List<WeaponConfiguration> WeaponConfigurations {
        get {
            return new() {
                new WeaponConfiguration(
                    LengthOfSight: 700,
                    AngleOfSight: Mathf.Pi / 8,
                    ProjectileSpeed: 1200,
                    ProjectileReloadSeconds: 1.5f,
                    Damage: 1,
                    IsAutoAim: false,
                    PierceCount: 0,
                    ProjectileCount: 1,
                    ProjectileGroupSpreadAngle: 0,
                    ProjectileType: WeaponConstants.ProjectileType.RedBullet,
                    WeaponTarget: WeaponConstants.WeaponTarget.Player,
                    PrefireAction: async () => {
                        var animationPlayer = Body.GetTypedChildByName<AnimationPlayer>("LaserBeetleAnimationPlayer");
                        animationPlayer.Play("telegraph");
                        await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
                        animationPlayer.Play("fire");
                        return;
                    })
            };
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Body.AddToGroup(Groups.Obstacle.ToString());
    }
}