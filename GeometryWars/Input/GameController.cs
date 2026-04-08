using GMDCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars;

/// <summary>
/// Abstracts raw hardware input into semantic game actions.
/// Uses Core.Input rather than reading hardware state directly.
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

    public static bool WasPausePressed
        => Core.Input.Keyboard.WasKeyJustPressed(Keys.P) ||
           Core.Input.GamePad.WasButtonJustPressed(Buttons.Start);

    public static bool WasDebugTogglePressed
        => Core.Input.Keyboard.WasKeyJustPressed(Keys.F3);

    public static bool WasConfirmPressed
        => Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter) ||
           Core.Input.GamePad.WasButtonJustPressed(Buttons.A);

    public static bool WasExitPressed
        => Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape) ||
           Core.Input.GamePad.WasButtonJustPressed(Buttons.Back);

    public static Vector2 MousePosition
        => new(Core.Input.Mouse.X, Core.Input.Mouse.Y);

    /// <summary>
    /// Updates the semantic state. Called once per logic frame (60Hz).
    /// </summary>
    public static void Update()
    {
        var mousePos = Core.Input.Mouse.Position;

        // Detect if the user is using the keyboard/gamepad to aim.
        bool hasAimKeys = Core.Input.Keyboard.IsKeyDown(Keys.Left) ||
                          Core.Input.Keyboard.IsKeyDown(Keys.Right) ||
                          Core.Input.Keyboard.IsKeyDown(Keys.Up) ||
                          Core.Input.Keyboard.IsKeyDown(Keys.Down);
        bool hasAimStick = Core.Input.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;

        if (hasAimKeys || hasAimStick)
            _isAimingWithMouse = false;
        else if (mousePos != _lastLogicMousePos)
            _isAimingWithMouse = true;

        _lastLogicMousePos = mousePos;
    }

    private static Vector2 GetMovementDirection()
    {
        Vector2 direction = Core.Input.GamePad.ThumbSticks.Left;
        direction.Y *= -1;

        if (Core.Input.Keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.D)) direction.X += 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.S)) direction.Y += 1;

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

        Vector2 aim = Core.Input.GamePad.ThumbSticks.Right;
        aim.Y *= -1;

        if (Core.Input.Keyboard.IsKeyDown(Keys.Left)) aim.X -= 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.Right)) aim.X += 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.Up)) aim.Y -= 1;
        if (Core.Input.Keyboard.IsKeyDown(Keys.Down)) aim.Y += 1;

        return aim == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(aim);
    }

    private static bool GetIsShooting()
    {
        if (_isAimingWithMouse)
            return Core.Input.Mouse.IsLeftButtonDown;

        return Core.Input.Keyboard.IsKeyDown(Keys.Left) ||
               Core.Input.Keyboard.IsKeyDown(Keys.Right) ||
               Core.Input.Keyboard.IsKeyDown(Keys.Up) ||
               Core.Input.Keyboard.IsKeyDown(Keys.Down) ||
               Core.Input.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;
    }
}
