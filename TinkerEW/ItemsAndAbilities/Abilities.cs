using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Helpers;

namespace TinkerEW.ItemsAndAbilities.Abilities;

internal partial class Abilities
{
    private readonly Sleeper UpdateSleeper = new();
    public readonly DefenseMatrix defenseMatrix;
    public readonly HeatSeekingMissile heatSeekingMissile;
    public readonly KeenConveyance keenConveyance;
    public readonly Laser laser;
    public readonly MarchOfTheMachines marchOfTheMachines;
    public readonly Rearm rearm;

    public Abilities()
    {
        defenseMatrix = new DefenseMatrix();
        heatSeekingMissile = new HeatSeekingMissile();
        keenConveyance = new KeenConveyance();
        laser = new Laser();
        marchOfTheMachines = new MarchOfTheMachines();
        rearm = new Rearm();
    }

    public void Update()
    {
        if (!UpdateSleeper.Sleeping)
        {
            var localHero = EntityManager.LocalHero;

            if (localHero == null)
                return;

            //Update abils here
            defenseMatrix.Update(localHero.GetAbilityById(AbilityId.tinker_defense_matrix));
            heatSeekingMissile.Update(localHero.GetAbilityById(AbilityId.tinker_heat_seeking_missile));
            keenConveyance.Update(localHero.GetAbilityById(AbilityId.tinker_keen_teleport));
            laser.Update(localHero.GetAbilityById(AbilityId.tinker_laser));
            marchOfTheMachines.Update(localHero.GetAbilityById(AbilityId.tinker_march_of_the_machines));
            rearm.Update(localHero.GetAbilityById(AbilityId.tinker_rearm));

            UpdateSleeper.Sleep(500);
        }
    }
}
