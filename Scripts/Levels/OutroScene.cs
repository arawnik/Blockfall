namespace Jetris.Scripts.Levels;

using Godot;

/// <summary>
/// Script for outro scene on campaign.
/// </summary>
public partial class OutroScene : Node2D
{
    /// <summary>
    /// Handle Restart button pressed event.
    /// </summary>
    public void OnRestartButtonPressed()
    {
        GetParent<Main>().RestartCampaign();
    }

    /// <summary>
    /// Handle Menu button pressed event.
    /// </summary>
    public void OnMenuButtonPressed()
    {
        GetParent<Main>().OnMenuState();
    }
}
