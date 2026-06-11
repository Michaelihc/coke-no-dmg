using CustomPlayerEffects;
using HarmonyLib;

namespace CokeNoDmg.Services;

[HarmonyPatch(typeof(PlayerEffectsController), "TargetRpcReceivePulse")]
internal static class PoisonPulseDisplayPatch
{
    private static bool Prefix(PlayerEffectsController __instance, byte effectIndex)
    {
        CokeNoDmgPlugin? plugin = CokeNoDmgPlugin.Instance;
        if (plugin?.Config.DisablePoisonPulseDisplay != true)
        {
            return true;
        }

        return effectIndex >= __instance.EffectsLength
            || __instance.AllEffects[effectIndex] is not Poisoned;
    }
}
