using StrideTemplate.Input;

namespace StrideTemplate.Tests;

public class InputActionTests
{
    [Fact]
    public void AllExpectedActions_AreDefined()
    {
        var actions = Enum.GetValues<InputAction>();

        Assert.Contains(InputAction.MoveLeft, actions);
        Assert.Contains(InputAction.MoveRight, actions);
        Assert.Contains(InputAction.Shoot, actions);
        Assert.Contains(InputAction.Pause, actions);
        Assert.Contains(InputAction.Confirm, actions);
        Assert.Contains(InputAction.Cancel, actions);
    }

    [Fact]
    public void AllExpectedActions_IncludesQuit()
    {
        Assert.Contains(InputAction.Quit, Enum.GetValues<InputAction>());
    }

    [Fact]
    public void ActionCount_IsSeven()
    {
        Assert.Equal(7, Enum.GetValues<InputAction>().Length);
    }
}
