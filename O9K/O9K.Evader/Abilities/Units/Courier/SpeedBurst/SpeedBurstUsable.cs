namespace O9K.Evader.Abilities.Units.Courier.SpeedBurst
{
    using System;
    using System.Linq;

    using Base.Usable.CounterAbility;

    using Core.Data;
    using Core.Entities.Abilities.Base;
    using Core.Logger;
    using Core.Managers.Entity;

    using Divine;

    using Metadata;

    internal class SpeedBurstUsable : CounterAbility, IDisposable
    {
        public SpeedBurstUsable(Ability9 ability, IMainMenu menu)
            : base(ability, menu)
        {
            UpdateManager.CreateIngameUpdate(500, this.OnUpdate);
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
        }

        private void OnUpdate()
        {
            try
            {
                if (!this.ActiveAbility.CanBeCasted() || this.Owner.HasModifier(ModifierNames.FountainRegeneration))
                {
                    return;
                }

                var enemiesNear = EntityManager9.Heroes.Any(
                    x => x.IsVisible && x.IsAlive && !x.IsAlly(this.Owner) && x.Distance(this.Owner) < x.GetAttackRange(this.Owner, 300));

                if (enemiesNear)
                {
                    this.ActiveAbility.UseAbility();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}