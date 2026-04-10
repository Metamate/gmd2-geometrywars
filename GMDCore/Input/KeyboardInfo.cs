using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GMDCore.Input;

public class KeyboardInfo
{
    private KeyboardState _sampledState;
    private readonly HashSet<Keys> _pressedBuffer = [];
    private readonly HashSet<Keys> _releasedBuffer = [];
    private readonly HashSet<Keys> _pressedThisTick = [];
    private readonly HashSet<Keys> _releasedThisTick = [];

    public KeyboardState PreviousState { get; private set; }
    public KeyboardState CurrentState { get; private set; }

    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
        _sampledState = CurrentState;
    }

    public void SampleFrame()
    {
        var nextState = Keyboard.GetState();

        foreach (var key in nextState.GetPressedKeys())
        {
            if (_sampledState.IsKeyUp(key))
                _pressedBuffer.Add(key);
        }

        foreach (var key in _sampledState.GetPressedKeys())
        {
            if (nextState.IsKeyUp(key))
                _releasedBuffer.Add(key);
        }

        _sampledState = nextState;
    }

    public void AdvanceLogicTick()
    {
        PreviousState = CurrentState;
        CurrentState = _sampledState;

        _pressedThisTick.Clear();
        foreach (var key in _pressedBuffer)
            _pressedThisTick.Add(key);

        _releasedThisTick.Clear();
        foreach (var key in _releasedBuffer)
            _releasedThisTick.Add(key);

        _pressedBuffer.Clear();
        _releasedBuffer.Clear();
    }

    public bool IsKeyDown(Keys key) => _sampledState.IsKeyDown(key);
    public bool IsKeyUp(Keys key) => _sampledState.IsKeyUp(key);

    public bool WasKeyJustPressed(Keys key)
        => _pressedThisTick.Contains(key);

    public bool WasKeyJustReleased(Keys key)
        => _releasedThisTick.Contains(key);
}
