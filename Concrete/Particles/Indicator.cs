using System;
using System.Linq;
using Godot;

public partial class Indicator : AnimatedSprite2D
{
    public static string ScenePath = EditorConstants.GUIScenePathBase + "Indicator.tscn";
    public Node2D Target;
    private Label _label;
    private ControlledCamera _camera;
    private float _maxDistance = 4000;
    
    public override void _Ready()
    {
        base._Ready();
        _label = this.GetTypedChildByName<Label>(NodeName.Indicator.DistanceLabel);
        _camera = GetNode("/root/Game").GetTypedChildByName<ControlledCamera>();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Target != null && Target.IsValid())
        {
            if (!CameraUtils.IsTargetInScreen(_camera, Target))
            {
                var center = CameraUtils.GetCenter(_camera);
                Rotation = center.DirectionTo(Target.GlobalPosition).Angle() + Mathf.Pi/2;
                GlobalPosition = CameraUtils.CameraClamped(_camera, Target.GlobalPosition);

                var distance = Math.Abs((int)center.DistanceTo(Target.GlobalPosition));
                _label.Text = distance < 1000 ? distance.ToString() : $"{Math.Round(distance/1000f, 1)}K";
                _label.Rotation = -Rotation;

                Modulate = new(Modulate)
                {
                    A = Mathf.Min(1, _maxDistance / 2 / distance),
                };
                _label.SelfModulate = new(_label.SelfModulate)
                {
                    A = distance > _maxDistance ? 0 : 1,
                };

                Visible = true;
            }
            else
            {
                Visible = false;
            }
        }
        else
        {
            QueueFree();
        }
    }
}
