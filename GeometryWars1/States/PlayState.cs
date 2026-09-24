using GMDCore.States;
using GeometryWars1.Services;
using GeometryWars1.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars1.States;

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
    }

    public override void Exit()
    {
        _session?.Shutdown();
        _session = null;
    }

    public override void Update(GameTime gameTime)
    {
        if (_context.Controller.WasPausePressed)
            _paused = !_paused;

        if (_context.Controller.WasDebugTogglePressed)
            _context.Performance.Toggle();

        if (_paused) return;

        _session.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Entities.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(_context.Assets.Pointer, _context.Controller.MousePosition, Color.White);
        _context.Performance.Draw(spriteBatch, _context.Assets.Font, new Vector2(5, 35), _session.Entities.Count);

        if (_paused)
        {
            string text = "PAUSED";
            Vector2 size = _context.Assets.Font.MeasureString(text);
            spriteBatch.DrawString(_context.Assets.Font, text, _context.Frame.ScreenSize / 2 - size / 2, Color.White);
        }

        spriteBatch.End();
    }
}
