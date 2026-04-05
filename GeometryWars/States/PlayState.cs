using GeometryWars.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars.States;

public sealed class PlayState : GameStateBase
{
    private readonly GameCore _game;
    private bool _paused;

    public PlayState(GameCore game)
    {
        _game = game;
    }

    public override void Enter()
    {
        _paused = false;
        EntityManager.Clear();
        EntityManager.Add(PlayerShip.Instance);
        PlayerStatus.Reset();
        EnemySpawner.Reset();
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(Sound.Music);
    }

    public override void Update(GameContext ctx)
    {
        if (Input.WasKeyPressed(Keys.P))
            _paused = !_paused;

        if (_paused)
            return;

        PlayerStatus.Update(ctx);
        EntityManager.Update(ctx);
        EnemySpawner.Update(ctx);

        ctx.Grid.Update();
        ctx.Particles.Update();

        // Transition to game over once the death animation completes
        if (PlayerStatus.IsGameOver && !PlayerShip.Instance.IsDead)
            _game.SetState(new GameOverState(_game));
    }

    public override void DrawWorld(SpriteBatch spriteBatch, GameContext ctx)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        EntityManager.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        ctx.Grid.Draw(spriteBatch);
        ctx.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch, GameContext ctx)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
        spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        DrawRightAligned(spriteBatch, ctx, "Score: " + PlayerStatus.Score, 5);
        DrawRightAligned(spriteBatch, ctx, "Multiplier: " + PlayerStatus.Multiplier, 35);

        if (_paused)
        {
            string paused = "PAUSED";
            Vector2 size = Art.Font.MeasureString(paused);
            spriteBatch.DrawString(Art.Font, paused, ctx.ScreenSize / 2 - size / 2, Color.White);
        }

        spriteBatch.End();
    }

    private static void DrawRightAligned(SpriteBatch spriteBatch, GameContext ctx, string text, float y)
    {
        float width = Art.Font.MeasureString(text).X;
        spriteBatch.DrawString(Art.Font, text, new Vector2(ctx.ScreenSize.X - width - 5, y), Color.White);
    }
}
