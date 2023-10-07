namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using System;

/// <summary>
/// Spawns <see cref="Tetromino"/>s to Board.
/// </summary>
public partial class Spawner : Node
{
    /// <summary>
    /// The currently active <see cref="TetrominoPawn"/> that is controlled by user.
    /// </summary>
	public TetrominoType CurrentTetromino;

    /// <summary>
    /// Next <see cref="Tetromino"/> displayed on HUD.
    /// </summary>
    public TetrominoType NextTetromino;

    /// <summary>
    /// The board we spawn <see cref="CurrentTetromino"/>s to.
    /// </summary>
    protected Board Board;

    /// <summary>
    /// The HUD where we display <see cref="NextTetromino"/>.
    /// </summary>
    protected Hud HUD;

    /// <summary>
    /// Is game over?
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        HUD = GetNode<Hud>(Resources.HUD);

        Board = GetNode<Board>(Resources.Board);
        Board.TetrominoLocked += OnTetrominoLocked;
        Board.GameOver += OnGameOver;

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
        HUD.ShowGameOver();
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
        Board.SpawnTetromino(CurrentTetromino);
        HUD.DisplayNextTetromino(NextTetromino);
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
        public const string Board = "../Board";
    }
}
