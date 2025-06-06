﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using SoulNETLib.Common.Extension;

namespace Blockfall.Scripts.Data;

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
        get { return this[CurrentLevel]; }
    }

    /// <summary>
    /// Next campaign level scene. Also update <see cref="CurrentLevel"/> into next.
    /// </summary>
    public PackedScene Next
    {
        get
        {
            Advance();
            return this[CurrentLevel];
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
    /// Move campaign to next level.
    /// </summary>
    public void Advance()
    {
        int index = Array.IndexOf(_levels, CurrentLevel);
        var next = _levels[index + 1];
        if (next != null)
        {
            CurrentLevel = next;
        }
    }

    /// <summary>
    /// All levels keys.
    /// </summary>
    public static IEnumerable<string> Keys => _levels;

    /// <summary>
    /// Amount of levels.
    /// </summary>
    public static int Count => _levels.Length;

    /// <summary>
    /// Get Campaign level by key.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <returns>Campaign level scene.</returns>
    public PackedScene this[string key] => GetValue(key);

    /// <summary>
    /// Check if campaign level by <see cref="key"/> exists.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <returns><see cref="true"/> if campaign level by <see cref="key"/> exists, <see cref="false"/> otherwise.</returns>
    public static bool ContainsKey(string key)
    {
        return _levels.Contains(key);
    }

    /// <summary>
    /// Try get campaign level by <see cref="key"/>.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <param name="value">Campaign level scene, or <see cref="null"/> if it doesn't exist.</param>
    /// <returns><see cref="true"/> if campaign level by <see cref="key"/> exists, <see cref="false"/> otherwise.</returns>
    public static bool TryGetValue(string key, [MaybeNullWhen(false)] out PackedScene value)
    {
        value = GetValue(key);
        return value != null;
    }

    /// <summary>
    /// Get campaign level by <see cref="key"/>.
    /// </summary>
    /// <param name="key">The key of level.</param>
    /// <returns>Campaign level scene.</returns>
    private static PackedScene GetValue(string key)
    {
        return ContainsKey(key)
            ? ResourceLoader.Load<PackedScene>($"res://Scenes/Levels/{key}.tscn")
            : null;
    }

    /// <summary>
    /// Turn level string into a parsed name
    /// </summary>
    /// <param name="levelText">The string that will be parsed</param>
    /// <returns>Parsed <paramref name="levelText"/>.</returns>
    public static string LevelToName(string levelText)
    {
        if (levelText.TryRemoveEnd("_board", out var boardText))
        {
            boardText = boardText.FirstCharToUpper();
            boardText = boardText.Replace('_', ' ');
            return boardText;
        }

        if (levelText.TryRemoveEnd("_scene", out var sceneText))
        {
            sceneText = sceneText.FirstCharToUpper();
            return sceneText;
        }
        return string.Empty;
    }

    /// <summary>
    /// All level names in order.
    /// Levels should end with _board if they are game board.
    /// There has to be a scene that matches each string on the list in "Scenes/Levels" folder.
    /// </summary>
    private static readonly string[] _levels =
    [
        "intro_scene",
        // Preset with increasing difficulty
        "level_1_board",
        "level_2_board",
        "level_3_board",
        "level_4_board",
        "level_5_board",
        "level_6_board",
        "level_7_board",
        "level_8_board",
        "level_9_board",
        "level_10_board",
        // Preset with limited pawns
        "level_11_board",
        "level_12_board",
        "level_13_board",
        "level_14_board",
        "level_15_board",
        "level_16_board",
        "level_17_board",
        "level_18_board",
        "level_19_board",
        "level_20_board",
        //TODO: Move down preset increasing difficulty
        // Next difficulty
        //"level_x_board",
        "outro_scene",
    ];
}
