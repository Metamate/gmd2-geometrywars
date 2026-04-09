namespace GMDCore.Input;

/// <summary>
/// Owns one instance of each input device and advances them together each logic tick.
/// Inject into the systems that need it rather than reading hardware state directly.
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
