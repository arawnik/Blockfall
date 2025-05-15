namespace Blockfall.Scripts.Models.RuleConditions;

/// <summary>
/// Endless game with points condition for <see cref="GameRules"/>.
/// </summary>
public partial class LoseEndless : ConditionLose
{
    /// <summary>
    /// Display for scoring.
    /// </summary>
    public override string ScoringText => "Points";

    /// <summary>
    /// Display for best score.
    /// </summary>
    public override string BestScoreText => "Highscore";

    /// <summary>
    /// Displayed difficulty description with difficulty explanation for lose condition.
    /// </summary>
    public override string DifficultyExtensionText => "with endless board";

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        GameRules.Board.LinesRemoved += UpdatePoints;
    }

    /// <summary>
    /// Check if best score should be updated.
    /// </summary>
    /// <returns><see cref="true"/> if active <see cref="Scoring"/> is new best score, <see cref="false"/> otherwise.</returns>
    public override bool CheckBestScore()
    {
        return GameRules.Scoring > GameRules.BestScore;
    }

    /// <summary>
    /// Update points based on <paramref name="removedLines"/>.
    /// </summary>
    /// <param name="removedLines">The lines that got removed.</param>
    public void UpdatePoints(Line[] removedLines)
    {
        var currentDifficultyLevel = 1; // Mathf.FloorToInt(GameRules.getScoringMultiplier());
        switch (removedLines.Length)
        {
            case 1:
                GameRules.Scoring += 40 * currentDifficultyLevel;
                GameRules.SignalScoreUpdated();
                break;
            case 2:
                GameRules.Scoring += 100 * currentDifficultyLevel;
                GameRules.SignalScoreUpdated();
                break;
            case 3:
                GameRules.Scoring += 300 * currentDifficultyLevel;
                GameRules.SignalScoreUpdated();
                break;
            case 4:
                GameRules.Scoring += 1200 * currentDifficultyLevel;
                GameRules.SignalScoreUpdated();
                break;
            default:
                break;
        }
    }
}
