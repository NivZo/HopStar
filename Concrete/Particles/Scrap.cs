using System;
using Godot;

public partial class Scrap : AnimatedSprite2D
{
    public static string ScenePath = EditorConstants.ParticlesScenePathBase + "Scrap.tscn";

    private CombatantBody _playerBody;
    private float _pickupRange = 250000; // 500 ** 2
    private (int,int) _valueRange = (2,3);

    public override void _Ready()
    {
        base._Ready();
        
        _playerBody = this.GetPlayerShipBody();

        var animationPlayer = this.GetChildByType<AnimationPlayer>();
        animationPlayer.GetAnimation("float").LoopMode = Godot.Animation.LoopModeEnum.Linear;
        animationPlayer.Play("float");

        var upgrades = this.GetAutoload<GameStateService>().PlayerStatus.Upgrades;
        _pickupRange = _pickupRange * upgrades.PickupRangePr;
        _valueRange = ((int)(_valueRange.Item1 * upgrades.ScrapValuePr), (int)(_valueRange.Item2 * upgrades.ScrapValuePr));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var distance = _playerBody.GlobalPosition.DistanceSquaredTo(GlobalPosition);

        if (distance < 250000) // 500 ** 2
        {
            GlobalPosition = GlobalPosition.Lerp(_playerBody.GlobalPosition, (float)delta * (90000 / distance));
            
            if (distance < 2500) // 50 ** 2
            {
                if (this.IsValid())
                {
                    this.GetAutoload<GameStateService>().PlayerStatus.Coins += new Random().Next(2, 3);
                    this.GetAutoload<GUIService>().UpdateBars();
                }
                QueueFree();
            }
        }
    }
}
