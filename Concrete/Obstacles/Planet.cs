using Godot;

public partial class Planet : Node2D, IConfigurable<PlanetConfiguration>
{
    public static string ScenePath = EditorConstants.BodiesScenePathBase + "Planet.tscn";
    public PlanetConfiguration Configuration { get; set; }

    public void Configure(Node parent)
    {
        AddToGroup(Groups.Obstacle.ToString());
        GlobalPosition = Configuration.GlobalPosition;

        var planetBody = this.GetTypedChildByName<StaticBody2D>(NodeName.Planet.PtBody);        
        planetBody.Scale *= Configuration.RadiusScale;

        var planetSprite = planetBody.GetTypedChildByName<AnimatedSprite2D>(NodeName.Planet.PtSprite);
        planetSprite.Animation = Configuration.PlanetTexture.ToString();
        planetSprite.SpeedScale = 0.25f;
        planetSprite.Play();

        var orbitArea = this.GetTypedChildByName<OrbitArea>();
        orbitArea.Gravity = Configuration.Gravity;
        orbitArea.Scale *= Configuration.GravityRadiusScale;
        
        parent.AddChild(this);
    }
}