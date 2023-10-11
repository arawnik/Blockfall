namespace Jetris.Scripts.Models.Nodes;

using Godot;
using Jetris.Scripts.Models;
using System;

/// <summary>
/// Spawns <see cref="TetrominoPawn"/>s to Board.
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
    /// Action that should be called when Spawner changes active Tetrominos.
    /// </summary>
    private UpdateTetrominos _activeTetrominosChangedAction;

    /// <summary>
    /// Called from _Ready function to initialize data.
    /// </summary>
    /// <param name="activeTetrominosChangedAction">Delegate for what will be called when Spawner changes active Tetrominos.</param>
    public void OnReady(UpdateTetrominos activeTetrominosChangedAction)
    {
        _activeTetrominosChangedAction = activeTetrominosChangedAction;
        CurrentTetromino = GetRandomTetromino();
        NextTetromino = GetRandomTetromino();
        _activeTetrominosChangedAction(CurrentTetromino, NextTetromino);
    }

    /// <summary>
    /// Advance the state of <see cref="CurrentTetromino"/> and <see cref="NextTetromino"/>.
    /// </summary>
    public void Advance()
    {
        CurrentTetromino = NextTetromino;
        NextTetromino = GetRandomTetromino();
        _activeTetrominosChangedAction(CurrentTetromino, NextTetromino);
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

    /// <summary>
    /// Delegate for updating detrominos.
    /// </summary>
    /// <param name="CurrentTetromino">The currently active <see cref="TetrominoPawn"/> that is controlled by user.</param>
    /// <param name="NextTetromino">Next <see cref="Tetromino"/> displayed on HUD.</param>
    public delegate void UpdateTetrominos(TetrominoType CurrentTetromino, TetrominoType NextTetromino);
}
