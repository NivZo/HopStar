using Godot;
using System;

public partial class PlayerController : Node2D, IController
{
    public bool DidStart { get; set; } = false;
    public bool IsActive = false;

    private CollisionShape2D @area;
    private Node2D @zone;
    private Node2D @pos;
    private float _radius;
    private Vector2 _scale = Vector2.One;
    private float _deadzoneRatio = 0.4f;

    public override void _Ready()
    {
        base._Ready();
        @area = this.GetChildByName("ControllerArea") as CollisionShape2D;
        @zone = this.GetChildByName("ControllerZone") as Sprite2D;
        @pos = @zone.GetChildByName("ControllerPosition") as Sprite2D;
        _radius = (@area.Shape as CircleShape2D).Radius;
        _scale = @zone.Transform.Scale;

        Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        var isDrag = IsActive && (@event is InputEventScreenDrag || @event is InputEventMouseMotion);
        var isRelease = !isDrag && @event.IsReleased();
        var isPress = !isDrag && @event.IsPressed();
        var isFirstPress = !IsActive && isPress && !GetTree().Paused;

        if (isFirstPress)
        {
            IsActive = true;
            Visible = true;
            @zone.Position = GetViewport().GetMousePosition();
            @pos.Position = Vector2.Zero;
        }

        else if (isDrag)
        {
            var mousePos = GetGlobalMousePosition();
            @pos.Position = MathUtils.ClampCircle(mousePos - @zone.GlobalPosition, _radius) / _scale;
            var extra = (mousePos - @zone.GlobalPosition).Length() - _radius;
            if (extra > 0)//(_radius * _scale.Length() / 5))
            {
                @zone.Position = @zone.Position.Lerp(@zone.Position + @pos.Position.Normalized() * extra, 1);
            }
            
            if (!DidStart && GetDeadzonedPosition() != Vector2.Zero)
            {
                DidStart = true;
            }
        }

        else if (isRelease)
        {
            IsActive = false;
            Visible = false;
        }
    }
    
    public Vector2 GetVelocity() => IsActive ? GetDeadzonedPosition().Normalized().Mult((@pos.Position * _scale).Length() / _radius) : Vector2.Zero;

    public Vector2 GetRawVelocity() => IsActive ? 
        @pos.Position.Normalized().Mult(@pos.Position.Length() / _radius) * _scale : 
        Vector2.Zero;
    
    public float GetVelocityPercentage() => IsActive ? (GetVelocity().Length() - _deadzoneRatio) / 0.6f : 0;

    public float GetRotation() => @pos.Position.Angle();

    private Vector2 GetDeadzonedPosition()
    {
        if (((@pos.Position * _scale).Length() / _radius) > _deadzoneRatio)
        {
            return @pos.Position * _scale;
        }

        return Vector2.Zero;
    }
}
