namespace Blockfall.Scripts;

using System.Collections.Generic;
using Blockfall.Scripts.Models;
using Godot;

/// <summary>
/// The tetrominos that include <see cref="Piece"/>s displayed on board.
/// </summary>
public partial class Tetromino : Node2D
{
    /// <summary>
    /// Scene that is used to instantiate new <see cref="Piece"/>s.
    /// </summary>
    public PackedScene PieceScene;

    //TODO: Check if needed, are used for calculating rotation of pieces
    public Vector2[] TetrominoCells;

    /// <summary>
    /// List of all <see cref="Piece"/>s of the Tetromino.
    /// </summary>
    public List<Piece> Pieces { get; set; } = new();

    /// <summary>
    /// Data of <see cref="Piece"/>s that belong to the Tetromino.
    /// </summary>
    public PieceData PieceData { get; set; }

    /// <summary>
    /// Get the texture for This tetromino.
    /// </summary>
    protected virtual Texture2D PieceTexture => PieceData.PieceTexture;

    /// <summary>
    /// Instantiate Pieces, cells and resources.
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PieceScene = ResourceLoader.Load<PackedScene>(Resources.Piece);

        TetrominoCells = Autoload.TetrominoCells(PieceData.TetrominoType);
        foreach (var cell in TetrominoCells)
        {
            var piece = PieceScene.Instantiate<Piece>();
            Pieces.Add(piece);
            AddChild(piece);
            piece.SetTexture(PieceTexture);
            piece.Position = cell * Piece.Size;
        }
    }

    /// <summary>
    /// Create new instance of Tetromino. Instantiate with <paramref name="tetrominoScene"/> and set required data.
    /// </summary>
    /// <typeparam name="T">Strictier type of created <see cref="Tetromino"/>.</typeparam>
    /// <param name="tetrominoScene">The Scene that is used to Instantiate.</param>
    /// <param name="type">Type of created Tetromino.</param>
    /// <returns>Initialized Tetromino.</returns>
    public static T Create<T>(PackedScene tetrominoScene, TetrominoType type)
        where T : Tetromino
    {
        var pieceData = Autoload.TetrominoResources[type];
        return Create<T>(tetrominoScene, pieceData);
    }

    /// <summary>
    /// Create new instance of Tetromino. Instantiate with <paramref name="tetrominoScene"/> and set required data.
    /// </summary>
    /// <typeparam name="T">Strictier type of created <see cref="Tetromino"/>.</typeparam>
    /// <param name="tetrominoScene">The Scene that is used to Instantiate.</param>
    /// <param name="pieceData">Data for initializing the <see cref="Tetromino"/>.</param>
    /// <returns>Initialized Tetromino.</returns>
    public static T Create<T>(PackedScene tetrominoScene, PieceData pieceData)
        where T : Tetromino
    {
        var tetromino = tetrominoScene.Instantiate<T>();
        tetromino.PieceData = pieceData;

        return tetromino;
    }

    public static class Resources
    {
        public const string MoveTimer = "MoveTimer";
        public const string Board = "MoveTimer";

        public const string GhostPieceSprite = "res://Assets/Pieces/Ghost.png";

        public const string Piece = "res://Scenes/piece.tscn";
        public const string GhostTetromino = "res://Scenes/tetromino_ghost.tscn";
    }
}
