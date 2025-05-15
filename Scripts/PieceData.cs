namespace Blockfall.Scripts;

using Blockfall.Scripts.Models;
using Godot;

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
    /// Tells if this <see cref="TetrominoType"/> should be offset to left on spawn.
    /// </summary>
    [Export]
    public bool SpawnOffsetLeft { get; set; }
}
