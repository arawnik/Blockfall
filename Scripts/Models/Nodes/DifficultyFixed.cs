namespace Blockfall.Scripts.Models;

using Godot;

/// <summary>
/// Difficulty that will always be fixed to <see cref="Difficulty"/>.
/// </summary>
public partial class DifficultyFixed : Difficulty
{
    /// <summary>
    /// The fixed difficulty value.
    /// </summary>
    [Export]
    public float Difficulty { get; private set; } = 1;

    /// <summary>
    ///  Displayed difficulty description.
    /// </summary>
    public override string DifficultyText => "Fixed difficulty";

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        Current = Difficulty;
    }
}