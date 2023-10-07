namespace Jetris.Scripts;

using Godot;

/// <summary>
/// The ghost that displays where controlled <see cref="Tetromino"/> would land if it was dropped at the time.
/// </summary>
public partial class TetrominoGhost : Tetromino
{
    [Signal]
    public delegate void TetrominoLockedEventHandler();

    /// <summary>
    /// Get the texture for This tetromino.
    /// </summary>
    protected override Texture2D PieceTexture => GhostPieceSprite;

    /// <summary>
    /// The texture used for Ghost pieces.
    /// </summary>
    private Texture2D GhostPieceSprite;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        // Initialize Ghost sprite before we do common.
        GhostPieceSprite = GD.Load<Texture2D>(Resources.GhostPieceSprite);

        // Do all of the common initialization.
        base._Ready();
    }

    /// <summary>
    /// Call to update ghost when controlled <see cref="Tetromino"/> changes.
    /// </summary>
    /// <param name="newPosition">New position for the ghost.</param>
    /// <param name="piecesPosition">new position for pieces of the ghost.</param>
    public void SetGhostTetromino(Vector2 newPosition, Vector2[] piecesPosition)
    {
        GlobalPosition = newPosition;

        for (int i = 0; i < Pieces.Count; i++)
        {
            Pieces[i].Position = piecesPosition[i];
        }
    }
}
