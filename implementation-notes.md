# Implementation Notes

## Scope

CokeNoDmg removes health damage from SCP-207 and poison while preserving the native status effects.

## API Decision

The plugin subscribes to LabAPI `PlayerEvents.Hurting` and cancels matching damage via `PlayerHurtingEventArgs.IsAllowed = false`.

Evidence:

- `..\.references\LabAPI\LabApi\Events\Arguments\PlayerEvents\PlayerHurtingEventArgs.cs` exposes a cancellable player hurt event with mutable `DamageHandler`.
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\CustomPlayerEffects\Scp207.cs` applies SCP-207 tick damage with `new UniversalDamageHandler(damage, DeathTranslations.Scp207)`.
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\CustomPlayerEffects\Poisoned.cs` applies poison tick damage with `new UniversalDamageHandler(damagePerTick, DeathTranslations.Poisoned)`.
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\PlayerStatsSystem\UniversalDamageHandler.cs` stores the translation ID used for matching.

## Conflict Avoidance

No Harmony patches, polling, per-frame work, direct health rewrites, or effect removals are used. The plugin cancels only `UniversalDamageHandler` instances whose translation ID is `DeathTranslations.Scp207.Id` or `DeathTranslations.Poisoned.Id`.

## Testing Notes

Build verification should be run with `dotnet build CokeNoDmg.csproj`.

Live dummy testing on port `7777` passed on 2026-06-10. The boundary checks verified that SCP-207 and Poisoned damage were canceled, while adjacent status/SCP damage and cola/pink/grenade explosions were not canceled. This server's current native dummy state can reduce allowed damage probes to `0` health damage, so the live check treated `PlayerEvents.Hurting`'s post-plugin `eventAllowed` value as the source of truth for cancellation boundaries.

Live verification needs a visible SCP:SL test server:

1. Deploy `CokeNoDmg.dll` to `%APPDATA%\SCP Secret Laboratory\LabAPI\plugins\8888`.
2. Restart the visible local test server on port `8888`.
3. Give a test player SCP-207 and confirm movement/stamina behavior remains active while health no longer decreases from SCP-207 ticks.
4. Apply Poisoned and confirm the poison pulse/effect remains active while health no longer decreases from poison ticks.
