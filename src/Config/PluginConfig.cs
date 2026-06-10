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

    [Description("Suppresses the Poisoned client pulse/vignette display. Disabled by default to preserve vanilla visuals.")]
    public bool DisablePoisonPulseDisplay { get; set; } = false;
}
