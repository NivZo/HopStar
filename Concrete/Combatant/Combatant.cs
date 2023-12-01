using Godot;
using System.Collections.Generic;

public abstract partial class Combatant : Node2D, IConfigurable<CombatantConfiguration>
{
    public virtual CombatantConfiguration Configuration { get; set; }
    
    protected virtual IController Controller { get; set; }
    protected virtual List<WeaponConfiguration> WeaponConfigurations { get; set; }
    protected CombatantBody Body;
    protected CollisionPolygon2D Collision;
    protected Sprite2D Sprite;
    protected AnimationPlayer AnimationPlayer;
    protected int CurrentHealth;
    


    public override void _Ready()
    {
        base._Ready();

        Body = this.GetTypedChildByName<CombatantBody>();
        Body.TargetableType = Configuration.TargetableType;
        Body.Controller = Controller;
        Body.Speed = Configuration.Speed;
        Body.RotationSpeedPerDelta = Configuration.RotationSpeedPerDelta;
        AnimationPlayer = Body.GetTypedChildByName<AnimationPlayer>(NodeName.Combatant.CombatantAnimationPlayer);
        Collision = Body.GetTypedChildByName<CollisionPolygon2D>(NodeName.Combatant.CombatantCollPolygon);
        Sprite = Body.GetTypedChildByName<Sprite2D>(NodeName.Combatant.CombatantSprite);
        Sprite.Material = (Sprite.Material as ShaderMaterial).Duplicate() as Material;
        
        CurrentHealth = Configuration.MaxHealth;

        foreach(var weaponConfig in WeaponConfigurations)
        {
            var weaponScene = weaponConfig.IsPlayerWeapon ?
                GD.Load<PackedScene>(PlayerWeapon.ScenePath) :
                GD.Load<PackedScene>(EnemyWeapon.ScenePath);
            Weapon weapon = weaponConfig.IsPlayerWeapon ?
                weaponScene.Instantiate<PlayerWeapon>() :
                weaponScene.Instantiate<EnemyWeapon>();
            weapon.Configuration = weaponConfig;
            weapon.Configure(Body);
        }
    }

    public virtual void Configure(Node parent)
    {
        parent.AddChild(this);
    }

    public virtual void HandleBodyCollision(Node2D body)
    {
        HandleBodyDamage(1);
    }

    public virtual void HandleBodyDamage(int damage)
    {
        AnimationPlayer.Play("hitflash");
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        Body.Controller = null;
        Body.LinearVelocity = Vector2.Zero;
        
        foreach (var weapon in this.GetChildrenByName(nameof(NodeName.Weapon)))
        {
            weapon.QueueFree();
            weapon.Dispose();
        }
        
        this.AddExplosion(
            ParticleConstants.ExplosionType.Destruction,
            overridePosition: Body.GlobalPosition,
            scale: 2f);
        
        Body.AddScrap();
        
        QueueFree();
        Dispose();
    }
}