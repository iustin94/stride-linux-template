using StrideTemplate.Core;

namespace StrideTemplate.Tests;

public class GameStateTests
{
    [Fact]
    public void Reset_SetsInitialValues()
    {
        var state = new GameState();
        state.Reset(3);

        Assert.Equal(0, state.Score);
        Assert.Equal(3, state.Lives);
        Assert.False(state.IsGameOver);
        Assert.False(state.IsVictory);
    }

    [Fact]
    public void AddScore_IncrementsScore()
    {
        var state = new GameState();
        state.Reset();

        state.AddScore(10);
        Assert.Equal(10, state.Score);

        state.AddScore(25);
        Assert.Equal(35, state.Score);
    }

    [Fact]
    public void AddScore_FiresEvent()
    {
        var state = new GameState();
        state.Reset();
        int? reported = null;
        state.ScoreChanged += s => reported = s;

        state.AddScore(10);

        Assert.Equal(10, reported);
    }

    [Fact]
    public void LoseLife_DecrementsLives()
    {
        var state = new GameState();
        state.Reset(3);

        state.LoseLife();

        Assert.Equal(2, state.Lives);
        Assert.False(state.IsGameOver);
    }

    [Fact]
    public void LoseLife_FiresEvent()
    {
        var state = new GameState();
        state.Reset(3);
        int? reported = null;
        state.LivesChanged += l => reported = l;

        state.LoseLife();

        Assert.Equal(2, reported);
    }

    [Fact]
    public void LoseLife_TriggersGameOver_AtZero()
    {
        var state = new GameState();
        state.Reset(1);
        bool gameOverFired = false;
        state.GameOverTriggered += () => gameOverFired = true;

        state.LoseLife();

        Assert.Equal(0, state.Lives);
        Assert.True(state.IsGameOver);
        Assert.False(state.IsVictory);
        Assert.True(gameOverFired);
    }

    [Fact]
    public void LoseLife_DoesNotTriggerGameOver_WhenLivesRemain()
    {
        var state = new GameState();
        state.Reset(2);
        bool gameOverFired = false;
        state.GameOverTriggered += () => gameOverFired = true;

        state.LoseLife();

        Assert.False(state.IsGameOver);
        Assert.False(gameOverFired);
    }

    [Fact]
    public void TriggerVictory_SetsFlags()
    {
        var state = new GameState();
        state.Reset();
        bool victoryFired = false;
        state.VictoryTriggered += () => victoryFired = true;

        state.TriggerVictory();

        Assert.True(state.IsGameOver);
        Assert.True(state.IsVictory);
        Assert.True(victoryFired);
    }

    [Fact]
    public void Reset_ClearsGameOverState()
    {
        var state = new GameState();
        state.Reset(1);
        state.AddScore(100);
        state.LoseLife(); // game over

        state.Reset(3);

        Assert.Equal(0, state.Score);
        Assert.Equal(3, state.Lives);
        Assert.False(state.IsGameOver);
        Assert.False(state.IsVictory);
    }

    [Fact]
    public void Reset_DefaultLives_IsThree()
    {
        var state = new GameState();
        state.Reset();

        Assert.Equal(3, state.Lives);
    }
}
