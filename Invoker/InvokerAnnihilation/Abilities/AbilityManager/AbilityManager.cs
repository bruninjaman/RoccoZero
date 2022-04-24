using System.Reflection;
using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Extensions;
using Divine.Update;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Abilities.MainAbilities;
using InvokerAnnihilation.Abilities.MainAbilities.Items;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.AbilityManager;

public class AbilityManager : IAbilityManager
{
    private Dictionary<AbilityId, IAbility?> Abilities  { get; } = new();
    private Dictionary<AbilityId, Func<Ability?, IAbility>> DictHandlers { get; } = new();
    public AbilityManager()
    {
        var localHero = EntityManager.LocalHero!;
        var types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<AbilityAttribute>();
            if (attribute == null)
                continue;
            // Activator.CreateInstance(type);
            var isItem = !attribute.Spheres.Any();
            if (isItem)
            {
                var mainAbility = attribute.AbilityIds.First();
                DictHandlers.Add(mainAbility, ability =>
                {
                    var instance = ability != null
                        ? (IAbility) Activator.CreateInstance(type, ability)!
                        : (IAbility) Activator.CreateInstance(type, EntityManager.LocalHero!, mainAbility)!;
                    if (instance is BaseItemAbility item)
                    {
                        item.OwnerAbility = mainAbility;
                    }
                    return instance;
                });
                TryToAdd(attribute.AbilityIds);
                UpdateManager.CreateUpdate(1000, () =>
                {
                    TryToUpdate(attribute.AbilityIds);
                });
            }
            else
            {
                IAbility ability = (IAbility) Activator.CreateInstance(type, localHero.GetAbilityById(attribute.AbilityIds.First()), attribute.Spheres)!;
                AddAbility(ability);
            }
        }


        // DictHandlers = new Dictionary<AbilityId, Func<Ability?, BaseAbstractAbility>>
        // {
        //     {
        //         AbilityId.item_cyclone,
        //         ability => ability != null
        //             ? new Eul(ability)
        //             : new Eul(EntityManager.LocalHero!, AbilityId.item_cyclone)
        //     },
        //     {
        //         AbilityId.item_sheepstick,
        //         ability => ability != null
        //             ? new Hex(ability)
        //             : new Hex(EntityManager.LocalHero!, AbilityId.item_sheepstick)
        //     },
        //     {
        //         AbilityId.item_refresher,
        //         ability => ability != null
        //             ? new Refresher(ability)
        //             : new Refresher(EntityManager.LocalHero!, AbilityId.item_refresher)
        //     },
        //     {
        //         AbilityId.item_blink,
        //         ability => ability != null
        //             ? new Blink(ability)
        //             : new Blink(EntityManager.LocalHero!, AbilityId.item_blink)
        //     },
        // };
        
        
        /*
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
        AddAbility(invokerTornado);*/
        // TryToAdd(AbilityId.item_cyclone);
        // TryToAdd(AbilityId.item_sheepstick);
        // TryToAdd(AbilityId.item_blink);
        // TryToAdd(AbilityId.item_refresher);

    }

    

    private void TryToUpdate(AbilityId[] abilitiesId)
    {
        var ownerAbility = abilitiesId.First();
        var tempAbilityList = new List<IAbility>();
        foreach (var abilityId in abilitiesId)
        {
            if (!Abilities.TryGetValue(ownerAbility, out var storedItem))
            {
                return;
            }

            if (storedItem is {IsValid: true})
            {
            }
            else
            {
                if (storedItem?.BaseAbility != null && !storedItem.BaseAbility.IsValid)
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
    }

    private void TryToAdd(AbilityId[] abilitiesId)
    {
        var ownerAbility = abilitiesId.First();
        var tempAbilityList = new List<IAbility>();
        foreach (var abilityId in abilitiesId)
        {
            var item = EntityManager.LocalHero!.GetItemById(abilityId);
            if (DictHandlers.TryGetValue(ownerAbility, out var action))
            {
                var ability = action(item);
                tempAbilityList.Add(ability);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Cant find handler for ability: {abilityId}");
            }
        }

        var firstWithValidId = tempAbilityList.FirstOrDefault(x => x.IsValid);
        if (firstWithValidId != null)
        {
            AddAbility(ownerAbility, firstWithValidId);
        }
        else
        {
            AddAbility(tempAbilityList.First());
        }
    }

    private void AddAbility(IAbility ability)
    {
        Abilities.Add(ability.AbilityId, ability);
    }
    private void AddAbility(AbilityId abilityId, IAbility ability)
    {
        Abilities.Add(abilityId, ability);
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