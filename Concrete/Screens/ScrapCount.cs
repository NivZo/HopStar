using Godot;
using System;

public partial class ScrapCount : Label
{
    public override void _Ready()
    {
        base._Ready();

        Text = this.GetAutoload<GameStateService>().PlayerStatus.Coins.ToString();
    }
}
