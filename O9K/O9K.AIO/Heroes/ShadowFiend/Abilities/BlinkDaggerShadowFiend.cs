namespace O9K.AIO.Heroes.ShadowFiend.Ability
{
    using Abilities;
    using AIO.Abilities;
    using AIO.Abilities.Menus;
    using Core.Entities.Abilities.Base;

    internal class BlinkDaggerShadowFiend : BlinkAbility
    {
        public BlinkDaggerShadowFiend(ActiveAbility ability)
            : base(ability)
        {
        }

        public override UsableAbilityMenu GetAbilityMenu(string simplifiedName)
        {
            return new BlinkDaggerShadowFiendMenu(this.Ability, "Dont use eul in combo");
        }
    }
}