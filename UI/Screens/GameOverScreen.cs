using Stride.Core.Mathematics;
using Stride.UI;

namespace StrideTemplate.UI.Screens;

public class GameOverScreen : MenuScreen
{
    private readonly Core.GameState _gameState;
    private readonly Action _onRestart;
    private readonly Action _onMainMenu;

    public GameOverScreen(Core.GameState gameState, Action onRestart, Action onMainMenu)
    {
        _gameState = gameState;
        _onRestart = onRestart;
        _onMainMenu = onMainMenu;
    }

    protected override UIElement BuildLayout()
    {
        var titleText = _gameState.IsVictory ? "YOU WIN!" : "GAME OVER";
        var titleColor = _gameState.IsVictory ? new Color(0, 200, 80) : new Color(220, 40, 40);

        var title = UIHelper.CreateText(titleText, 48f, titleColor);
        var score = UIHelper.CreateText($"Score: {_gameState.Score}", 28f);
        var restartButton = UIHelper.CreateButton("Play Again", _onRestart, 24f);
        var menuButton = UIHelper.CreateButton("Main Menu", _onMainMenu, 24f);

        var stack = UIHelper.CreateVerticalStack(20f, title, score, restartButton, menuButton);
        return UIHelper.CreateCenteredContainer(stack);
    }
}
