namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;

/// <summary>
/// Head-Up Display also known as HUD. Will be used to display data around the screen.
/// </summary>
public partial class Hud : CanvasLayer
{
    /// <summary>
    /// Scene that is used to instantiate new <see cref="Tetromino"/>'s.
    /// </summary>
	[Export]
    public PackedScene TetrominoScene { get; set; }

    /// <summary>
    /// Container for displaying next <see cref="Tetromino"/>.
    /// </summary>
    public PanelContainer NextPieceContainer;

    /// <summary>
    /// Container that will be shown when it's game over.
    /// </summary>
    public PanelContainer GameOverContainer;

    /// <summary>
    /// Position where next <see cref="Tetromino"/> should be displayed.
    /// </summary>
    public static Vector2 NextTetrominoPosition = new(65, 35);

    /// <summary>
    /// Scale of displayed next <see cref="Tetromino"/>.
    /// </summary>
    public static Vector2 NextTetrominoScale = new(.35f, .35f);

    /// <summary>
    /// Reference for the displayed next <see cref="Tetromino"/>.
    /// </summary>
    protected Tetromino NextTetromino;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        NextPieceContainer = GetNode<PanelContainer>(Resources.NextPieceContainer);
        GameOverContainer = GetNode<PanelContainer>(Resources.GameOverContainer);
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
    public override void _Process(double delta)
    {
    }

    /// <summary>
    /// Show game over to user.
    /// </summary>
    public void ShowGameOver()
    {
        //FreeNextTetromino();
        GameOverContainer.Show();
    }

    /// <summary>
    /// Called when user clicks Restart button.
    /// </summary>
    public void OnRestartButtonPressed()
    {
        GetTree().ReloadCurrentScene(); //TODO: Create proper restart and states
    }

    /// <summary>
    /// Display <paramref name="tetromino"/> on the box for next <see cref="Tetromino"/>.
    /// </summary>
    /// <param name="tetromino">The <see cref="Tetromino"/> that will spawn next.</param>
    public void DisplayNextTetromino(TetrominoType tetrominoType)
    {
        FreeNextTetromino();
        NextTetromino = Tetromino.Create<Tetromino>(TetrominoScene, tetrominoType);
        NextTetromino.Position = NextTetrominoPosition;
        NextTetromino.Scale = NextTetrominoScale;
        GetNode<PanelContainer>(Resources.NextPieceContainer).AddChild(NextTetromino);
    }

    private void FreeNextTetromino()
    {
        if (NextTetromino != null && !NextTetromino.IsQueuedForDeletion())
        {
            NextTetromino.QueueFree();
        }
    }

    public static class Resources
    {
        public const string NextPieceContainer = "NextPieceContainer";
        public const string GameOverContainer = "GameOverContainer";
        public const string NextPieceLabel = "NextPieceContainer/NextPieceLabel";
    }
}
