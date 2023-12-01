using Godot;

public partial class PlayerWeapon : Weapon
{
    public static string ScenePath = EditorConstants.WeaponsScenePathBase + "PlayerWeapon.tscn";

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Configuration.IsControllerAim)
        {
            GlobalRotation = this.GetPlayerController().GetRotation();
        }
        
        if (_timer?.TimeLeft == 0)
        {
            Shoot();
            _timer.Start();
        }
    }
}