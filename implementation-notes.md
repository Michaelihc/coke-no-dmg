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

For damage removal, no Harmony patches, polling, per-frame work, direct health rewrites, or effect removals are used. The plugin cancels only `UniversalDamageHandler` instances whose translation ID is `DeathTranslations.Scp207.Id` or `DeathTranslations.Poisoned.Id`.

`disable_poison_pulse_display` is the only Harmony-backed feature because LabAPI does not expose an event around poison pulse sends. Harmony cannot patch the open generic `PlayerEffectsController.ServerSendPulse<T>()` method safely in this environment, so the plugin patches the non-generic private `TargetRpcReceivePulse` send method and suppresses only pulses whose resolved effect index is `Poisoned`. Patch setup is wrapped so damage blocking still enables if pulse suppression fails.

## SCP-500 full-heal rework (server-wide)

`scp500_full_heal_no_purge` (default on) reworks every SCP-500 on the server so it no longer strips status effects (the native pill purges all `IHealableEffect`s — including the SCP-207 "coke" buff this plugin otherwise protects). `Scp500HealPatch` is a Harmony **prefix on `Scp500.OnEffectsActivated`** — the single method that does the 100 HP heal, the 100-HP/10s regen, **and** the `PlayerEffectsController.UseMedicalItem` purge, and the one chokepoint every activation path funnels through (use completion, the equip-update activation tick, and on-removal). The prefix returns `false` to skip that native body entirely (no heal, no regen, no purge), then re-applies just the wanted half: a full heal (`HealthStat.ServerHeal(MaxValue)`) plus a short regeneration via the public `InventorySystem.Items.Usables.Scp330.Scp330Bag.AddSimpleRegeneration(hub, rate, duration)` (`scp500_regen_hp_per_second` over `scp500_regen_seconds`). The pill is still consumed — `ServerRemoveSelf` runs in `ServerOnUsingCompleted`/`OnRemoved`, outside the patched method. No status effect is touched, so SCP-207 and every other buff survive. Harmony is now enabled whenever this OR the poison-pulse feature is on; each patch is internally gated by its config flag, so `PatchAll` is safe.

**Why a Harmony patch, not the LabAPI `ItemUsageEffectsApplying` event:** that event was tried first (`IsAllowed = false`) and headless-verified to NOT prevent the purge — `scp207 3->0` while the heal still ran. The native activation fires from several paths and the event only gates one, so the buff was still stripped. Patching `OnEffectsActivated` directly covers every path. (This is the same reason the old reinforcements-system implementation patched `UseMedicalItem` rather than cancel the event.)

Evidence:

- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\InventorySystem\Items\Usables\Scp500.cs` — `OnEffectsActivated` heals 100, regenerates 100 over 10s, then calls `UseMedicalItem(this)` (the purge).
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\InventorySystem\Items\Usables\Consumable.cs` — `ServerOnUsingCompleted`/`EquipUpdate`/`OnRemoved` all funnel `ActivateEffects` → `OnEffectsActivated`; consume (`ServerRemoveSelf`) is separate.
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\InventorySystem\Items\ItemBase.cs` — `public ReferenceHub Owner`.
- `..\.references\Decompiled\DedicatedServer\Assembly-CSharp\InventorySystem\Items\Usables\Scp330\Scp330Bag.cs` — public static `AddSimpleRegeneration(ReferenceHub, rate, duration)`.

This moved out of the reinforcements-system plugin (which previously gated a Harmony `UseMedicalItem` prefix to serial-tracked kit pills) so it now applies to ALL SCP-500 on the server, not just reinforcement kits. Headless-verified on 2026-06-18 via the reinforcements test harness on port 7777 (`Scp500HealVerifier`): a wounded dummy with SCP-207 uses an SCP-500 → `scp207 3->3 hp 20->100 consumed=True`.

## Testing Notes

Build verification should be run with `dotnet build CokeNoDmg.csproj`.

Live dummy testing on port `7777` passed on 2026-06-10. The boundary checks verified that SCP-207 and Poisoned damage were canceled, while adjacent status/SCP damage and cola/pink/grenade explosions were not canceled. This server's current native dummy state can reduce allowed damage probes to `0` health damage, so the live check treated `PlayerEvents.Hurting`'s post-plugin `eventAllowed` value as the source of truth for cancellation boundaries.

Live verification needs a visible SCP:SL test server:

1. Deploy `CokeNoDmg.dll` to `%APPDATA%\SCP Secret Laboratory\LabAPI\plugins\8888`.
2. Restart the visible local test server on port `8888`.
3. Give a test player SCP-207 and confirm movement/stamina behavior remains active while health no longer decreases from SCP-207 ticks.
4. Apply Poisoned and confirm health no longer decreases from poison ticks; with the default config, the poison pulse/vignette display should also be suppressed.
