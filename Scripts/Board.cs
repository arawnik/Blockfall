namespace Blockfall.Scripts;

using System.Collections.Generic;
using System.Linq;
using Blockfall.Scripts.Models;
using Blockfall.Scripts.Models.Nodes;
using Blockfall.Scripts.Models.RuleConditions;
using Godot;

/// <summary>
/// The grid board playarea. Childnodes define the difficulty and rules.
/// </summary>
public partial class Board : Node2D
{
    /// <summary>
    /// Rules for board. Win condition, gameover etc.
    /// </summary>
    [Export]
    public GameRules GameRules { get; set; }

    /// <summary>
    /// Difficulty for board.
    /// </summary>
    [Export]
    public Difficulty Difficulty { get; set; }

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
    /// Boolean that tells if board is campaign board.
    /// </summary>
    [Export]
    public bool IsCampaignBoard { get; set; } = false;

    /// <summary>
    /// Handler of signal that is emitted when <see cref="TetrominoPawn"/> reaches end destination and is locked.
    /// </summary>
    [Signal]
    public delegate void TetrominoLockedEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when <see cref="TetrominoPawn"/> reaches end destination and is locked.
    /// </summary>
    [Signal]
    public delegate void LinesRemovedEventHandler(Line[] lines);

    /// <summary>
    /// Amount of rows on the board.
    /// </summary>
    public const int ROW_COUNT = 20;

    /// <summary>
    /// Amount of columns on the board.
    /// </summary>
    public const int COLUMN_COUNT = 11;

    /// <summary>
    /// Size of each <see cref="Piece" /> designed for the board.
    /// </summary>
    public const int PIECE_SIZE = 48;

    /// <summary>
    /// Size of each <see cref="Piece" /> designed for the board.
    /// </summary>
    public const int HALF_PIECE_SIZE = PIECE_SIZE / 2;

    /// <summary>
    /// Vector for <see cref="TetrominoPawn"/> spawn point.
    /// </summary>
    public static readonly Vector2 SPAWN_POINT = new(
        (COLUMN_COUNT + 1) / 2 * PIECE_SIZE - HALF_PIECE_SIZE,
        Bounds.MinY
    );

    /// <summary>
    /// Min values for GlobalPosition that belongs to board.
    /// </summary>
    public Vector2 MinVector;

    /// <summary>
    /// Max values for GlobalPosition that belongs to board.
    /// </summary>
    public Vector2 MaxVector;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        MinVector = GlobalPosition + new Vector2(Bounds.MinX, Bounds.MinY);
        MaxVector = GlobalPosition + new Vector2(Bounds.MaxX, Bounds.MaxY);

        ProcessPredefinedLines();
    }

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
            var line = GetLines()
                .FirstOrDefault(line => line.GlobalPosition.Y == piece.GlobalPosition.Y);
            if (line != null)
            {
                piece.Reparent(line);
            }
            else
            {
                var newLine = LineScene.Instantiate<Line>();
                AddChild(newLine); // Have to add before setting pos or it will break.
                newLine.GlobalPosition = new Vector2(0, piece.GlobalPosition.Y);

                piece.Reparent(newLine);
            }
        });
        tetromino.QueueFree();
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
            pieces.AddRange(
                line.GetChildren().Where(piece => piece is Piece).Cast<Piece>().ToList()
            );
        }
        return pieces;
    }

    /// <summary>
    /// Get all <see cref="Line"/>s of board.
    /// </summary>
    /// <returns><see cref="List{Line}"/> of lines on board.</returns>
    public IEnumerable<Line> GetLines()
    {
        return GetChildren(false).Where(child => child is Line).Cast<Line>();
    }

    /// <summary>
    /// Get all <see cref="PredefinedLine"/>s that were added to board.
    /// </summary>
    /// <returns><see cref="List{PredefinedLine}"/> of predefined lines on board.</returns>
    public IEnumerable<PredefinedLine> GetPredefinedLines()
    {
        return GetChildren(false).Where(child => child is PredefinedLine).Cast<PredefinedLine>();
    }

    /// <summary>
    /// Check if position is out of Boards game bounds.
    /// </summary>
    /// <param name="position">The checked position.</param>
    /// <returns><see cref="true"/> if is out of bounds, <see cref="false"/> otherwise.</returns>
    public bool IsPositionOutOfBoard(Vector2 position)
    {
        return position.X < MinVector.X || position.X > MaxVector.X || position.Y > MaxVector.Y;
    }

    /// <summary>
    /// Lock <paramref name="tetromino"/> on the board and handle related logic.
    /// Called when <see cref="TetrominoPawn.LockTetromino"/> signal is emitted.
    /// </summary>
    /// <param name="tetromino">The <see cref="TetrominoPawn"/> that was locked.</param>
    protected void OnLockTetromino(TetrominoPawn tetromino)
    {
        AddTetrominoToLines(tetromino);
        RemoveFullLines();
        EmitSignal(SignalName.TetrominoLocked);
    }

    /// <summary>
    /// Remove all <see cref="Line"/>s from board that are full.
    /// </summary>
    private void RemoveFullLines()
    {
        var fullLines = GetLines().Where(line => line.IsLineFull()).ToArray();

        EmitSignal(SignalName.LinesRemoved, fullLines);
        foreach (var line in fullLines)
        {
            MoveLinesDown(line.GlobalPosition.Y);
            line.Free();
        }
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
            .ForEach(line =>
                line.GlobalPosition = new Vector2(
                    line.GlobalPosition.X,
                    (line.GlobalPosition.Y + PIECE_SIZE)
                )
            );
    }

    /// <summary>
    /// Process all <see cref="PredefinedLine"/>s into regular <see cref="Line"/>s on board.
    /// </summary>
    private void ProcessPredefinedLines()
    {
        if (GameRules.WinCondition is WinClearPreset WinClearPreset)
        {
            var pieceScene = ResourceLoader.Load<PackedScene>(Resources.Piece);
            var presetPieceSprite = GD.Load<Texture2D>(Resources.PresetPieceSprite);

            foreach (var predefinedLine in GetPredefinedLines())
            {
                var newLine = LineScene.Instantiate<Line>();
                newLine.GlobalPosition = new Vector2(
                    -HALF_PIECE_SIZE,
                    predefinedLine.Row * PIECE_SIZE - HALF_PIECE_SIZE
                );
                foreach (var pieceIndex in predefinedLine.Pieces)
                {
                    var piece = pieceScene.Instantiate<Piece>();
                    newLine.AddChild(piece);
                    piece.SetTexture(presetPieceSprite);
                    piece.Position = new Vector2(pieceIndex * Piece.Size.X, 0); //new Vector2(pieceIndex, 0) * piece.Size;
                }

                WinClearPreset.ClearableLines.Add(newLine);

                RemoveChild(predefinedLine);
                AddChild(newLine);
            }
        }
        else
        {
            //Just remove if not valid for our board.
            foreach (var predefinedLine in GetPredefinedLines())
            {
                RemoveChild(predefinedLine);
            }
        }
    }

    /// <summary>
    /// Bounds coordinates of the board.
    /// </summary>
    public static class Bounds
    {
        public const int MinX = HALF_PIECE_SIZE;
        public const int MaxX = COLUMN_COUNT * PIECE_SIZE;
        public const int MinY = HALF_PIECE_SIZE;
        public const int MaxY = ROW_COUNT * PIECE_SIZE;
    }

    public static class Resources
    {
        public const string PresetPieceSprite = "res://Assets/Pieces/Preset.png";
        public const string Piece = "res://Scenes/piece.tscn";
    }
}
