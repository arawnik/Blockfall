namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class that will be autoloaded. Should be used for shared common logic.
/// </summary>
public partial class Autoload : Node
{
    public static Vector2[] TetrominoCells(TetrominoType tetrominoType)
    {
        return TetrominoBlocks[tetrominoType].Select(block => new Vector2(block.X, block.Y)).ToArray();
    }

    private static readonly Dictionary<TetrominoType, Vector2[]> TetrominoBlocks = new()
    {
        { TetrominoType.I, [ new Vector2(-1, 0), new Vector2( 0, 0), new Vector2( 1, 0), new Vector2(2, 0)] },
        { TetrominoType.J, [ new Vector2(-1, 1), new Vector2(-1, 0), new Vector2( 0, 0), new Vector2(1, 0)] },
        { TetrominoType.L, [ new Vector2( 1, 1), new Vector2(-1, 0), new Vector2( 0, 0), new Vector2(1, 0)] },
        { TetrominoType.O, [ new Vector2( 0, 1), new Vector2( 1, 1), new Vector2( 0, 0), new Vector2(1, 0)] },
        { TetrominoType.S, [ new Vector2( 0, 1), new Vector2( 1, 1), new Vector2(-1, 0), new Vector2(0, 0)] },
        { TetrominoType.T, [ new Vector2( 0, 1), new Vector2(-1, 0), new Vector2( 0, 0), new Vector2(1, 0)] },
        { TetrominoType.Z, [ new Vector2(-1, 1), new Vector2( 0, 1), new Vector2( 0, 0), new Vector2(1, 0)] }
    };

    public static readonly Vector2[][] WallKicksI = [
        [new Vector2(0, 0), new Vector2(-2, 0), new Vector2( 1, 0), new Vector2(-2, -1), new Vector2( 1,  2)],
        [new Vector2(0, 0), new Vector2( 2, 0), new Vector2(-1, 0), new Vector2( 2,  1), new Vector2(-1, -2)],
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2( 2, 0), new Vector2(-1,  2), new Vector2( 2, -1)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2(-2, 0), new Vector2( 1, -2), new Vector2(-2,  1)],
        [new Vector2(0, 0), new Vector2( 2, 0), new Vector2(-1, 0), new Vector2( 2,  1), new Vector2(-1, -2)],
        [new Vector2(0, 0), new Vector2(-2, 0), new Vector2( 1, 0), new Vector2(-2, -1), new Vector2( 1,  2)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2(-2, 0), new Vector2( 1, -2), new Vector2(-2,  1)],
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2( 2, 0), new Vector2(-1,  2), new Vector2( 2, -1)]
    ];

    public static readonly Vector2[][] WallKicksOthers = [
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1,  1), new Vector2(0, -2), new Vector2(-1, -2)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2( 1, -1), new Vector2(0,  2), new Vector2( 1,  2)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2( 1, -1), new Vector2(0,  2), new Vector2( 1,  2)],
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1,  1), new Vector2(0, -2), new Vector2(-1, -2)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2( 1,  1), new Vector2(0, -2), new Vector2( 1, -2)],
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0,  2), new Vector2(-1,  2)],
        [new Vector2(0, 0), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0,  2), new Vector2(-1,  2)],
        [new Vector2(0, 0), new Vector2( 1, 0), new Vector2( 1,  1), new Vector2(0, -2), new Vector2( 1, -2)]
    ];


    public static readonly Dictionary<TetrominoType, Resource> TetrominoResources = new()
    {
        { TetrominoType.I, ResourceLoader.Load("res://Resources/I_PieceData.tres") },
        { TetrominoType.J, ResourceLoader.Load("res://Resources/J_PieceData.tres") },
        { TetrominoType.L, ResourceLoader.Load("res://Resources/L_PieceData.tres") },
        { TetrominoType.O, ResourceLoader.Load("res://Resources/O_PieceData.tres") },
        { TetrominoType.S, ResourceLoader.Load("res://Resources/S_PieceData.tres") },
        { TetrominoType.T, ResourceLoader.Load("res://Resources/T_PieceData.tres") },
        { TetrominoType.Z, ResourceLoader.Load("res://Resources/Z_PieceData.tres") }
    };

    public static readonly Vector2[] ClockwiseRotationMatrix = [new Vector2(0, -1), new Vector2(1, 0)];
    public static readonly Vector2[] CounterClockwiseRotationMatrix = [new Vector2(0, 1), new Vector2(-1, 0)];

    public static class PlayerInputs
    {
        public const string Right = "Right";
        public const string Left = "Left";
        public const string Down = "Down";
        public const string Drop = "Drop";
        public const string Rotate = "Rotate";
    }
}
