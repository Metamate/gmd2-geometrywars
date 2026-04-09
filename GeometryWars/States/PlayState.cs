using GMDCore;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars.States;

// The main game loop state where combat and movement occur.
public sealed class PlayState : GameStateBase
{
    private readonly Game1 _game;
    private readonly GameRuntime _runtime;
    private PlaySession _session;
    private bool _paused;

    public PlayState(Game1 game, GameRuntime runtime)
    {
        _game = game;
        _runtime = runtime;
    }

    public override void Enter()
    {
        _paused = false;
        _session = new PlaySession(_runtime, _runtime.Frame.Viewport.Bounds);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_runtime.Assets.Music);
    }

    public override void Update()
    {
        if (_runtime.Controller.WasPausePressed)
            _paused = !_paused;

        if (_runtime.Controller.WasDebugTogglePressed)
            _runtime.Performance.Toggle();

        if (_paused) return;

        _session.Update();

        if (_session.Score.IsGameOver && !_session.Player.IsDead)
        {
            _session.Score.SaveHighScore();
            _game.SetState(new GameOverState(_game, _runtime, _session));
        }
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Entities.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Grid.Draw(spriteBatch, _runtime.Assets.Pixel);
        _session.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(_runtime.Assets.Pointer, _runtime.Controller.MousePosition, Color.White);
        spriteBatch.DrawString(_runtime.Assets.Font, "Lives: " + _session.Score.Lives, new Vector2(5), Color.White);
        _runtime.Performance.Draw(spriteBatch, _runtime.Assets.Font, new Vector2(5, 35), _session.Entities.Count);
        
        DrawRightAligned(spriteBatch, "Score: " + _session.Score.Score, 5);
        DrawRightAligned(spriteBatch, "Multiplier: " + _session.Score.Multiplier, 35);

        if (_paused)
        {
            string text = "PAUSED";
            Vector2 size = _runtime.Assets.Font.MeasureString(text);
            spriteBatch.DrawString(_runtime.Assets.Font, text, _runtime.Frame.ScreenSize / 2 - size / 2, Color.White);
        }

        spriteBatch.End();
    }

    private void DrawRightAligned(SpriteBatch spriteBatch, string text, float y)
    {
        float width = _runtime.Assets.Font.MeasureString(text).X;
        spriteBatch.DrawString(_runtime.Assets.Font, text,
            new Vector2(_runtime.Frame.ScreenSize.X - width - 5, y), Color.White);
    }
}
