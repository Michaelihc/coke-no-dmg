using HarmonyLib;
using InventorySystem.Items.Usables;
using InventorySystem.Items.Usables.Scp330;
using PlayerStatsSystem;
using UnityEngine;

namespace CokeNoDmg.Services;

/// <summary>
/// Reworks SCP-500 server-wide. The native pill's <see cref="Scp500.OnEffectsActivated"/> instant-heals
/// 100, regenerates 100 HP over ~10s, and then PURGES healable status effects via
/// <c>PlayerEffectsController.UseMedicalItem</c> - which strips buffs like SCP-207 (the "coke" buff this
/// plugin otherwise protects). <see cref="OnEffectsActivated"/> is the single chokepoint every activation
/// path funnels through (use completion, the equip-update activation tick, and on-removal), so a prefix
/// here cancels the native heal+regen+purge in one place, then re-applies just the wanted half: a full
/// heal plus a short, configurable regeneration. No status effect is touched, so every buff survives. The
/// pill is still consumed - that happens in <c>ServerOnUsingCompleted</c>/<c>OnRemoved</c>, outside this
/// method. Gated by <c>scp500_full_heal_no_purge</c>; when off (or the plugin is unavailable) the prefix
/// returns true and SCP-500 behaves vanilla.
/// </summary>
[HarmonyPatch(typeof(Scp500), "OnEffectsActivated")]
internal static class Scp500HealPatch
{
    private static bool Prefix(Scp500 __instance)
    {
        Config.PluginConfig? config = CokeNoDmgPlugin.Instance?.Config;
        if (config?.Scp500FullHealNoPurge != true)
        {
            return true;
        }

        ReferenceHub owner = __instance.Owner;
        if (owner == null)
        {
            return true;
        }

        // Full heal (the native pill's +100), without the native effect purge.
        HealthStat health = owner.playerStats.GetModule<HealthStat>();
        if (health != null)
        {
            health.ServerHeal(health.MaxValue);
        }

        // Short regeneration window so damage taken right after the pill is topped back up.
        float seconds = Mathf.Max(0f, config.Scp500RegenSeconds);
        float rate = Mathf.Max(0f, config.Scp500RegenHpPerSecond);
        if (seconds > 0f && rate > 0f)
        {
            Scp330Bag.AddSimpleRegeneration(owner, rate, seconds);
        }

        // Skip the native activation (heal + regen + the UseMedicalItem purge).
        return false;
    }
}
