using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RayPathfinder : Node2D
{
    private IList<RayCast2D> _rays;
    private RayCast2D _rayToPlayer;
    private Node2D _playerBody;
    private int _rayCount = 16;
    

    public override void _Ready()
    {
        base._Ready();

        _playerBody = this.GetPlayerShipBody();
        _rays = Enumerable.Range(0, _rayCount)
            .Select(i => {
                return new RayCast2D()
                {
                    TargetPosition = Vector2.Right.Rotated(Mathf.Tau / _rayCount * i).Normalized() * 500,
                    CollideWithBodies = true,
                    CollideWithAreas = false,
                    ExcludeParent = true,
                    Enabled = true,
                };
            })
            .ToList();
        
        foreach (var ray in _rays)
        {
            AddChild(ray);
        }


    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        GlobalRotation = 0;
    }

    public bool CanLookAtPlayer() => GetNonCollidingRays()
        .Where(ray => GetRayDeltaFromPlayer(ray) < (Mathf.Tau / _rayCount))
        .Count() > 0;
    
    public Vector2 GetNextDirection()
    {
        var valid = GetNonCollidingRays();
        if (valid.Count() > 0)
        {
            return valid.MinBy(GetRayDeltaFromPlayer).TargetPosition.Normalized();
        }
        return Vector2.Zero;
    }

    private float GetRayDeltaFromPlayer(RayCast2D ray)
    {
        var toPlayer = GlobalPosition.DirectionTo(_playerBody.GlobalPosition).Normalized();
        return Mathf.Abs(toPlayer.DistanceSquaredTo(ray.TargetPosition.Normalized()));
    }

    private IEnumerable<RayCast2D> GetNonCollidingRays() => _rays.Where(ray => !IsCollision(ray.GetCollider() as Node2D));

    private bool IsCollision(Node2D collider) =>
        collider != null &&
        collider.IsInGroup(Groups.Obstacle.ToString());
}
