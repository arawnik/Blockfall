namespace Jetris.Scripts.Models.Nodes;

/// <summary>
/// Basic game rules for endless board where there is no real win condition, just making new highscore.
/// </summary>
public partial class GameRulesNormal : GameRules
{
    /// <summary>
    /// Explanation for the win condition that will be displayed in HUD.
    /// </summary>
    public override string WinConditionText => string.Empty;
}
