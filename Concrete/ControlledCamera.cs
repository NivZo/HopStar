using Godot;

public partial class ControlledCamera : Camera2D
{
    private IController _controller = null;
    private CombatantBody _ship = null;
    private float _maxDistance = 150;

    public override void _Ready()
    {
        base._Ready();
        var game = this.GetGame();
        _controller = game.GetTypedChildByName<PlayerController>();
        _ship = this.GetPlayerShipBody();
        Enabled = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        var shipCurrSpeed = _ship.LinearVelocity.Length();
        var speedRatio = shipCurrSpeed / _ship.Speed;
        var isBraking = _controller.GetVelocity() == Vector2.Zero;
        if (speedRatio > 0.05 && !isBraking)
        {
            if (Position.Length() < _maxDistance)
            {
                Position = Position.Lerp(
                    Position + new Vector2(0, -100).Mult(speedRatio),
                    (float)delta);
            }
        }
        else
        {
            Position = Position.Lerp(Vector2.Zero, (float)delta * 2);
        }
    }
}
