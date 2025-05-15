namespace Blockfall.Scripts;

using Blockfall.Scripts.Models;
using Blockfall.Scripts.Models.Nodes;
using Godot;
using static Blockfall.Scripts.Save.GameData;

/// <summary>
/// The game of blocks falling.
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
    public Board Board;

    /// <summary>
    /// Callback for updating best score.
    /// </summary>
    private UpdateBestScore _updateBestScore;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        HUD = GetNode<Hud>(Resources.HUD);
        HUD.InitDifficulty(Board.Difficulty);
        HUD.InitGameRulesOnHud(Board.GameRules);

        Spawner.OnReady(UpdateTetrominos);
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
    public override void _Process(double delta)
    {
        base._Process(delta);

        //Stop here on gameover.
        if (Board.GameRules.IsGameOver)
            return;
        Board.Difficulty.OnProcess((float)delta);
        HUD.DisplayDifficulty(Board.Difficulty.Current);
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
    /// Advance to next <see cref="TetrominoPawn"/>.
    /// </summary>
    public void NextPawn()
    {
        Spawner.Advance();
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.GameOver"/> signal.
    /// </summary>
    private void OnGameOver()
    {
        if (
            Board.GameRules.WinCondition.UpdateBestScoreOnLose
            && Board.GameRules.CheckUpdateBestScore(out var bestScore)
        )
        {
            _updateBestScore(bestScore);
        }

        HUD.ShowGameOver();
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.GameWin"/> signal.
    /// </summary>
    private void OnGameWin()
    {
        if (Board.GameRules.CheckUpdateBestScore(out var bestScore))
        {
            _updateBestScore(bestScore);
        }
        HUD.ShowGameWin();
    }

    /// <summary>
    /// Update HUD when scoring is updated.
    /// </summary>
    private void OnScoringUpdated()
    {
        HUD.DisplayPoints(Board.GameRules.Scoring);
    }

    /// <summary>
    /// Create new Game.
    /// </summary>
    /// <param name="gameScene">Scene for creating new instances of game.</param>
    /// <param name="board">Board that is being played.</param>
    /// <returns>New game.</returns>
    public static Game Create(
        PackedScene gameScene,
        Board board,
        int highScore,
        UpdateBestScore updateBestScore
    )
    {
        var game = gameScene.Instantiate<Game>();
        game._updateBestScore = updateBestScore;

        var boardNode = game.GetNode<Node2D>(Resources.BoardNode);
        game.Board = board;
        boardNode.AddChild(board);

        board.GameRules.BestScore = highScore;
        board.GameRules.ScoringUpdated += game.OnScoringUpdated;
        board.GameRules.NextPawn += game.NextPawn;
        board.GameRules.GameOver += game.OnGameOver;
        board.GameRules.GameWin += game.OnGameWin;

        return game;
    }

    public static class Resources
    {
        public const string Board = "Board";
        public const string BoardNode = "BoardNode";
        public const string HUD = "HUD";
    }
}
