namespace Jetris.Scripts;

using Godot;
using Jetris.Scripts.Models;
using Jetris.Scripts.Save;
using static Jetris.Scripts.Save.GameData;

/// <summary>
/// The main frame for the game, we handle global stuff and states here.
/// </summary>
public partial class Main : Node2D
{
    /// <summary>
    /// Scene that is used to instantiate new <see cref="Game"/>.
    /// </summary>
	[Export]
    public PackedScene GameScene { get; set; }

    /// <summary>
    /// Scene that is used to instantiate new <see cref="MainMenu"/>.
    /// </summary>
	[Export]
    public PackedScene MainMenuScene { get; set; }

    /// <summary>
    /// Game data. Keeps track of everything.
    /// </summary>
    private GameData GameData;

    /// <summary>
    /// Currently active gamestate.
    /// </summary>
    public AppState CurrentAppState { get; private set; }

    /// <summary>
    /// Currently active node. Exists directly under Main.
    /// </summary>
    private Node _activeNode;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        GameData = GameData.Load();

        UpdateAppState(AppState.MainMenu, true);
    }

    /// <summary>
    /// Update the current gamestate. Unloads old and loads in new state scene.
    /// </summary>
    /// <param name="gameState">The new activated <see cref="AppState"/>.</param>
    /// <param name="force">If <see cref="true"/>, always resets <see cref="_activeNode"/>. Otherwise only reload when state changes.</param>
    public void UpdateAppState(AppState gameState, bool force = false)
    {
        if (gameState == CurrentAppState && !force)
            return;
        CurrentAppState = gameState;

        switch (gameState)
        {
            case AppState.MainMenu:
                ActivateNode(MainMenuScene.Instantiate<MainMenu>());
                break;
            case AppState.Campaign:
                CampaignCurrentLevel();
                break;
            case AppState.GameVanilla:
                GameVanillaRestart();
                break;
            case AppState.GameIncreasingDifficulty:
                GameIncreasingDifficultyRestart();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Function for HUD button to return to <see cref="AppState.MainMenu"/>.
    /// </summary>
    public void OnMenuState()
    {
        UpdateAppState(AppState.MainMenu);
    }

    /// <summary>
    /// Function for advancing from HUD.
    /// </summary>
    public void OnAdvanceState()
    {
        switch (CurrentAppState)
        {
            case AppState.Campaign:
                CampaignNextLevel();
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Function for restarting from HUD.
    /// </summary>
    public void OnRestartState()
    {
        switch (CurrentAppState)
        {
            case AppState.Campaign:
                CampaignCurrentLevel();
                break;
            case AppState.GameVanilla:
                GameVanillaRestart();
                break;
            case AppState.GameIncreasingDifficulty:
                GameIncreasingDifficultyRestart();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Move to next campaign level and start it.
    /// </summary>
    public void CampaignNextLevel()
    {
        var scene = GameData.CampaignLevels.Next;
        var isBoard = GameData.CampaignLevels.CurrentIsBoard;
        ActivateCampaignGame(scene, isBoard);
    }

    /// <summary>
    /// Start current campaign level.
    /// </summary>
    public void CampaignCurrentLevel()
    {
        var scene = GameData.CampaignLevels.Current;
        var isBoard = GameData.CampaignLevels.CurrentIsBoard;
        ActivateCampaignGame(scene, isBoard);
    }

    /// <summary>
    /// Restart campaign.
    /// </summary>
    public void RestartCampaign()
    {
        GameData.CampaignLevels.Restart();
        CampaignCurrentLevel();
    }

    /// <summary>
    /// Restart <see cref="AppState.GameVanilla"/>.
    /// </summary>
    public void GameVanillaRestart()
    {
        var boardScene = ResourceLoader.Load<PackedScene>(Resources.BoardVanillaScene);
        var board = boardScene.Instantiate<Board>();
        ActivateGame(board, GameData.VanillaHighScore, GameData.CheckVanillaHighScore);
    }

    /// <summary>
    /// Restart <see cref="AppState.GameIncreasingDifficulty"/>.
    /// </summary>
    public void GameIncreasingDifficultyRestart()
    {
        var boardScene = ResourceLoader.Load<PackedScene>(Resources.BoardIncreasingDifficultyScene);
        var board = boardScene.Instantiate<Board>();
        ActivateGame(board, GameData.IncreasingDifficultyHighScore, GameData.CheckIncreasingDifficultyHighScore);
    }

    /// <summary>
    /// Activate <paramref name="node"/> as <see cref="_activeNode"/> under Main.
    /// </summary>
    /// <param name="node"></param>
    private void ActivateNode(Node node)
    {
        _activeNode?.QueueFree();
        _activeNode = node;
        AddChild(_activeNode);
    }

    /// <summary>
    /// Activate a campaign game.
    /// </summary>
    /// <param name="scene">Scene that should be activated, either scene or board, depending on <paramref name="isBoard"/>.</param>
    /// <param name="isBoard">if <see cref="true"/> <paramref name="scene"/> is a <see cref="Board"/>. Otherwise it's a Scene.</param>
    /// <returns>Initialized <see cref="Node"/></returns>
    private void ActivateCampaignGame(PackedScene scene, bool isBoard)
    {
        GameData.Save();
        if (isBoard)
        {
            var board = scene.Instantiate<Board>();
            ActivateGame(board, GameData.CurrentCampaignHighScore(), GameData.CheckCampaignHighScore);
        } else
        {
            ActivateNode(scene.Instantiate());
        }
    }

    /// <summary>
    /// Init's <see cref="Game"/> with <paramref name="board"/> and linking to Main.
    /// </summary>
    /// <param name="board">Active board for game.</param>
    /// <param name="highScore">Highscore for <paramref name="board"/>.</param>
    /// <returns>Initialized <see cref="Game"/></returns>
    private void ActivateGame(Board board, int highScore, CheckHighScore checkHighScore)
    {
        var game = Game.Create(GameScene, board, highScore, checkHighScore);
        ActivateNode(game);
        game.HUD.RestartState += OnRestartState;
        game.HUD.AdvanceState += OnAdvanceState;
        game.HUD.MenuState += OnMenuState;
    }

    public static class Resources
    {
        public const string BoardVanillaScene = "res://Scenes/board_vanilla.tscn";
        public const string BoardIncreasingDifficultyScene = "res://Scenes/board_increasing_difficulty.tscn";
    }
}
