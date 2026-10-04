using Stride.Core.Mathematics;
using Stride.UI;

namespace StrideTemplate.UI.Screens;

public class MainMenuScreen : MenuScreen
{
    private readonly Action _onStart;
    private readonly Action _onQuit;

    public MainMenuScreen(Action onStart, Action onQuit)
    {
        _onStart = onStart;
        _onQuit = onQuit;
    }

    protected override UIElement BuildLayout()
    {
        var title = UIHelper.CreateText("SPACE INVADERS", 48f, new Color(0, 200, 80));
        var startButton = UIHelper.CreateButton("Start Game", _onStart, 28f);
        var quitButton = UIHelper.CreateButton("Quit", _onQuit, 28f);

        var stack = UIHelper.CreateVerticalStack(20f, title, startButton, quitButton);
        return UIHelper.CreateCenteredContainer(stack);
    }
}
