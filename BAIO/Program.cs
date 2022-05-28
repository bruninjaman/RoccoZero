﻿namespace BAIO
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Service;

    using Interfaces;

    //[ExportPlugin("BAIO", StartupMode.Auto, "beminee", "2.0.0.0", "", 460,
    //    HeroId.npc_dota_hero_huskar, HeroId.npc_dota_hero_rattletrap,
    //    HeroId.npc_dota_hero_storm_spirit, HeroId.npc_dota_hero_nevermore,
    //    HeroId.npc_dota_hero_sven, HeroId.npc_dota_hero_antimage, HeroId.npc_dota_hero_tusk,
    //    HeroId.npc_dota_hero_puck, HeroId.npc_dota_hero_slark, HeroId.npc_dota_hero_slardar,
    //    HeroId.npc_dota_hero_magnataur, HeroId.npc_dota_hero_winter_wyvern, HeroId.npc_dota_hero_bristleback,
    //    HeroId.npc_dota_hero_ember_spirit, HeroId.npc_dota_hero_juggernaut, HeroId.npc_dota_hero_spectre,
    //    HeroId.npc_dota_hero_grimstroke, HeroId.npc_dota_hero_drow_ranger, HeroId.npc_dota_hero_dark_willow,
    //    HeroId.npc_dota_hero_obsidian_destroyer, HeroId.npc_dota_hero_pudge)]
    public class Program : Bootstrapper
    {
        private Hero Owner;

        private IHero Hero;

        private readonly HeroId[] SupportedHeroes =
        {
            HeroId.npc_dota_hero_huskar,
            HeroId.npc_dota_hero_rattletrap,
            HeroId.npc_dota_hero_storm_spirit,
            HeroId.npc_dota_hero_nevermore,
            HeroId.npc_dota_hero_sven,
            HeroId.npc_dota_hero_antimage,
            HeroId.npc_dota_hero_tusk,
            HeroId.npc_dota_hero_puck,
            HeroId.npc_dota_hero_slark,
            HeroId.npc_dota_hero_slardar,
            HeroId.npc_dota_hero_magnataur,
            HeroId.npc_dota_hero_winter_wyvern,
            HeroId.npc_dota_hero_bristleback,
            HeroId.npc_dota_hero_ember_spirit,
            HeroId.npc_dota_hero_juggernaut,
            HeroId.npc_dota_hero_spectre,
            HeroId.npc_dota_hero_grimstroke,
            HeroId.npc_dota_hero_drow_ranger,
            HeroId.npc_dota_hero_dark_willow,
            HeroId.npc_dota_hero_obsidian_destroyer,
            HeroId.npc_dota_hero_pudge,
            HeroId.npc_dota_hero_pugna,
            HeroId.npc_dota_hero_void_spirit,
            HeroId.npc_dota_hero_snapfire
        };

        private readonly HeroId[] ExcludeFromDynamicCombo = new[] { HeroId.npc_dota_hero_phantom_lancer, HeroId.npc_dota_hero_broodmother };

        protected override void OnActivate()
        {
            Owner = EntityManager.LocalHero;

            if (SupportedHeroes.Contains(this.Owner.HeroId))
            {
                var type = Assembly.GetExecutingAssembly().ExportedTypes.FirstOrDefault(x => x.GetCustomAttribute<ExportHeroAttribute>()?.Id == Owner.HeroId);

                this.Hero = (IHero)Activator.CreateInstance(type);
                this.Hero.Activate();
            }

            if (Hero == null && !ExcludeFromDynamicCombo.Contains(this.Owner.HeroId))
            {
                var type = Assembly.GetExecutingAssembly().ExportedTypes.FirstOrDefault(x => x.GetCustomAttribute<ExportHeroAttribute>()?.Id == HeroId.npc_dota_hero_base);

                this.Hero = (IHero)Activator.CreateInstance(type);
                this.Hero.Activate();
            }
        }

        protected override void OnDeactivate()
        {
            this.Hero?.Deactivate();
        }
    }
}