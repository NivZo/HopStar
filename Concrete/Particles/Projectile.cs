using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Projectile : Area2D
{
    public static string ScenePath = EditorConstants.ParticlesScenePathBase + "Projectile.tscn";
    public int Damage = 1;
    public int PierceCount = 0;
    public float Speed = 1500;
    public WeaponConstants.WeaponTarget TargetType;
    private float _timeToLiveSeconds = 2;
    private List<StringName> _history;

    public override async void _Ready()
    {
        base._Ready();
        await ToSignal(GetTree().CreateTimer(_timeToLiveSeconds), SceneTreeTimer.SignalName.Timeout);
        Destroy();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition += GlobalTransform.X.Mult((float)delta * Speed);
        Scale = new(
            Math.Min(3, Scale.X * (float)(1 + 2*delta)),
            Math.Min(1.5f, Scale.Y * (float)(1 + delta)));
    }

    public void _OnBulletBodyEntered(Node2D body)
    {
        if (this.IsValid() &&
            body.IsValid() &&
            body is ITargetable targetable &&
            targetable.TargetableType == TargetType)
        {
            if (body is IDamageable damageable)
            {
                body.AddExplosion(
                    ParticleConstants.ExplosionType.Impact,
                    overridePosition: GlobalPosition,
                    overrideRotation: GlobalRotation,
                    scale: 2);
                
                if (targetable.TargetableType == WeaponConstants.WeaponTarget.Enemy)
                {
                    CreateFloatingDamageNumber();
                }

                if (PierceCount > 0)
                {
                    if (_history == null)
                    {
                        _history = new();
                    }
                    if (!_history?.Contains(body.Name) ?? true)
                    {
                        _history.Add(body.Name);
                        PierceCount -= 1;
                        damageable.TakeDamage(Damage);
                    }
                    
                }
                else
                {
                    damageable.TakeDamage(Damage);
                    Destroy();
                }
                
            }
        }
    }

    private void CreateFloatingDamageNumber()
    {
        var floatingDamageNumber = GD.Load<PackedScene>(FloatingDamageNumber.ScenePath).Instantiate<FloatingDamageNumber>();
        floatingDamageNumber.GetChildByType<Label>().Text = Damage.ToString();
        floatingDamageNumber.GlobalPosition = GlobalPosition;
        AddSibling(floatingDamageNumber);
    }

    private void Destroy()
    {
        if (this.IsValid())
            QueueFree();
    }
}
