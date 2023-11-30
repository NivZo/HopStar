using System;
using Godot;

public partial class ThreeChoiceModal : Control
{
	public static string ScenePath = EditorConstants.GUIScenePathBase + "ThreeChoiceModal.tscn";
	private bool _isActive = true;
	private UpgradeConstants.PortalUpgrade[] _options;

	public override void _Ready()
	{
		base._Ready();

		_options = new UpgradeConstants.PortalUpgrade[3];
		for (int i = 0; i <= 2; i++)
		{
			_options[i] = Enum.GetValues<UpgradeConstants.PortalUpgrade>().PickRandomElement(except: _options);
			this.GetTypedChildByName<Label>($"ChoiceLabel{i}").Text = _options[i].Description();
		}

		QueueRedraw();
	}
	public void OnButtonRelease(int buttonId)
	{
		if (_isActive)
		{
			var gs = this.GetAutoload<GameStateService>();
			_options[buttonId].AddTo(gs.PlayerStatus);

			_isActive = false;
			
			QueueFree();
			this.GetGame().RandomStage();
		}
	}
}
