using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars;

/// <summary>
/// Tracks raw hardware states for Keyboard, Mouse, and GamePad.
/// Handles the difference between current and previous frames to detect 'just pressed'.
/// </summary>
static class RawInput
{
    private static KeyboardState _keyboardState, _lastKeyboardState;
    private static MouseState    _mouseState, _lastMouseState;
    private static GamePadState  _gamepadState, _lastGamepadState;

    public static KeyboardState Keyboard => _keyboardState;
    public static MouseState    Mouse    => _mouseState;
    public static GamePadState  GamePad  => _gamepadState;

    public static void Update()
    {
        _lastKeyboardState = _keyboardState;
        _lastMouseState    = _mouseState;
        _lastGamepadState  = gamepadState;

        _keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        _mouseState    = Microsoft.Xna.Framework.Input.Mouse.GetState();
        _gamepadState  = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);
    }

    private static GamePadState gamepadState => Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

    /// <summary>
    /// Updates only the mouse state. Called every frame (even between logic ticks) 
    /// to keep the UI/cursor responsive at high framerates.
    /// </summary>
    public static void UpdateMouseOnly()
    {
        _mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
    }

    public static bool IsKeyDown(Keys key) => _keyboardState.IsKeyDown(key);
    
    public static bool WasKeyPressed(Keys key) 
        => _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);

    public static bool WasButtonPressed(Buttons button) 
        => _lastGamepadState.IsButtonUp(button) && _gamepadState.IsButtonDown(button);
}
