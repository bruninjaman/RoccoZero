namespace O9K.AutoUsage.Abilities.TreeCutter.TangoCut;

using Core.Entities.Abilities.Base.Components.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

using Settings;

[AbilityId(AbilityId.item_tango)]
[AbilityId(AbilityId.item_tango_single)]
internal class TangoAbility : TreeCutAbility
{
    public TangoAbility(IActiveAbility ability, GroupSettings settings) : base(ability)
    {
        this.settings = new TreeCutSettings(settings.Menu, ability);
    }
}