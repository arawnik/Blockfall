namespace Blockfall.Scripts;

using System;
using Godot;

public partial class LevelBlock : PanelContainer
{
    /// <summary>
    /// Label for name of level.
    /// </summary>
    public Label LevelNameLabel;

    /// <summary>
    /// Label for difficulty text.
    /// </summary>
    public Label DifficultyTextLabel;

    /// <summary>
    /// Label for addition to difficulty text.
    /// </summary>
    public Label LoseConditionTextLabel;

    /// <summary>
    /// Label for displaying text for best score.
    /// </summary>
    public Label BestScoreTextLabel;

    /// <summary>
    /// Label for displaying best score.
    /// </summary>
    public Label BestScoreLabel;

    /// <summary>
    /// Action for activating the level on press.
    /// </summary>
    public Action ActivateAction { get; internal set; }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        LevelNameLabel = GetNode<Label>(Resources.LevelNameLabel);
        DifficultyTextLabel = GetNode<Label>(Resources.DifficultyTextLabel);
        LoseConditionTextLabel = GetNode<Label>(Resources.LoseConditionTextLabel);
        BestScoreLabel = GetNode<Label>(Resources.BestScoreLabel);
        BestScoreTextLabel = GetNode<Label>(Resources.BestScoreTextLabel);
    }

    /// <summary>
    /// Called when Gui input happens.
    /// </summary>
    /// <param name="event">The input event.</param>
    public void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                ActivateAction();
            }
        }
    }

    public static class Resources
    {
        public const string LevelNameLabel = "MarginContainer/VBoxContainer/LevelNameLabel";

        public const string DifficultyTextLabel =
            "MarginContainer/VBoxContainer/DifficultyBox/DifficultyTextLabel";
        public const string LoseConditionTextLabel =
            "MarginContainer/VBoxContainer/DifficultyBox/LoseConditionTextLabel";

        public const string BestScoreLabel =
            "MarginContainer/VBoxContainer/BestScoreBox/BestScoreLabel";
        public const string BestScoreTextLabel =
            "MarginContainer/VBoxContainer/BestScoreBox/BestScoreTextLabel";
    }
}
