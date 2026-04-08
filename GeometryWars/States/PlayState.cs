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
    private PlaySession _session;
    private bool _paused;

    public PlayState(Game1 game) => _game = game;

    public override void Enter()
    {
        _paused = false;
        _session = new PlaySession(FrameContext.Viewport.Bounds);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(Sound.Music);
    }

    public override void Update()
    {
        if (GameController.WasPausePressed)
            _paused = !_paused;

        if (GameController.WasDebugTogglePressed)
            GameServices.Performance.Toggle();

        if (_paused) return;

        _session.Update();

        if (_session.Score.IsGameOver && !_session.Player.IsDead)
        {
            _session.Score.SaveHighScore();
            _game.SetState(new GameOverState(_game, _session));
        }
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Entities.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _session.Grid.Draw(spriteBatch);
        _session.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(Art.Pointer, GameController.MousePosition, Color.White);
        spriteBatch.DrawString(Art.Font, "Lives: " + _session.Score.Lives, new Vector2(5), Color.White);
        GameServices.Performance.Draw(spriteBatch, Art.Font, new Vector2(5, 35), _session.Entities.Count);
        
        DrawRightAligned(spriteBatch, "Score: " + _session.Score.Score, 5);
        DrawRightAligned(spriteBatch, "Multiplier: " + _session.Score.Multiplier, 35);

        if (_paused)
        {
            string text = "PAUSED";
            Vector2 size = Art.Font.MeasureString(text);
            spriteBatch.DrawString(Art.Font, text, FrameContext.ScreenSize / 2 - size / 2, Color.White);
        }

        spriteBatch.End();
    }

    private static void DrawRightAligned(SpriteBatch spriteBatch, string text, float y)
    {
        float width = Art.Font.MeasureString(text).X;
        spriteBatch.DrawString(Art.Font, text,
            new Vector2(FrameContext.ScreenSize.X - width - 5, y), Color.White);
    }
}
