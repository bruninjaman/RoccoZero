namespace O9K.AutoUsage.Abilities.TreeCutter.QuellingBlade
{
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    using Settings;

    [AbilityId(AbilityId.item_quelling_blade)]
    internal class QuellingBladeAbility : TreeCutAbility
    {
        public QuellingBladeAbility(IActiveAbility ability, GroupSettings settings) : base(ability)
        {
            this.settings = new TreeCutSettings(settings.Menu, ability);

        }
    }
}