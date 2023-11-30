using Godot;

public partial class EnemyStateMachine : Node2D
{
    public EnemyConstants.EnemyState State;

    private Node2D _enemy;
    private RayPathfinder _enemyPathfinder;
    private Node2D _player;
    private float _maxChaseDistance = 4000;
    private float _minChaseDistance = 700;

    public EnemyStateMachine(Node2D enemy)
    {
        State = EnemyConstants.EnemyState.Idle;

        _enemy = enemy;
        _enemyPathfinder = _enemy.GetTypedChildByName<RayPathfinder>();
        _player = enemy.GetPlayerShipBody();
        _enemy.AddChild(this);

        var timer = new Timer()
        {
            OneShot = false,
            WaitTime = 0.5,
        };
        AddChild(timer);
        timer.Connect("timeout", new(this, MethodName.CalculateNextState));
        timer.Start();
    }

    public void CalculateNextState()
    {
        var distance = GetDistanceFromPlayer();

        if (distance > _maxChaseDistance)
        {
            State = EnemyConstants.EnemyState.Idle;
            return;
        }

        if (distance > _minChaseDistance && distance < _maxChaseDistance)
        {
            State = EnemyConstants.EnemyState.Chase;
            return;
        }

        if (!_enemyPathfinder.CanLookAtPlayer())
        {
            State = EnemyConstants.EnemyState.Chase;
            return;
        }

        State = EnemyConstants.EnemyState.Attack;
    }

    public Vector2 GetStateVelocity()
    {
        switch (State)
        {
            case EnemyConstants.EnemyState.Chase:
                return GetNextDirection();
            case EnemyConstants.EnemyState.Idle:
            case EnemyConstants.EnemyState.Attack:
            default:
                return Vector2.Zero;
        }
    }

    public float GetStateRotation()
    {
        switch (State)
        {
            case EnemyConstants.EnemyState.Chase:
                return GetNextDirection().Angle();
            case EnemyConstants.EnemyState.Attack:
                return GetDirectionToPlayer().Angle();
            case EnemyConstants.EnemyState.Idle:
            default:
                return _enemy.GlobalRotation;
        }
    }

    private float GetDistanceFromPlayer() => Mathf.Abs(_enemy.GlobalPosition.DistanceTo(_player.GlobalPosition));

    private Vector2 GetDirectionToPlayer()
    {
        return _enemy.GlobalPosition.DirectionTo(_player.GlobalPosition);
    }

    private Vector2 GetNextDirection()
    {
        var direction = _enemyPathfinder.GetNextDirection();
        var length = direction.Length();
        if (length > _minChaseDistance)
        {
            return direction;
        }

        return direction * 0.8f;
    }
}