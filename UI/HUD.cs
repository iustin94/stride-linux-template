using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;

namespace StrideTemplate.UI;

public class HUD
{
    private readonly Core.GameState _gameState;
    private Entity? _entity;
    private TextBlock? _scoreText;
    private TextBlock? _livesText;

    public HUD(Core.GameState gameState)
    {
        _gameState = gameState;
        _gameState.ScoreChanged += OnScoreChanged;
        _gameState.LivesChanged += OnLivesChanged;
    }

    public void Show(Scene rootScene)
    {
        _entity ??= UIHelper.CreateUIEntity("HUD", BuildLayout());
        _entity.Scene = rootScene;
    }

    public void Hide()
    {
        if (_entity != null)
            _entity.Scene = null;
    }

    public void Dispose()
    {
        _gameState.ScoreChanged -= OnScoreChanged;
        _gameState.LivesChanged -= OnLivesChanged;
        Hide();
    }

    private UIElement BuildLayout()
    {
        _scoreText = UIHelper.CreateText($"SCORE: {_gameState.Score}", 22f);
        _scoreText.HorizontalAlignment = HorizontalAlignment.Left;
        _scoreText.VerticalAlignment = VerticalAlignment.Top;
        _scoreText.Margin = new Thickness(20, 20, 0, 0);

        _livesText = UIHelper.CreateText($"LIVES: {_gameState.Lives}", 22f);
        _livesText.HorizontalAlignment = HorizontalAlignment.Right;
        _livesText.VerticalAlignment = VerticalAlignment.Top;
        _livesText.Margin = new Thickness(0, 20, 20, 0);

        var canvas = new Grid();
        canvas.Children.Add(_scoreText);
        canvas.Children.Add(_livesText);
        return canvas;
    }

    private void OnScoreChanged(int score)
    {
        if (_scoreText != null)
            _scoreText.Text = $"SCORE: {score}";
    }

    private void OnLivesChanged(int lives)
    {
        if (_livesText != null)
            _livesText.Text = $"LIVES: {lives}";
    }
}
