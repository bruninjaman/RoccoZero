using Divine.SDK.Managers.Log;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class ToggleItem : ActiveItem
    {
        private static readonly Log Log = LogManager.GetCurrentClassLogger();

        protected ToggleItem(Item item)
            : base(item)
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
                if (!CanBeCasted)
                {
                    Log.Debug($"blocked {this}");
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
                        result = Base.CastToggle();
                    }
                }

                if (result)
                {
                    LastCastAttempt = GameManager.RawGameTime;
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