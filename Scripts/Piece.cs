namespace Jetris.Scripts;

using Godot;

/// <summary>
/// Piece of <see cref="Tetromino"/>.
/// </summary>
public partial class Piece : Area2D
{
    /// <summary>
    /// Static <see cref="Vector2"/> that defines the size of each Piece.
    /// </summary>
    private static readonly Vector2 SizeVector = new(Board.PIECE_SIZE, Board.PIECE_SIZE);

    /// <summary>
    /// Size of Piece.
    /// </summary>
    public static Vector2 Size => SizeVector;

    /// <summary>
    /// Set the texture for Piece.
    /// </summary>
    /// <param name="texture">The texture that will be set.</param>
    public void SetTexture(Texture2D texture)
    {
        GetNode<Sprite2D>(Resources.Sprite).Texture = texture;
    }

    public static class Resources
    {
        public const string Sprite = "Sprite2D";
    }
}
