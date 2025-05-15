namespace Blockfall.Scripts;

using System.Collections.Generic;
using System.Linq;
using Blockfall.Scripts.Models;
using Godot;

/// <summary>
/// Class that will be autoloaded. Should be used for shared common logic.
/// </summary>
public partial class Autoload : Node
{
    /// <summary>
    /// Get <see cref="Tetromino"/> cells data.
    /// </summary>
    /// <param name="tetrominoType">Type of <see cref="Tetromino"/>.</param>
    /// <returns><see cref="Vector2[]"/> with vector for each cell.</returns>
    public static Vector2[] TetrominoCells(TetrominoType tetrominoType)
    {
        return [.. TetrominoBlocks[tetrominoType].Select(block => new Vector2(block.X, block.Y))];
    }

    private static readonly Dictionary<TetrominoType, Vector2[]> TetrominoBlocks = new()
    {
        {
            TetrominoType.I,
            [new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)]
        },
        {
            TetrominoType.J,
            [new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)]
        },
        {
            TetrominoType.L,
            [new Vector2(1, 1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)]
        },
        {
            TetrominoType.O,
            [new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0)]
        },
        {
            TetrominoType.S,
            [new Vector2(0, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(0, 0)]
        },
        {
            TetrominoType.T,
            [new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)]
        },
        {
            TetrominoType.Z,
            [new Vector2(-1, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 0)]
        },
    };

    /// <summary>
    /// Wall kicks for <see cref="TetrominoType.I"/>.
    /// </summary>
    public static readonly Vector2[][] WallKicksI =
    [
        [
            new Vector2(0, 0),
            new Vector2(-2, 0),
            new Vector2(1, 0),
            new Vector2(-2, -1),
            new Vector2(1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(2, 0),
            new Vector2(-1, 0),
            new Vector2(2, 1),
            new Vector2(-1, -2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(2, 0),
            new Vector2(-1, 2),
            new Vector2(2, -1),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(-2, 0),
            new Vector2(1, -2),
            new Vector2(-2, 1),
        ],
        [
            new Vector2(0, 0),
            new Vector2(2, 0),
            new Vector2(-1, 0),
            new Vector2(2, 1),
            new Vector2(-1, -2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-2, 0),
            new Vector2(1, 0),
            new Vector2(-2, -1),
            new Vector2(1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(-2, 0),
            new Vector2(1, -2),
            new Vector2(-2, 1),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(2, 0),
            new Vector2(-1, 2),
            new Vector2(2, -1),
        ],
    ];

    /// <summary>
    /// Wall kicks for any other <see cref="TetrominoType"/>, except for <see cref="TetrominoType.I"/>.
    /// </summary>
    public static readonly Vector2[][] WallKicksOthers =
    [
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(0, -2),
            new Vector2(-1, -2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, -1),
            new Vector2(0, 2),
            new Vector2(1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, -1),
            new Vector2(0, 2),
            new Vector2(1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(0, -2),
            new Vector2(-1, -2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, -2),
            new Vector2(1, -2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, 2),
            new Vector2(-1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(0, 2),
            new Vector2(-1, 2),
        ],
        [
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, -2),
            new Vector2(1, -2),
        ],
    ];

    /// <summary>
    /// <see cref="Resource"/>/<see cref="PieceData"/> info for each <see cref="TetrominoType"/>.
    /// </summary>
    public static readonly Dictionary<TetrominoType, PieceData> TetrominoResources = new()
    {
        {
            TetrominoType.I,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/I_PieceData.tres")
        },
        {
            TetrominoType.J,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/J_PieceData.tres")
        },
        {
            TetrominoType.L,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/L_PieceData.tres")
        },
        {
            TetrominoType.O,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/O_PieceData.tres")
        },
        {
            TetrominoType.S,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/S_PieceData.tres")
        },
        {
            TetrominoType.T,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/T_PieceData.tres")
        },
        {
            TetrominoType.Z,
            ResourceLoader.Load<PieceData>("res://Resources/Pieces/Z_PieceData.tres")
        },
    };

    /// <summary>
    /// Rotation matrix for clockwise rotation.
    /// </summary>
    public static readonly Vector2[] ClockwiseRotationMatrix =
    [
        new Vector2(0, -1),
        new Vector2(1, 0),
    ];

    /// <summary>
    /// Rotation matrix for counter-clockwise rotation.
    /// </summary>
    public static readonly Vector2[] CounterClockwiseRotationMatrix =
    [
        new Vector2(0, 1),
        new Vector2(-1, 0),
    ];

    /// <summary>
    /// All defined Inputs.
    /// </summary>
    public static class PlayerInputs
    {
        public const string Right = "Right";
        public const string Left = "Left";
        public const string Down = "Down";
        public const string Drop = "Drop";
        public const string Rotate = "Rotate";
    }
}
