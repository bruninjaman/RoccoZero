﻿namespace O9K.AutoUsage.Abilities.Shield;

using Core.Entities.Abilities.Base;
using Core.Entities.Abilities.Base.Components;
using Core.Entities.Abilities.Base.Types;
using Core.Managers.Menu;
using Core.Managers.Menu.Items;

internal class ShieldSettings
{
    private readonly MenuHeroToggler allyHeroes;

    private readonly MenuHeroToggler enemyHeroes;

    public ShieldSettings(Menu settings, IShield ability)
    {
        var menu = settings.GetOrAdd(new Menu(ability.DisplayName, ability.Name).SetTexture(ability.Name));

        this.HpThreshold = menu.GetOrAdd(new MenuSlider("HP% threshold (target)", 60, 1, 100));
        this.HpThreshold.SetTooltip("Use when target hp% is lower");
        this.HpThreshold.AddTranslation(Lang.Ru, "Порог ХП% (цель)");
        this.HpThreshold.AddTooltipTranslation(Lang.Ru, "Использовать способность, когда у цели меньше здоровья%");
        this.HpThreshold.AddTranslation(Lang.Cn, "生命值％（目标）");
        this.HpThreshold.AddTooltipTranslation(Lang.Cn, "当目标健康状况较差 % 时，使用能力");

        if (ability.BaseAbility.AbilityData.GetManaCost(0) > 0)
        {
            this.CriticalHpThreshold = menu.GetOrAdd(new MenuSlider("Critical HP% threshold (target)", 30, 1, 100));
            this.CriticalHpThreshold.SetTooltip("Use when target hp% is lower, mp% check is ignored");
            this.CriticalHpThreshold.AddTranslation(Lang.Ru, "Критический порог ХП% (цель)");
            this.CriticalHpThreshold.AddTooltipTranslation(
                Lang.Ru,
                "Использовать способность, когда у цели меньше здоровья%, игнорируя проверку маны");
            this.CriticalHpThreshold.AddTranslation(Lang.Cn, "关键生命值％（目标）");
            this.CriticalHpThreshold.AddTooltipTranslation(Lang.Cn, "当目标hp低于％时，mp％检查被忽略时使用");

            this.MpThreshold = menu.GetOrAdd(new MenuSlider("MP% threshold", 30, 1, 100));
            this.MpThreshold.SetTooltip("Use when ability owner mp% is higher");
            this.MpThreshold.AddTranslation(Lang.Ru, "Порог МП%");
            this.MpThreshold.AddTooltipTranslation(Lang.Ru, "Использовать способность, только если у героя больше маны%");
            this.MpThreshold.AddTranslation(Lang.Cn, "魔法值％");
            this.MpThreshold.AddTooltipTranslation(Lang.Cn, "只有当英雄有更多的法力 % 时，才使用该能力");
        }
        else
        {
            this.CriticalHpThreshold = new MenuSlider("Critical HP% threshold (target)", 0, 0, 0);
            this.MpThreshold = new MenuSlider("MP% threshold", 0, 0, 0);
        }

        if (ability.ShieldsAlly)
        {
            if (ability.ShieldsOwner)
            {
                this.SelfOnly = menu.GetOrAdd(new MenuSwitcher("Self only", false));
                this.SelfOnly.SetTooltip("Use only on self");
                this.SelfOnly.AddTranslation(Lang.Ru, "Только на себя");
                this.SelfOnly.AddTooltipTranslation(Lang.Ru, "Использовать способность, только на своего героя");
                this.SelfOnly.AddTranslation(Lang.Cn, "仅自己");
                this.SelfOnly.AddTooltipTranslation(Lang.Cn, "只对他的英雄使用这种能力");
            }
            else
            {
                this.SelfOnly = new MenuSwitcher("Self only", false);
            }
        }
        else
        {
            this.SelfOnly = new MenuSwitcher("Self only");
        }

        if (ability.ShieldsAlly || (ability.ShieldsOwner && ((ActiveAbility)ability).CanBeCastedWhileChanneling))
        {
            this.OnChannel = menu.GetOrAdd(new MenuSwitcher("Use on channel", false));
            this.OnChannel.SetTooltip("Use ability when ally is channeling");
            this.OnChannel.AddTranslation(Lang.Ru, "Использовать при ченнелинге");
            this.OnChannel.AddTooltipTranslation(Lang.Ru, "Использовать способность, когда союзник ченнелит");
            this.OnChannel.AddTranslation(Lang.Cn, "持续施法时使用");
            this.OnChannel.AddTooltipTranslation(Lang.Cn, "当盟友持续施法时使用该能力");
        }
        else
        {
            this.OnChannel = new MenuSwitcher("Use on channel", false);
        }

        if (ability is IHasRadius)
        {
            this.AlliesCount = menu.GetOrAdd(new MenuSlider("Number of allies", 1, 1, 5));
            this.AlliesCount.SetTooltip("Use only when you can shield equals/more allies");
            this.AlliesCount.AddTranslation(Lang.Ru, "Число союзников");
            this.AlliesCount.AddTooltipTranslation(Lang.Ru, "Использовать способность, если попадет по больше или равно союзников");
            this.AlliesCount.AddTranslation(Lang.Cn, "盟友数量");
            this.AlliesCount.AddTooltipTranslation(Lang.Cn, "使用能力，如果它击中更多或相等的盟友");
        }
        else
        {
            this.AlliesCount = new MenuSlider("Number of allies", 0, 0, 0);
        }

        this.EnemiesCount = menu.GetOrAdd(new MenuSlider("Number of enemies", 1, 0, 5));
        this.EnemiesCount.SetTooltip("Use only when there are equals/more enemies near target");
        this.EnemiesCount.AddTranslation(Lang.Ru, "Число врагов");
        this.EnemiesCount.AddTooltipTranslation(Lang.Ru, "Использовать способность, если врагов больше или равно");
        this.EnemiesCount.AddTranslation(Lang.Cn, "敌人数量");
        this.EnemiesCount.AddTooltipTranslation(Lang.Cn, "仅在附近有相等/更多敌人的情况下使用");

        this.Distance = menu.GetOrAdd(new MenuSlider("Distance", 900, 0, 2000));
        this.Distance.SetTooltip("Use ability only when enemy is closer");
        this.Distance.AddTranslation(Lang.Ru, "Дистанция");
        this.Distance.AddTooltipTranslation(Lang.Ru, "Использовать способность, если враг находится ближе");
        this.Distance.AddTranslation(Lang.Cn, "距离");
        this.Distance.AddTooltipTranslation(Lang.Cn, "如果敌人更近，使用能力");

        this.enemyHeroes = menu.GetOrAdd(new MenuHeroToggler("Enemies:", false, true));
        this.enemyHeroes.AddTranslation(Lang.Ru, "Враги:");
        this.enemyHeroes.AddTranslation(Lang.Cn, "敌人：");

        if (ability.ShieldsAlly)
        {
            this.allyHeroes = menu.GetOrAdd(new MenuHeroToggler("Use on:", true));
            this.allyHeroes.AddTranslation(Lang.Ru, "Использовать на:");
            this.allyHeroes.AddTranslation(Lang.Cn, "用于：");
        }
    }

    public MenuSlider AlliesCount { get; }

    public MenuSlider CriticalHpThreshold { get; }

    public MenuSlider Distance { get; }

    public MenuSlider EnemiesCount { get; }

    public MenuSlider HpThreshold { get; }

    public MenuSlider MpThreshold { get; }

    public MenuSwitcher OnChannel { get; }

    public MenuSwitcher SelfOnly { get; }

    public bool IsAllyHeroEnabled(string name)
    {
        return this.allyHeroes?.IsEnabled(name) == true;
    }

    public bool IsEnemyHeroEnabled(string name)
    {
        return this.enemyHeroes.IsEnabled(name);
    }
}