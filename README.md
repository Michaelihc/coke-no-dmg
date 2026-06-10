# CokeNoDmg

[中文](#中文) | [English](#english)

## 中文

CokeNoDmg 是一个极简 LabAPI 插件，用来取消 SCP-207 和中毒状态造成的扣血。

它只拦截玩家受伤事件中的两类原版伤害：

- SCP-207 tick 伤害
- Poisoned 状态 tick 伤害

它不会移除 SCP-207 或 Poisoned 效果本身，也不会修改 SCP-207 的移速、耐力或中毒脉冲显示。因此它和其他修改效果、物品或 HUD 的插件冲突面更小。

### 配置

配置文件位置：

```text
%APPDATA%\SCP Secret Laboratory\LabAPI\configs\<端口>\CokeNoDmg\
```

可用配置：

- `is_enabled`: 是否启用插件。
- `language`: `""` 或 `"cn"` 使用中文日志，`"en"` 使用英文日志。
- `block_scp207_damage`: 是否取消 SCP-207 扣血。
- `block_poison_damage`: 是否取消中毒扣血。

### 命令

没有玩家命令或 RA 命令。

### 已知冲突

如果其他插件也在 `PlayerEvents.Hurting` 中强制替换或重新允许同一次伤害，最终结果取决于插件加载和事件执行顺序。CokeNoDmg 不使用 Harmony，不改原版效果状态，只取消匹配的原版伤害事件。

## English

CokeNoDmg is a minimal LabAPI plugin that cancels health damage from SCP-207 and the Poisoned status effect.

It only intercepts two vanilla damage sources in the player hurting event:

- SCP-207 tick damage
- Poisoned status tick damage

It does not remove the SCP-207 or Poisoned effects themselves, and it does not change SCP-207 movement, stamina behavior, or poison pulse display. This keeps the conflict surface small with plugins that modify effects, items, or HUD behavior.

### Config

Config location:

```text
%APPDATA%\SCP Secret Laboratory\LabAPI\configs\<port>\CokeNoDmg\
```

Available settings:

- `is_enabled`: Enables or disables the plugin.
- `language`: `""` or `"cn"` uses Chinese logs, `"en"` uses English logs.
- `block_scp207_damage`: Cancels SCP-207 damage.
- `block_poison_damage`: Cancels poison damage.

### Commands

No player commands or RA commands.

### Known Conflicts

If another plugin also handles `PlayerEvents.Hurting` and forcefully replaces or re-allows the same damage, the final result depends on plugin load and event order. CokeNoDmg does not use Harmony, does not modify vanilla effect state, and only cancels matching vanilla damage events.
