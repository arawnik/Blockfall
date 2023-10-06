namespace Jetris.Scripts;

using Godot;

/// <summary>
/// Head-Up Display also known as HUD. Will be used to display data around the screen.
/// </summary>
public partial class Hud : CanvasLayer
{
    public PanelContainer NextPieceContainer;
    public PanelContainer GameOverContainer;

    public static Vector2 NextTetrominoPosition = new(65, 35);
    public static Vector2 NextTetrominoScale = new(.35f, .35f);

    protected Tetromino nextTetromino;

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
        nextTetromino?.QueueFree();
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
    public void DisplayNextTetromino(Tetromino tetromino)
    {
        nextTetromino = tetromino;
        tetromino.Position = NextTetrominoPosition;
        tetromino.Scale = NextTetrominoScale;
        GetNode<PanelContainer>(Resources.NextPieceContainer).AddChild(tetromino);
    }

    public static class Resources
    {
        public const string NextPieceContainer = "NextPieceContainer";
        public const string GameOverContainer = "GameOverContainer";
        public const string NextPieceLabel = "NextPieceContainer/NextPieceLabel";
    }
}
