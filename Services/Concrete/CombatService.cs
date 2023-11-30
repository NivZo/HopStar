using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CombatService : Node, ICombatService, IGameAutoLoad
{
    public bool IsEnabled { get; set; }
    public int CombatantCount = 0;

    private Game _game;
    private bool _isCombat = false;

    public bool IsNoEnemies() => CombatantCount == 0;

    public void CountCombatants()
    {
        if (IsEnabled && _isCombat)
        {
            CombatantCount = this.GetGame().GetChildrenByType<Combatant>()
                .Where(cmb => cmb is not Player)
                .Count();

            if (IsNoEnemies())
            {
                _isCombat = false;
                this.GetAutoload<SceneService>()?.GetCurrentStage()?.InitPortals();
            }
        }
    }

    public void InitService()
    {
        var timer = new Timer()
        {
            OneShot = false,
            WaitTime = 0.5,
        };
        AddChild(timer);
        timer.Connect("timeout", new(this, MethodName.CountCombatants));
        timer.Start();
        IsEnabled = true;
        _isCombat = true;
    }

    public void StartCombat() => _isCombat = true;

    public bool IsCombat() => _isCombat;
}
