using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GMDCore.Input;

public class GamePadInfo
{
    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; }

    public GamePadInfo()
    {
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(PlayerIndex.One);
    }

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex.One);
    }

    public bool IsButtonDown(Buttons button)     => CurrentState.IsButtonDown(button);
    public bool IsButtonUp(Buttons button)        => CurrentState.IsButtonUp(button);

    public bool WasButtonJustPressed(Buttons button)
        => CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);

    public GamePadThumbSticks ThumbSticks => CurrentState.ThumbSticks;
}
