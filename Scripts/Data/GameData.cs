namespace Blockfall.Scripts.Save;

using System.Collections.Generic;
using System.Linq;
using Blockfall.Scripts.Data;
using Godot;
using SoulNETLib.Common.Extension;

/// <summary>
/// Resource to handle save game.
/// </summary>
public partial class GameData : Resource
{
    /// <summary>
    /// Best score for Vanilla game mode.
    /// </summary>
    [Export]
    public int VanillaBestScore { get; private set; } = 0;

    /// <summary>
    /// Best score for Increasing difficulty game mode.
    /// </summary>
    [Export]
    public int IncreasingDifficultyBestScore { get; private set; } = 0;

    /// <summary>
    /// Best scores for campaign levels.
    /// </summary>
    [Export]
    public Godot.Collections.Dictionary<string, int> CampaignBestScores { get; set; } = [];

    /// <summary>
    /// Information related to campaign levels, and current state.
    /// </summary>
    [Export]
    public CampaignLevels CampaignLevels { get; private set; } = new();

    /// <summary>
    /// Path where game data is stored.
    /// </summary>
    private const string GAME_DATA_PATH = "user://gamedata.tres";

    /// <summary>
    /// Update <see cref="VanillaBestScore"/>.
    /// </summary>
    /// <param name="newBestScore">New best score.</param>
    public void UpdateVanillaBestScore(int newBestScore)
    {
        VanillaBestScore = newBestScore;
        this.Save();
    }

    /// <summary>
    /// Update <see cref="IncreasingDifficultyBestScore"/>.
    /// </summary>
    /// <param name="newBestScore">New best score.</param>
    public void UpdateIncreasingDifficultyBestScore(int newBestScore)
    {
        IncreasingDifficultyBestScore = newBestScore;
        this.Save();
    }

    /// <summary>
    /// Update campaign level best score for <paramref name="levelName"/>.
    /// </summary>
    /// <param name="levelName">Name of the campaign level.</param>
    /// <param name="newBestScore">New best score.</param>
    public void UpdateCampaignBestScore(string levelName, int newBestScore)
    {
        CampaignBestScores[levelName] = newBestScore;
        Save();
    }

    /// <summary>
    /// Best score for <paramref name="levelName"/> campaign level.
    /// </summary>
    /// <param name="levelName">Name of the level that is checked.</param>
    /// <returns>Current best score that matches <paramref name="levelName"/>.</returns>
    public int CampaignBestScore(string levelName)
    {
        if (!CampaignBestScores.TryGetValue(levelName, out int value))
        {
            value = 0;
            CampaignBestScores[levelName] = value;
        }
        return value;
    }

    /// <summary>
    /// Best score for currently active campaign best score.
    /// </summary>
    public List<KeyValuePair<string, int>> CampaignClearedLevels()
    {
        return [.. CampaignBestScores.OrderBy(level => level.Key.GetDigitsInt())];
    }

    /// <summary>
    /// Commit save to Game data.
    /// </summary>
    public void Save()
    {
        ResourceSaver.Save(this, GAME_DATA_PATH);
    }

    /// <summary>
    /// Load gamedata from last commit.
    /// </summary>
    /// <returns>Loaded / new GameData.</returns>
    public static GameData Load()
    {
        if (ResourceLoader.Exists(GAME_DATA_PATH))
        {
            return ResourceLoader.Load<GameData>(GAME_DATA_PATH);
        }
        var gameData = new GameData();
        gameData.Save();
        return gameData;
    }

    /// <summary>
    /// Delegate for updating best score for category.
    /// </summary>
    /// <param name="newBestScore">New best score.</param>
    public delegate void UpdateBestScore(int newBestScore);
}
