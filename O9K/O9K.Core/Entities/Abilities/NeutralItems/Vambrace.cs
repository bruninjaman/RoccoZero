namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.item_vambrace)]
public class Vambrace : ActiveAbility, IHasDamageAmplify
{
    private readonly Sleeper expectedAttributeSleeper = new();

    private readonly Divine.Entity.Entities.Abilities.Items.Vambrace vambrace;

    private Attribute expectedAttribute;

    private readonly SpecialData amplifierData;

    public Vambrace(Ability baseAbility)
        : base(baseAbility)
    {
        this.vambrace = (Divine.Entity.Entities.Abilities.Items.Vambrace)baseAbility;
        this.amplifierData = new SpecialData(baseAbility, "bonus_spell_amp");
    }

    public Attribute ActiveAttribute
    {
        get
        {
            if (this.expectedAttributeSleeper)
            {
                return this.expectedAttribute;
            }

            return this.vambrace.ActiveAttribute;
        }
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Magical;

    public string[] AmplifierModifierNames { get; } = { "modifier_item_vambrace" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Outgoing;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        if (!this.IsUsable || this.Owner.PrimaryAttribute != Attribute.Intelligence)
        {
            return 0;
        }

        return this.amplifierData.GetValue(this.Level) / 100f;
    }

    public override bool CanBeCasted(bool checkChanneling = true)
    {
        //if (this.ActionSleeper.IsSleeping)
        //{
        //    return false;
        //}

        if (!this.IsReady)
        {
            return false;
        }

        if (!this.Owner.IsAlive || !this.Owner.CanBeHealed)
        {
            return false;
        }

        if (checkChanneling && !this.CanBeCastedWhileChanneling && this.Owner.IsChanneling)
        {
            return false;
        }

        if (this.Owner.IsStunned && (this.Owner.UnitState & UnitState.OutOfGame) == 0)
        {
            return false;
        }

        if (this.Owner.IsMuted)
        {
            return false;
        }

        return true;
    }

    public void ChangeExpectedAttribute(bool single)
    {
        if (!this.expectedAttributeSleeper)
        {
            this.expectedAttribute = this.vambrace.ActiveAttribute;
        }

        switch (this.expectedAttribute)
        {
            case Attribute.Agility:
                this.expectedAttribute = single ? Attribute.Strength : Attribute.Intelligence;
                break;
            case Attribute.Strength:
                this.expectedAttribute = single ? Attribute.Intelligence : Attribute.Agility;
                break;
            case Attribute.Intelligence:
                this.expectedAttribute = single ? Attribute.Agility : Attribute.Strength;
                break;
        }

        this.expectedAttributeSleeper.Sleep(0.5f);
    }

    public override bool UseAbility(bool queue = false, bool bypass = false)
    {
        var result = this.BaseAbility.Cast(queue, bypass);
        if (result)
        {
            this.ChangeExpectedAttribute(true);
            // this.ActionSleeper.Sleep(0.03f);
        }

        return result;
    }

    public bool UseAbilitySimple()
    {
        return this.BaseAbility.Cast();
    }
}