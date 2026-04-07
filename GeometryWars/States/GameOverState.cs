using GMDCore;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars.States;

public sealed class GameOverState : GameStateBase
{
    private readonly GameCore _game;

    public GameOverState(GameCore game) => _game = game;

    public override void Update()
    {
        if (Input.WasKeyPressed(Keys.Enter) || Input.WasKeyPressed(Keys.Space) ||
            Input.WasButtonPressed(Microsoft.Xna.Framework.Input.Buttons.Start))
            _game.SetState(new PlayState(_game));
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        string text = $"Game Over\nScore: {PlayerStatus.Score}\nHigh Score: {PlayerStatus.HighScore}\n\nPress Enter to restart";
        Vector2 textSize = Art.Font.MeasureString(text);
        spriteBatch.DrawString(Art.Font, text, FrameContext.ScreenSize / 2 - textSize / 2, Color.White);

        spriteBatch.End();
    }
}
