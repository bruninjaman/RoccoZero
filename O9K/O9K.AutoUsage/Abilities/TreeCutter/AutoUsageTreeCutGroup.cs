namespace O9K.AutoUsage.Abilities.TreeCutter
{
    using System;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Helpers;

    using Settings;

    internal class AutoUsageTreeCutGroup<TType, TAbility> : AutoUsageGroup<TType, TAbility>
        where TType : class, IActiveAbility where TAbility : UsableAbility
    {

        public AutoUsageTreeCutGroup(MultiSleeper sleeper, GroupSettings settings)
            : base(sleeper, settings)
        {
            settings.AddSettingsMenu();
        }

        public override void AddAbility(Ability9 ability)
        {
            var type = ability as TType;

            if (type == null)
            {
                return;
            }

            if (!this.UniqueAbilities.TryGetValue(ability.Id, out var uniqueType))
            {
                return;
            }

            var usableAbility = (TAbility)Activator.CreateInstance(uniqueType, type, this.Settings);
            this.Abilities.Add(usableAbility);
            this.Settings.AddAbility(usableAbility);

            if (this.Settings.GroupEnabled)
            {
                this.Handler.IsEnabled = true;
            }
        }

    }
}