using Godot;

public partial class Portal : Node2D, IConfigurable<PortalConfiguration>
{
    public static string ScenePath = EditorConstants.BodiesScenePathBase + "Portal.tscn";
    public PortalConfiguration Configuration { get; set; }

    private Area2D _area;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var bodies = _area?.GetOverlappingBodies();

        if (bodies != null && 
            bodies.Count == 1 && 
            bodies[0].GetParent() is Player)
        {
            this.GetAutoload<GUIService>().CreateThreeChoiceModal();
        }
    }

    public void Configure(Node parent)
    {
        AddToGroup(Groups.LargeBodies.ToString());
        GlobalPosition = Configuration.GlobalPosition;
        
        _area = this.GetTypedChildByName<Area2D>(NodeName.Portal.PtlArea);
        _area.Scale *= Configuration.RadiusScale;

        var planetSprite = this.GetTypedChildByName<AnimatedSprite2D>(NodeName.Portal.PtlSprite);
        planetSprite.Scale *= Configuration.RadiusScale;
        planetSprite.Animation = Configuration.PortalTexture.ToString();
        planetSprite.SpeedScale = 0.25f;
        planetSprite.Play();

        var orbitArea = this.GetTypedChildByName<OrbitArea>();
        orbitArea.Gravity = Configuration.Gravity;
        orbitArea.Scale *= Configuration.GravityRadiusScale;
        
        parent.AddChild(this);
    }
}