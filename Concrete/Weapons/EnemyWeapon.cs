using System;
using Godot;

public partial class EnemyWeapon : Weapon
{
    public static string ScenePath = EditorConstants.WeaponsScenePathBase + "EnemyWeapon.tscn";


    public override void Configure(Node parent)
    {
        base.Configure(parent);

        _timer.Connect("timeout", new(this, MethodName.Shoot));
        _timer.OneShot = false;
        var randomOffset = (float)new Random().NextDouble() * Configuration.ProjectileReloadSeconds;
        TimerUtils.DelayAction(randomOffset, () => {
            if (_timer.IsValid())
                _timer.Start();
        });
    }
}