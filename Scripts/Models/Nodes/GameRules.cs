namespace Jetris.Scripts.Models.Nodes;

using Godot;
using System.Linq;

/// <summary>
/// Node responsible for handling rules of <see cref="Game"/>. Win / Lose conditions etc.
/// </summary>
public abstract partial class GameRules : Node
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
    /// Reference to the board of which rules are being checked.
    /// </summary>
    [Export]
    public Board Board {  get; set; }

    /// <summary>
    /// Check if game is over and emit <see cref="GameOver"/> signal if so.
    /// </summary>
    public void CheckGameOver()
    {
        // We only create lines when there are pieces for them. So if top line exists, game over.
        if (IsGameOver())
            EmitSignal(SignalName.GameOver);
    }

    /// <summary>
    /// Check if game is over.
    /// </summary>
    /// <returns><see cref="true"/> if game is over/lost, <see cref="false"/> otherwise.</returns>
    public virtual bool IsGameOver()
    {
        // We only create lines when there are pieces for them. So if top line exists, game over.
        return Board.GetLines().Any(line => line.GlobalPosition.Y == Board.MinVector.Y);
    }
}
