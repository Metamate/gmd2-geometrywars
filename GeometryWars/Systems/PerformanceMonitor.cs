using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

/// <summary>
/// Tracks FPS, heap memory, and entity count.
/// Toggle display on/off with F3 during play.
///
/// Useful for comparing the baseline Grid/ParticleManager against their
/// Optimized counterparts: swap the implementation in Game1.Initialize(),
/// run the game, press F3, and watch the FPS and RAM numbers change.
/// </summary>
public sealed class PerformanceMonitor
{
    private int _frameCounter;
    private float _elapsedTime;
    private int _fps;
    private long _totalMemory;

    // Press F3 in play to reveal — useful when benchmarking DOD optimisations.
    public bool IsVisible { get; private set; } = false;

    public void Toggle() => IsVisible = !IsVisible;

    public int FPS => _fps;
    public string MemoryMB => (_totalMemory / 1024.0 / 1024.0).ToString("F2");

    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _frameCounter++;

        if (_elapsedTime >= 1.0f)
        {
            _fps = _frameCounter;
            _frameCounter = 0;
            _elapsedTime -= 1.0f;

            // GC.GetTotalMemory(false) returns a snapshot of current heap usage.
            // Watch this number rise if you introduce allocations in the hot path
            _totalMemory = GC.GetTotalMemory(false);
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, int entityCount)
    {
        if (!IsVisible) return;

        string text = $"FPS: {_fps} | RAM: {MemoryMB} MB | Entities: {entityCount}";
        spriteBatch.DrawString(font, text, position, Color.Yellow);
    }
}
