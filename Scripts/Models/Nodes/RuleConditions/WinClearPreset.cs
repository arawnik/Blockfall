namespace Jetris.Scripts.Models.RuleConditions;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
public partial class WinClearPreset : ConditionWin
{
    /// <summary>
    /// The lines that need to be cleared to win.
    /// </summary>
    public List<Line> ClearableLines { get; set; } = new List<Line>();

    /// <summary>
    /// Explanation for the win condition that will be displayed in HUD.
    /// </summary>
    public override string WinConditionText => "Clear all preset rows to win";

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        GameRules.Board.LineRemoved += CheckLineRemove;
    }

    /// <summary>
    /// Check if removed line was one of the ones needed to clear for win.
    /// </summary>
    /// <param name="line">The checked line.</param>
    private void CheckLineRemove(Line line)
    {
        if (ClearableLines.Remove(line) && !ClearableLines.Any())
        {
            GameRules.ProcessGameWin();
        }
    }
}
