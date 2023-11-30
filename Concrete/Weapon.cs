using System;
using System.Linq;
using System.Threading;
using Godot;

public partial class Weapon : Node2D, IConfigurable<WeaponConfiguration>
{
    public static string ScenePath = EditorConstants.WeaponsScenePathBase + "Weapon.tscn";
    public WeaponConfiguration Configuration { get; set; }
    private ISceneService _sceneService { get; set; }
    private ICombatService _combatService { get; set; }
    private PackedScene _projectileScene = null;
    private Area2D _visionArea = null;
    
    public void Configure(Node parent)
    {
        _visionArea = this.GetTypedChildByName<Area2D>(NodeName.Weapon.WpnVisionArea);
        _projectileScene = GD.Load<PackedScene>(Projectile.ScenePath);

        var _visionCollShape = _visionArea.GetTypedChildByName<CollisionPolygon2D>(NodeName.Weapon.WpnVisionCollShape);
        _visionCollShape.Polygon = CreateConeOfSight();


        parent.AddChild(this);
        _sceneService = this.GetAutoload<SceneService>();
        _combatService = this.GetAutoload<CombatService>();

        var timer = new Godot.Timer()
        {
            OneShot = false,
            WaitTime = Configuration.ProjectileReloadSeconds,
        };
        AddChild(timer);
        timer.Connect("timeout", new(this, MethodName.Shoot));
        if (Configuration.WeaponTarget == WeaponConstants.WeaponTarget.Player)
        {
            var randomOffset = (float)new Random().NextDouble() * Configuration.ProjectileReloadSeconds;
            TimerUtils.DelayAction(randomOffset, () => {
                if (timer.IsValid())
                    timer.Start();
            }
        );
        }
        else
        {
            timer.Start();
        }
    }

    public async void Shoot()
    {
        if (IsInstanceValid(this) && _combatService.IsCombat())
        {
            var _targetInRange = _visionArea.GetOverlappingBodies()
                .Where(body => body is ITargetable targetable && targetable.TargetableType == Configuration.WeaponTarget);
            
            var closestTarget = _targetInRange.Count() > 0 ?
                _targetInRange.MinBy(target => target.GlobalPosition.DistanceSquaredTo(GlobalPosition)) :
                null;

            if (Configuration.IsAlwaysFire || closestTarget != null)
            {
                if (Configuration.PrefireAction != null)
                {
                    await Configuration.PrefireAction();
                }
                CreateProjectiles(closestTarget?.GlobalPosition);
            }
        }
    }

    private void CreateProjectiles(Vector2? target = null)
    {
        var angleStep = Configuration.ProjectileCount == 1 ? 0 : Configuration.ProjectileGroupSpreadAngle / (Configuration.ProjectileCount - 1);
        var angleAddition = -Configuration.ProjectileGroupSpreadAngle / 2;

        Enumerable.Range(0, Configuration.ProjectileCount)
            .Select(_ => {
                var proj = CreateProjectile(target);
                proj.Rotation += angleAddition;
                angleAddition += angleStep;
                return proj;
            }).ToList()
            .ForEach(proj => _sceneService.GetCurrentStage().AddChild(proj));
    }

    private Projectile CreateProjectile(Vector2? target = null)
    {
        var proj = _projectileScene.Instantiate() as Projectile;
        proj.TargetType = Configuration.WeaponTarget;
        proj.Damage = Configuration.Damage;
        proj.GlobalPosition = GlobalPosition;
        proj.Speed = Configuration.ProjectileSpeed;
        proj.PierceCount = Configuration.PierceCount;
        proj.GetTypedChildByName<AnimatedSprite2D>(NodeName.Projectile.PrjSprite).Animation = Configuration.ProjectileType.ToString();
        if (target != null)
        {
            if (Configuration.IsAutoAim || Mathf.Abs(GlobalPosition.AngleTo(target.Value)) < Configuration.AngleOfSight * Configuration.AccuracyAngle)
            {
                proj.LookAt(target.Value);
            }

            if (proj.GlobalRotation != ClampAngleInConeOfSight(proj.GlobalRotation))
            {
                proj.GlobalRotation = GlobalRotation;
            }
        }
        else
        {
            proj.GlobalRotation = GlobalRotation;
        }

        return proj;
    }

    private Vector2[] CreateConeOfSight()
    {
        var cone = new Vector2[4];
        cone[0] = Vector2.Zero;
        cone[1] = Vector2.Right.Rotated(Configuration.AngleOfSight/2) * Configuration.LengthOfSight;
        cone[2] = Vector2.Right * Configuration.LengthOfSight;
        cone[3] = Vector2.Right.Rotated(-Configuration.AngleOfSight/2) * Configuration.LengthOfSight;

        return cone;
    }

    private float ClampAngleInConeOfSight(float angle) => Mathf.Clamp(
        angle,
        Mathf.Clamp(GlobalRotation - Configuration.AngleOfSight/2, -Mathf.Pi, Mathf.Pi),
        Mathf.Clamp(GlobalRotation + Configuration.AngleOfSight/2, -Mathf.Pi, Mathf.Pi));
}
