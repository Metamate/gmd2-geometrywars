using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Awards score when the owner is destroyed.
public sealed class AwardScoreOnDestroyed : Component
{
    private readonly IScoreTracker _score;
    private readonly int _points;
    private readonly bool _increaseMultiplier;
    private Destroyable _destroyable;

    public AwardScoreOnDestroyed(IScoreTracker score, int points, bool increaseMultiplier = true)
    {
        _score = score;
        _points = points;
        _increaseMultiplier = increaseMultiplier;
    }

    public override void OnStart(Entity owner)
    {
        _destroyable = owner.RequireComponent<Destroyable>();
        _destroyable.Destroyed += OnDestroyed;
    }

    public override void OnRemoved(Entity owner)
    {
        if (_destroyable != null)
            _destroyable.Destroyed -= OnDestroyed;
    }

    private void OnDestroyed(Entity owner)
    {
        _score.AddPoints(_points);
        if (_increaseMultiplier)
            _score.IncreaseMultiplier();
    }
}
