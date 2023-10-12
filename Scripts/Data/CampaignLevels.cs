using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jetris.Scripts.Data;

/// <summary>
/// Resource for handling and storing campaign levels.
/// </summary>
public partial class CampaignLevels : Resource
{
    /// <summary>
    /// Name / index of currently active level.
    /// </summary>
    [Export]
    public string CurrentLevel { get; set; } = _levels[0];

    /// <summary>
    /// Is current level a board of information scene.
    /// </summary>
    public bool CurrentIsBoard => CurrentLevel.EndsWith("_board");

    /// <summary>
    /// Currently active campaign level scene.
    /// </summary>
    public PackedScene Current
    {
        get
        {
            return this[CurrentLevel];
        }
    }

    /// <summary>
    /// Next campaign level scene. Also update <see cref="CurrentLevel"/> into next.
    /// </summary>
    public PackedScene Next
    {
        get
        {
            int index = Array.IndexOf(_levels, CurrentLevel);
            var next = _levels[index + 1];
            if (next != null) {
                CurrentLevel = next;
                return this[next];
            }
            return null;
        }
    }

    /// <summary>
    /// Restart campaign. Reset <see cref="CurrentLevel"/> to first level.
    /// </summary>
    public void Restart()
    {
        CurrentLevel = _levels[0];
    }

    /// <summary>
    /// All levels keys.
    /// </summary>
    public IEnumerable<string> Keys => _levels;

    /// <summary>
    /// Amount of levels.
    /// </summary>
    public int Count => _levels.Length;

    /// <summary>
    /// Get Campaign level by key.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <returns>Campaign level scene.</returns>
    public PackedScene this[string key] => ContainsKey(key)
        ? ResourceLoader.Load<PackedScene>($"res://Scenes/Levels/{key}.tscn")
        : null;

    /// <summary>
    /// Check if campaign level by <see cref="key"/> exists.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <returns><see cref="true"/> if campaign level by <see cref="key"/> exists, <see cref="false"/> otherwise.</returns>
    public bool ContainsKey(string key)
    {
        return _levels.Contains(key);
    }

    /// <summary>
    /// Try get campaign level by <see cref="key"/>.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <param name="value">Campaign level scene, or <see cref="null"/> if it doesn't exist.</param>
    /// <returns><see cref="true"/> if campaign level by <see cref="key"/> exists, <see cref="false"/> otherwise.</returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out PackedScene value)
    {
        value = this[key];
        return value != null;
    }

    /// <summary>
    /// All level names in order. 
    /// Levels should end with _board if they are game board.
    /// There has to be a scene that matches each string on the list in "Scenes/Levels" folder.
    /// </summary>
    private static readonly string[] _levels =
    [
        "intro_scene",
        // Just preset with increasing difficulty
        "level_1_board",
        "level_2_board",
        "level_3_board",
        "level_4_board",
        "level_5_board",
        "level_6_board",
        "level_7_board",
        "level_8_board",
        "level_10_board",
        // Next difficulty
        "level_x_board",
        "outro_scene",
    ];
}
