using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars;

/// <summary>
/// Abstracts raw hardware input into semantic game actions.
/// Aligns with the architecture found in the Zelda and Pokemon projects.
/// </summary>
public static class GameController
{
    private static bool _isAimingWithMouse = true;
    private static Point _lastLogicMousePos;

    // Movement: Vector2 derived from WASD or Left Stick
    public static Vector2 Movement => GetMovementDirection();

    // Aiming: Vector2 derived from Mouse Position or Right Stick/Arrows
    public static Vector2 AimDirection(Vector2 shipPosition) => GetAimDirection(shipPosition);

    // Shooting: True if holding mouse button or using a controller/keyboard aim input
    public static bool IsShooting => GetIsShooting();

    // Semantic Actions (Abstracted from specific keys)
    public static bool WasBombPressed => RawInput.WasButtonPressed(Buttons.LeftTrigger) || 
                                         RawInput.WasButtonPressed(Buttons.RightTrigger) || 
                                         RawInput.WasKeyPressed(Keys.Space);

    public static bool WasPausePressed => RawInput.WasKeyPressed(Keys.P) || 
                                          RawInput.WasButtonPressed(Buttons.Start);

    public static bool WasDebugTogglePressed => RawInput.WasKeyPressed(Keys.F3);

    public static bool WasConfirmPressed => RawInput.WasKeyPressed(Keys.Enter) || 
                                            RawInput.WasButtonPressed(Buttons.A);

    public static bool WasExitPressed => RawInput.WasKeyPressed(Keys.Escape) || 
                                         RawInput.WasButtonPressed(Buttons.Back);

    public static Vector2 MousePosition => new(RawInput.Mouse.X, RawInput.Mouse.Y);

    /// <summary>
    /// Updates the semantic state. Called once per logic frame (60Hz).
    /// </summary>
    public static void Update()
    {
        var mousePos = RawInput.Mouse.Position;

        // Detect if the user is using the keyboard/gamepad to aim.
        bool hasAimKeys = new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(k => RawInput.IsKeyDown(k));
        bool hasAimStick = RawInput.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;

        if (hasAimKeys || hasAimStick)
        {
            _isAimingWithMouse = false;
        }
        else if (mousePos != _lastLogicMousePos)
        {
            _isAimingWithMouse = true;
        }

        _lastLogicMousePos = mousePos;
    }

    private static Vector2 GetMovementDirection()
    {
        Vector2 direction = RawInput.GamePad.ThumbSticks.Left;
        direction.Y *= -1;

        if (RawInput.IsKeyDown(Keys.A)) direction.X -= 1;
        if (RawInput.IsKeyDown(Keys.D)) direction.X += 1;
        if (RawInput.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (RawInput.IsKeyDown(Keys.S)) direction.Y += 1;

        if (direction.LengthSquared() > 1)
            direction.Normalize();

        return direction;
    }

    private static Vector2 GetAimDirection(Vector2 shipPosition)
    {
        if (_isAimingWithMouse)
        {
            Vector2 direction = MousePosition - shipPosition;
            return direction == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(direction);
        }

        Vector2 aim = RawInput.GamePad.ThumbSticks.Right;
        aim.Y *= -1;

        if (RawInput.IsKeyDown(Keys.Left))  aim.X -= 1;
        if (RawInput.IsKeyDown(Keys.Right)) aim.X += 1;
        if (RawInput.IsKeyDown(Keys.Up))    aim.Y -= 1;
        if (RawInput.IsKeyDown(Keys.Down))  aim.Y += 1;

        return aim == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(aim);
    }

    private static bool GetIsShooting()
    {
        if (_isAimingWithMouse)
            return RawInput.Mouse.LeftButton == ButtonState.Pressed;

        bool hasAimKeys = new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(k => RawInput.IsKeyDown(k));
        bool hasAimStick = RawInput.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;

        return hasAimKeys || hasAimStick;
    }
}
