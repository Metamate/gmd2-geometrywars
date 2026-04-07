using System.Linq;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars;

static class Input
{
    private static KeyboardState keyboardState, lastKeyboardState;
    private static MouseState mouseState, logicLastMouseState;
    private static GamePadState gamepadState, lastGamepadState;

    private static bool isAimingWithMouse;

    // MousePosition always returns the most recent hardware state for smooth UI/cursor.
    public static Vector2 MousePosition => new(mouseState.X, mouseState.Y);

    public static void Update()
    {
        lastKeyboardState = keyboardState;
        lastGamepadState  = gamepadState;

        keyboardState = Keyboard.GetState();
        gamepadState  = GamePad.GetState(PlayerIndex.One);

        // Update the current hardware state.
        var currentMouseState = Mouse.GetState();

        // Detect aiming mode based on logic-frame delta (60Hz) rather than
        // render-frame delta, so slow mouse movements are still detected.
        if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)) || gamepadState.ThumbSticks.Right != Vector2.Zero)
        {
            isAimingWithMouse = false;
        }
        else if (currentMouseState.X != logicLastMouseState.X || currentMouseState.Y != logicLastMouseState.Y)
        {
            isAimingWithMouse = true;
        }

        // Sync for the next logic update.
        logicLastMouseState = currentMouseState;
        mouseState = currentMouseState;
    }

    // Call every frame from Game1.Update() to ensure the cursor is responsive
    // even if the logic loop hasn't run yet.
    public static void UpdateMouseOnly()
    {
        mouseState = Mouse.GetState();
    }

    public static bool WasKeyPressed(Keys key)
    {
        return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
    }

    public static bool WasButtonPressed(Buttons button)
    {
        return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
    }

    public static Vector2 GetMovementDirection()
    {
        Vector2 direction = gamepadState.ThumbSticks.Left;
        direction.Y *= -1;

        if (keyboardState.IsKeyDown(Keys.A))
            direction.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D))
            direction.X += 1;
        if (keyboardState.IsKeyDown(Keys.W))
            direction.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S))
            direction.Y += 1;

        if (direction.LengthSquared() > 1)
            direction.Normalize();

        return direction;
    }

    public static bool IsShooting()
    {
        // 1. Mouse mode: only shoot if holding the left button.
        if (isAimingWithMouse)
            return mouseState.LeftButton == ButtonState.Pressed;

        // 2. Controller/Keyboard mode: shoot if any aiming input is active.
        Vector2 stick = gamepadState.ThumbSticks.Right;
        bool hasStickInput = stick.LengthSquared() > 0.01f;
        bool hasKeyInput = new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x));

        return hasStickInput || hasKeyInput;
    }

    public static Vector2 GetAimDirection(Vector2 shipPosition)
    {
        if (isAimingWithMouse)
            return GetMouseAimDirection(shipPosition);

        Vector2 direction = gamepadState.ThumbSticks.Right;
        direction.Y *= -1;

        if (keyboardState.IsKeyDown(Keys.Left))  direction.X -= 1;
        if (keyboardState.IsKeyDown(Keys.Right)) direction.X += 1;
        if (keyboardState.IsKeyDown(Keys.Up))    direction.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.Down))  direction.Y += 1;

        return direction == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(direction);
    }

    private static Vector2 GetMouseAimDirection(Vector2 shipPosition)
    {
        Vector2 direction = MousePosition - shipPosition;
        return direction == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(direction);
    }

    public static bool WasBombButtonPressed()
    {
        return WasButtonPressed(Buttons.LeftTrigger) || WasButtonPressed(Buttons.RightTrigger) || WasKeyPressed(Keys.Space);
    }
}
