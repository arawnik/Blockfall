namespace Blockfall.Scripts.Levels;

using Godot;

/// <summary>
/// Script for intro scene on campaign.
/// </summary>
public partial class IntroScene : Node2D
{
    /// <summary>
    /// Handle Advance button pressed event.
    /// </summary>
	public void OnAdvanceButtonPressed()
	{
        var main = GetParent<Main>();
        main.CampaignAdvanceCurrent();
        main.CampaignCurrentLevel();
    }
}
