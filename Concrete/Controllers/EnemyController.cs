using System;
using Godot;

public partial class EnemyController : IStateController
{
    public bool DidStart { get; set; } = true;

    private EnemyStateMachine _stateMachine;

    public EnemyController(Node2D controlled)
    {
        _stateMachine = new(controlled);
    }

    public Vector2 GetVelocity() => _stateMachine.GetStateVelocity();

    public float GetRotation() => _stateMachine.GetStateRotation();
}
