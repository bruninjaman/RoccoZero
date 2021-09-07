namespace O9K.AutoUsage.Abilities.TreeCutter.BattleFury;

using Core.Entities.Abilities.Base.Components.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

using Settings;

[AbilityId(AbilityId.item_bfury)]
internal class BattleFuryAbility : TreeCutAbility
{

    public BattleFuryAbility(IActiveAbility ability, GroupSettings settings) : base(ability)
    {
        this.settings = new TreeCutSettings(settings.Menu, ability);

    }
}