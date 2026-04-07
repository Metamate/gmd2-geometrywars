using System;
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

    // Use ReadOnlySpan to avoid array allocations in hot paths.
    private static readonly Keys[] AimKeys = [Keys.Left, Keys.Right, Keys.Up, Keys.Down];

    public static Vector2 MousePosition => new(mouseState.X, mouseState.Y);

    public static void Update()
    {
        lastKeyboardState = keyboardState;
        lastGamepadState  = gamepadState;

        keyboardState = Keyboard.GetState();
        gamepadState  = GamePad.GetState(PlayerIndex.One);

        var currentMouseState = Mouse.GetState();

        if (IsAnyAimKeyPressed() || gamepadState.ThumbSticks.Right != Vector2.Zero)
        {
            isAimingWithMouse = false;
        }
        else if (currentMouseState.X != logicLastMouseState.X || currentMouseState.Y != logicLastMouseState.Y)
        {
            isAimingWithMouse = true;
        }

        logicLastMouseState = currentMouseState;
        mouseState = currentMouseState;
    }

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

        if (keyboardState.IsKeyDown(Keys.A)) direction.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D)) direction.X += 1;
        if (keyboardState.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S)) direction.Y += 1;

        if (direction.LengthSquared() > 1)
            direction.Normalize();

        return direction;
    }

    public static bool IsShooting()
    {
        if (isAimingWithMouse)
            return mouseState.LeftButton == ButtonState.Pressed;

        return gamepadState.ThumbSticks.Right.LengthSquared() > 0.01f || IsAnyAimKeyPressed();
    }

    private static bool IsAnyAimKeyPressed()
    {
        // Iterating a Span is zero-allocation.
        ReadOnlySpan<Keys> keys = AimKeys;
        for (int i = 0; i < keys.Length; i++)
            if (keyboardState.IsKeyDown(keys[i])) return true;
        return false;
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
