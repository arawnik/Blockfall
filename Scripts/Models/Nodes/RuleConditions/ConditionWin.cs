namespace Jetris.Scripts.Models.RuleConditions;

using Godot;
using Jetris.Scripts.Models.Nodes;

/// <summary>
/// Define win condition for <see cref="GameRules"/>.
/// </summary>
public abstract partial class ConditionWin : Node
{
    /// <summary>
    /// Reference to the gamerules that are being defined.
    /// </summary>
    [Export]
    public GameRules GameRules { get; set; }

    /// <summary>
    /// Display win condition if <see cref="true"/>. Hide otherwise.
    /// </summary>
    public bool DisplayWinCondition => WinConditionText != string.Empty;

    /// <summary>
    /// Explanation for the win condition that will be displayed in HUD.
    /// </summary>
    public virtual string WinConditionText => string.Empty;

    /// <summary>
    /// Check if only win should update Best scores.
    /// </summary>
    public virtual bool UpdateBestScoreOnLose => false;
}
