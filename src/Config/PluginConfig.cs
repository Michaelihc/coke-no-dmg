using System.ComponentModel;

namespace CokeNoDmg.Config;

public sealed class PluginConfig
{
    public bool IsEnabled { get; set; } = true;

    [Description("Language for user-facing text. Empty/default uses Chinese, 'cn' forces Chinese, and 'en' forces English.")]
    public string Language { get; set; } = string.Empty;

    [Description("Cancels health damage caused by SCP-207 ticking damage.")]
    public bool BlockScp207Damage { get; set; } = true;

    [Description("Cancels health damage caused by the Poisoned status effect.")]
    public bool BlockPoisonDamage { get; set; } = true;

    [Description("Suppresses the Poisoned client pulse/vignette display.")]
    public bool DisablePoisonPulseDisplay { get; set; } = true;

    [Description("Reworks every SCP-500 on the server: cancels the native use (skipping the heal-and-purge that strips buffs like SCP-207), then heals the user to FULL health and grants a short regeneration. Status effects are preserved. Set false to leave SCP-500 vanilla.")]
    public bool Scp500FullHealNoPurge { get; set; } = true;

    [Description("Seconds of regeneration granted after an SCP-500 full-heal (the 'regen for a few seconds' window that tops the user back up if they take damage right after).")]
    public float Scp500RegenSeconds { get; set; } = 5f;

    [Description("Regeneration rate (HP per second) during the SCP-500 regen window.")]
    public float Scp500RegenHpPerSecond { get; set; } = 20f;
}
