namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;

/// <summary>
/// Additional data for <see cref="Piece"/> resource.
/// </summary>
public partial class PieceData : Resource
{
    /// <summary>
    /// Texture of <see cref="Piece"/>.
    /// </summary>
	[Export]
	public Texture2D PieceTexture { get; set; }

    /// <summary>
    /// The type of <see cref="Tetromino"/> that piece belongs to.
    /// </summary>
    [Export]
    public TetrominoType TetrominoType { get; set; }

    /// <summary>
    /// The position this specific <see cref="Piece"/> should be spawned to.
    /// </summary>
    [Export]
    public Vector2 SpawnPosition { get; set; }
}
