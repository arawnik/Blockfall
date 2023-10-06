namespace Jetris.Scripts;

using Godot;
using System.Linq;

/// <summary>
/// The ghost that displays where controlled <see cref="Tetromino"/> would land if it was dropped at the time.
/// </summary>
public partial class GhostTetromino : Node2D
{
    [Signal]
    public delegate void TetrominoLockedEventHandler();

    public PieceData TetrominoData { get; set; }

    protected PackedScene PieceScene;
    protected Texture2D GhostPieceSprite;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PieceScene = ResourceLoader.Load<PackedScene>(Resources.Piece);
        GhostPieceSprite = GD.Load<Texture2D>(Resources.GhostPieceSprite);
        //TetrominoData = (PieceData)Main.TetrominoResources[type];

        ResourceLoader.Load("res://Resources/I_PieceData.tres");

        var cells = Autoload.TetrominoCells(TetrominoData.TetrominoType);
        foreach (var cell in cells)
        {
            var piece = PieceScene.Instantiate<Piece>();
            AddChild(piece);
            piece.SetTexture(GhostPieceSprite);
            piece.Position = cell + piece.Size;
        }
    }

    /// <summary>
    /// Call to update ghost when controlled <see cref="Tetromino"/> changes.
    /// </summary>
    /// <param name="newPosition">New position for the ghost.</param>
    /// <param name="piecesPosition">new position for pieces of the ghost.</param>
    public void SetGhostTetromino(Vector2 newPosition, Vector2[] piecesPosition)
    {
        GlobalPosition = newPosition;

        //TODO: Tetromino should include this logic.
        var pieces = GetChildren()
            .Where(piece => piece is Piece)
            .Cast<Piece>()
            .ToArray();

        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].Position = piecesPosition[i];
        }
    }

    public static class Resources
    {
        public const string GhostPieceSprite = "res://Assets/Ghost.png";
        public const string Piece = "res://Scenes/piece.tscn";
    }
}
