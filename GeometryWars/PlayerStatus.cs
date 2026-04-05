using System.IO;
using GeometryWars.Services;

namespace GeometryWars;

public static class PlayerStatus
{
    private const float MultiplierExpiryTime = 0.8f;
    private const int MaxMultiplier = 20;
    private const string HighScoreFilename = "highscore.txt";

    private static float _multiplierTimeLeft;
    private static int _scoreForExtraLife;

    static PlayerStatus()
    {
        HighScore = LoadHighScore();
        Reset();
    }

    public static int Lives { get; private set; }
    public static int Score { get; private set; }
    public static int Multiplier { get; private set; }
    public static bool IsGameOver => Lives == 0;
    public static int HighScore { get; private set; }

    public static void Reset()
    {
        if (Score > HighScore)
            SaveHighScore(HighScore = Score);

        Score = 0;
        Multiplier = 1;
        Lives = 4;
        _scoreForExtraLife = 2000;
        _multiplierTimeLeft = 0;
    }

    public static void Update()
    {
        if (Multiplier > 1 && (_multiplierTimeLeft -= GameServices.ElapsedSeconds) <= 0)
        {
            _multiplierTimeLeft = MultiplierExpiryTime;
            Multiplier = 1;
        }
    }

    public static void AddPoints(int basePoints)
    {
        Score += basePoints * Multiplier;
        while (Score >= _scoreForExtraLife)
        {
            _scoreForExtraLife += 2000;
            Lives++;
        }
    }

    public static void IncreaseMultiplier()
    {
        _multiplierTimeLeft = MultiplierExpiryTime;
        if (Multiplier < MaxMultiplier) Multiplier++;
    }

    public static void RemoveLife() => Lives--;

    private static int LoadHighScore()
        => File.Exists(HighScoreFilename) && int.TryParse(File.ReadAllText(HighScoreFilename), out int s) ? s : 0;

    private static void SaveHighScore(int score)
        => File.WriteAllText(HighScoreFilename, score.ToString());
}
