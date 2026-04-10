using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GMDCore.Input;

public class MouseInfo
{
    private MouseState _sampledState;
    private bool _leftPressedBuffer;
    private bool _leftPressedThisTick;

    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; }

    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
        _sampledState = CurrentState;
    }

    public void SampleFrame()
    {
        var nextState = Mouse.GetState();

        if (_sampledState.LeftButton == ButtonState.Released &&
            nextState.LeftButton == ButtonState.Pressed)
        {
            _leftPressedBuffer = true;
        }

        _sampledState = nextState;
    }

    public void AdvanceLogicTick()
    {
        PreviousState = CurrentState;
        CurrentState = _sampledState;
        _leftPressedThisTick = _leftPressedBuffer;
        _leftPressedBuffer = false;
    }

    public int X => _sampledState.X;
    public int Y => _sampledState.Y;
    public Point Position => _sampledState.Position;

    public bool IsLeftButtonDown
        => _sampledState.LeftButton == ButtonState.Pressed;

    public bool WasLeftButtonJustPressed
        => _leftPressedThisTick;
}
