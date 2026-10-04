using Stride.Engine;
using Stride.Games;
using StrideTemplate.Input;
using StrideTemplate.Scenes;
using StrideTemplate.UI.Screens;

namespace StrideTemplate.Core;

public enum GamePhase { MainMenu, Gameplay, Paused, GameOver }

public class GameManager
{
    public GamePhase CurrentPhase { get; private set; } = GamePhase.MainMenu;
    public InputMap InputMap { get; } = new();

    private readonly SceneManager _sceneManager;
    private readonly Game _game;
    private readonly Scene _rootScene;

    private GameState? _gameState;
    private PauseScreen? _pauseScreen;
    private GameOverScreen? _gameOverScreen;

    public GameManager(SceneManager sceneManager, Game game, Scene rootScene)
    {
        _sceneManager = sceneManager;
        _game = game;
        _rootScene = rootScene;
    }

    public void GoToMainMenu()
    {
        HideOverlays();
        CurrentPhase = GamePhase.MainMenu;
        _gameState = null;
        _sceneManager.LoadScene(new MainMenuScene(
            onStart: StartGame,
            onQuit: () => Environment.Exit(0)
        ), fade: true);
    }

    public void StartGame()
    {
        HideOverlays();
        CurrentPhase = GamePhase.Gameplay;
        _gameState = new GameState();
        _gameState.Reset();
        _sceneManager.LoadScene(new GameplayScene(
            _game, InputMap, _gameState, HandleGameOver
        ), fade: true);
    }

    public void Update(GameTime time)
    {
        // Check pause input during gameplay
        if (CurrentPhase == GamePhase.Gameplay && InputMap.IsPressed(_game.Input, InputAction.Pause))
        {
            CurrentPhase = GamePhase.Paused;
            _pauseScreen ??= new PauseScreen(
                onResume: Resume,
                onMainMenu: GoToMainMenu
            );
            _pauseScreen.Show(_rootScene);
            return;
        }

        // Check resume input during pause
        if (CurrentPhase == GamePhase.Paused && InputMap.IsPressed(_game.Input, InputAction.Pause))
        {
            Resume();
            return;
        }

        // Only update scene when not paused and not game over
        if (CurrentPhase is GamePhase.Gameplay or GamePhase.MainMenu)
            _sceneManager.Update(time);

        // Still tick scene manager for fade during other states
        if (CurrentPhase is GamePhase.GameOver or GamePhase.Paused)
            _sceneManager.Update(time);
    }

    private void Resume()
    {
        CurrentPhase = GamePhase.Gameplay;
        _pauseScreen?.Hide();
    }

    private void HandleGameOver()
    {
        CurrentPhase = GamePhase.GameOver;
        _gameOverScreen = new GameOverScreen(
            _gameState!,
            onRestart: StartGame,
            onMainMenu: GoToMainMenu
        );
        _gameOverScreen.Show(_rootScene);
    }

    private void HideOverlays()
    {
        _pauseScreen?.Hide();
        _gameOverScreen?.Hide();
    }
}
