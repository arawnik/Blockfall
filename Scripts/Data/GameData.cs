namespace Jetris.Scripts.Save;

using Godot;
using Jetris.Scripts.Data;

/// <summary>
/// Resource to handle save game.
/// </summary>
public partial class GameData : Resource
{
    /// <summary>
    /// Highscore for Vanilla game mode.
    /// </summary>
    [Export]
    public int VanillaHighScore { get; private set; } = 0;

    /// <summary>
    /// Highscore for Increasing difficulty game mode.
    /// </summary>
    [Export]
    public int IncreasingDifficultyHighScore { get; private set; } = 0;

    /// <summary>
    /// Highscores for campaign levels.
    /// </summary>
    [Export]
    public Godot.Collections.Dictionary<string, int> CampaignHighScores { get; set; } = new();

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
    /// Check if <see cref="VanillaHighScore"/> should be updated.
    /// </summary>
    /// <param name="currentPoints">Current points to check.</param>
    /// <returns><see cref="true"/> if <paramref name="currentPoints"/> is new <see cref="VanillaHighScore"/>, <see cref="false"/> otherwise.</returns>
    public bool CheckVanillaHighScore(int currentPoints)
    {
        if (currentPoints > VanillaHighScore)
        {
            VanillaHighScore = currentPoints;
            this.Save();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if <see cref="IncreasingDifficultyHighScore"/> should be updated.
    /// </summary>
    /// <param name="currentPoints">Current points to check.</param>
    /// <returns><see cref="true"/> if <paramref name="currentPoints"/> is new <see cref="IncreasingDifficultyHighScore"/>, <see cref="false"/> otherwise.</returns>
    public bool CheckIncreasingDifficultyHighScore(int currentPoints)
    {
        if (currentPoints > IncreasingDifficultyHighScore)
        {
            IncreasingDifficultyHighScore = currentPoints;
            this.Save();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if campaign level's highscore should be updated.
    /// </summary>
    /// <param name="currentPoints">Current points to check.</param>
    /// <returns><see cref="true"/> if <paramref name="currentPoints"/> is new highscore for campaign level, <see cref="false"/> otherwise.</returns>
    public bool CheckCampaignHighScore(int currentPoints)
    {
        if (currentPoints > CurrentCampaignHighScore())
        {
            CampaignHighScores[CampaignLevels.CurrentLevel] = currentPoints;
            Save();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Highscore for currently active campaign highscore.
    /// </summary>
    public int CurrentCampaignHighScore()
    {
        if(!CampaignHighScores.ContainsKey(CampaignLevels.CurrentLevel))
        {
            CampaignHighScores[CampaignLevels.CurrentLevel] = 0;
        }
        return CampaignHighScores[CampaignLevels.CurrentLevel];
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


    public delegate bool CheckHighScore(int currentScore);
}
