using System.Reflection;
using CustomPlayerEffects;
using HarmonyLib;

namespace CokeNoDmg.Services;

[HarmonyPatch(typeof(PlayerEffectsController), nameof(PlayerEffectsController.ServerSendPulse))]
internal static class PoisonPulseDisplayPatch
{
    private static bool Prefix(MethodBase __originalMethod)
    {
        CokeNoDmgPlugin? plugin = CokeNoDmgPlugin.Instance;
        if (plugin?.Config.DisablePoisonPulseDisplay != true)
        {
            return true;
        }

        return !IsPoisonPulse(__originalMethod);
    }

    private static bool IsPoisonPulse(MethodBase originalMethod)
    {
        if (!originalMethod.IsGenericMethod)
        {
            return false;
        }

        System.Type[] genericArguments = originalMethod.GetGenericArguments();
        return genericArguments.Length == 1 && genericArguments[0] == typeof(Poisoned);
    }
}
