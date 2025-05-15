namespace Blockfall.Scripts;

using System;
using Godot;

/// <summary>
/// Predefined line that is added to board before start. Mainly used for campaign levels.
/// </summary>
public partial class PredefinedLine : Node2D
{
    /// <summary>
    /// Private handle for <see cref="Row"/>.
    /// </summary>
    private int _row;

    /// <summary>
    /// Row that matching <see cref="Line"/> should be added to.
    /// </summary>
    [Export(PropertyHint.Range, "0,20,")]
    public int Row
    {
        get { return _row; }
        set
        {
            if (value < 0 || value > Board.ROW_COUNT)
                throw new Exception($"Invalid row, Row has to be betwee 0 and {Board.ROW_COUNT}");
            _row = value;
        }
    }

    /// <summary>
    /// Pieces on the created <see cref="Line"/>. 0 being leftmost <see cref="Piece"/>, and 11 on the right.
    /// </summary>
    [Export(PropertyHint.Range, "0,11,")]
    public int[] Pieces { get; set; }

    public static class Resources
    {
        public const string PresetPieceSprite = "res://Assets/Pieces/Preset.png";

        public const string Piece = "res://Scenes/piece.tscn";
    }
}
