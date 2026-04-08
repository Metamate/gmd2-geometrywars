using System.IO;

namespace GeometryWars.Systems;

// Manages player score, lives, and score multiplier for one play session.
public sealed class ScoreTracker : IScoreTracker
{
    private readonly string _highScorePath;
    private int _score;
    private int _lives;
    private int _multiplier;
    private int _highScore;
    private float _multiplierTimer;

    public int Score => _score;
    public int Lives => _lives;
    public int Multiplier => _multiplier;
    public int HighScore => _highScore;
    public bool IsGameOver => _lives <= 0;

    public ScoreTracker(string highScorePath)
    {
        _highScorePath = Path.GetFullPath(highScorePath);
        _highScore = LoadHighScore();
    }

    public void StartNewRun()
    {
        _score = 0;
        _multiplier = 1;
        _lives = GameSettings.Player.StartingLives;
        _multiplierTimer = 0;
    }

    public void Update()
    {
        if (_multiplier > 1)
        {
            _multiplierTimer += FrameContext.ElapsedSeconds;
            if (_multiplierTimer > GameSettings.Player.MultiplierExpiry)
            {
                _multiplierTimer = 0;
                ResetMultiplier();
            }
        }
    }

    public void AddPoints(int basePoints)
    {
        if (IsGameOver) return;

        _score += basePoints * _multiplier;
        if (_score > _highScore)
            _highScore = _score;
    }

    public void IncreaseMultiplier()
    {
        if (IsGameOver) return;

        _multiplierTimer = 0;
        if (_multiplier < GameSettings.Player.MaxMultiplier)
            _multiplier++;
    }

    public void ResetMultiplier()
    {
        _multiplier = 1;
    }

    public void RemoveLife()
    {
        _lives--;
    }

    public void SaveHighScore()
    {
        string directory = Path.GetDirectoryName(_highScorePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(_highScorePath, _highScore.ToString());
    }

    private int LoadHighScore()
    {
        return File.Exists(_highScorePath) && int.TryParse(File.ReadAllText(_highScorePath), out int score) ? score : 0;
    }
}
