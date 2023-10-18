namespace Blockfall.Scripts.Models.RuleConditions;

using Godot;

/// <summary>
/// Define lose condition and scoring for <see cref="GameRules"/>.
/// </summary>
public partial class LoseLimitedMoves : ConditionLose
{
    /// <summary>
    /// Display for scoring.
    /// </summary>
    public override string ScoringText => "Pawns";

    /// <summary>
    /// Display for best score.
    /// </summary>
    public override string BestScoreText => "Most left";

    /// <summary>
    /// Displayed difficulty description with difficulty explanation for lose condition.
    /// </summary>
    public override string DifficultyExtensionText => "with limited pawns";

    /// <summary>
    /// Amount of <see cref="TetrominoPawn"/>s available.
    /// </summary>
    [Export]
    public int TotalPawns { get; set; } = 90;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        GameRules.Scoring = TotalPawns;
        base._Ready();
    }

    /// <summary>
    /// Pile up the game over conditions that happen on Tetromino Locked event.
    /// </summary>
    /// <returns><see cref="true"/> if game is over/lost, <see cref="false"/> otherwise.</returns>
    public override bool CheckGameOverOnTetrominoLocked()
    {
        return CheckUpdatePawns() || base.CheckGameOverOnTetrominoLocked();
    }

    /// <summary>
    /// Check if best score should be updated.
    /// </summary>
    /// <returns><see cref="true"/> if active <see cref="Scoring"/> is new best score, <see cref="false"/> otherwise.</returns>
    public override bool CheckBestScore()
    {
        return GameRules.Scoring < GameRules.BestScore;
    }

    /// <summary>
    /// Check pawns amount and update it.
    /// </summary>
    /// <returns><see cref="true"/> if game over, false otherwise. <see cref="Scoring"/> is new best score, <see cref="false"/> otherwise.</returns>
    private bool CheckUpdatePawns()
    {
        GameRules.Scoring -= 1;
        GameRules.SignalScoreUpdated();
        return GameRules.Scoring <= 0;
    }
}
