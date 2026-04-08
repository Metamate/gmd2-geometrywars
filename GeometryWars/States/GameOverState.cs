using GMDCore;
using GeometryWars;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.States;

public sealed class GameOverState : GameStateBase
{
    private readonly Game1 _game;

    public GameOverState(Game1 game) => _game = game;

    public override void Update()
    {
        if (GameController.WasConfirmPressed)
            _game.SetState(new PlayState(_game));
    }

    public override void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        string text = "GAME OVER";
        Vector2 textSize = Art.Font.MeasureString(text);
        spriteBatch.DrawString(Art.Font, text, FrameContext.ScreenSize / 2 - textSize, Color.White);

        text = "Score: " + PlayerStatus.Score;
        textSize = Art.Font.MeasureString(text);
        spriteBatch.DrawString(Art.Font, text, FrameContext.ScreenSize / 2 - new Vector2(textSize.X, 0), Color.White);

        text = "Press Enter to Restart";
        textSize = Art.Font.MeasureString(text);
        spriteBatch.DrawString(Art.Font, text, FrameContext.ScreenSize / 2 - textSize / 2, Color.White);

        spriteBatch.End();
    }
}
