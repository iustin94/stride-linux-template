using Stride.Core.Mathematics;
using Stride.UI;
using Stride.UI.Panels;

namespace StrideTemplate.UI.Screens;

public class PauseScreen : MenuScreen
{
    private readonly Action _onResume;
    private readonly Action _onMainMenu;

    public PauseScreen(Action onResume, Action onMainMenu)
    {
        _onResume = onResume;
        _onMainMenu = onMainMenu;
    }

    protected override UIElement BuildLayout()
    {
        var title = UIHelper.CreateText("PAUSED", 42f);
        var resumeButton = UIHelper.CreateButton("Resume", _onResume, 24f);
        var menuButton = UIHelper.CreateButton("Main Menu", _onMainMenu, 24f);

        var stack = UIHelper.CreateVerticalStack(20f, title, resumeButton, menuButton);

        // Dark overlay background
        var overlay = new Grid
        {
            BackgroundColor = new Color(0, 0, 0, 150)
        };
        stack.HorizontalAlignment = HorizontalAlignment.Center;
        stack.VerticalAlignment = VerticalAlignment.Center;
        overlay.Children.Add(stack);
        return overlay;
    }
}
