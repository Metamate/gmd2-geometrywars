using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GMDCore.Input;

public class MouseInfo
{
    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; }

    public MouseInfo()
    {
        PreviousState = new MouseState();
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Full update: saves previous state and samples fresh hardware state.
    /// Called once per logic tick (60Hz).
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    /// <summary>
    /// Samples only the current state without advancing previous.
    /// Called every render frame to keep the cursor responsive between logic ticks.
    /// </summary>
    public void UpdatePositionOnly() => CurrentState = Mouse.GetState();

    public int X        => CurrentState.X;
    public int Y        => CurrentState.Y;
    public Point Position => CurrentState.Position;

    public bool IsLeftButtonDown
        => CurrentState.LeftButton == ButtonState.Pressed;

    public bool WasLeftButtonJustPressed
        => CurrentState.LeftButton == ButtonState.Pressed &&
           PreviousState.LeftButton == ButtonState.Released;
}
