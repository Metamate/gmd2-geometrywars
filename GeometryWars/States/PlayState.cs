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
    private readonly PlayContext _context;
    private PlaySession _session;
    private bool _paused;

    public PlayState(Game1 game, PlayContext context)
    {
        _game = game;
        _context = context;
    }

    public override void Enter()
    {
        _paused = false;
        _session = new PlaySession(_context, _context.Frame.Viewport.Bounds);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_context.Assets.Music);
    }

    public override void Update()
    {
        if (_context.Controller.WasPausePressed)
            _paused = !_paused;

        if (_context.Controller.WasDebugTogglePressed)
            _context.Performance.Toggle();

        if (_paused) return;

        _session.Update();

        if (_session.Score.IsGameOver && !_session.IsPlayerRespawning)
        {
            _session.Score.SaveHighScore();
            _game.SetState(new GameOverState(_game, _context, _session));
        }
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Entities.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Grid.Draw(spriteBatch, _context.Assets.Pixel);
        _session.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(_context.Assets.Pointer, _context.Controller.MousePosition, Color.White);
        spriteBatch.DrawString(_context.Assets.Font, "Lives: " + _session.Score.Lives, new Vector2(5), Color.White);
        _context.Performance.Draw(spriteBatch, _context.Assets.Font, new Vector2(5, 35), _session.Entities.Count);
        
        DrawRightAligned(spriteBatch, "Score: " + _session.Score.Score, 5);
        DrawRightAligned(spriteBatch, "Multiplier: " + _session.Score.Multiplier, 35);

        if (_paused)
        {
            string text = "PAUSED";
            Vector2 size = _context.Assets.Font.MeasureString(text);
            spriteBatch.DrawString(_context.Assets.Font, text, _context.Frame.ScreenSize / 2 - size / 2, Color.White);
        }

        spriteBatch.End();
    }

    private void DrawRightAligned(SpriteBatch spriteBatch, string text, float y)
    {
        float width = _context.Assets.Font.MeasureString(text).X;
        spriteBatch.DrawString(_context.Assets.Font, text,
            new Vector2(_context.Frame.ScreenSize.X - width - 5, y), Color.White);
    }
}
