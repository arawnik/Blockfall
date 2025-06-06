﻿namespace Blockfall.Scripts.Models.RuleConditions;

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
    public List<Line> ClearableLines { get; set; } = [];

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

        GameRules.Board.LinesRemoved += CheckLineRemove;
    }

    /// <summary>
    /// Check if removed lines were the ones needed to clear for win, and if it's all gone.
    /// </summary>
    /// <param name="lines">The checked lines.</param>
    private void CheckLineRemove(Line[] lines)
    {
        ClearableLines = [.. ClearableLines.Except(lines)];
        if (ClearableLines.Count == 0)
        {
            GameRules.ProcessGameWin();
        }
    }
}
