namespace GeometryWars.Systems;

public interface IScoreTracker
{
    bool IsGameOver { get; }
    void AddPoints(int basePoints);
    void IncreaseMultiplier();
    void RemoveLife();
}
