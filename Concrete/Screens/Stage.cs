using System;
using Godot;

public partial class Stage : Node, IConfigurable<StageConfiguration>, IScreen
{
    public static string ScenePath = EditorConstants.ScreensScenePathBase + "Stage.tscn";
    public StageConfiguration Configuration { get; set; }

    private void InitPlanets()
    {
        var portalScene = GD.Load<PackedScene>(Planet.ScenePath);
        foreach (var config in Configuration.PlanetConfigurations)
        {
            var planet = portalScene.Instantiate<Planet>();
            planet.Configuration = config;
            planet.Configure(this);
        }
    }

    public void InitPortals()
    {
        var portalScene = GD.Load<PackedScene>(Portal.ScenePath);
        foreach (var config in Configuration.PortalConfigurations)
        {
            var portal = portalScene.Instantiate<Portal>();
            portal.Configuration = config;
            portal.Configure(this);
            InitIndicator(portal);
        }
    }

    private void InitBackground()
    {
        // var backgroundNode = @Game.GetChildByName("SpaceBg") as TextureRect;
        // backgroundNode.Size = Size;
    }

    private void InitPlayerShip()
    {
        var player = GD.Load<PackedScene>(Player.ScenePath).Instantiate<Player>();

        switch (Configuration.StartPosition)
        {
            case StageConstants.StartPosition.Bottom:
                player.GlobalPosition = new(Configuration.Size.X / 2, Configuration.Size.Y * 0.9f);
                player.GetTypedChildByName<CombatantBody>().StartingRotation = -(float)(Math.PI / 2);
                break;
            case StageConstants.StartPosition.Right:
                player.GlobalPosition = new(Configuration.Size.X * 0.9f, Configuration.Size.Y / 2);
                player.GetTypedChildByName<CombatantBody>().StartingRotation = (float)(Math.PI / 2);
                break;
            case StageConstants.StartPosition.Left:
                player.GlobalPosition = new(Configuration.Size.X * 0.1f, Configuration.Size.Y / 2);
                break;
        }

        var camera = player.GetTypedChildByName<Camera2D>(NodeName.Player.ControlledCamera);
        camera.LimitBottom = (int)Configuration.Size.Y;
        camera.LimitTop = 0;
        camera.LimitRight = (int)Configuration.Size.X;
        camera.LimitLeft = 0;

        player.Configure(this);
    }

    private void InitIndicator(Node2D target)
    {
        var indicator = GD.Load<PackedScene>(Indicator.ScenePath).Instantiate<Indicator>();
        indicator.Target = target;
        AddChild(indicator);
    }

    private void InitEnemies()
    {

        foreach (var enemyConfig in Configuration.EnemyConfigurations)
        {
            var positionStep = (Configuration.Size.X + 200) / (enemyConfig.AmountOfInstances-1);
            var positionAddition = -(Configuration.Size.X + 200) / 2;
            for (int i = 0; i < enemyConfig.AmountOfInstances; i++)
            {
                var enemyNode = GD.Load<PackedScene>(LaserBeetle.ScenePath).Instantiate<LaserBeetle>();
                enemyNode.Configuration = new CombatantConfiguration(
                    MaxHealth:3,
                    Speed:500f,
                    StartingRotation:0,
                    RotationSpeedPerDelta:Mathf.Pi/8,
                    TargetableType: WeaponConstants.WeaponTarget.Enemy);

                enemyNode.GlobalPosition = enemyConfig.GlobalPosition + new Vector2(positionAddition, 0);
                positionAddition += positionStep;
                enemyNode.Configure(this);
                InitIndicator(enemyNode.GetTypedChildByName<CombatantBody>());
            }
        }
    }

    public void Configure(Node parent)
    {
        InitBackground();
        InitPlayerShip();
        InitEnemies();
        InitPlanets();
    }
}