using System.Linq;
using Divine.Numerics;
using O9K.AIO.Abilities;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Techies;
using O9K.Core.Entities.Units;
using O9K.Core.Extensions;
using O9K.Core.Helpers;
using O9K.Core.Managers.Entity;

namespace O9K.AIO.Heroes.Techies.Abilities;

internal class ProximityMinesAbility : AoeAbility
{
    private readonly ProximityMines baseAbility;
    private readonly string EntityName = "npc_dota_techies_land_mine";

    public ProximityMinesAbility(ActiveAbility ability) : base(ability)
    {
        this.baseAbility = (ProximityMines) ability;
    }

    public uint Charges => baseAbility.Charges;

    private Vector3 Plant(Unit9 target)
    {
        var mePos = Owner.BasePosition;
        var targetPos = target.BasePosition;
        var extend2D = targetPos.Extend2D(mePos, 150);
        var closestMine = EntityManager9.Units.FirstOrDefault(z =>
            z.IsValid && z.IsAlive && z.IsAlly(Owner) && z.Name == EntityName && z.Distance(extend2D) <= 500);
        if (closestMine != null)
        {
            extend2D = closestMine.BasePosition.Extend2D(targetPos, 501);
            return extend2D;
        }

        return Charges == 1 ? Vector3.Zero : extend2D;
    }

    public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    {
        var plantPos = Plant(targetManager.Target);
        if (plantPos.IsZero)
        {
            return base.UseAbility(targetManager, comboSleeper, aoe);
        }

        if (!Ability.UseAbility(plantPos, false, true))
        {
            return false;
        }

        var delay = Ability.GetCastDelay(plantPos);
        comboSleeper.Sleep(delay);
        Sleeper.Sleep(delay);
        OrbwalkSleeper.Sleep(delay + 0.5f);
        return true;
    }
}