namespace Blockfall.Scripts;

using System;
using System.Collections.Generic;
using System.Linq;
using Blockfall.Scripts.Data;
using Blockfall.Scripts.Models;
using Blockfall.Scripts.Models.Nodes;
using Godot;

public partial class LevelSelect : CanvasLayer
{
    /// <summary>
    /// Scene that is used to instantiate new <see cref="LevelBlock"/>.
    /// </summary>
    [Export]
    public PackedScene LevelBlockScene { get; set; }

    /// <summary>
    /// GridContainer that holds all the <see cref="LevelBlock"/>s.
    /// </summary>
    protected GridContainer LevelsGridContainer;

    /// <summary>
    /// All levels that will be displayed.
    /// </summary>
    public List<KeyValuePair<string, int>> Levels { get; set; }

    /// <summary>
    /// Action used for activating the level.
    /// </summary>
    public Action<PackedScene, string, bool> ActivateAction { get; internal set; }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        LevelsGridContainer = GetNode<GridContainer>(Resources.LevelsGridContainer);

        foreach (var level in Levels)
        {
            if (CampaignLevels.TryGetValue(level.Key, out var scene))
            {
                var difficultyNode = scene
                    .Instantiate()
                    .GetChildren()
                    .Where(l => l is Difficulty)
                    .Cast<Difficulty>()
                    .FirstOrDefault();
                var gameRulesNode = scene
                    .Instantiate()
                    .GetChildren()
                    .Where(l => l is GameRules)
                    .Cast<GameRules>()
                    .FirstOrDefault();

                var newLevelBlock = LevelBlockScene.Instantiate<LevelBlock>();
                LevelsGridContainer.AddChild(newLevelBlock);

                newLevelBlock.ActivateAction = () => ActivateAction(scene, level.Key, true);

                newLevelBlock.DifficultyTextLabel.Text = difficultyNode.DifficultyText;
                newLevelBlock.LoseConditionTextLabel.Text = gameRulesNode
                    .LoseCondition
                    .DifficultyExtensionText;

                newLevelBlock.BestScoreLabel.Text = gameRulesNode.LoseCondition.BestScoreText;

                newLevelBlock.LevelNameLabel.Text = CampaignLevels.LevelToName(level.Key);
                newLevelBlock.BestScoreTextLabel.Text = level.Value.ToString();
            }
        }
    }

    /// <summary>
    /// Signal handler for back to menu button.
    /// </summary>
    public void OnBackToMenuButtonPressed()
    {
        GetParent<Main>().UpdateAppState(AppState.MainMenu);
    }

    public static class Resources
    {
        public const string LevelsGridContainer =
            "ScrollContainer/CenterContainer/VBoxContainer/LevelsGridContainer";

        //public const string LevelBlockScene = "res://Scenes/level_block.tscn";
    }
}
