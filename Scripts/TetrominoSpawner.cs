namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using System;

/// <summary>
/// Spawns <see cref="Tetromino"/>s to Board.
/// </summary>
public partial class TetrominoSpawner : Node
{
    /// <summary>
    /// The currently active <see cref="Tetromino"/> that is controlled by user.
    /// </summary>
	public TetrominoType CurrentTetromino;

    /// <summary>
    /// Next <see cref="Tetromino"/> displayed on HUD.
    /// </summary>
    public TetrominoType NextTetromino;

    /// <summary>
    /// The board we spawn <see cref="Tetromino"/>s to.
    /// </summary>
    public Board board;

    /// <summary>
    /// Is game over?
    /// </summary>
    private bool isGameOver = false;

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
	public override void _Ready()
	{
        board = GetNode<Board>("../Board");
        board.TetrominoLocked += OnTetrominoLocked;
        board.GameOver += OnGameOver;

        CurrentTetromino = GetRandomTetromino();
        NextTetromino = GetRandomTetromino();

        UpdateTetrominos();
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.GameOver"/> signal.
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;
        ((Hud)GetNode<CanvasLayer>(Resources.HUD)).ShowGameOver();
    }

    /// <summary>
    /// Called when <see cref="Board"/> emits <see cref="Board.TetrominoLocked"/> signal.
    /// </summary>
    private void OnTetrominoLocked()
    {
        if (isGameOver)
            return;
        CurrentTetromino = NextTetromino;
        NextTetromino = GetRandomTetromino();
        UpdateTetrominos();
    }

    /// <summary>
    /// Update <see cref="Tetromino"/>s on <see cref="board"/>.
    /// </summary>
    private void UpdateTetrominos()
    {
        board.SpawnTetromino(CurrentTetromino, false);
        board.SpawnTetromino(NextTetromino, true);
    }

    /// <summary>
    /// Randomize a <see cref="TetrominoType"/>.
    /// </summary>
    /// <returns>Random <see cref="TetrominoType"/>.</returns>
    public static TetrominoType GetRandomTetromino()
	{
        var values = (TetrominoType[])Enum.GetValues(typeof(TetrominoType));
        return values[GD.Randi() % values.Length];
    }

    public static class Resources
    {
        public const string HUD = "../HUD";
    }
}
