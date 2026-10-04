using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Panels;

namespace StrideTemplate.UI;

public class FadeOverlay
{
    private readonly Entity _entity;
    private readonly Grid _overlay;
    private readonly Color _color;

    private float _fadeOutDuration;
    private float _fadeInDuration;
    private float _elapsed;
    private Action? _onMidpoint;
    private bool _midpointFired;

    public bool IsActive { get; private set; }

    public FadeOverlay(Color? color = null)
    {
        _color = color ?? Color.Black;
        _overlay = new Grid
        {
            BackgroundColor = _color,
            Opacity = 0f
        };
        _entity = UIHelper.CreateUIEntity("FadeOverlay", _overlay);
    }

    public void StartFade(float fadeOutDuration, float fadeInDuration, Action onMidpoint)
    {
        _fadeOutDuration = fadeOutDuration;
        _fadeInDuration = fadeInDuration;
        _onMidpoint = onMidpoint;
        _midpointFired = false;
        _elapsed = 0f;
        _overlay.Opacity = 0f;
        IsActive = true;
    }

    public void Show(Scene rootScene)
    {
        _entity.Scene = rootScene;
    }

    public void Hide()
    {
        _entity.Scene = null;
    }

    public void Update(float dt)
    {
        if (!IsActive) return;

        _elapsed += dt;
        var totalDuration = _fadeOutDuration + _fadeInDuration;

        if (_elapsed < _fadeOutDuration)
        {
            // Fading out (opacity increasing)
            _overlay.Opacity = _elapsed / _fadeOutDuration;
        }
        else if (!_midpointFired)
        {
            // Midpoint — fully opaque, do the scene swap
            _overlay.Opacity = 1f;
            _midpointFired = true;
            _onMidpoint?.Invoke();
        }
        else if (_elapsed < totalDuration)
        {
            // Fading in (opacity decreasing)
            var fadeInElapsed = _elapsed - _fadeOutDuration;
            _overlay.Opacity = 1f - (fadeInElapsed / _fadeInDuration);
        }
        else
        {
            // Done
            _overlay.Opacity = 0f;
            IsActive = false;
        }
    }
}
