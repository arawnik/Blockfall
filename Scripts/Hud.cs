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
    /// Handler of signal that is emitted when RestartButton is pressed.
    /// </summary>
    [Signal]
    public delegate void RestartStateEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when AdvanceButton is pressed.
    /// </summary>
    [Signal]
    public delegate void AdvanceStateEventHandler();

    /// <summary>
    /// Handler of signal that is emitted when MenuButton is pressed.
    /// </summary>
    [Signal]
    public delegate void MenuStateEventHandler();

    /// <summary>
    /// Container for displaying next <see cref="Tetromino"/>.
    /// </summary>
    public PanelContainer NextPieceContainer;

    /// <summary>
    /// Container that will be shown when it's game's end.
    /// </summary>
    public PanelContainer GameEndContainer;

    /// <summary>
    /// Label that displays info at game end.
    /// </summary>
    public Label GameEndLabel;

    /// <summary>
    /// Label that displays scoring info.
    /// </summary>
    public Label ScoringLabel;

    /// <summary>
    /// Label that displays highscore info.
    /// </summary>
    public Label HighScoreLabel;

    /// <summary>
    /// Button used for restarting.
    /// </summary>
    public Button RestartButton;

    /// <summary>
    /// Button used for advancing.
    /// </summary>
    public Button AdvanceButton;

    /// <summary>
    /// Button used for returning to menu.
    /// </summary>
    public Button MenuButton;

    /// <summary>
    /// Position where next <see cref="Tetromino"/> should be displayed.
    /// </summary>
    public static readonly Vector2 NextTetrominoPosition = new(65, 35);

    /// <summary>
    /// Scale of displayed next <see cref="Tetromino"/>.
    /// </summary>
    public static readonly Vector2 NextTetrominoScale = new(.35f, .35f);

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
        GameEndContainer = GetNode<PanelContainer>(Resources.GameEndContainer);
        GameEndLabel = GetNode<Label>(Resources.GameEndLabel);
        ScoringLabel = GetNode<Label>(Resources.ScoringLabel);
        HighScoreLabel = GetNode<Label>(Resources.HighScoreLabel);

        RestartButton = GetNode<Button>(Resources.RestartButton);
        AdvanceButton = GetNode<Button>(Resources.AdvanceButton);
        MenuButton = GetNode<Button>(Resources.MenuButton);
    }

    /// <summary>
    /// Show game over to user.
    /// </summary>
    public void ShowGameOver()
    {
        GameEndLabel.Text = "Game over";
        RestartButton.Show();
        AdvanceButton.Hide();
        GameEndContainer.Show();
    }

    /// <summary>
    /// Show game win to user.
    /// </summary>
    public void ShowGameWin()
    {
        GameEndLabel.Text = "You win!";
        RestartButton.Hide();
        AdvanceButton.Show();
        GameEndContainer.Show();
    }

    /// <summary>
    /// Display <paramref name="tetromino"/> on the box for next <see cref="Tetromino"/>.
    /// </summary>
    /// <param name="tetrominoType">The type of <see cref="Tetromino"/> that will spawn next.</param>
    public void DisplayNextTetromino(TetrominoType tetrominoType)
    {
        FreeNextTetromino();
        NextTetromino = Tetromino.Create<Tetromino>(TetrominoScene, tetrominoType);
        NextTetromino.Position = NextTetrominoPosition;
        NextTetromino.Scale = NextTetrominoScale;
        GetNode<PanelContainer>(Resources.NextPieceContainer).AddChild(NextTetromino);
    }

    /// <summary>
    /// Display scoring info. So it could be points or pawns left/used etc depending on mode.
    /// </summary>
    /// <param name="scoring">The scoring value.</param>
    public void DisplayPoints(int scoring)
    {
        ScoringLabel.Text = scoring.ToString();
    }

    /// <summary>
    /// Display highscore
    /// </summary>
    /// <param name="highscore">The highscore value.</param>
    public void DisplayHighscore(int highscore)
    {
        HighScoreLabel.Text = highscore.ToString();
    }

    /// <summary>
    /// Handler for when RestartButton is pressed.
    /// </summary>
    public void OnRestartButtonPressed()
    {
        EmitSignal(SignalName.RestartState);
    }

    /// <summary>
    /// Handler for when AdvanceButton is pressed.
    /// </summary>
    public void OnAdvanceButtonPressed()
    {
        EmitSignal(SignalName.AdvanceState);
    }

    /// <summary>
    /// Handler for when MenuButton is pressed.
    /// </summary>
    public void OnMenuButtonPressed()
    {
        EmitSignal(SignalName.MenuState);
    }

    /// <summary>
    /// Remove next tetromino from HUD.
    /// </summary>
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
        public const string GameEndContainer = "GameEndContainer";

        public const string GameEndLabel = "GameEndContainer/MarginContainer/VBoxContainer/GameEndLabel";
        public const string ScoringLabel = "ScoringContainer/ScoringLabel";
        public const string HighScoreLabel = "HighScoreContainer/HighScoreLabel";

        public const string RestartButton = "GameEndContainer/MarginContainer/VBoxContainer/RestartButton";
        public const string AdvanceButton = "GameEndContainer/MarginContainer/VBoxContainer/AdvanceButton";
        public const string MenuButton = "GameEndContainer/MarginContainer/VBoxContainer/MenuButton";
    }
}
