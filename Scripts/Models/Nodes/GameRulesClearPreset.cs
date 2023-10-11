namespace Jetris.Scripts.Models.Nodes;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Game rules for boards where you have to clear preset lines to win.
/// </summary>
public partial class GameRulesClearPreset : GameRules
{
    /// <summary>
    /// The lines that need to be cleared to win.
    /// </summary>
    public List<Line> ClearableLines { get; set; } = new List<Line>();

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        Board.LineRemoved += CheckLineRemove;
    }

    /// <summary>
    /// Check if removed line was one of the ones needed to clear for win.
    /// </summary>
    /// <param name="line">The checked line.</param>
    private void CheckLineRemove(Line line)
    {
        if(ClearableLines.Remove(line) && !ClearableLines.Any())
        {
            EmitSignal(SignalName.GameWin);
        }
    }
}
