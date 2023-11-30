using System.Linq;
using Godot;

public partial class GUIService : Node, IGUIService, IGameAutoLoad
{
    public bool IsEnabled { get; set; }
    private GameStateService _gameState = null;
    private Control _playerBars = null;

    public override void _Ready()
    {
        base._Ready();

        UpdateBars();
    }

    public void InitService()
    {
        _gameState = this.GetAutoload<GameStateService>();
        IsEnabled = true;
        UpdateBars();
    }


    public void UpdateBars()
    {
        if (IsEnabled)
        {
            if (_playerBars == null || !_playerBars.IsValid())
            {
                _playerBars = this.GetGame().GetTypedChildByName<Control>(NodeName.Player.PlayerBars);
            }

            var healthBar = _playerBars.GetTypedChildByName<Sprite2D>(NodeName.Player.HealthBar);
            (healthBar.Material as ShaderMaterial).SetShaderParameter(
                "percentage",
                _gameState.PlayerStatus.CurrentHealth / (_gameState.PlayerStatus.BaseMaxHealth * _gameState.PlayerStatus.Upgrades.MaxHealthPr));
            
            var scrapCount = _playerBars.GetTypedChildByName<Label>(NodeName.Player.ScrapCount);
            scrapCount.Text = _gameState.PlayerStatus.Coins.ToString();
            
            _playerBars.QueueRedraw();
        }
    }

    public void CreateThreeChoiceModal()
    {
        if (IsEnabled)
        {
            var modal = GD.Load<PackedScene>(ThreeChoiceModal.ScenePath).Instantiate<ThreeChoiceModal>();
            modal.GlobalPosition = CameraUtils.GetCenter(this.GetGame().GetTypedChildByName<ControlledCamera>());
            modal.GlobalPosition = new()
            {
                X = modal.GlobalPosition.X,
                Y = modal.GlobalPosition.Y - 100,
            };
            this.GetGame().AddChild(modal);
            GetTree().Paused = true;
        }
    }
}