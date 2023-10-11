namespace Jetris.Scripts.Models;

using Godot;
using System;
using System.Linq;

/// <summary>
/// Node responsible for handling the difficulty of <see cref="Game"/>.
/// </summary>
[Tool]
public abstract partial class Difficulty : Node
{
    /// <summary>
    /// Timer that decides when next move should be forced.
    /// </summary>
    [Export]
    public Timer MoveTimer { get; set; }

    /// <summary>
    /// Private handle for <see cref="Current"/>.
    /// </summary>
    private float _current;

    /// <summary>
    /// Current active difficulty. Is a value between <see cref="Min"/> and <see cref="Max"/>.
    /// </summary>
    public float Current
    {
        get { return _current; }
        protected set
        {
            if (value < Min || value > Max)
                throw new Exception($"Difficulty must be between {Min} and {Max}.");
            _current = value;
        }
    }

    /// <summary>
    /// Minimum value allowed for difficulty.
    /// </summary>
    public float Min => 1;

    /// <summary>
    /// Maximum value allowed for difficulty.
    /// </summary>
    public float Max => 10;

    /// <summary>
    /// Current wait time that matches <see cref="Current"/> difficulty.
    /// </summary>
    public float WaitTime => 1 / Current;

    /// <summary>
    /// Function that should be called at _Ready function.
    /// </summary>
    /// <param name="onMoveTimerTimeout">Action that happens when <see cref="MoveTimer.Timeout"/> is reached.</param>
    public void OnReady(Action onMoveTimerTimeout)
    {
        MoveTimer.WaitTime = WaitTime;
        MoveTimer.Timeout += onMoveTimerTimeout;
        MoveTimer.Start();
    }

    /// <summary>
    /// Set configuration warnings.
    /// </summary>
    /// <returns></returns>
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = base._GetConfigurationWarnings();
        if (MoveTimer == null)
            warnings = warnings.Append($"Please set an instance of {nameof(MoveTimer)}").ToArray();

        return warnings;
    }

    /// <summary>
    /// Function that should be called at _Process function.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
    public virtual void OnProcess(float delta)
    {

    }
}