namespace Blockfall.Scripts.Models;

using System;
using Godot;

/// <summary>
/// Difficulty that starts from <see cref="InitialDifficulty"/> and will increasy over time depending on <see cref="DeltaMultiplier"/>.
/// </summary>
public partial class DifficultyIncreasing : Difficulty
{
    /// <summary>
    /// Starting value for difficulty
    /// </summary>
    [Export]
    public float InitialDifficulty { get; private set; } = 1;

    /// <summary>
    /// Multiplier for how fast difficulty will increase.
    /// </summary>
    [Export]
    public float DeltaMultiplier { get; private set; } = 0.01f;

    /// <summary>
    ///  Displayed difficulty description.
    /// </summary>
    public override string DifficultyText => "Rising difficulty";

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        Current = InitialDifficulty;
    }

    /// <summary>
    /// Function that should be called at _Process function.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
    public override void OnProcess(float delta)
    {
        if (Current >= Max)
            return;
        Current = Math.Clamp(Current + delta * DeltaMultiplier, Min, Max);
        MoveTimer.WaitTime = WaitTime;
    }
}
