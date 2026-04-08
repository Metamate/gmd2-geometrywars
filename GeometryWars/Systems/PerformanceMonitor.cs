using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

public sealed class PerformanceMonitor
{
    private int _frameCounter;
    private float _elapsedTime;
    private int _fps;
    private long _totalMemory;

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
