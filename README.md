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

## Repository Layout

The repository is intentionally split into two projects:

- `GMDCore` contains reusable engine-style code such as the game shell, input handling, the entity/component model, generic physics/collision primitives, particle infrastructure, and pooling.
- `GeometryWars` contains the actual game: states, gameplay systems, archetype composition, assets, and Geometry Wars-specific components and rules.

## Architecture Overview

At a high level, the game is structured like this:

1. `Game1` owns the runtime shell.
2. `PlayState` owns one active play session.
3. `PlaySession` builds and updates the mutable game world for a run.
4. `EntityWorld` updates entities, collisions, and pooled bullets.
5. `EntityFactory` defines entity recipes by composing components.
6. Components implement the actual behavior attached to each entity.

This means the code is intentionally split between:
- shell/runtime concerns
- session/world concerns
- entity composition
- reusable gameplay behavior

## Runtime Layer

[Game1](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Game1.cs) is the MonoGame application root.

Its job is to:
- update frame timing, input, assets, audio, and performance tracking
- own the active game state
- coordinate drawing

Mutable runtime state is grouped into small service objects and exposed through [PlayContext](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Services/PlayContext.cs), which is passed into gameplay code.

The shell samples raw input every rendered frame, but gameplay still runs on a fixed 60 Hz step. Button presses/releases are buffered so quick taps are still visible to the next logic tick.

## State Layer

[PlayState](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/States/PlayState.cs) represents the main gameplay state.

Its job is to:
- create a new [PlaySession](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/PlaySession.cs)
- update pause/debug flow
- switch to game-over state when a run ends
- draw world and HUD separately

This keeps menu/state transitions outside the entity/component layer.

## Session Layer

[PlaySession](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/PlaySession.cs) owns the mutable state for one run.

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

[EntityWorld](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/EntityWorld.cs) coordinates:
- entity registration and updates
- collision handling
- pending additions during update
- pooled bullet spawning

Supporting classes such as [EntityCatalog](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/EntityCatalog.cs) and [CollisionSystem](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/CollisionSystem.cs) keep those responsibilities separated.

## Entity Composition

[EntityFactory](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/EntityFactory.cs) defines the entity recipes.

This is where you can read how an entity is assembled.

Examples:
- the player is composed from movement input, weapon trigger/input, weapon firing pattern, weapon feedback, collision response, respawn state, respawn effects, rendering, and physics
- bullets are composed from physics, collision, facing velocity, viewport expiry, and grid force
- black holes are composed from gravity, orbiting particles, grid force, health, bullet damage, expiry-on-zero-health, and hit effects

This is an important teaching point: entities should emerge from composition rather than from deep inheritance trees.

## Component Model

All components inherit from [Component](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GMDCore/ECS/Components/Component.cs).

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

[Entity](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GMDCore/ECS/Entity.cs) runs those phases in a fixed order every frame.

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
- pooled entity management in `EntityWorld`
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
- [Health](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Components/Combat/Health.cs) publishes `Damaged` and `Depleted`
- [RespawnState](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Components/Lifecycle/RespawnState.cs) publishes `Died` and `Respawned`

This allows reactive components such as:
- [PlayHitParticlesOnDamage](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Components/Visuals/PlayHitParticlesOnDamage.cs)
- [DestroyWhenHealthDepleted](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Components/Lifecycle/DestroyWhenHealthDepleted.cs)
- [PlayRespawnEffects](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Components/Lifecycle/PlayRespawnEffects.cs)

to react without tightly coupling everything together.

The intent is to show a simple use of events where they help, without making control flow hard to follow.

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

1. [Game1](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Game1.cs)
2. [PlayState](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/States/PlayState.cs)
3. [PlaySession](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/PlaySession.cs)
4. [EntityFactory](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GeometryWars/Systems/EntityFactory.cs)
5. [Entity](C:/Users/jakik/projects/GMDPlayground/gmd2-geometrywars/GMDCore/ECS/Entity.cs)
6. a few concrete components from `Components/`

That gives the clearest top-down view of how the game fits together.
