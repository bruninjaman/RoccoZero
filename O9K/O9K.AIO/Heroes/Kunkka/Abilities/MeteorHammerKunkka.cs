namespace O9K.AIO.Heroes.Kunkka.Abilities
{
    using AIO.Abilities;
    using AIO.Abilities.Menus;
    using AIO.Modes.Combo;

    using Core.Entities.Abilities.Base;
    using Core.Helpers;

    using Divine.Numerics;

    using TargetManager;

    internal class MeteorHammerKunkka : DisableAbility
    {
        public MeteorHammerKunkka(ActiveAbility ability)
            : base(ability)
        {
        }


        private bool shouldReturn;

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            var menu = comboMenu.GetAbilitySettingsMenu<XMarkOnlyMenu>(this);
            if (menu.XMarkOnly)
            {
                return false;
            }

            return base.CanHit(targetManager, comboMenu);
        }

        public override UsableAbilityMenu GetAbilityMenu(string simplifiedName)
        {
            return new XMarkOnlyMenu(this.Ability, simplifiedName);
        }

        public bool ShouldReturn(ActiveAbility xReturn, Vector3 xMarkPosition)
        {

            if (shouldReturn)
            {
                shouldReturn = false;
                return true;
            }
            return false;
        }

        public void setShouldReturn(bool s)
        {
            shouldReturn = s;
        }

        public Vector3 Position { get; set; }

        public bool UseAbility(Vector3 position, TargetManager targetManager, Sleeper comboSleeper)
        {
            if (!this.Ability.UseAbility(position))
            {
                return false;
            }

            var hitTime = this.Ability.GetHitTime(targetManager.Target) + 0.5f;
            var delay = this.Ability.GetCastDelay(targetManager.Target);

            comboSleeper.Sleep(delay);
            this.OrbwalkSleeper.Sleep(delay);
            this.Sleeper.Sleep(hitTime);

            return true;
        }
    }
}