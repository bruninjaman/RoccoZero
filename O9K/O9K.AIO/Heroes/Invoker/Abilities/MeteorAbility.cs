using O9K.AIO.Abilities;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Invoker;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Logger;
using SharpDX;

namespace O9K.AIO.Heroes.Invoker.Abilities
{
    internal class MeteorAbility : InvokerAoeAbility
    {
        private readonly ChaosMeteor meteor;

        public MeteorAbility(ActiveAbility ability)
            : base(ability)
        {
            this.meteor = (ChaosMeteor) ability;
        }
    }
}