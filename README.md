# gmd2-geometrywars

Geometry Wars-style sample project used in second-semester software engineering teaching.

The codebase is designed to support discussion around:
- software architecture
- game loops and state management
- component-based design
- data locality and data-oriented thinking
- object pooling
- simple event-driven decoupling
- rendering and post-processing

## Steps

The game is built up in steps. Each step is a separate project that builds on the previous
one, mostly by adding components to the entity recipes in `EntityFactory` and the systems
they need. Compare two neighbouring steps (e.g. with a diff tool) to see exactly what changed.

| Step | Topic | What's new |
| --- | --- | --- |
| `GeometryWars0` | Entities & components | The player ship, composed from components: sprite, rigidbody, movement input, clamp to screen |
| `GeometryWars1` | Shooting & Object Pool | A weapon component and bullets reused from an object pool |
| `GeometryWars2` | Enemies & collisions | Seeker and wanderer AI, the collision system, score, lives, respawning and game over |
| `GeometryWars3` | Particles | A particle manager for thousands of short-lived particles: exhaust, explosions, bullet sparks |
| `GeometryWars4` | Grid & black holes | The spring grid (data-oriented, flat arrays) and black holes that pull everything in |
| `GeometryWars5` | Bloom | Post-processing shaders for the neon glow |
| `GeometryWars6` | Audio | Music and sound effects (the finished game) |

## New in GMDCore

Compared with the core in [gmd2-pokemon](https://github.com/Metamate/gmd2-pokemon):

- `ECS/` (new): entities made of components.
- `Physics/`, `Collision/CollisionRegistry` (new): colliders, rigidbodies and collision pairs.
- `Particles/` (new): a data-oriented particle system.
- `Collections/ObjectPool` (new).
- `Input/MouseInfo`, `Input/GamePadInfo` (new).
- `Core`: runs game logic in fixed 60 Hz steps with an accumulator, so the game can render
  as fast as it likes while the simulation stays stable.
- `Input/InputManager`, `Input/KeyboardInfo`: input is sampled every frame and handed to
  the game once per logic step, so quick taps are never lost.
- `States/`: a `DrawHUD` pass after post-processing (bloom), and `Draw` is optional.

## Repository Layout

The repository is intentionally split into a core library and the game, plus a content builder:

- `GMDCore` contains reusable engine-style code such as the game shell, input handling, the entity/component model, generic physics/collision primitives, particle infrastructure, and pooling.
- `GeometryWars0`–`GeometryWars6` contain the actual game, built up step by step (see [Steps](#steps)): states, gameplay systems, entity recipe composition, and Geometry Wars-specific components and rules. `GeometryWars6` is the finished game, and the one this walkthrough describes.
- `Content` contains the game's raw assets (textures, fonts, sounds, shaders) and the C# rules that build them (see [Content](#content)).

## Architecture Overview

At a high level, the game is structured like this:

1. `Game1` owns the runtime shell.
2. `PlayState` owns one active play session.
3. `PlaySession` builds and updates the mutable game world for a run.
4. `EntityWorld` updates entities and collisions.
5. `EntityFactory` defines entity recipes by composing components.
6. Components implement the actual behavior attached to each entity.

This means the code is intentionally split between:
- shell/runtime concerns
- session/world concerns
- entity composition
- reusable gameplay behavior

## Runtime Layer

[Game1](GeometryWars6/Game1.cs) is the MonoGame application root.

Its job is to:
- update frame timing, input, assets, audio, and performance tracking
- own the state stack (play and game-over states)
- coordinate drawing

Mutable runtime state is grouped into small service objects and exposed through [PlayContext](GeometryWars6/Services/PlayContext.cs), which is passed into gameplay code.

The shell samples raw input every rendered frame, but gameplay still runs on a fixed 60 Hz step. Button presses/releases are buffered so quick taps are still visible to the next logic tick.

## State Layer

[PlayState](GeometryWars6/States/PlayState.cs) represents the main gameplay state.

Its job is to:
- create a new [PlaySession](GeometryWars6/Systems/PlaySession.cs)
- update pause/debug flow
- switch to game-over state when a run ends
- draw world and HUD separately

This keeps menu/state transitions outside the entity/component layer.

## Session Layer

[PlaySession](GeometryWars6/Systems/PlaySession.cs) owns the mutable state for one run.

It builds:
- the score tracker
- the particle manager
- the grid
- the entity world
- the entity factory
- the enemy director
- the player entity

`PlaySession` is the main composition root for gameplay.

## World Layer

[EntityWorld](GeometryWars6/Systems/EntityWorld.cs) coordinates:
- entity registration and updates
- collision handling
- pending additions during update
- deferred entity removal

Supporting classes such as [EntityCatalog](GeometryWars6/Systems/EntityCatalog.cs) and [CollisionSystem](GeometryWars6/Systems/CollisionSystem.cs) keep those responsibilities separated.

Projectile pooling lives in [BulletSpawner](GeometryWars6/Systems/BulletSpawner.cs), so the world can stay focused on generic entity lifetime and update flow.

## Entity Composition

[EntityFactory](GeometryWars6/Systems/EntityFactory.cs) defines the entity recipes.

This is where you can read how an entity is assembled.

Examples:
- the player is composed from movement input, weapon trigger/input, weapon firing pattern, weapon feedback, collision response, respawn state, respawn effects, rendering, and physics
- bullets are composed from physics, collision, facing velocity, viewport expiry, and grid force
- black holes are composed from gravity, orbiting particles, grid force, health, bullet damage, expiry-on-zero-health, and hit effects

This is an important teaching point: entities should emerge from composition rather than from deep inheritance trees.

## Typed Definitions

The project uses small typed definition records in [GameplayDefinitions.cs](GeometryWars6/Definitions/GameplayDefinitions.cs) for content variants such as the player, bullets, enemy types, and black holes. They group related tuning values by gameplay object and keep balancing data out of component code while letting the factory compose entities from shared reusable definitions.

## Component Model

All components inherit from [Component](GMDCore/ECS/Components/Component.cs).

The lifecycle is callback-based:
- `OnAdded`
- `OnStart`
- `PreUpdate`
- `Update`
- `Simulate`
- `PostUpdate`
- `OnCollision`
- `Draw`
- `OnRemoved`

[Entity](GMDCore/ECS/Entity.cs) runs those phases in a fixed order every frame.

`OnRemoved` is intended for engine-level cleanup such as unregistering subscriptions when an entity leaves the world. It is not the same as a gameplay destruction event like `Destroyable.Destroyed`.

The intended design rule is:
- each component should represent one clear capability or behavior
- components should be reusable where practical
- entity-specific naming should be avoided unless the behavior is genuinely unique

Good examples:
- `Health`
- `TakeDamageOnBulletCollision`
- `FaceVelocity`
- `ApplyMovementInput`

## Components Vs Systems

This project does not use a strict ECS where components are data-only.

Instead, it uses a hybrid model:
- components may contain behavior, as long as that behavior is local to one entity
- systems/session objects coordinate behavior that spans multiple entities or the whole run

A good rule of thumb is:
- use a component when the logic is mostly about the owner and its own state
- use a system when the logic touches many entities, owns game/session rules, or needs central ordering

Good component responsibilities:
- `Health`
- `RespawnState`
- `Weapon`
- `FaceVelocity`
- `FadeInOnSpawn`
- `SeekTarget`

Good system/session responsibilities:
- collision detection in `CollisionSystem`
- spawn pacing in `EnemyDirector`
- projectile pooling in `BulletSpawner`
- run-level consequences in `PlaySession`

Examples from this codebase:
- `BeginRespawnOnLethalCollision` is a component because it only decides when the player has taken a lethal hit
- `PlaySession` handles the arena-wide consequences of player death, because clearing enemies and resetting spawning are run-level rules
- the weapon flow is split so `FireWeaponOnInput` handles trigger input, `Weapon` handles cadence, `SpawnTwinBulletsOnFired` handles the projectile pattern, and `PlaySoundOnWeaponFired` handles feedback

For a larger game, prefer this rule:
- local behavior in components
- cross-entity orchestration in systems
- explicit composition in `EntityFactory`
- move hot/shared processing into systems when scale or performance demands it

## Local Events

The project uses small local events inside an entity's component graph, not a global event bus.

Examples:
- [Health](GeometryWars6/Components/Combat/Health.cs) publishes `Damaged` and `Depleted`
- [RespawnState](GeometryWars6/Components/Lifecycle/RespawnState.cs) publishes `Died` and `Respawned`

This allows reactive components such as:
- [PlayHitParticlesOnDamage](GeometryWars6/Components/Visuals/PlayHitParticlesOnDamage.cs)
- [DestroyWhenHealthDepleted](GeometryWars6/Components/Lifecycle/DestroyWhenHealthDepleted.cs)
- [PlayRespawnEffects](GeometryWars6/Components/Lifecycle/PlayRespawnEffects.cs)

to react without tightly coupling everything together.

The intent is to show a simple use of events where they help, without making control flow hard to follow.

## Data-Oriented Notes

Not every subsystem uses the same style on purpose.

- `Entity` and gameplay components favor clarity and composition.
- [Grid](GeometryWars6/Systems/Grid.cs) is a denser simulation-oriented subsystem that favors flat arrays and tight loops for better data locality.
- [ParticleManager](GMDCore/Particles/ParticleManager.cs) is a specialized high-volume visual system rather than a normal entity/component workflow.
- [GameAssets](GeometryWars6/Services/GameAssets.cs) acts as a simple shared asset catalog, which is a lightweight example of a flyweight-style resource holder.

## Design Guidelines For Students

When adding or changing gameplay code, prefer these rules:

1. Put shell and application concerns in `Game1` or game states.
2. Put run-specific orchestration in `PlaySession` and systems.
3. Put entity recipe assembly in `EntityFactory`.
4. Put reusable behavior in components.
5. Put cross-entity or run-level orchestration in systems or `PlaySession`.
6. Prefer capability-based component names over entity-specific names.
7. Use direct calls for core flow, and local events only for state-change reactions.
8. Split a component when it has multiple unrelated reasons to change.
9. Avoid over-fragmenting behavior into tiny components if it makes the design harder to teach.

## Suggested Reading Order

If you are new to the project, a good reading order is:

1. [Game1](GeometryWars6/Game1.cs)
2. [PlayState](GeometryWars6/States/PlayState.cs)
3. [PlaySession](GeometryWars6/Systems/PlaySession.cs)
4. [EntityFactory](GeometryWars6/Systems/EntityFactory.cs)
5. [Entity](GMDCore/ECS/Entity.cs)
6. a few concrete components from `Components/`

That gives the clearest top-down view of how the game fits together.

## Content

All steps share the same raw assets, built by the **content builder** (MonoGame 3.8.5+):

```text
Content/
├── Assets/                  # The raw assets: textures, fonts, sounds, shaders
├── Builder/Builder.cs       # The rules for building the assets, in C#
├── BuildContent.targets     # Runs the builder when the game project builds
└── Content.csproj
```

There is no `.mgcb` file and no MGCB Editor. `Builder.cs` decides how each kind of asset is
processed. Each step project imports `BuildContent.targets`, so building a step also builds
the assets into its output folder, where `Content.Load` finds them.

To add an asset, put it in `Content/Assets` and, if no existing rule matches it, add a rule
in `Builder.cs`.

## Running

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```sh
dotnet run --project GeometryWars6
```
