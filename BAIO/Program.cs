using System.Threading;
using BAIO.Heroes.Base;

namespace BAIO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using log4net;
    using PlaySharp.Toolkit.Logging;

    using Ensage;
    using Ensage.SDK.Renderer.Particle;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Interfaces;

    [ExportPlugin("BAIO", StartupMode.Auto, "beminee", "2.0.0.0", "", 460/*,
        HeroId.npc_dota_hero_huskar, HeroId.npc_dota_hero_rattletrap,
        HeroId.npc_dota_hero_storm_spirit, HeroId.npc_dota_hero_nevermore,
        HeroId.npc_dota_hero_sven, HeroId.npc_dota_hero_antimage, HeroId.npc_dota_hero_tusk,
        HeroId.npc_dota_hero_puck, HeroId.npc_dota_hero_slark, HeroId.npc_dota_hero_slardar,
        HeroId.npc_dota_hero_magnataur, HeroId.npc_dota_hero_winter_wyvern, HeroId.npc_dota_hero_bristleback,
        HeroId.npc_dota_hero_ember_spirit, HeroId.npc_dota_hero_juggernaut, HeroId.npc_dota_hero_spectre,
        HeroId.npc_dota_hero_grimstroke, HeroId.npc_dota_hero_drow_ranger, HeroId.npc_dota_hero_dark_willow,
        HeroId.npc_dota_hero_obsidian_destroyer, HeroId.npc_dota_hero_pudge*/)]
    public class Program : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Hero Owner;

        private readonly Lazy<IParticleManager> particleManager;

        private readonly IServiceContext Context;

        private Lazy<IHero, IHeroMetadata> Hero;

        private HeroId[] SupportedHeroes =
        {
            HeroId.npc_dota_hero_huskar, HeroId.npc_dota_hero_rattletrap,
            HeroId.npc_dota_hero_storm_spirit, HeroId.npc_dota_hero_nevermore,
            HeroId.npc_dota_hero_sven, HeroId.npc_dota_hero_antimage, HeroId.npc_dota_hero_tusk,
            HeroId.npc_dota_hero_puck, HeroId.npc_dota_hero_slark, HeroId.npc_dota_hero_slardar,
            HeroId.npc_dota_hero_magnataur, HeroId.npc_dota_hero_winter_wyvern, HeroId.npc_dota_hero_bristleback,
            HeroId.npc_dota_hero_ember_spirit, HeroId.npc_dota_hero_juggernaut, HeroId.npc_dota_hero_spectre,
            HeroId.npc_dota_hero_grimstroke, HeroId.npc_dota_hero_drow_ranger, HeroId.npc_dota_hero_dark_willow,
            HeroId.npc_dota_hero_obsidian_destroyer, HeroId.npc_dota_hero_pudge, HeroId.npc_dota_hero_pugna, HeroId.npc_dota_hero_void_spirit,
            HeroId.npc_dota_hero_snapfire
        };

        private HeroId[] ExcludeFromDynamicCombo =
        {
            HeroId.npc_dota_hero_phantom_lancer, HeroId.npc_dota_hero_broodmother, 
        };

        [ImportingConstructor]
        public Program([Import] IServiceContext context, [Import] Lazy<IParticleManager> particleManager)
        {
            this.Context = context;
            this.particleManager = particleManager;
            this.Owner = Context.Owner as Hero;
        }

        [ImportMany(typeof(IHero))]
        protected IEnumerable<Lazy<IHero, IHeroMetadata>> Heroes { get; set; }

        protected override void OnActivate()
        {
            var unlocker = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "BAIO.DynamicCombo");
            if (SupportedHeroes.Contains(this.Owner.HeroId))
            {
                this.Hero = this.Heroes.FirstOrDefault(e => e.Metadata.Id == this.Owner.HeroId);
                if (Hero != null)
                {
                    this.Hero.Value.Activate();
                }
            }
            if ((Hero == null || !Hero.IsValueCreated) && unlocker != null)
            {
                
                var menuConfiguration = unlocker.GetType("Ensage.SDK.MenuConfiguration");
                var supportedHeroes = (HashSet<HeroId>)unlocker.GetType($"{unlocker.GetName().Name}.Program").GetMethod("SupportedHeroes")?
                    .Invoke(null, null);
                this.Hero = this.Heroes.FirstOrDefault(e => e.Metadata.Id == supportedHeroes?.First());
                if (Hero != null && !ExcludeFromDynamicCombo.Contains(this.Owner.HeroId) && menuConfiguration != null &&
                    (int?) menuConfiguration?.GetMethod("get_Version")?.Invoke(null, null) == MenuConfigurationKey)
                {
                    this.Hero.Value.Activate();
                }
            }
            if (!SupportedHeroes.Contains(this.Owner.HeroId) && unlocker == null)
            {
                Game.PrintMessage("This hero is not supported. You need BAIO.DynamicCombo plugin for that hero.");
            }
        }

        private int MenuConfigurationKey
        {
            get
            {
                return 289;
            }
        }

        protected override void OnDeactivate()
        {
            this.Hero.Value.Deactivate();
        }
    }
}