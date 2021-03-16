using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Extensions;
using SharpDX;

namespace TinkerCrappahilationPaid.Abilities
{
    public class Rearm : ActiveAbility
    {
        public Rearm(Ability ability) : base(ability)
        {

        }


        public override float ActivationDelay
        {
            get
            {
                var channelTime = Ability.GetAbilitySpecialData("AbilityChannelTime");
                return channelTime * 1000;
            }
        }

        public override bool CanBeCasted
        {
            get
            {
                if (!IsReady)
                {
                    return false;
                }

                var owner = Owner;
                var isItem = Ability is Item;
                if (owner.IsStunned() || isItem && owner.IsMuted() || !isItem && owner.IsSilenced() || Ability.IsInAbilityPhase)
                {
                    return false;
                }

                if (GameManager.RawGameTime - LastCastAttempt < 0.1f)
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetCastDelay(Unit target)
        {
            return base.GetCastDelay(target) + (int)ActivationDelay;
        }

        public override int GetCastDelay(Vector3 position)
        {
            return base.GetCastDelay(position) + (int)ActivationDelay;
        }

        public override int GetCastDelay()
        {
            return base.GetCastDelay() + (int) ActivationDelay;
        }
    }
}