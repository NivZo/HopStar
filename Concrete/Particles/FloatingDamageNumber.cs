using Godot;
using System;

public partial class FloatingDamageNumber : Node2D
{
    public static string ScenePath = EditorConstants.ParticlesScenePathBase +  "FloatingDamageNumber.tscn";

    private const string animationName = "damagenumber";
    private AnimationPlayer _animationPlayer;

    public async override void _Ready()
    {
        base._Ready();
        _animationPlayer = this.GetChildByType<AnimationPlayer>();
        _animationPlayer.Play(animationName);
        
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        Destroy();
    }

    private void Destroy()
    {
        if (this.IsValid())
            QueueFree();
    }
}
