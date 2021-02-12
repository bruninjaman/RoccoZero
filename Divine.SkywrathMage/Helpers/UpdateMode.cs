using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Renderer.Particle;
using Divine.Core.Managers.TargetSelector;
using Divine.SkywrathMage.Menus;

using Ensage;
using Ensage.SDK.Extensions;

using SharpDX;

namespace Divine.SkywrathMage.Helpers
{
    internal sealed class UpdateMode : BaseUpdateHandler
    {
        private readonly BaseBlinkDaggerMenu BlinkDaggerMenu;

        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly BaseFarmMenu FarmMenu;

        private readonly RadiusMenu RadiusMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        public CUnit UnitTarget { get; private set; }

        public UpdateMode(Common common)
        {
            BlinkDaggerMenu = common.MenuConfig.ComboMenu.BlinkDaggerMenu;
            SmartArcaneBoltMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartArcaneBoltMenu;
            FarmMenu = common.MenuConfig.FarmMenu;
            RadiusMenu = (RadiusMenu)common.MenuConfig.RadiusMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;

            Run();
        }

        protected override void Execute()
        {
            var arcaneBolt = Abilities.ArcaneBolt;
            if (RadiusMenu.ArcaneBoltItem || FarmMenu.FarmHotkeyItem && arcaneBolt.Level > 0)
            {
                ParticleManager.DrawRange(
                    "ArcaneBolt",
                    Owner,
                    arcaneBolt.CastRange,
                    arcaneBolt.IsReady ? (FarmMenu.FarmHotkeyItem ? Color.Yellow : Color.Aqua) : Color.Gray);
            }
            else
            {
                ParticleManager.Remove("ArcaneBolt");
            }

            var concussiveShot = Abilities.ConcussiveShot;
            if (RadiusMenu.ConcussiveShotItem && concussiveShot.Level > 0)
            {
                ParticleManager.DrawRange(
                    "ConcussiveShot",
                    Owner,
                    concussiveShot.Radius,
                    concussiveShot.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.Remove("ConcussiveShot");
            }

            var ancientSeal = Abilities.AncientSeal;
            if (RadiusMenu.AncientSealItem && ancientSeal.Level > 0)
            {
                ParticleManager.DrawRange(
                    "AncientSeal",
                    Owner,
                    ancientSeal.CastRange,
                    ancientSeal.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.Remove("AncientSeal");
            }

            var mysticFlare = Abilities.MysticFlare;
            if (RadiusMenu.MysticFlareItem && mysticFlare.Level > 0)
            {
                ParticleManager.DrawRange(
                    "MysticFlare",
                    Owner,
                    mysticFlare.CastRange,
                    mysticFlare.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.Remove("MysticFlare");
            }

            var blink = Abilities.Blink;
            if (RadiusMenu.BlinkDaggerItem && blink != null)
            {
                var color = Color.Red;
                if (!blink.IsReady)
                {
                    color = Color.Gray;
                }
                else if (Owner.Distance2D(Game.MousePosition) > BlinkDaggerMenu.BlinkActivationItem.Value)
                {
                    color = Color.Aqua;
                }

                ParticleManager.DrawRange(
                    "Blink",
                    Owner,
                    blink.CastRange,
                    color);
            }
            else
            {
                ParticleManager.Remove("Blink");
            }

            if (FarmMenu.FarmHotkeyItem)
            {
                ParticleManager.DrawRange(
                    "Farm",
                    Owner,
                    Owner.AttackRange(Owner),
                    Color.Yellow);
            }
            else
            {
                ParticleManager.Remove("Farm");
            }

            var targetHit = concussiveShot.TargetHit;
            if (RadiusMenu.TargetHitConcussiveShotItem && targetHit != null && concussiveShot.Cooldown <= 1 && concussiveShot.Level > 0)
            {
                var position = targetHit.Position + new Vector3(0, 200, targetHit.HealthBarOffset);

                ParticleManager.AddOrUpdate(
                    "TargetHitConcussiveShot",
                    @"particles\units\heroes\hero_skywrath_mage\skywrath_mage_concussive_shot.vpcf",
                    targetHit,
                    ParticleAttachment.AbsOrigin,
                    new ControlPoint(0, position),
                    new ControlPoint(1, position),
                    new ControlPoint(2, 1000));
            }
            else
            {
                ParticleManager.Remove("TargetHitConcussiveShot");
            }
        }
    }
}
