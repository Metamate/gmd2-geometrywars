using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;

namespace GeometryWars.Components.Audio;

// Plays a sound when the owner is destroyed.
public sealed class PlaySoundOnDestroyed : Component
{
    private readonly Action _playSound;
    private Destroyable _destroyable;

    public PlaySoundOnDestroyed(Action playSound)
    {
        _playSound = playSound;
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
        _playSound();
    }
}
