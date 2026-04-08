using System.IO;

namespace GeometryWars;

public static class PlayerStatus
{
    private static int _score;
    private static int _lives;
    private static int _multiplier;
    private static int _highScore;
    private static float _multiplierTimer;

    private const string HighScoreFile = "highscore.txt";

    public static int Score => _score;
    public static int Lives => _lives;
    public static int Multiplier => _multiplier;
    public static int HighScore => _highScore;
    public static bool IsGameOver => _lives <= 0;

    static PlayerStatus()
    {
        _highScore = LoadHighScore();
    }

    public static void Reset()
    {
        if (_score > _highScore)
        {
            _highScore = _score;
            SaveHighScore(_highScore);
        }

        _score = 0;
        _multiplier = 1;
        _lives = GameSettings.Player.StartingLives;
        _multiplierTimer = 0;
    }

    public static void Update()
    {
        if (_multiplier > 1)
        {
            _multiplierTimer += (float)FrameContext.Time.ElapsedGameTime.TotalSeconds;
            if (_multiplierTimer > GameSettings.Player.MultiplierExpiry)
            {
                _multiplierTimer = 0;
                ResetMultiplier();
            }
        }
    }

    public static void AddPoints(int basePoints)
    {
        if (IsGameOver) return;

        _score += basePoints * _multiplier;
    }

    public static void IncreaseMultiplier()
    {
        if (IsGameOver) return;

        _multiplierTimer = 0;
        if (_multiplier < GameSettings.Player.MaxMultiplier)
            _multiplier++;
    }

    public static void ResetMultiplier()
    {
        _multiplier = 1;
    }

    public static void RemoveLife()
    {
        _lives--;
    }

    private static int LoadHighScore()
    {
        return File.Exists(HighScoreFile) && int.TryParse(File.ReadAllText(HighScoreFile), out int score) ? score : 0;
    }

    private static void SaveHighScore(int score)
    {
        File.WriteAllText(HighScoreFile, score.ToString());
    }
}
