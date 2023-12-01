using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Combatant
{
    public static string ScenePath = EditorConstants.PlayerScenePathBase + "Player.tscn";
    public override CombatantConfiguration Configuration
    {
        get {
            var gs = this.GetAutoload<GameStateService>();
            var upgrades = gs.PlayerStatus.Upgrades;
            return new(
                MaxHealth: (int)Math.Ceiling(gs.PlayerStatus.BaseMaxHealth * upgrades.MaxHealthPr),
                Speed: 500f * upgrades.FlyingSpeedPr,
                StartingRotation: 0f,
                RotationSpeedPerDelta: 1f * upgrades.TurningSpeedPr,
                TargetableType: WeaponConstants.WeaponTarget.Player);
        }
    }

    protected override IController Controller
    {
        get => this.GetTypedChildByName<PlayerController>();
    }
    protected override List<WeaponConfiguration> WeaponConfigurations {
        get {
            var upgrades = this.GetAutoload<GameStateService>().PlayerStatus.Upgrades;
            return new() {
                new(IsPlayerWeapon: true,
                    LengthOfSight: 1000,
                    AngleOfSight: Mathf.Pi / 3 * upgrades.AimAccuracyPr,
                    AccuracyAngle: Mathf.Pi / 20 * upgrades.AimAccuracyPr,
                    ProjectileSpeed: 1200 * upgrades.BulletSpeedPr,
                    ProjectileReloadSeconds: 0.4f * (1 / upgrades.AttackSpeedPr),
                    PierceCount: 0 + upgrades.PierceCountAb,
                    Damage: (int)Math.Ceiling(4 * upgrades.AttackDamagePr),
                    ProjectileCount: 1 + upgrades.BulletCountAb,
                    ProjectileGroupSpreadAngle: upgrades.BulletCountAb * Mathf.Pi / 24,
                    IsAutoAim: false,
                    IsControllerAim: true,
                    IsAlwaysFire: true,
                    ProjectileType: WeaponConstants.ProjectileType.BlueLaser,
                    WeaponTarget: WeaponConstants.WeaponTarget.Enemy),
            };
        }
    }

    private IGUIService _guiService;
    private GameStateService _gameState;
    private Camera2D _camera;
    private Timer _invulnerableTimer;

    public override void _Ready()
    {
        base._Ready();

        _gameState = this.GetAutoload<GameStateService>();
        _guiService = this.GetAutoload<GUIService>();

        _camera = this.GetTypedChildByName<Camera2D>(NodeName.Player.ControlledCamera);

        
        CurrentHealth = _gameState.PlayerStatus.CurrentHealth
            + _gameState.PlayerStatus.BaseMaxHealth * (int)(_gameState.PlayerStatus.Upgrades.MaxHealthPr - 1);
        _guiService.UpdateBars();

        SetupInvulnerabilityTimer();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Body.GlobalPosition = CameraUtils.CameraClamped(_camera, Body.GlobalPosition);
    }

    public override void _Draw()
    {
        base._Draw();
        if (IsInvulnerable())
        {
            Sprite.SelfModulate = new(Sprite.SelfModulate)
            {
                A = 0.5f,
            };
            (Sprite.Material as ShaderMaterial).SetShaderParameter("masked_alpha", 0.5f);
        }
        else
        {
            Sprite.SelfModulate = new(Sprite.SelfModulate)
            {
                A = 1f,
            };
            (Sprite.Material as ShaderMaterial).SetShaderParameter("masked_alpha", 1f);
        }
    }

    public override void HandleBodyDamage(int damage)
    {
        if (!IsInvulnerable())
        {
            base.HandleBodyDamage(damage);

            _gameState.PlayerStatus.CurrentHealth -= damage;
            _guiService.UpdateBars();

            _invulnerableTimer.Start();
            
            QueueRedraw();
        }
    }

    private void SetupInvulnerabilityTimer()
    {
        _invulnerableTimer = new()
        {
            OneShot = true,
            WaitTime = 1,
        };
        _invulnerableTimer.Connect("timeout", new(this, MethodName._Draw));
        AddChild(_invulnerableTimer);
    }

    private bool IsInvulnerable() => _invulnerableTimer.TimeLeft > 0;

    public override void Destroy()
    {
        this.GetAutoload<SceneService>().ToMainMenu();
    }
}
