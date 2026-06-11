using System;
using CokeNoDmg.Config;
using CokeNoDmg.Services;
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;

namespace CokeNoDmg;

public sealed class CokeNoDmgPlugin : Plugin<PluginConfig>
{
    private Harmony? _harmony;
    private DamageBlockService? _damageBlockService;

    public override string Name => "CokeNoDmg";

    public override string Description => "Removes SCP-207 and poison health damage without changing their other effects.";

    public override string Author => "Codex";

    public override Version Version => new(0, 1, 2);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Instance = this;

        if (!Config.IsEnabled)
        {
            Logger.Info(Text("CokeNoDmg is disabled by config.", "CokeNoDmg 已被配置禁用。"));
            return;
        }

        if (Config.DisablePoisonPulseDisplay)
        {
            TryEnablePoisonPulsePatch();
        }

        _damageBlockService = new DamageBlockService(Config);
        _damageBlockService.Enable();

        Logger.Info(Text("CokeNoDmg enabled.", "CokeNoDmg 已启用。"));
    }

    public override void Disable()
    {
        _damageBlockService?.Disable();
        _damageBlockService = null;
        if (_harmony is not null)
        {
            _harmony.UnpatchAll(_harmony.Id);
        }

        _harmony = null;
        Instance = null;

        Logger.Info(Text("CokeNoDmg disabled.", "CokeNoDmg 已禁用。"));
    }

    internal static CokeNoDmgPlugin? Instance { get; private set; }

    private void TryEnablePoisonPulsePatch()
    {
        try
        {
            _harmony = new Harmony("com.codex.scpsl.cokenodmg");
            _harmony.PatchAll(typeof(CokeNoDmgPlugin).Assembly);
        }
        catch (Exception ex)
        {
            _harmony = null;
            Logger.Error(Text(
                $"CokeNoDmg could not enable poison pulse suppression; damage blocking will still run. {ex.GetType().Name}: {ex.Message}",
                $"CokeNoDmg 无法启用中毒脉冲隐藏；扣血拦截仍会继续运行。{ex.GetType().Name}: {ex.Message}"));
        }
    }

    private string Text(string english, string chinese)
    {
        string language = (Config.Language ?? string.Empty).Trim().ToLowerInvariant();
        return language == "en" ? english : chinese;
    }
}
