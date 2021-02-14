using Divine.Core.Managers.Log;
using Divine.SDK.Managers.Log;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class ToggleSpell : ActiveSpell
    {
        private static readonly Log Log = LogManager.GetCurrentClassLogger();

        protected ToggleSpell(Ability ability)
            : base(ability)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return IsToggled;
            }

            set
            {
                if (CastSleeper.Sleeping)
                {
                    return;
                }

                if (!CanBeCasted)
                {
                    return;
                }

                var result = false;
                if (value)
                {
                    if (!Enabled)
                    {
                        result = CastToggle();
                    }
                }
                else
                {
                    if (Enabled)
                    {
                        result = CastToggle();
                    }
                }

                if (result)
                {
                    CastSleeper.Sleep(100); //TODO Re
                }
            }
        }

        public override bool Cast()
        {
            Enabled = !Enabled;
            return true; // TODO: return if setter was successful?
        }
    }
}