using Stride.Input;

namespace StrideTemplate.Input;

public class InputMap
{
    private readonly Dictionary<InputAction, Keys[]> _bindings = new()
    {
        [InputAction.MoveLeft] = [Keys.Left, Keys.A],
        [InputAction.MoveRight] = [Keys.Right, Keys.D],
        [InputAction.Shoot] = [Keys.Space, Keys.W, Keys.Up],
        [InputAction.Pause] = [Keys.Escape],
        [InputAction.Confirm] = [Keys.Enter, Keys.Space],
        [InputAction.Cancel] = [Keys.Escape],
        [InputAction.Quit] = [Keys.Q]
    };

    public bool IsDown(InputManager input, InputAction action)
    {
        if (!_bindings.TryGetValue(action, out var keys)) return false;
        foreach (var key in keys)
            if (input.IsKeyDown(key)) return true;
        return false;
    }

    public bool IsPressed(InputManager input, InputAction action)
    {
        if (!_bindings.TryGetValue(action, out var keys)) return false;
        foreach (var key in keys)
            if (input.IsKeyPressed(key)) return true;
        return false;
    }

    public void Rebind(InputAction action, params Keys[] keys)
    {
        _bindings[action] = keys;
    }
}
