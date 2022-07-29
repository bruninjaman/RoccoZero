using System.Globalization;
using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;

namespace Farmling.Helpers;

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
            talent = EntityManager.GetEntities<Ability>().FirstOrDefault(x => x.Id == talentId && x.Owner?.Handle == talentOwner.Handle);

            if (talent != null)
            {
                talentValue = talent.AbilitySpecialData.First(x => x.Name == "value").Value;
                getDataFunc = GetTalentValue;
            }
            else
            {
                getDataFunc = _ => 1;
            }
        }
        catch
        {
            getDataFunc = _ => 0;

            // var ex = new BrokenAbilityException(talentId.ToString());
            // if (this.talent?.IsValid == true)
            // {
            //     ex.Data["Ability"] = new
            //     {
            //         Ability = this.talent.Name
            //     };
            // }
            //
            // Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, AbilityId talentId, string name)
    {
        try
        {
            AbilitySpecialData = ability.AbilitySpecialData.First(x => x.Name == name);

            var unit = ability.Owner as Unit;
            if (unit != null) talent = unit.Spellbook.Spells.FirstOrDefault(x => x.Id == talentId);

            if (talent != null)
            {
                talentValue = AbilitySpecialData.Bonuses.FirstOrDefault(x => x.Name == talentId.ToString())?.Value ?? talent.AbilitySpecialData.First(x => x.Name == "value")?.Value ?? 0f;
                getDataFunc = GetValueWithTalent;
            }
            else
            {
                getDataFunc = GetValueDefault;
            }

            value = new float[AbilitySpecialData.Count];

            for (var i = 0u; i < value.Length; i++) value[i] = AbilitySpecialData.GetValue(i);
        }
        catch
        {
            getDataFunc = _ => 1;

            // var ex = new BrokenAbilityException(ability.Name);
            // if (ability.IsValid)
            // {
            //     ex.Data["Ability"] = new
            //     {
            //         Ability = ability.Name,
            //         SpecialData = name,
            //     };
            // }
            //
            // Logger.Error(ex);
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
                if (unit != null) talent = unit.Spellbook.Spells.FirstOrDefault(x => x.Id == talentId);
            }

            if (talent != null)
            {
                talentValue = AbilitySpecialData.Bonuses.FirstOrDefault(x => x.Name == talentId.ToString())?.Value ?? talent.AbilitySpecialData.First(x => x.Name == "value")?.Value ?? 0f;
                getDataFunc = GetValueWithTalent;
            }
            else
            {
                getDataFunc = GetValueDefault;
            }

            if (AbilitySpecialData.Count == 0)
            {
                value = ability.KeyValue.GetSubKey(name)
                    .GetString().Split()
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
            }
            else
            {
                value = new float[AbilitySpecialData.Count];

                for (var i = 0u; i < value.Length; i++) value[i] = AbilitySpecialData.GetValue(i);
            }
        }
        catch
        {
            getDataFunc = _ => 1;

            // var ex = new BrokenAbilityException(ability.Name);
            // if (ability.IsValid)
            // {
            //     ex.Data["Ability"] = new
            //     {
            //         Ability = ability.Name,
            //         SpecialData = name,
            //     };
            // }
            //
            // Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, Func<uint, int> baseData)
    {
        try
        {
            value = new float[Math.Max(ability.MaximumLevel, 1)];

            for (var i = 0u; i < value.Length; i++) value[i] = baseData(i);

            getDataFunc = GetValueDefault;
        }
        catch
        {
            getDataFunc = _ => 1;

            // var ex = new BrokenAbilityException(ability.Name);
            // if (ability.IsValid)
            // {
            //     ex.Data["Ability"] = new
            //     {
            //         Ability = ability.Name,
            //         BaseSpecialData = baseData.Method.Name
            //     };
            // }
            //
            // Logger.Error(ex);
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
                value = new float[AbilitySpecialData.Count];

                for (var i = 0u; i < value.Length; i++) value[i] = AbilitySpecialData.GetValue(i);
            }
            else
            {
                var keyData = abilityData.KeyValue.GetSubKey(key).GetString();
                var stringValues = keyData.Split(' ');

                value = new float[stringValues.Length];

                for (var i = 0u; i < value.Length; i++) value[i] = float.Parse(stringValues[i], CultureInfo.InvariantCulture);
            }

            getDataFunc = GetValueDefault;
        }
        catch
        {
            getDataFunc = _ => 0;
            // var ex = new BrokenAbilityException(abilityId + "/" + key);
            // Logger.Error(ex);
        }
    }

    public SpecialData(Ability ability, Func<uint, float> baseData)
    {
        try
        {
            value = new float[Math.Max(ability.MaximumLevel, 1)];

            for (var i = 0u; i < value.Length; i++) value[i] = baseData(i + 1);

            getDataFunc = GetValueDefault;
        }
        catch
        {
            getDataFunc = _ => 1;

            // var ex = new BrokenAbilityException(ability.Name);
            // if (ability.IsValid)
            // {
            //     ex.Data["Ability"] = new
            //     {
            //         Ability = ability.Name,
            //         BaseSpecialData = baseData.Method.Name
            //     };
            // }
            //
            // Logger.Error(ex);
        }
    }

    public AbilitySpecialData AbilitySpecialData { get; }

    // ReSharper disable once RedundantAssignment
    public float GetTalentValue(uint level)
    {
        level = talent.Level;

        if (level == 0) return 0;

        return talentValue;
    }

    public float GetValue(uint level)
    {
        return getDataFunc(level);
    }

    public float GetValueDefault(uint level)
    {
        if (level == 0) return 0;

        return value[Math.Min(level, value.Length) - 1];
    }

    public float GetValueWithTalent(uint level)
    {
        if (level == 0) return 0;

        var data = value[Math.Min(level, value.Length) - 1];

        if (talent?.Level > 0) data += talentValue;

        return data;
    }

    public float GetValueWithTalentMultiply(uint level)
    {
        if (level == 0) return 0;

        var data = value[Math.Min(level, value.Length) - 1];

        if (talent?.Level > 0) data *= talentValue / 100 + 1;

        return data;
    }

    public float GetValueWithTalentMultiplySimple(uint level)
    {
        if (level == 0) return 0;

        var data = value[Math.Min(level, value.Length) - 1];

        if (talent?.Level > 0) data *= talentValue;

        return data;
    }

    public float GetValueWithTalentSubtract(uint level)
    {
        if (level == 0) return 0;

        var data = value[Math.Min(level, value.Length) - 1];

        if (talent?.Level > 0) data -= talentValue;

        return data;
    }
}
