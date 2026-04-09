using GMDCore;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.States;

// State displayed when the player has no lives remaining.
public sealed class GameOverState : GameStateBase
{
    private readonly Game1 _game;
    private readonly PlayContext _context;
    private readonly PlaySession _session;

    public GameOverState(Game1 game, PlayContext context, PlaySession session)
    {
        _game = game;
        _context = context;
        _session = session;
    }

    public override void Update()
    {
        _session.Grid.Update();
        _session.Particles.Update();

        if (_context.Controller.WasConfirmPressed)
            _game.SetState(new PlayState(_game, _context));
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Grid.Draw(spriteBatch, _context.Assets.Pixel);
        _session.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        Vector2 center = _context.Frame.ScreenSize / 2;
        const float lineSpacing = 60f;

        DrawCentered(spriteBatch, "GAME OVER",               center + new Vector2(0, -lineSpacing * 2), Color.White);
        DrawCentered(spriteBatch, "Score: " + _session.Score.Score,    center + new Vector2(0, -lineSpacing),     Color.LightGray);
        DrawCentered(spriteBatch, "High Score: " + _session.Score.HighScore, center,                           Color.LightGray);
        DrawCentered(spriteBatch, "Press Enter to Restart",  center + new Vector2(0,  lineSpacing * 1.5f),   Color.Gray);

        spriteBatch.End();
    }

    private void DrawCentered(SpriteBatch spriteBatch, string text, Vector2 center, Color color)
    {
        Vector2 size = _context.Assets.Font.MeasureString(text);
        spriteBatch.DrawString(_context.Assets.Font, text, center - size / 2, color);
    }
}
