using Godot;
using System;

public partial class MainMenu : Node2D, IScreen
{
    public static string ScenePath = EditorConstants.ScreensScenePathBase + "MainMenu.tscn";

    public enum MainMenuButton
    {
        Play,
        Shop,
        Exit
    }

    [OnReady]
    public TouchScreenButton PlayButton;
    
    [OnReady(IsAutoload = true)]
    public ISceneService SceneService;

    public override void _Ready()
    {
        base._Ready();
        this.InitOnReady<MainMenu>();
    }

    public void OnButtonRelease(string buttonType)
    {
        switch ((MainMenuButton)Enum.Parse(typeof(MainMenuButton), buttonType, true))
        {
            case MainMenuButton.Play:
                this.GetGame().ResetGame();
                break;
            case MainMenuButton.Shop:
                break;
            case MainMenuButton.Exit:
                GetTree().Quit();
                break;
            default:
                break;
        }
    }
}
