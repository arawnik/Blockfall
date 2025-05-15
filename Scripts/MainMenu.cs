namespace Blockfall.Scripts;

using Blockfall.Scripts.Data;
using Godot;

/// <summary>
/// The main menu of app. Display choises for different game modes.
/// </summary>
public partial class MainMenu : CanvasLayer
{
    /// <summary>
    /// Handler for when CampaignButton is pressed.
    /// </summary>
    public void OnCampaignButtonPressed()
    {
        GetParent<Main>().UpdateAppState(AppState.Campaign);
    }

    /// <summary>
    /// Handler for when LevelButton is pressed.
    /// </summary>
    public void OnLevelButtonPressed()
    {
        GetParent<Main>().UpdateAppState(AppState.LevelSelect);
    }

    /// <summary>
    /// Handler for when VanillaButton is pressed.
    /// </summary>
    public void OnVanillaButtonPressed()
    {
        GetParent<Main>().UpdateAppState(AppState.GameVanilla);
    }

    /// <summary>
    /// Handler for when IncreasingDifficultyButton is pressed.
    /// </summary>
    public void OnIncreasingDifficultyButtonPressed()
    {
        GetParent<Main>().UpdateAppState(AppState.GameIncreasingDifficulty);
    }
}
