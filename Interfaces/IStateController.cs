using Godot;

public interface IStateController : IController
{
    public EnemyConstants.EnemyState State { get; set; }
}