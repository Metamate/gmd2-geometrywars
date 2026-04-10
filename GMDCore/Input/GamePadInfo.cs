using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GMDCore.Input;

public class GamePadInfo
{
    private static readonly Buttons[] AllButtons = System.Enum.GetValues<Buttons>();

    private GamePadState _sampledState;
    private readonly HashSet<Buttons> _pressedBuffer = [];
    private readonly HashSet<Buttons> _pressedThisTick = [];

    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; }

    public GamePadInfo()
    {
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(PlayerIndex.One);
        _sampledState = CurrentState;
    }

    public void SampleFrame()
    {
        var nextState = GamePad.GetState(PlayerIndex.One);

        foreach (var button in AllButtons)
        {
            if (_sampledState.IsButtonUp(button) && nextState.IsButtonDown(button))
                _pressedBuffer.Add(button);
        }

        _sampledState = nextState;
    }

    public void AdvanceLogicTick()
    {
        PreviousState = CurrentState;
        CurrentState = _sampledState;

        _pressedThisTick.Clear();
        foreach (var button in _pressedBuffer)
            _pressedThisTick.Add(button);

        _pressedBuffer.Clear();
    }

    public bool IsButtonDown(Buttons button) => _sampledState.IsButtonDown(button);
    public bool IsButtonUp(Buttons button) => _sampledState.IsButtonUp(button);

    public bool WasButtonJustPressed(Buttons button)
        => _pressedThisTick.Contains(button);

    public GamePadThumbSticks ThumbSticks => _sampledState.ThumbSticks;
}
