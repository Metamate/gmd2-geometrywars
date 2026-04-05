using System;
using BloomPostprocess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars;

public class Game1 : Game
{
    private bool _paused;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    BloomComponent bloom;

    public static Game1 Instance { get; private set; }
    public static GameTime GameTime { get; private set; }
    public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
    public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
    public static ParticleManager<ParticleState> ParticleManager { get; private set; }
    public static Grid Grid { get; private set; }
    public Game1()
    {
        Instance = this;
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;

        bloom = new BloomComponent(this);
        Components.Add(bloom);
        bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        EntityManager.Add(PlayerShip.Instance);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(Sound.Music);
        ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        const int maxGridPoints = 1600;
        Vector2 gridSpacing = new Vector2((float)Math.Sqrt(Viewport.Width * Viewport.Height / maxGridPoints));
        Grid = new Grid(Viewport.Bounds, gridSpacing);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        GameTime = gameTime;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Input.Update();

        if (Input.WasKeyPressed(Keys.P))
            _paused = !_paused;

        if (!_paused)
        {
            PlayerStatus.Update();
            EntityManager.Update();
            EnemySpawner.Update();
        }

        Grid.Update();
        ParticleManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        bloom.BeginDraw();



        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        EntityManager.Draw(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        Grid.Draw(_spriteBatch);
        ParticleManager.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        _spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
        _spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
        DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
        DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

        if (PlayerStatus.IsGameOver)
        {
            string text = "Game Over\n" +
                "Your Score: " + PlayerStatus.Score + "\n" +
                "High Score: " + PlayerStatus.HighScore;
            Vector2 textSize = Art.Font.MeasureString(text);
            _spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
        }
        _spriteBatch.End();
    }

    private void DrawRightAlignedString(string text, float y)
    {
        var textWidth = Art.Font.MeasureString(text).X;
        _spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
    }
}
