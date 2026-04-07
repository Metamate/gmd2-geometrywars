using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

/// <summary>
/// A high-performance frame counter and memory monitor.
/// </summary>
public sealed class PerformanceMonitor
{
    private int _frameCounter;
    private float _elapsedTime;
    private int _fps;
    private long _totalMemory;

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

            // GC.GetTotalMemory(false) provides a snapshot of current heap usage.
            _totalMemory = GC.GetTotalMemory(false);
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 position)
    {
        string text = $"FPS: {_fps} | RAM: {MemoryMB}MB";
        spriteBatch.DrawString(font, text, position, Color.Yellow);
    }
}
