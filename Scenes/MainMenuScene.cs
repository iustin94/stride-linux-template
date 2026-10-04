using Stride.Core;
using Stride.Engine;
using Stride.Games;
using StrideTemplate.UI.Screens;

namespace StrideTemplate.Scenes;

public class MainMenuScene : IGameScene
{
    private readonly MainMenuScreen _menuScreen;

    public MainMenuScene(Action onStart, Action onQuit)
    {
        _menuScreen = new MainMenuScreen(onStart, onQuit);
    }

    public void Load(Scene rootScene, IServiceRegistry services)
    {
        _menuScreen.Show(rootScene);
    }

    public void Update(GameTime time)
    {
    }

    public void Unload(Scene rootScene)
    {
        _menuScreen.Hide();
    }
}
