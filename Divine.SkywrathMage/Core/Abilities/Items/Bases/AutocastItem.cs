namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class AutocastItem : RangedItem
    {
        protected AutocastItem(Item item)
            : base(item)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return IsAutoCastEnabled;
            }

            set
            {
                if (value)
                {
                    if (!Enabled)
                    {
                        CastToggleAutocast();
                    }
                }
                else
                {
                    if (Enabled)
                    {
                        CastToggleAutocast();
                    }
                }
            }
        }
    }
}