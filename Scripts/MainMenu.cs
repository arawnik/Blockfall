namespace Blockfall.Scripts;

using Godot;

/// <summary>
/// The main menu of app.
/// </summary>
public partial class MainMenu : CanvasLayer
{
	/// <summary>
	/// Handler for when CampaignButton is pressed.
	/// </summary>
	public void OnCampaignButtonPressed()
	{
		GetParent<Main>().UpdateAppState(Models.AppState.Campaign);
	}

	/// <summary>
	/// Handler for when LevelButton is pressed.
	/// </summary>
	public void OnLevelButtonPressed()
	{
		GetParent<Main>().UpdateAppState(Models.AppState.LevelSelect);
	}

	/// <summary>
	/// Handler for when VanillaButton is pressed.
	/// </summary>
	public void OnVanillaButtonPressed()
	{
		GetParent<Main>().UpdateAppState(Models.AppState.GameVanilla);
	}

	/// <summary>
	/// Handler for when IncreasingDifficultyButton is pressed.
	/// </summary>
	public void OnIncreasingDifficultyButtonPressed()
	{
		GetParent<Main>().UpdateAppState(Models.AppState.GameIncreasingDifficulty);
	}
}
