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
    public Vector2 Size => SizeVector;

    public Sprite2D Sprite2D;
    //public CollisionShape2D CollisionShape2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>("./Sprite2D");
        //CollisionShape2D = GetNode<CollisionShape2D>("./CollisionShape2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SetTexture(Texture2D texture)
    {
        Sprite2D.Texture = texture;
    }

    /*public Vector2 GetSize()
    {
        return Board.PIECE_SIZE;//return CollisionShape2D.Shape.GetRect().Size;
    }*/
}
