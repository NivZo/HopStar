using Godot;

public partial class Explosion : AnimatedSprite2D
{
    public static string ScenePath = EditorConstants.ParticlesScenePathBase + "Explosion.tscn";
    public override void _Ready()
    {
        base._Ready();
        TopLevel = true;
        Connect(AnimatedSprite2D.SignalName.AnimationFinished, new(this, MethodName.QueueFree));
        Play();
    }
}
