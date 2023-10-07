namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The grid board playarea.
/// </summary>
public partial class Board : Node
{
    /// <summary>
    /// Scene that is used to instantiate new <see cref="TetrominoPawn"/>s.
    /// </summary>
	[Export]
    public PackedScene TetrominoPawnScene { get; set; }

    /// <summary>
    /// Scene that is used to instantiate new <see cref="Line"/>s into the board.
    /// </summary>
	[Export]
    public PackedScene LineScene { get; set; }

    /// <summary>
    /// Handler of signal that is emitted when <see cref="TetrominoPawn"/> reaches end destination and is locked.
    /// </summary>
    [Signal]
    public delegate void TetrominoLockedEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when there is <see cref="Piece"/> locked on top <see cref="Line"/> of board.
    /// </summary>
    [Signal]
    public delegate void GameOverEventHandler();

    /// <summary>
    /// Amount of rows on the board.
    /// </summary>
    public const int ROW_COUNT = 20;
    /// <summary>
    /// Amount of columns on the board.
    /// </summary>
    public const int COLUMN_COUNT = 10;
    /// <summary>
    /// Size of each <see cref="Piece" /> designed for the board.
    /// </summary>
	public const int PIECE_SIZE = 48;

    /// <summary>
    /// Spawn new <see cref="TetrominoPawn"/> on board of type <see cref="TetrominoType"/>.
    /// </summary>
    /// <param name="type">Type of the spawned <see cref="TetrominoPawn"/>.</param>
    public void SpawnTetromino(TetrominoType type)
    {
        var tetromino = Tetromino.Create<TetrominoPawn>(TetrominoPawnScene, type);
        tetromino.LockTetromino += OnLockTetromino;
        AddChild(tetromino);
    }

    /// <summary>
    /// Add <see cref="Piece"/>s of <paramref name="tetromino"/> <see cref="Line"/>s on the board.
    /// </summary>
    /// <param name="tetromino">The <see cref="TetrominoPawn"/> that will be added.</param>
    public void AddTetrominoToLines(TetrominoPawn tetromino)
    {
        tetromino.Pieces.ForEach(piece =>
        {
            var line = GetLines().FirstOrDefault(line => line.GlobalPosition.Y == piece.GlobalPosition.Y);
            if (line != null)
            {
                piece.Reparent(line);
            }
            else
            {
                var newLine = LineScene.Instantiate<Line>();
                newLine.GlobalPosition = new Vector2(0, piece.GlobalPosition.Y);
                AddChild(newLine);
                piece.Reparent(newLine);
            }
        });
    }

    /// <summary>
    /// Get all <see cref="Piece"/>s that are locked in <see cref="Line"/>s of board.
    /// </summary>
    /// <returns><see cref="List{Piece}"/> of pieces on board.</returns>
    public List<Piece> GetAllPiecesInLines()
    {
        var pieces = new List<Piece>();
        foreach (var line in GetLines())
        {
            pieces.AddRange(line.GetChildren()
                .Where(piece => piece is Piece)
                .Cast<Piece>()
                .ToList());
        }
        return pieces;
    }

    /// <summary>
    /// Get all <see cref="Line"/>s of board.
    /// </summary>
    /// <returns><see cref="List{Line}"/> of lines on board.</returns>
    public IEnumerable<Line> GetLines()
    {
        return GetChildren(false)
            .Where(child => child is Line)
            .Cast<Line>();
    }

    /// <summary>
    /// Lock <paramref name="tetromino"/> on the board and handle related logic.
    /// Called when <see cref="TetrominoPawn.LockTetromino"/> signal is emitted.
    /// </summary>
    /// <param name="tetromino">The <see cref="TetrominoPawn"/> that was locked.</param>
    protected void OnLockTetromino(TetrominoPawn tetromino)
    {
        AddTetrominoToLines(tetromino);
        var linesRemoved = RemoveFullLines();
        EmitSignal(SignalName.TetrominoLocked);
        CheckGameOver();
    }

    /// <summary>
    /// Remove all <see cref="Line"/>s from board that are full.
    /// </summary>
    /// <returns>Number of <see cref="Line"/>s removed.</returns>
    private int RemoveFullLines()
    {
        var fullLines = GetLines()
            .Where(line => line.IsLineFull())
            .ToList();

        foreach (var line in fullLines)
        {
            MoveLinesDown(line.GlobalPosition.Y);
            line.Free();
        }
        return fullLines.Count;
    }

    /// <summary>
    /// Check if game is over and emit <see cref="GameOver"/> signal if so.
    /// </summary>
    private void CheckGameOver()
    {
        // We only create lines when there are pieces for them. So if top line exists, game over.
        if(GetLines().Any(line => line.GlobalPosition.Y.Equals(Bounds.MinY)))
            EmitSignal(SignalName.GameOver);

        //if (GetAllPiecesInLines().Any(piece => piece.GlobalPosition.Y.Equals(Bounds.MinY)))
        //    EmitSignal(SignalName.GameOver);
    }

    /// <summary>
    /// Move all lines above <paramref name="y"/> one cell down.
    /// </summary>
    /// <param name="y">The breaking point of moved lines.</param>
    private void MoveLinesDown(float y)
    {
        GetLines()
            .Where(line => line.GlobalPosition.Y < y)
            .ToList()
            .ForEach(line => line.GlobalPosition = new Vector2(line.GlobalPosition.X, (line.GlobalPosition.Y + PIECE_SIZE)));
    }

    /// <summary>
    /// Check if position is out of Boards game bounds.
    /// </summary>
    /// <param name="position">The checked position.</param>
    /// <returns><see cref="true"/> if is out of bounds, <see cref="false"/> otherwise.</returns>
    public static bool IsPositionOutOfGameBounds(Vector2 position)
    {
        return position.X < Bounds.MinX || position.X > Bounds.MaxX || position.Y > Bounds.MaxY;
    }

    /// <summary>
    /// Bounds coordinates of the board.
    /// </summary>
    public static class Bounds
    {
        public const int MinX = -216;
        public const int MaxX = 216;
        public const int MaxY = 457;
        public const int MinY = -456;
    }

    public static class Resources
    {
        public const string Line = "res://Scenes/line.tscn";
        public const string HUD = "../HUD";
    }
}
