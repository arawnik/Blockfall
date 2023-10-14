namespace Jetris.Scripts.Models.RuleConditions;

using Godot;
using Jetris.Scripts.Models.Nodes;
using System.Linq;

/// <summary>
/// Define lose condition and scoring for <see cref="GameRules"/>.
/// </summary>
public abstract partial class ConditionLose : Node
{
    /// <summary>
    /// Reference to the gamerules that are being defined.
    /// </summary>
    [Export]
    public GameRules GameRules { get; set; }

    /// <summary>
    /// Display for scoring.
    /// </summary>
    public abstract string ScoringText { get; }

    /// <summary>
    /// Display for best score.
    /// </summary>
    public abstract string BestScoreText { get; }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        GameRules.Board.TetrominoLocked += CheckTetrominoLocked;
    }

    /// <summary>
    /// Check if best score should be updated.
    /// </summary>
    /// <returns><see cref="true"/> if active <see cref="Scoring"/> is new best score, <see cref="false"/> otherwise.</returns>
    public abstract bool CheckBestScore();


    /// <summary>
    /// Check if game is over and emit <see cref="GameOver"/> signal if so.
    /// <paramref name="linesRemoved">Number of lines removed on event.</paramref>
    /// </summary>
    public void CheckTetrominoLocked(int linesRemoved)
    {
        if (GameRules.IsGameOver)
            return;

        // We only create lines when there are pieces for them. So if top line exists, game over.
        if (CheckGameOverConditionOnTetrominoLocked(linesRemoved))
        {
            GameRules.ProcessGameOver();
            return;
        }
        GameRules.Advance();
    }

    /// <summary>
    /// Pile up the game over conditions that happen on Tetromino Locked event.
    /// </summary>
    /// <paramref name="linesRemoved">Number of lines removed on event.</paramref>
    /// <returns><see cref="true"/> if game is over/lost, <see cref="false"/> otherwise.</returns>
    public virtual bool CheckGameOverConditionOnTetrominoLocked(int linesRemoved)
    {
        // We only create lines when there are pieces for them. So if top line exists, game over.
        return GameRules.Board.GetLines().Any(line => line.GlobalPosition.Y == GameRules.Board.MinVector.Y);
    }
}
