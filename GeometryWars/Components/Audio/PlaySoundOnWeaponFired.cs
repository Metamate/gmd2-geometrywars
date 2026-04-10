using System;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Utils;
using Microsoft.Xna.Framework.Audio;

namespace GeometryWars.Components.Audio;

// Plays a firing sound whenever the owner's weapon reports a shot.
public sealed class PlaySoundOnWeaponFired : Component
{
    private readonly AudioManager _audio;
    private readonly Func<SoundEffect> _getSound;
    private Weapon _weapon;

    public PlaySoundOnWeaponFired(AudioManager audio, Func<SoundEffect> getSound)
    {
        _audio = audio;
        _getSound = getSound;
    }

    public override void OnStart(Entity owner)
    {
        if (_weapon != null)
            _weapon.Fired -= HandleFired;

        _weapon = owner.RequireComponent<Weapon>();
        _weapon.Fired += HandleFired;
    }

    private void HandleFired(WeaponFired shot)
    {
        _audio.Play(_getSound(), 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
