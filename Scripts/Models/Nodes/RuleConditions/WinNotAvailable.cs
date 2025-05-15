namespace Blockfall.Scripts.Models.RuleConditions;

/// <summary>
///
/// </summary>
public partial class WinNotAvailable : ConditionWin
{
    /// <summary>
    /// Update best score on lose as there is no winning.
    /// </summary>
    public override bool UpdateBestScoreOnLose => true;
}
