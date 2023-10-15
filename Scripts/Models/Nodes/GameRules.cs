namespace Jetris.Scripts.Models.Nodes;

using Godot;
using Jetris.Scripts.Models.RuleConditions;
using System;

/// <summary>
/// Node responsible for handling rules of <see cref="Game"/>. Win / Lose conditions etc.
/// </summary>
public partial class GameRules : Node
{
    /// <summary>
    /// Handler of signal that is emitted when game is lost.
    /// </summary>
    [Signal]
    public delegate void GameOverEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when Game is won, if such condition exists.
    /// </summary>
    [Signal]
    public delegate void GameWinEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when campaign event is cleared.
    /// </summary>
    [Signal]
    public delegate void CampaignLevelClearedEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when <see cref="TetrominoPawn"/> reaches end destination and is locked.
    /// </summary>
    [Signal]
    public delegate void ScoringUpdatedEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when next <see cref="TetrominoPawn"/> should be spawned.
    /// </summary>
    [Signal]
    public delegate void NextPawnEventHandler();

    /// <summary>
    /// Reference to the board of which rules are being checked.
    /// </summary>
    [Export]
    public Board Board {  get; set; }

    /// <summary>
    /// Reference to the lose condition.
    /// </summary>
    [Export]
    public ConditionLose LoseCondition { get; set; }

    /// <summary>
    /// Reference to the win condition.
    /// </summary>
    [Export]
    public ConditionWin WinCondition { get; set; }

    /// <summary>
    /// Delegate for fetching multiplier for scoring events.
    /// </summary>
    public GetScoringMultiplier getScoringMultiplier { get; set; }

    /// <summary>
    /// Current scoring.
    /// </summary>
    public int Scoring { get; set; }

    /// <summary>
    /// Best score on category.
    /// </summary>
    public int BestScore { get; set; }

    /// <summary>
    /// Is game over?
    /// </summary>
    public bool IsGameOver { get; set; } = false;

    /// <summary>
    /// Send signal when score is updated.
    /// </summary>
    public void SignalScoreUpdated()
    {
        EmitSignal(SignalName.ScoringUpdated);
    }

    /// <summary>
    /// Advance to next <see cref="TetrominoPawn"/>.
    /// </summary>
    public void Advance()
    {
        EmitSignal(SignalName.NextPawn);
    }

    /// <summary>
    /// Send signal when game is over.
    /// </summary>
    public void ProcessGameOver()
    {
        IsGameOver = true;
        EmitSignal(SignalName.GameOver);
    }

    /// <summary>
    /// Send signal when game is won.
    /// </summary>
    public void ProcessGameWin()
    {
        IsGameOver = true;
        EmitSignal(SignalName.GameWin);
        if (Board.IsCampaignBoard)
            EmitSignal(SignalName.CampaignLevelCleared);
    }

    /// <summary>
    /// Check if best score should be updated, and update if necessary.
    /// </summary>
    /// <param name="bestScore">the current best score.</param>
    /// <returns><see cref="true"/> if active <see cref="Scoring"/> is new best score, <see cref="false"/> otherwise.</returns>
    public bool CheckUpdateBestScore(out int bestScore)
    {
        if (LoseCondition.CheckBestScore())
        {
            BestScore = Scoring;
            bestScore = BestScore;
            return true;
        }
        bestScore = BestScore;
        return false;
    }

    /// <summary>
    /// Delegate for fetching multiplier for scoring events.
    /// </summary>
    /// <returns>Multiplier for scoring events.</returns>
    public delegate float GetScoringMultiplier();
}
