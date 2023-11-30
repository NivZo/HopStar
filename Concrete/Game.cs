using Godot;

public partial class Game : Node2D
{
    public static string ScenePath = EditorConstants.ScreensScenePathBase + "Game.tscn";
    public Stage CurrentStage;
    
    [OnReady(IsAutoload = true)]
    public ICombatService CombatService;
    
    [OnReady(IsAutoload = true)]
    public ISceneService SceneService;
    
    [OnReady(IsAutoload = true)]
    public IGUIService GUIService;
    
    private StageFactory _stageFactory;

    public override void _Ready()
    {
        base._Ready();
        this.InitOnReady();
        SceneService.InitService();
        _stageFactory = new StageFactory();

        SceneService.ToMainMenu(skipTransitions: true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public async void RandomStage(int? levelOverride = null)
    {
        var stageConfiguration = _stageFactory.GeneratePlanetStage(levelOverride ?? (SceneService as SceneService).Count);
        await SceneService.SetCurrentStage(stageConfiguration);
        GetTree().Paused = false;
        InitGameAutoloads();
    }

    private void ResetGameDeferred()
    {
        this.GetAutoload<GameStateService>().ResetGameState();
        RandomStage(1);
    }

    public void ResetGame()
    {
        (SceneService as SceneService).Count = 1;
        new Callable(this, MethodName.ResetGameDeferred).CallDeferred();
    }

    private void InitGameAutoloads()
    {
        GUIService.InitService();
        CombatService.InitService();
    }
}
