using Godot;

public partial class PlayerCrosshair : AnimatedSprite2D
{
    private float _maxDistance = 500f;
    private AnimatedSprite2D _nose;
    private PlayerController _controller;
    private Camera2D _camera;
    private CombatantBody _ship;
    public override void _Ready()
    {
        base._Ready();
        var game = this.GetGame();
        _nose = this.GetChildByType<AnimatedSprite2D>();
        _ship = this.GetPlayerShipBody();
        _controller = game.GetTypedChildByName<PlayerController>();
        _camera = game.GetChildByType<Camera2D>();
    }

    public override void _Process(double delta)
    {
        if (_controller.IsActive)
        {
            base._Process(delta);

            Visible = true;
            
            var shipRotation = _ship.Rotation;
            var contRotation = _controller.GetRotation();
            Rotation = contRotation - shipRotation;

            var rawVelocity = _controller.GetRawVelocity();
            GlobalPosition = CameraUtils.CameraClamped(_camera, _ship.GlobalPosition + rawVelocity.Mult(_maxDistance));
            
            _nose.GlobalPosition = _ship.GlobalPosition
                + Vector2.Right.Rotated(_ship.GlobalRotation) * (_ship.GlobalPosition - GlobalPosition).Length();
            
            QueueRedraw();
        }
        else
        {
            Visible = false;
        }
    }

    public override void _Draw()
    {
        base._Draw();

        DrawLine(GlobalPosition, _nose.GlobalPosition, new(1,1,1,1));
    }
}
