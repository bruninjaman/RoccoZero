using O9K.AIO.Abilities;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Invoker;
using O9K.Core.Entities.Units;

namespace O9K.AIO.Heroes.Invoker.Abilities
{
    internal class SunStrikeAbility : InvokerAoeAbility
    {
        private readonly SunStrike sunStrike;

        public SunStrikeAbility(ActiveAbility ability) : base(ability)
        {
            this.sunStrike = (SunStrike) ability;
        }
    }
}