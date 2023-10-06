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
    /// Scene that is used to instantiate new <see cref="Tetromino"/>s.
    /// </summary>
	[Export]
	public PackedScene TetrominoScene { get; set; }

    /// <summary>
    /// Scene that is used to instantiate new <see cref="Line"/>s into the board.
    /// </summary>
	[Export]
    public PackedScene LineScene { get; set; }

    /// <summary>
    /// Handler of signal that is emitted when <see cref="Tetromino"/> reaches end destination and is locked.
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

    //TODO: Get rid
    protected Tetromino NextTetromino;

    /// <summary>
    /// Spawn new <see cref="Tetromino"/> on board of type <see cref="TetrominoType"/>.
    /// </summary>
    /// <param name="type">Type of the spawned <see cref="Tetromino"/>.</param>
    /// <param name="isNext"><see cref="true"/> if spawned <see cref="Tetromino"/> is for display. <see cref="false"/> if spawning to board.</param>
    public void SpawnTetromino(TetrominoType type, bool isNext)
    {
        var tetrominoData = (PieceData)Autoload.TetrominoResources[type];

        var tetromino = TetrominoScene.Instantiate<Tetromino>();
        tetromino.PieceData = tetrominoData;
        tetromino.IsNextPiece = isNext;

        //TODO: Refactor isNext out, and spawn tetromino to HUD directly.
        if (isNext)
        {
            NextTetromino = tetromino;
            ((Hud)GetNode<CanvasLayer>(Resources.HUD)).DisplayNextTetromino(tetromino);
        }
        else
        {
            tetromino.Position = tetrominoData.SpawnPosition;
            tetromino.OtherPieces = GetAllPieces();
            tetromino.LockTetromino += OnLockTetromino;
            AddChild(tetromino);
        }
    }

    /// <summary>
    /// Add <see cref="Piece"/>s of <paramref name="tetromino"/> <see cref="Line"/>s on the board.
    /// </summary>
    /// <param name="tetromino">The <see cref="Tetromino"/> that will be added.</param>
    public void AddTetrominoToLines(Tetromino tetromino)
	{
        //TODO: Refactor logic of getting pieces into Tetromino itself
		var pieces = tetromino.GetChildren(false)
			.Where(child => child is Piece)
			.Cast<Piece>()
			.ToList();
        foreach (var piece in pieces)
        {
			var lineExists = false;
			foreach (var line in GetLines())
			{
				if(line.GlobalPosition.Y == piece.GlobalPosition.Y)
				{
					piece.Reparent(line);
					lineExists = true;
				}
			}
			if (!lineExists)
			{
				var newLine = LineScene.Instantiate<Line>();
				newLine.GlobalPosition = new Vector2 (0, piece.GlobalPosition.Y);
				AddChild(newLine);
                piece.Reparent(newLine);
            }
        }
    }

    /// <summary>
    /// Lock <paramref name="tetromino"/> on the board and handle related logic.
    /// Called when <see cref="Tetromino.LockTetromino"/> signal is emitted.
    /// </summary>
    /// <param name="tetromino">The <see cref="Tetromino"/> that was locked.</param>
    protected void OnLockTetromino(Tetromino tetromino)
    {
        NextTetromino.QueueFree();
        AddTetrominoToLines(tetromino);
        RemoveFullLines();
        EmitSignal(SignalName.TetrominoLocked);
        CheckGameOver();
    }

    /// <summary>
    /// Remove all <see cref="Line"/>s from board that are full.
    /// </summary>
    private void RemoveFullLines()
    {
        //TODO: We should probably return number of rows that were removed to use it elsewhere for points etc.
        foreach (var line in GetLines())
        {
            if (line.IsLineFull())
            {
                MoveLinesDown(line.GlobalPosition.Y);
                line.Free();
            }
        }
    }

    /// <summary>
    /// Check if game is over and emit <see cref="GameOver"/> signal if so.
    /// </summary>
    private void CheckGameOver()
    {
        foreach (var piece in GetAllPieces())
        {
            if (piece.GlobalPosition.Y == Bounds.MinY)
            {
                EmitSignal(SignalName.GameOver);
                break;
            }
        }
    }

    /// <summary>
    /// Move all lines above <paramref name="y"/> one cell down.
    /// </summary>
    /// <param name="y">The breaking point of moved lines.</param>
    private void MoveLinesDown(float y)
    {
        foreach (var line in GetLines())
        {
            if (line.GlobalPosition.Y < y)
                line.GlobalPosition = new Vector2(line.GlobalPosition.X, (line.GlobalPosition.Y + PIECE_SIZE));
        }
    }

    /// <summary>
    /// Get all <see cref="Line"/>s of board.
    /// </summary>
    /// <returns><see cref="List{Line}"/> of lines on board.</returns>
    private List<Line> GetLines()
    {
        return GetChildren(false)
            .Where(child => child is Line)
            .Cast<Line>()
            .ToList();
    }

    /// <summary>
    /// Get all <see cref="Piece"/> that are locked on board.
    /// </summary>
    /// <returns><see cref="List{Piece}"/> of pieces on board.</returns>
    private List<Piece> GetAllPieces()
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
