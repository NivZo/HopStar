using Godot;

public partial class OrbitArea : Area2D
{
    public override void _Ready()
    {
        GravitySpaceOverride = SpaceOverride.Combine;
        GravityPoint = true;
        GravityPointCenter = Position;
        GravityDirection = Position;
    }

    public override void _Draw()
    {
        base._Draw();
        
        var collShape = this.GetTypedChildByName<CollisionShape2D>(NodeName.Orbit.OrbCollShape);
        var fillColor =  new Color(1, 1, 1, 0.0125f);
        var outlineColor =  new Color(2, 2, 2, 0.25f);
        DrawCircle(collShape.Position, (collShape.Shape as CircleShape2D).Radius, fillColor);
        DrawArc(collShape.Position, (collShape.Shape as CircleShape2D).Radius, 0, Mathf.Tau, 90, outlineColor);
    }
}
