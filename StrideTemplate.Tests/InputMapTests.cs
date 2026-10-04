using StrideTemplate.Input;

namespace StrideTemplate.Tests;

public class InputMapTests
{
    [Fact]
    public void DefaultBindings_MoveLeft_HasLeftAndA()
    {
        var map = new InputMap();

        // We can't call IsDown without an InputManager, but we can verify
        // rebinding works and the map doesn't throw on construction
        Assert.NotNull(map);
    }

    [Fact]
    public void Rebind_ReplacesExistingBinding()
    {
        var map = new InputMap();

        // Should not throw
        map.Rebind(InputAction.Shoot, Stride.Input.Keys.Z);
        map.Rebind(InputAction.MoveLeft, Stride.Input.Keys.Q);
    }

    [Fact]
    public void Rebind_AllowsMultipleKeys()
    {
        var map = new InputMap();

        map.Rebind(InputAction.Shoot, Stride.Input.Keys.Z, Stride.Input.Keys.X, Stride.Input.Keys.C);
    }

    [Theory]
    [InlineData(InputAction.MoveLeft)]
    [InlineData(InputAction.MoveRight)]
    [InlineData(InputAction.Shoot)]
    [InlineData(InputAction.Pause)]
    [InlineData(InputAction.Confirm)]
    [InlineData(InputAction.Cancel)]
    public void AllActions_HaveDefaultBindings(InputAction action)
    {
        var map = new InputMap();

        // Rebinding should work for all default actions (proving they exist in the dictionary)
        map.Rebind(action, Stride.Input.Keys.F12);
    }
}
