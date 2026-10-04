namespace StrideTemplate.Core;

public class GameState
{
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsVictory { get; private set; }

    public event Action<int>? ScoreChanged;
    public event Action<int>? LivesChanged;
    public event Action? GameOverTriggered;
    public event Action? VictoryTriggered;

    public void Reset(int startLives = 3)
    {
        Score = 0;
        Lives = startLives;
        IsGameOver = false;
        IsVictory = false;
        ScoreChanged?.Invoke(Score);
        LivesChanged?.Invoke(Lives);
    }

    public void AddScore(int points)
    {
        Score += points;
        ScoreChanged?.Invoke(Score);
    }

    public void LoseLife()
    {
        Lives--;
        LivesChanged?.Invoke(Lives);
        if (Lives <= 0)
        {
            IsGameOver = true;
            GameOverTriggered?.Invoke();
        }
    }

    public void TriggerVictory()
    {
        IsVictory = true;
        IsGameOver = true;
        VictoryTriggered?.Invoke();
    }
}
