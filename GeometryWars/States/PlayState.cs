using GMDCore;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars.States;

public sealed class PlayState : GameStateBase
{
    private readonly GameCore _game;
    private PlayerShip _player;
    private bool _paused;

    public PlayState(GameCore game) => _game = game;

    public override void Enter()
    {
        _paused = false;
        EntityManager.Clear();
        _player = new PlayerShip();
        EntityManager.Add(_player);
        PlayerStatus.Reset();
        EnemySpawner.Reset();
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(Sound.Music);
    }

    public override void Update()
    {
        if (Input.WasKeyPressed(Keys.P))
            _paused = !_paused;

        if (_paused) return;

        PlayerStatus.Update();
        EntityManager.Update();
        EnemySpawner.Update(!_player.IsDead, () => _player.Position);

        GameServices.Grid.Update();
        GameServices.Particles.Update();

        if (PlayerStatus.IsGameOver && !_player.IsDead)
            _game.SetState(new GameOverState(_game));
    }

    public override void DrawWorld(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        EntityManager.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        GameServices.Grid.Draw(spriteBatch);
        GameServices.Particles.Draw(spriteBatch);
        spriteBatch.End();
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
        spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        GameServices.Performance.Draw(spriteBatch, Art.Font, new Vector2(5, 35));
        DrawRightAligned(spriteBatch, "Score: " + PlayerStatus.Score, 5);
        DrawRightAligned(spriteBatch, "Multiplier: " + PlayerStatus.Multiplier, 35);

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
