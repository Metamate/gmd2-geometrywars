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

    public GameOverState(Game1 game) => _game = game;

    public override void Update()
    {
        GameServices.Grid.Update();
        GameServices.Particles.Update();

        if (GameController.WasConfirmPressed)
            _game.SetState(new PlayState(_game));
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        GameServices.Grid.Draw(spriteBatch);
        GameServices.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        Vector2 center = FrameContext.ScreenSize / 2;
        const float lineSpacing = 60f;

        DrawCentered(spriteBatch, "GAME OVER",               center + new Vector2(0, -lineSpacing * 2), Color.White);
        DrawCentered(spriteBatch, "Score: " + PlayerStatus.Score,    center + new Vector2(0, -lineSpacing),     Color.LightGray);
        DrawCentered(spriteBatch, "High Score: " + PlayerStatus.HighScore, center,                           Color.LightGray);
        DrawCentered(spriteBatch, "Press Enter to Restart",  center + new Vector2(0,  lineSpacing * 1.5f),   Color.Gray);

        spriteBatch.End();
    }

    private static void DrawCentered(SpriteBatch spriteBatch, string text, Vector2 center, Color color)
    {
        Vector2 size = Art.Font.MeasureString(text);
        spriteBatch.DrawString(Art.Font, text, center - size / 2, color);
    }
}
