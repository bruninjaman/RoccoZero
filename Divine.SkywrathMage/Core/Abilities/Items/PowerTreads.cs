using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_power_treads)]
    public sealed class PowerTreads : ActiveItem
    {
        public PowerTreads(Item item)
            : base(item)
        {
            Base = item as Divine.Items.PowerTreads;
        }

        public new Divine.Items.PowerTreads Base { get; }

        public Attribute ActiveAttribute
        {
            get
            {
                return Base.ActiveAttribute;
            }
        }

        public bool SwitchAttribute(Attribute attribute)
        {
            if (!CanBeCasted)
            {
                return false;
            }

            var result = false;
            var activeAttribute = ActiveAttribute;

            switch (attribute)
            {
                case Attribute.Strength:
                    if (activeAttribute == Attribute.Intelligence)
                    {
                        result = Cast() && Cast();
                    }
                    else if (activeAttribute == Attribute.Agility)
                    {
                        result = Cast();
                    }
                    break;
                case Attribute.Intelligence:
                    if (activeAttribute == Attribute.Agility)
                    {
                        result = Cast() && Cast();
                    }
                    else if (activeAttribute == Attribute.Strength)
                    {
                        result = Cast();
                    }
                    break;
                case Attribute.Agility:
                    if (activeAttribute == Attribute.Strength)
                    {
                        result = Cast() && Cast();
                    }
                    else if (activeAttribute == Attribute.Intelligence)
                    {
                        result = Cast();
                    }
                    break;
            }

            if (result)
            {
                LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }
    }
}