namespace Blockfall.Scripts.Models.RuleConditions;

using System.Linq;
using Blockfall.Scripts.Models.Nodes;
using Godot;

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
    /// Displayed difficulty description with difficulty explanation for lose condition.
    /// </summary>
    public abstract string DifficultyExtensionText { get; }

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
    /// </summary>
    public void CheckTetrominoLocked()
    {
        if (GameRules.IsGameOver)
            return;

        // We only create lines when there are pieces for them. So if top line exists, game over.
        if (CheckGameOverOnTetrominoLocked())
        {
            GameRules.ProcessGameOver();
            return;
        }
        GameRules.Advance();
    }

    /// <summary>
    /// Pile up the game over conditions that happen on Tetromino Locked event.
    /// </summary>
    /// <returns><see cref="true"/> if game is over/lost, <see cref="false"/> otherwise.</returns>
    public virtual bool CheckGameOverOnTetrominoLocked()
    {
        // We only create lines when there are pieces for them. So if top line exists, game over.
        return GameRules
            .Board.GetLines()
            .Any(line => line.GlobalPosition.Y == GameRules.Board.MinVector.Y);
    }
}
