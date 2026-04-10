namespace GMDCore.Input;

/// <summary>
/// Owns one instance of each input device.
/// Raw hardware is sampled every render frame, then buffered edge events are
/// exposed to gameplay on the next fixed logic tick.
/// </summary>
public class InputManager
{
    public KeyboardInfo Keyboard { get; } = new();
    public MouseInfo    Mouse    { get; } = new();
    public GamePadInfo  GamePad  { get; } = new();

    public void SampleFrame()
    {
        Keyboard.SampleFrame();
        Mouse.SampleFrame();
        GamePad.SampleFrame();
    }

    public void AdvanceLogicTick()
    {
        Keyboard.AdvanceLogicTick();
        Mouse.AdvanceLogicTick();
        GamePad.AdvanceLogicTick();
    }
}
