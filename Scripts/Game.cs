namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using Jetris.Scripts.Models.Nodes;
using static Jetris.Scripts.Save.GameData;

/// <summary>
/// The game of tetris.
/// </summary>
public partial class Game : Node
{
    /// <summary>
    /// Responsible for logic of which Tetrominos to spawn on board.
    /// </summary>
    [Export]
    public Spawner Spawner { get; set; }

    /// <summary>
    /// Reference to the <see cref="Hud"/>.
    /// </summary>
    public Hud HUD;

    /// <summary>
    /// Reference to the <see cref="Board"/>.
    /// </summary>
    protected Board Board;

    /// <summary>
    /// Is game over?
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// Current points.
    /// </summary>
    private int _points = 0;

    /// <summary>
    /// Highscore
    /// </summary>
    private int _highScore = 0;

    /// <summary>
    /// Callback for checking highscore.
    /// </summary>
    private CheckHighScore _checkHighScore;

    /// <summary>
    /// TODO: Link difficulty into point calculations
    /// </summary>
    private int currentDifficultyLevel = 1;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        HUD = GetNode<Hud>(Resources.HUD);
        HUD.DisplayHighscore(_highScore);

        Spawner.OnReady(UpdateTetrominos);
    }

    /// <summary>
    /// Update <see cref="Tetromino"/>s on <see cref="board"/>.
    /// </summary>
    /// <param name="CurrentTetromino">The currently active <see cref="TetrominoPawn"/> that is controlled by user.</param>
    /// <param name="NextTetromino">Next <see cref="Tetromino"/> displayed on HUD.</param>
    public void UpdateTetrominos(TetrominoType CurrentTetromino, TetrominoType NextTetromino)
    {
        Board.SpawnTetromino(CurrentTetromino);
        HUD.DisplayNextTetromino(NextTetromino);
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.GameOver"/> signal.
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;
        HUD.ShowGameOver();
        _checkHighScore(_points);
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.GameWin"/> signal.
    /// </summary>
    private void OnGameWin()
    {
        isGameOver = true;
        HUD.ShowGameWin();
        _checkHighScore(_points);
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.TetrominoLocked"/> signal.
    /// </summary>
    private void OnTetrominoLocked(int linesRemoved)
    {
        if (isGameOver)
            return;

        UpdatePoints(linesRemoved);

        Spawner.Advance();
    }

    /// <summary>
    /// Update points based on amount of <paramref name="linesRemoved"/>.
    /// </summary>
    /// <param name="linesRemoved">Amount of lines removed when <see cref="TetrominoPawn"/> reach bottom.</param>
    private void UpdatePoints(int linesRemoved)
    {
        switch (linesRemoved)
        {
            case 1:
                _points += 40 * currentDifficultyLevel;
                HUD.DisplayPoints(_points);
                break;
            case 2:
                _points += 100 * currentDifficultyLevel;
                HUD.DisplayPoints(_points);
                break;
            case 3:
                _points += 300 * currentDifficultyLevel;
                HUD.DisplayPoints(_points);
                break;
            case 4:
                _points += 1200 * currentDifficultyLevel;
                HUD.DisplayPoints(_points);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Create new Game.
    /// </summary>
    /// <param name="gameScene">Scene for creating new instances of game.</param>
    /// <param name="board">Board that is being played.</param>
    /// <param name="highScore">Current highscore that matches <paramref name="board"/>.</param>
    /// <param name="checkHighScore">Callback for updating highscore that matches <paramref name="board"/>.</param>
    /// <returns>New game.</returns>
    public static Game Create(PackedScene gameScene, Board board, int highScore, CheckHighScore checkHighScore)
    {
        var game = gameScene.Instantiate<Game>();
        game._highScore = highScore;
        game._checkHighScore = checkHighScore;

        var boardNode = game.GetNode<Node2D>(Resources.BoardNode);
        game.Board = board;
        boardNode.AddChild(board);

        board.TetrominoLocked += game.OnTetrominoLocked;
        board.GameRules.GameOver += game.OnGameOver;
        board.GameRules.GameWin += game.OnGameWin;

        return game;
    }

    public static class Resources
    {
        public const string Board = "Board";
        public const string BoardNode = "BoardNode";
        public const string HUD = "HUD";

        public const string BoardScene = "res://Scenes/board.tscn";
    }
}
