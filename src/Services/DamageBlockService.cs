using CokeNoDmg.Config;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace CokeNoDmg.Services;

internal sealed class DamageBlockService
{
    private readonly PluginConfig _config;

    public DamageBlockService(PluginConfig config)
    {
        _config = config;
    }

    public void Enable()
    {
        PlayerEvents.Hurting += OnPlayerHurting;
    }

    public void Disable()
    {
        PlayerEvents.Hurting -= OnPlayerHurting;
    }

    private void OnPlayerHurting(PlayerHurtingEventArgs ev)
    {
        if (ev.DamageHandler is not UniversalDamageHandler damageHandler)
        {
            return;
        }

        if (ShouldBlock(damageHandler.TranslationId))
        {
            ev.IsAllowed = false;
        }
    }

    private bool ShouldBlock(byte translationId)
    {
        return (_config.BlockScp207Damage && translationId == DeathTranslations.Scp207.Id)
            || (_config.BlockPoisonDamage && translationId == DeathTranslations.Poisoned.Id);
    }
}
