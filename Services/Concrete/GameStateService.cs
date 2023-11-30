using Godot;

public partial class GameStateService : Node, IGameAutoLoad
{
    public bool IsEnabled { get; set; }
    public PlayerStatus PlayerStatus { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ResetGameState();
    }

    public void ResetGameState()
    {
        PlayerStatus = new()
        {
            BaseMaxHealth = 10,
            CurrentHealth = 10,
            Coins = 0,
            Upgrades = new()
        };
    }
}
