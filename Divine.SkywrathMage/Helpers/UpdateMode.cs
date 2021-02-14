using Divine.Core.ComboFactory;
using Divine.SDK.Extensions;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.Menus.Combo;
using Divine.SkywrathMage.TargetSelector;

using SharpDX;

namespace Divine.SkywrathMage.Helpers
{
    internal sealed class UpdateMode : BaseUpdateHandler
    {
        private readonly BlinkDaggerMenu BlinkDaggerMenu;

        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly FarmMenu FarmMenu;

        private readonly RadiusMenu RadiusMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        public Unit UnitTarget { get; private set; }

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
                ParticleManager.RangeParticle(
                    "ArcaneBolt",
                    Owner,
                    arcaneBolt.CastRange,
                    arcaneBolt.IsReady ? (FarmMenu.FarmHotkeyItem ? Color.Yellow : Color.Aqua) : Color.Gray);
            }
            else
            {
                ParticleManager.RemoveParticle("ArcaneBolt");
            }

            var concussiveShot = Abilities.ConcussiveShot;
            if (RadiusMenu.ConcussiveShotItem && concussiveShot.Level > 0)
            {
                ParticleManager.RangeParticle(
                    "ConcussiveShot",
                    Owner,
                    concussiveShot.Radius,
                    concussiveShot.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.RemoveParticle("ConcussiveShot");
            }

            var ancientSeal = Abilities.AncientSeal;
            if (RadiusMenu.AncientSealItem && ancientSeal.Level > 0)
            {
                ParticleManager.RangeParticle(
                    "AncientSeal",
                    Owner,
                    ancientSeal.CastRange,
                    ancientSeal.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.RemoveParticle("AncientSeal");
            }

            var mysticFlare = Abilities.MysticFlare;
            if (RadiusMenu.MysticFlareItem && mysticFlare.Level > 0)
            {
                ParticleManager.RangeParticle(
                    "MysticFlare",
                    Owner,
                    mysticFlare.CastRange,
                    mysticFlare.IsReady ? Color.Aqua : Color.Gray);
            }
            else
            {
                ParticleManager.RemoveParticle("MysticFlare");
            }

            var blink = Abilities.Blink;
            if (RadiusMenu.BlinkDaggerItem && blink != null)
            {
                var color = Color.Red;
                if (!blink.IsReady)
                {
                    color = Color.Gray;
                }
                else if (Owner.Distance2D(GameManager.MousePosition) > BlinkDaggerMenu.BlinkActivationItem.Value)
                {
                    color = Color.Aqua;
                }

                ParticleManager.RangeParticle(
                    "Blink",
                    Owner,
                    blink.CastRange,
                    color);
            }
            else
            {
                ParticleManager.RemoveParticle("Blink");
            }

            if (FarmMenu.FarmHotkeyItem)
            {
                ParticleManager.RangeParticle(
                    "Farm",
                    Owner,
                    Owner.AttackRange(Owner),
                    Color.Yellow);
            }
            else
            {
                ParticleManager.RemoveParticle("Farm");
            }

            var targetHit = concussiveShot.TargetHit;
            if (RadiusMenu.TargetHitConcussiveShotItem && targetHit != null && concussiveShot.Cooldown <= 1 && concussiveShot.Level > 0)
            {
                var position = targetHit.Position + new Vector3(0, 200, targetHit.HealthBarOffset);

                ParticleManager.CreateOrUpdateParticle(
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
                ParticleManager.RemoveParticle("TargetHitConcussiveShot");
            }
        }
    }
}
