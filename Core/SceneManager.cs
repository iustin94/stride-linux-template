using Stride.Core;
using Stride.Engine;
using Stride.Games;
using StrideTemplate.Scenes;
using StrideTemplate.UI;

namespace StrideTemplate.Core;

public class SceneManager
{
    private readonly Scene _rootScene;
    private readonly IServiceRegistry _services;
    private readonly FadeOverlay _fade = new();
    private IGameScene? _currentScene;
    private IGameScene? _pendingScene;

    public SceneManager(Scene rootScene, IServiceRegistry services)
    {
        _rootScene = rootScene;
        _services = services;
        _fade.Show(rootScene);
    }

    public void LoadScene(IGameScene newScene, bool fade = false)
    {
        if (fade)
        {
            _pendingScene = newScene;
            _fade.StartFade(0.3f, 0.3f, () =>
            {
                _currentScene?.Unload(_rootScene);
                _currentScene = _pendingScene;
                _currentScene?.Load(_rootScene, _services);
                _pendingScene = null;
            });
        }
        else
        {
            _currentScene?.Unload(_rootScene);
            _currentScene = newScene;
            _currentScene.Load(_rootScene, _services);
        }
    }

    public void Update(GameTime time)
    {
        var dt = (float)time.Elapsed.TotalSeconds;
        _fade.Update(dt);

        if (!_fade.IsActive)
            _currentScene?.Update(time);
    }
}
