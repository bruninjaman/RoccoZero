using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Update;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Abilities.MainAbilities;
using InvokerAnnihilation.Abilities.MainAbilities.Items;

namespace InvokerAnnihilation.Abilities.AbilityManager;

public class AbilityManager : IAbilityManager
{
    private Dictionary<AbilityId, IAbility?> Abilities;

    public AbilityManager()
    {
        DictHandlers = new Dictionary<AbilityId, Func<Ability?, BaseAbstractAbility>>
        {
            {
                AbilityId.item_cyclone,
                ability => ability != null
                    ? new Eul(ability)
                    : new Eul(EntityManager.LocalHero!, AbilityId.item_cyclone)
            },
            {
                AbilityId.item_sheepstick,
                ability => ability != null
                    ? new Hex(ability)
                    : new Hex(EntityManager.LocalHero!, AbilityId.item_sheepstick)
            },
            {
                AbilityId.item_refresher,
                ability => ability != null
                    ? new Refresher(ability)
                    : new Refresher(EntityManager.LocalHero!, AbilityId.item_refresher)
            },
            {
                AbilityId.item_blink,
                ability => ability != null
                    ? new Blink(ability)
                    : new Blink(EntityManager.LocalHero!, AbilityId.item_blink)
            },
        };
        var localHero = EntityManager.LocalHero!;
        Abilities = new Dictionary<AbilityId, IAbility?>();
        var invokerAlacrity = new Alacrity(localHero.GetAbilityById(AbilityId.invoker_alacrity), new[]
        {
            AbilityId.invoker_wex,
            AbilityId.invoker_wex,
            AbilityId.invoker_exort,
        });
        AddAbility(invokerAlacrity);
        var blast = new Blast(localHero.GetAbilityById(AbilityId.invoker_deafening_blast), new[]
        {
            AbilityId.invoker_quas,
            AbilityId.invoker_wex,
            AbilityId.invoker_exort,
        });
        AddAbility(blast);
        var invokerChaosMeteor = new ChaosMeteor(localHero.GetAbilityById(AbilityId.invoker_chaos_meteor), new[]
        {
            AbilityId.invoker_exort,
            AbilityId.invoker_exort,
            AbilityId.invoker_wex,
        });
        AddAbility(invokerChaosMeteor);
        var invokerColdSnap = new ColdSnap(localHero.GetAbilityById(AbilityId.invoker_cold_snap), new[]
        {
            AbilityId.invoker_quas,
            AbilityId.invoker_quas,
            AbilityId.invoker_quas,
        });
        AddAbility(invokerColdSnap);
        var invokerEmp = new Emp(localHero.GetAbilityById(AbilityId.invoker_emp), new[]
        {
            AbilityId.invoker_wex,
            AbilityId.invoker_wex,
            AbilityId.invoker_wex,
        });
        AddAbility(invokerEmp);
        var invokerForgeSpirit = new ForgeSpirit(localHero.GetAbilityById(AbilityId.invoker_forge_spirit), new[]
        {
            AbilityId.invoker_exort,
            AbilityId.invoker_exort,
            AbilityId.invoker_quas,
        });
        AddAbility(invokerForgeSpirit);
        var invokerGhostWalk = new GhostWalk(localHero.GetAbilityById(AbilityId.invoker_ghost_walk), new[]
        {
            AbilityId.invoker_quas,
            AbilityId.invoker_quas,
            AbilityId.invoker_wex,
        });
        AddAbility(invokerGhostWalk);
        var invokerIceWall = new IceWall(localHero.GetAbilityById(AbilityId.invoker_ice_wall), new[]
        {
            AbilityId.invoker_quas,
            AbilityId.invoker_quas,
            AbilityId.invoker_exort,
        });
        AddAbility(invokerIceWall);
        var invokerSunStrike = new SunStrike(localHero.GetAbilityById(AbilityId.invoker_sun_strike), new[]
        {
            AbilityId.invoker_exort,
            AbilityId.invoker_exort,
            AbilityId.invoker_exort,
        });
        AddAbility(invokerSunStrike);
        var invokerTornado = new Tornado(localHero.GetAbilityById(AbilityId.invoker_tornado), new[]
        {
            AbilityId.invoker_wex,
            AbilityId.invoker_wex,
            AbilityId.invoker_quas,
        });
        AddAbility(invokerTornado);
        TryToAdd(AbilityId.item_cyclone);
        TryToAdd(AbilityId.item_sheepstick);
        TryToAdd(AbilityId.item_blink);
        TryToAdd(AbilityId.item_refresher);
        UpdateManager.CreateUpdate(1000, () =>
        {
            TryToUpdate(AbilityId.item_cyclone);
            TryToUpdate(AbilityId.item_sheepstick);
            TryToUpdate(AbilityId.item_blink);
            TryToUpdate(AbilityId.item_refresher);
        });
    }

    private Dictionary<AbilityId, Func<Ability?, BaseAbstractAbility>> DictHandlers { get; set; }

    private void TryToUpdate(AbilityId abilityId)
    {
        if (!Abilities.TryGetValue(abilityId, out var storedItem))
        {
            return;
        }

        if (storedItem is {IsValid: true})
        {
        }
        else
        {
            if (storedItem?.BaseAbility!=null && !storedItem.BaseAbility.IsValid)
            {
                storedItem.SetAbility(null);
                return;
            }
            var item = EntityManager.LocalHero!.GetItemById(abilityId);
            if (item != null && item.IsValid)
            {
                storedItem?.SetAbility(item);
            }
        }
    }

    private void TryToAdd(AbilityId abilityId)
    {
        var item = EntityManager.LocalHero!.GetAbilityById(abilityId);
        if (DictHandlers.TryGetValue(abilityId, out var action))
        {
            var ability = action(item);
            AddAbility(ability);
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Cant find handler for ability: {abilityId}");
        }
    }

    private void AddAbility(IAbility ability)
    {
        Console.WriteLine($"adding {ability.AbilityId}");
        Abilities.Add(ability.AbilityId, ability);
    }

    public IAbility? GetAbility(AbilityId abilityId)
    {
        if (Abilities.TryGetValue(abilityId, out var ability))
        {
            return ability;
        }

        return null;
    }
}