namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class AutocastSpell : RangedSpell
    {
        protected AutocastSpell(Ability ability)
            : base(ability)
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