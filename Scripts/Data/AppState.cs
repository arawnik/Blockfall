namespace Blockfall.Scripts.Data;

/// <summary>
/// State of the app. Handled by <see cref="Main"/>.
/// </summary>
public enum AppState
{
    MainMenu,
    Campaign,
    LevelSelect,
    GameVanilla,
    GameIncreasingDifficulty,
}
