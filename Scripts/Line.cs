namespace Blockfall.Scripts;

using Godot;

/// <summary>
/// A line of <see cref="Piece"/>s that are locked on <see cref="Board"/>.
/// </summary>
public partial class Line : Node2D
{
    /// <summary>
    /// Check if line is full.
    /// </summary>
    /// <returns><see cref="true"/> if line is full, <see cref="false"/> otherwise.</returns>
    public bool IsLineFull()
    {
        return GetChildCount() == Board.COLUMN_COUNT;
    }
}
