using System;
using Godot;

public partial class CombatantBody : RigidBody2D, ICombatant
{
    public enum Strafe
    {
        None,
        Left,
        Right,
    }

    public float Speed = 400f;
    public float StartingRotation { get; set; } = 0;
    public float RotationSpeedPerDelta { get; set; } = (float)(Math.PI / 8);
    public WeaponConstants.WeaponTarget TargetableType { get; set; }
	public IController Controller = null;

    protected Strafe CurrentStrafe = Strafe.None;
    
    private GpuParticles2D _engineParticleEmitter;
    private Combatant _parent;
    private Combatant Parent { get => _parent.IsValid() ? _parent : null; }
    
    [Export]
    public bool IsLocked;

	public override void _Ready()
	{
		base._Ready();

        _engineParticleEmitter = this.GetTypedChildByName<GpuParticles2D>(NodeName.Combatant.CombatantEngineParticleEmitter);
        _parent = GetParent<Combatant>();

		Mass = 100;
	}

    public override void _PhysicsProcess(double delta)
	{
		base._Process(delta);

		if (Controller != null)
		{
			var v = Controller.GetVelocity();
			
			if (Controller.DidStart)
			{
                if (v != Vector2.Zero && !IsLocked)
                {
                    LinearVelocity = LinearVelocity.Lerp(v * Speed, (float)delta);
                    _engineParticleEmitter.Emitting = true;
                }
                else 
                {
                    LinearVelocity = LinearVelocity.Lerp(v, (float)delta);
                    _engineParticleEmitter.Emitting = false;
                }

                var contRotation = Controller.GetRotation();
				var rotationDiff = contRotation - Rotation;
                var rotationAmplifier = (v == Vector2.Zero ? 3 : 2) * (1 + Math.Abs(rotationDiff / Math.Tau));
                var rotationWeight = (float)(delta * RotationSpeedPerDelta * rotationAmplifier) * (IsLocked ? 0 : 1);
                Rotation = Mathf.LerpAngle(Rotation, contRotation, rotationWeight);
                if (rotationDiff > 0.5)
                {
                    CurrentStrafe = Strafe.Right;
                }
                else if (rotationDiff < -0.5)
                {
                    CurrentStrafe = Strafe.Left;
                }
                else
                {
                    CurrentStrafe = Strafe.None;
                }
			}
			else
			{
				Rotation = StartingRotation;
			}

            foreach (var body in GetCollidingBodies())
            {
                OnCollision(body);
            }
		}
	}

    private void OnCollision(Node2D body) => Parent?.HandleBodyCollision(body);

    public void TakeDamage(int damage) => Parent?.HandleBodyDamage(damage);

    public void LockBody() => IsLocked = true;

    public void UnlockBody() =>IsLocked = false;
}
