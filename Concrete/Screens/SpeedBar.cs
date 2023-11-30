using Godot;

public partial class SpeedBar : Sprite2D
{
    [OnReady]
    public PlayerController PlayerController;

    private float _stepPercent;
    private Timer _depletionTimer;

    public override void _Ready()
    {
        base._Ready();
        this.InitOnReady(overrideParent: this.GetGame());
        this.SetShaderParameter("percentage", 0);
        
        _stepPercent = 0.199f;
        _depletionTimer = new Timer()
        {
            WaitTime = 0.1f,
            OneShot = false,
        };
        AddChild(_depletionTimer);
        _depletionTimer.Connect("timeout", new(this, MethodName.DepleteStep));

    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var percentage = PlayerController.GetVelocityPercentage();
        if (percentage > 0)
        {
            _depletionTimer.Stop();
            percentage = Mathf.Lerp(_stepPercent, 1, percentage);
            this.SetShaderParameter("percentage", percentage - (percentage % _stepPercent));
        }
        else if (_depletionTimer.TimeLeft == 0
            && (float)this.GetShaderParameter("percentage") > 0)
        {
            _depletionTimer.Start();
        }
        // if (percentage > 0 && percentage < _stepPercent)
        // {
        //     this.SetShaderParameter("percentage", _stepPercent);
        // }
        // else
        // {
        //     this.SetShaderParameter("percentage", percentage - (percentage % _stepPercent));
        // }
    }

    private void DepleteStep()
    {
        var current = (float)this.GetShaderParameter("percentage");
        if (current > 0)
        {
            this.SetShaderParameter("percentage", current - _stepPercent);
        }
        else
        {
            _depletionTimer.Stop();
        }
    }
}
