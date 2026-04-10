using GMDCore.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars.Input;

/// Abstracts raw hardware input into semantic game actions.
/// Uses an injected InputManager rather than reading hardware state directly.
public sealed class GameController
{
    private readonly InputManager _input;
    private bool _isAimingWithMouse = true;
    private Point _lastLogicMousePos;

    public GameController(InputManager input) => _input = input;

    // Movement: Vector2 derived from WASD or Left Stick
    public Vector2 Movement => GetMovementDirection();

    // Aiming: Vector2 derived from Mouse Position or Right Stick/Arrows
    public Vector2 AimDirection(Vector2 shipPosition) => GetAimDirection(shipPosition);

    // Shooting: True if holding mouse button or using a controller/keyboard aim input
    public bool IsShooting => GetIsShooting();

    public bool WasPausePressed
        => _input.Keyboard.WasKeyJustPressed(Keys.P) ||
           _input.GamePad.WasButtonJustPressed(Buttons.Start);

    public bool WasDebugTogglePressed
        => _input.Keyboard.WasKeyJustPressed(Keys.F3);

    public bool WasConfirmPressed
        => _input.Keyboard.WasKeyJustPressed(Keys.Enter) ||
           _input.GamePad.WasButtonJustPressed(Buttons.A);

    public bool WasExitPressed
        => _input.Keyboard.WasKeyJustPressed(Keys.Escape) ||
           _input.GamePad.WasButtonJustPressed(Buttons.Back);

    public Vector2 MousePosition
        => new(_input.Mouse.X, _input.Mouse.Y);

    public void Update()
    {
        var mousePos = _input.Mouse.Position;

        // Detect if the user is using the keyboard/gamepad to aim.
        bool hasAimKeys = _input.Keyboard.IsKeyDown(Keys.Left) ||
                          _input.Keyboard.IsKeyDown(Keys.Right) ||
                          _input.Keyboard.IsKeyDown(Keys.Up) ||
                          _input.Keyboard.IsKeyDown(Keys.Down);
        bool hasAimStick = _input.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;

        if (hasAimKeys || hasAimStick)
            _isAimingWithMouse = false;
        else if (mousePos != _lastLogicMousePos)
            _isAimingWithMouse = true;

        _lastLogicMousePos = mousePos;
    }

    private Vector2 GetMovementDirection()
    {
        Vector2 direction = _input.GamePad.ThumbSticks.Left;
        direction.Y *= -1;

        if (_input.Keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
        if (_input.Keyboard.IsKeyDown(Keys.D)) direction.X += 1;
        if (_input.Keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (_input.Keyboard.IsKeyDown(Keys.S)) direction.Y += 1;

        if (direction.LengthSquared() > 1)
            direction.Normalize();

        return direction;
    }

    private Vector2 GetAimDirection(Vector2 shipPosition)
    {
        if (_isAimingWithMouse)
        {
            Vector2 direction = MousePosition - shipPosition;
            return direction == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(direction);
        }

        Vector2 aim = _input.GamePad.ThumbSticks.Right;
        aim.Y *= -1;

        if (_input.Keyboard.IsKeyDown(Keys.Left)) aim.X -= 1;
        if (_input.Keyboard.IsKeyDown(Keys.Right)) aim.X += 1;
        if (_input.Keyboard.IsKeyDown(Keys.Up)) aim.Y -= 1;
        if (_input.Keyboard.IsKeyDown(Keys.Down)) aim.Y += 1;

        return aim == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(aim);
    }

    private bool GetIsShooting()
    {
        if (_isAimingWithMouse)
            return _input.Mouse.IsLeftButtonDown;

        return _input.Keyboard.IsKeyDown(Keys.Left) ||
               _input.Keyboard.IsKeyDown(Keys.Right) ||
               _input.Keyboard.IsKeyDown(Keys.Up) ||
               _input.Keyboard.IsKeyDown(Keys.Down) ||
               _input.GamePad.ThumbSticks.Right.LengthSquared() > 0.01f;
    }
}
