namespace GMDCore.Input;

/// <summary>
/// Owns one instance of each input device and advances them together each logic tick.
/// Access via GameCore.Input (analogous to Core.Input in Zelda / Pokemon).
/// </summary>
public class InputManager
{
    public KeyboardInfo Keyboard { get; } = new();
    public MouseInfo    Mouse    { get; } = new();
    public GamePadInfo  GamePad  { get; } = new();

    public void Update()
    {
        Keyboard.Update();
        Mouse.Update();
        GamePad.Update();
    }
}
