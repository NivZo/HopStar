using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class SceneService : Node, ISceneService, IAutoLoad
{
    public bool IsEnabled { get; set; }
    private Stage _stage;
    private StageConfiguration _stageConfiguration;
    [OnReady]
    public CanvasLayer TransitionerLayer;
    [OnReady]
    public ColorRect ScreenTransitioner;
    [OnReady]
    public AnimationPlayer TransitionPlayer;

    public int Count = 1;

    public override void _Ready()
    {
        base._Ready();
        var game = this.GetGame();
        this.InitOnReady(overrideParent: game);
    }

    public void InitService()
    {
        IsEnabled = true;
        Count = 1;
    }

    public Stage GetCurrentStage() => _stage;
    
    public StageConfiguration GetCurrentStageConfiguration() => _stageConfiguration;

    public async void ToMainMenu(bool skipTransitions = false)
    {
        foreach (var autoload in GetTree().Root.GetChildren().Where(child => child is IGameAutoLoad).Select(child => child as IGameAutoLoad))
        {
            autoload.IsEnabled = false;
        }
        
        var mainMenuScreen = GD.Load<PackedScene>(MainMenu.ScenePath).Instantiate<MainMenu>();
        
        await TransitionToScreen(mainMenuScreen, !skipTransitions, !skipTransitions);
    }

    private void ToMainMenuDeferred()
    {
        GetTree().ChangeSceneToPacked(GD.Load<PackedScene>(MainMenu.ScenePath));
    }

    public async Task ActionWithTransition(Action action, bool transitionOut, bool transitionIn)
    {
        TransitionerLayer.Visible = true;
        if (transitionOut)
        {
            await TransitionOut();
        }
        action();
        if (transitionIn)
        {
            TransitionIn();
        }
        TransitionerLayer.Visible = false;
    }

    public async Task TransitionToScreen<T>(
        T screen,
        bool transitionOut = true,
        bool transitionIn = true)
        where T : Node, IScreen
    {
        await ActionWithTransition(() => SetCurrentScreen(screen), transitionOut, transitionIn);
    }

    public async Task SetCurrentStage(
        StageConfiguration stageConfiguration,
        bool transitionOut = true,
        bool transitionIn = true)
    {
        _stageConfiguration = stageConfiguration;
        _stage = GD.Load<PackedScene>(Stage.ScenePath).Instantiate<Stage>();
        _stage.Configuration = stageConfiguration;
        _stage.Configure(this.GetGame());
        
        await TransitionToScreen(_stage, transitionOut, transitionIn);

        GD.Print($"Starting level [{Count}]");
        Count++;
    }

    public async Task TransitionOut()
    {
        TransitionPlayer.Play("transitionout");
        await ToSignal(TransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
    }

    public void TransitionIn()
    {
        TransitionPlayer.Play("transitionin");
    }

    private void SetCurrentScreen<T>(T newScreen)
        where T : Node, IScreen
    {
        var game = this.GetGame();
        var currScreens = game.GetChildren().Where(child => child is IScreen);
        foreach (var screen in currScreens)
        {
            GD.Print($"Removing {screen.Name}");
            game.RemoveChild(screen);
            while (screen.IsValid())
            {
                screen.QueueFree();
            }
        }

        GD.Print($"Adding {newScreen.Name}");
        game.AddChild(newScreen);
    }
}