using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;

namespace GeometryWars.Components.Audio;

// Plays a sound when the owner is destroyed.
public sealed class PlaySoundOnDestroyedComponent : Component
{
    private readonly Action _playSound;
    private DestroyableComponent _destroyable;

    public PlaySoundOnDestroyedComponent(Action playSound)
    {
        _playSound = playSound;
    }

    public override void OnStart(Entity owner)
    {
        if (_destroyable != null)
            _destroyable.Destroyed -= OnDestroyed;

        _destroyable = owner.GetComponent<DestroyableComponent>();
        if (_destroyable != null)
            _destroyable.Destroyed += OnDestroyed;
    }

    private void OnDestroyed(Entity owner)
    {
        _playSound();
    }
}
