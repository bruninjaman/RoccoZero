namespace O9K.AIO.Heroes.ShadowFiend.Abilities
{
    using AIO.Abilities.Menus;

    using Core.Entities.Abilities.Base;
    using Core.Managers.Menu.Items;

    internal class BlinkDaggerShadowFiendMenu : UsableAbilityMenu
    {
        public BlinkDaggerShadowFiendMenu(Ability9 ability, string simplifiedName)
            : base(ability, simplifiedName)
        {
            this.DontUseEulInCombo =
                this.Menu.Add(new MenuSwitcher("Dont use eul in combo, if have arcane blink", simplifiedName));
        }

        public MenuSwitcher DontUseEulInCombo { get; }
    }
}