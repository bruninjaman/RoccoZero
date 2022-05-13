namespace O9K.Core.Helpers;

using System;
using System.Globalization;
using System.Linq;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Abilities.Components;

using Exceptions;

using Logger;

using Managers.Entity;

public class SpecialData
{
    private readonly Func<uint, float> getDataFunc;

    private readonly Ability talent;

    private readonly float talentValue;

    private readonly float[] value;

    public SpecialData(Entity talentOwner, AbilityId talentId)
    {
        try
        {
            this.talent = EntityManager9.BaseAbilities.FirstOrDefault(x => x.Id == talentId && x.Owner?.Handle == talentOwner.Handle);

            if (this.talent != null)
            {
                this.talentValue = this.talent.AbilitySpecialData.First(x => x.Name == "value").Value;
                this.getDataFunc = this.GetTalentValue;
            }
            else
            {
                this.getDataFunc = _ => 1;
            }
        }
        catch
        {
            this.getDataFunc = _ => 0;

            var ex = new BrokenAbilityException(talentId.ToString());
            if (this.talent?.IsValid == true)
            {
                ex.Data["Ability"] = new
                {
                    Ability = this.talent.Name
                };
            }

            Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, AbilityId talentId, string name)
    {
        try
        {
            AbilitySpecialData = ability.AbilitySpecialData.First(x => x.Name == name);

            var unit = ability.Owner as Unit;
            if (unit != null)
            {
                this.talent = unit.Spellbook.Spells.FirstOrDefault(x => x.Id == talentId);
            }

            if (this.talent != null)
            {
                this.talentValue = AbilitySpecialData.Bonuses.FirstOrDefault(x => x.Name == talentId.ToString())?.Value ?? this.talent.AbilitySpecialData.First(x => x.Name == "value")?.Value ?? 0f;
                this.getDataFunc = this.GetValueWithTalent;
            }
            else
            {
                this.getDataFunc = this.GetValueDefault;
            }

            this.value = new float[AbilitySpecialData.Count];

            for (var i = 0u; i < this.value.Length; i++)
            {
                this.value[i] = AbilitySpecialData.GetValue(i);
            }
        }
        catch
        {
            this.getDataFunc = _ => 1;

            var ex = new BrokenAbilityException(ability.Name);
            if (ability.IsValid)
            {
                ex.Data["Ability"] = new
                {
                    Ability = ability.Name,
                    SpecialData = name,
                };
            }

            Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, string name)
    {
        try
        {
            AbilitySpecialData = ability.AbilitySpecialData.First(x => x.Name == name);
            var talentId = AbilitySpecialData.SpecialBonusAbility;

            if (talentId != AbilityId.dota_base_ability)
            {
                var unit = ability.Owner as Unit;
                if (unit != null)
                {
                    this.talent = unit.Spellbook.Spells.FirstOrDefault(x => x.Id == talentId);
                }
            }

            if (this.talent != null)
            {
                this.talentValue = AbilitySpecialData.Bonuses.FirstOrDefault(x => x.Name == talentId.ToString())?.Value ?? this.talent.AbilitySpecialData.First(x => x.Name == "value")?.Value ?? 0f;
                this.getDataFunc = this.GetValueWithTalent;
            }
            else
            {
                this.getDataFunc = this.GetValueDefault;
            }

            if (AbilitySpecialData.Count == 0)
            {
                this.value = ability.KeyValue.GetSubKey(name)
                    .GetString().Split()
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            }
            else
            {
                this.value = new float[AbilitySpecialData.Count];

                for (var i = 0u; i < this.value.Length; i++)
                {
                    this.value[i] = AbilitySpecialData.GetValue(i);
                }
            }
        }
        catch
        {
            this.getDataFunc = _ => 1;

            var ex = new BrokenAbilityException(ability.Name);
            if (ability.IsValid)
            {
                ex.Data["Ability"] = new
                {
                    Ability = ability.Name,
                    SpecialData = name,
                };
            }

            Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, Func<uint, int> baseData)
    {
        try
        {
            this.value = new float[Math.Max(ability.MaximumLevel, 1)];

            for (var i = 0u; i < this.value.Length; i++)
            {
                this.value[i] = baseData(i);
            }

            this.getDataFunc = this.GetValueDefault;
        }
        catch
        {
            this.getDataFunc = _ => 1;

            var ex = new BrokenAbilityException(ability.Name);
            if (ability.IsValid)
            {
                ex.Data["Ability"] = new
                {
                    Ability = ability.Name,
                    BaseSpecialData = baseData.Method.Name
                };
            }

            Logger.Error(ex);
        }
    }

    public SpecialData(AbilityId abilityId, string key)
    {
        try
        {
            var abilityData = Ability.GetAbilityDataById(abilityId);
            AbilitySpecialData = abilityData.AbilitySpecialData.FirstOrDefault(x => x.Name == key);

            if (AbilitySpecialData != null)
            {
                this.value = new float[AbilitySpecialData.Count];

                for (var i = 0u; i < this.value.Length; i++)
                {
                    this.value[i] = AbilitySpecialData.GetValue(i);
                }
            }
            else
            {
                var keyData = abilityData.KeyValue.GetSubKey(key).GetString();
                var stringValues = keyData.Split(' ');

                this.value = new float[stringValues.Length];

                for (var i = 0u; i < this.value.Length; i++)
                {
                    this.value[i] = float.Parse(stringValues[i], CultureInfo.InvariantCulture);
                }
            }

            this.getDataFunc = this.GetValueDefault;
        }
        catch
        {
            this.getDataFunc = _ => 0;
            var ex = new BrokenAbilityException(abilityId + "/" + key);
            Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, Func<uint, float> baseData)
    {
        try
        {
            this.value = new float[Math.Max(ability.MaximumLevel, 1)];

            for (var i = 0u; i < this.value.Length; i++)
            {
                this.value[i] = baseData(i + 1);
            }

            this.getDataFunc = this.GetValueDefault;
        }
        catch
        {
            this.getDataFunc = _ => 1;

            var ex = new BrokenAbilityException(ability.Name);
            if (ability.IsValid)
            {
                ex.Data["Ability"] = new
                {
                    Ability = ability.Name,
                    BaseSpecialData = baseData.Method.Name
                };
            }

            Logger.Error(ex);
        }
    }

    public AbilitySpecialData AbilitySpecialData { get; }

    // ReSharper disable once RedundantAssignment
    public float GetTalentValue(uint level)
    {
        level = this.talent.Level;

        if (level == 0)
        {
            return 0;
        }

        return this.talentValue;
    }

    public float GetValue(uint level)
    {
        return this.getDataFunc(level);
    }

    public float GetValueDefault(uint level)
    {
        if (level == 0)
        {
            return 0;
        }

        return this.value[Math.Min(level, this.value.Length) - 1];
    }

    public float GetValueWithTalent(uint level)
    {
        if (level == 0)
        {
            return 0;
        }

        var data = this.value[Math.Min(level, this.value.Length) - 1];

        if (this.talent?.Level > 0)
        {
            data += this.talentValue;
        }

        return data;
    }

    public float GetValueWithTalentMultiply(uint level)
    {
        if (level == 0)
        {
            return 0;
        }

        var data = this.value[Math.Min(level, this.value.Length) - 1];

        if (this.talent?.Level > 0)
        {
            data *= (this.talentValue / 100) + 1;
        }

        return data;
    }

    public float GetValueWithTalentMultiplySimple(uint level)
    {
        if (level == 0)
        {
            return 0;
        }

        var data = this.value[Math.Min(level, this.value.Length) - 1];

        if (this.talent?.Level > 0)
        {
            data *= this.talentValue;
        }

        return data;
    }

    public float GetValueWithTalentSubtract(uint level)
    {
        if (level == 0)
        {
            return 0;
        }

        var data = this.value[Math.Min(level, this.value.Length) - 1];

        if (this.talent?.Level > 0)
        {
            data -= this.talentValue;
        }

        return data;
    }
}