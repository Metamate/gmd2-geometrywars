using System.IO;

namespace GeometryWars;

public static class PlayerStatus
{
    // amount of time it takes, in seconds, for a multiplier to expire. 
    private const float multiplierExpiryTime = 0.8f;
    private const int maxMultiplier = 20;
    private const string highScoreFilename = "highscore.txt";
    private static float multiplierTimeLeft;    // time until the current multiplier expires 
    private static int scoreForExtraLife;       // score required to gain an extra life 
                                                // Static constructor 

    static PlayerStatus()
    {
        HighScore = LoadHighScore();
        Reset();
    }

    public static int Lives { get; private set; }
    public static int Score { get; private set; }
    public static int Multiplier { get; private set; }
    public static bool IsGameOver { get { return Lives == 0; } }
    public static int HighScore { get; private set; }

    public static void Reset()
    {
        if (Score > HighScore)
            SaveHighScore(HighScore = Score);

        Score = 0;
        Multiplier = 1;
        Lives = 4;
        scoreForExtraLife = 2000;
        multiplierTimeLeft = 0;
    }
    public static void Update()
    {
        if (Multiplier > 1)
        {
            // update the multiplier timer 
            if ((multiplierTimeLeft -= (float)Game1.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
            {
                multiplierTimeLeft = multiplierExpiryTime;
                ResetMultiplier();
            }
        }
    }
    public static void AddPoints(int basePoints)
    {
        if (PlayerShip.Instance.IsDead)
            return;
        Score += basePoints * Multiplier;
        while (Score >= scoreForExtraLife)
        {
            scoreForExtraLife += 2000;
            Lives++;
        }
    }
    public static void IncreaseMultiplier()
    {
        if (PlayerShip.Instance.IsDead)
            return;
        multiplierTimeLeft = multiplierExpiryTime;
        if (Multiplier < maxMultiplier)
            Multiplier++;
    }
    public static void ResetMultiplier()
    {
        Multiplier = 1;
    }
    public static void RemoveLife()
    {
        Lives--;
    }
    private static int LoadHighScore()
    {
        // return the saved high score if possible and return 0 otherwise 
        return File.Exists(highScoreFilename) && int.TryParse(File.ReadAllText(highScoreFilename), out int score) ? score : 0;
    }
    private static void SaveHighScore(int score)
    {
        File.WriteAllText(highScoreFilename, score.ToString());
    }
}