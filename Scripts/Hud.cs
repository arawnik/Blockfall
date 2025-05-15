namespace Blockfall.Scripts;

using Blockfall.Scripts.Models;
using Blockfall.Scripts.Models.Nodes;
using Godot;

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
    /// Container that displays win condition.
    /// </summary>
    public VBoxContainer WinConditionContainer;

    /// <summary>
    /// Label to display text for wind condition.
    /// </summary>
    public RichTextLabel WinConditionTextLabel;

    /// <summary>
    /// Bar that displays the active difficulty level.
    /// </summary>
    public ProgressBar DifficultyBar;

    /// <summary>
    /// Label that displays info at game end.
    /// </summary>
    public Label GameEndLabel;

    /// <summary>
    /// Label that displays scoring info.
    /// </summary>
    public Label ScoringLabel;

    /// <summary>
    /// Label that displays scoring text.
    /// </summary>
    public Label ScoringTextLabel;

    /// <summary>
    /// Label that displays best score info.
    /// </summary>
    public Label BestScoreLabel;

    /// <summary>
    /// Label that displays best score text.
    /// </summary>
    public Label BestScoreTextLabel;

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

        WinConditionContainer = GetNode<VBoxContainer>(Resources.WinConditionContainer);
        WinConditionTextLabel = GetNode<RichTextLabel>(Resources.WinConditionTextLabel);
        DifficultyBar = GetNode<ProgressBar>(Resources.DifficultyBar);

        GameEndLabel = GetNode<Label>(Resources.GameEndLabel);
        ScoringLabel = GetNode<Label>(Resources.ScoringLabel);
        ScoringTextLabel = GetNode<Label>(Resources.ScoringTextLabel);

        BestScoreLabel = GetNode<Label>(Resources.BestScoreLabel);
        BestScoreTextLabel = GetNode<Label>(Resources.BestScoreTextLabel);

        RestartButton = GetNode<Button>(Resources.RestartButton);
        AdvanceButton = GetNode<Button>(Resources.AdvanceButton);
        MenuButton = GetNode<Button>(Resources.MenuButton);
    }

    /// <summary>
    /// Initialize difficultybar.
    /// </summary>
    /// <param name="difficulty">Instance of the difficulty that will be displayed.</param>
    public void InitDifficulty(Difficulty difficulty)
    {
        DifficultyBar.MinValue = Difficulty.Min;
        DifficultyBar.MaxValue = Difficulty.Max;
        DisplayDifficulty(difficulty.Current);
    }

    /// <summary>
    /// Initialize details related to <see cref="GameRules"/>
    /// </summary>
    /// <param name="difficulty">Instance of the gamerules that will be displayed.</param>
    public void InitGameRulesOnHud(GameRules rules)
    {
        InitBestscore(rules.BestScore, rules.LoseCondition.BestScoreText);
        InitScoring(rules.Scoring, rules.LoseCondition.ScoringText);

        if (rules.WinCondition.DisplayWinCondition)
        {
            WinConditionTextLabel.Text = rules.WinCondition.WinConditionText;
            WinConditionContainer.Show();
        }
        else
        {
            WinConditionContainer.Hide();
        }
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
    /// Display best score
    /// </summary>
    /// <param name="bestscore">The best score value.</param>
    /// <param name="bestScoreStr">The best score label string.</param>
    public void InitBestscore(int bestscore, string bestScoreStr)
    {
        BestScoreLabel.Text = bestscore.ToString();
        BestScoreTextLabel.Text = bestScoreStr;
    }

    /// <summary>
    /// Display scoring info
    /// </summary>
    /// <param name="scoring">Scoring current value.</param>
    /// <param name="scoringText">Scoring label string.</param>
    public void InitScoring(int scoring, string scoringText)
    {
        ScoringLabel.Text = scoring.ToString();
        ScoringTextLabel.Text = scoringText;
    }

    /// <summary>
    /// Update difficulty bar.
    /// </summary>
    /// <param name="current">Current difficulty value</param>
    public void DisplayDifficulty(float current)
    {
        var min = (float)DifficultyBar.MinValue;
        var max = (float)DifficultyBar.MaxValue;
        DifficultyBar.Value = current;

        var green = Map(max - current + min, min, max, 0f, 1f);
        var red = Map(current, min, max, 0f, 1f);
        DifficultyBar.Modulate = new Color(red, green, 0);
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

    private static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
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

        public const string WinConditionContainer = "WinConditionContainer";
        public const string WinConditionTextLabel = "WinConditionContainer/WinConditionTextLabel";
        public const string DifficultyBar = "DifficultyContainer/DifficultyBar";

        public const string GameEndLabel =
            "GameEndContainer/MarginContainer/VBoxContainer/GameEndLabel";
        public const string ScoringLabel = "ScoringContainer/ScoringLabel";
        public const string ScoringTextLabel = "ScoringContainer/ScoringTextLabel";
        public const string BestScoreLabel = "BestScoreContainer/BestScoreLabel";
        public const string BestScoreTextLabel = "BestScoreContainer/BestScoreTextLabel";

        public const string RestartButton =
            "GameEndContainer/MarginContainer/VBoxContainer/RestartButton";
        public const string AdvanceButton =
            "GameEndContainer/MarginContainer/VBoxContainer/AdvanceButton";
        public const string MenuButton =
            "GameEndContainer/MarginContainer/VBoxContainer/MenuButton";
    }
}
