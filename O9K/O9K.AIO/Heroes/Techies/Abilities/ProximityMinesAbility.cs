using System.Linq;
using Divine.Extensions;
using Divine.Numerics;
using O9K.AIO.Abilities;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Techies;
using O9K.Core.Entities.Units;
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

    private Vector2 Plant(Unit9 target)
    {
        var mePos = Owner.BasePosition;
        var targetPos = target.BasePosition.ToVector2();
        var targetDirection = target.BaseUnit.Direction().ToVector2().Rotated(180);
        var closestMines = EntityManager9.Units.Where(z =>
            z.IsValid && z.IsAlive && z.IsAlly(Owner) && z.Name == EntityName && z.Distance(target) <= 380);
        var perpendicular = Vector2.Zero;
        var count = closestMines.Count();
        if (count > 2) 
        {
            return Vector2.Zero;
        }
        if (count == 0)
        {
            perpendicular = targetDirection.Perpendicular().Normalized();
            return targetPos + targetDirection * 180 + perpendicular * 180;
        }
        var mineOnePos = closestMines.ElementAt(0).Position.ToVector2();
        if (count > 1)
        {
            var mineTwoPos = closestMines.ElementAt(1).Position.ToVector2();
            perpendicular = (mineOnePos - mineTwoPos).Perpendicular().Normalized();
            return mineOnePos.Extend(mineTwoPos, 180) + perpendicular * 360;
        }
        perpendicular = (targetPos - mineOnePos).Normalized().Rotated(MathUtil.DegreesToRadians(45));
        return targetPos + perpendicular * 180;
    }

    public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
    {
        var plantPos = Plant(targetManager.Target);
        if (plantPos.IsZero)
        {
            return false;
        }

        if (!Ability.UseAbility(plantPos.ToVector3(), false, true))
        {
            return false;
        }

        var delay = Ability.GetCastDelay(plantPos.ToVector3());
        comboSleeper.Sleep(delay);
        Sleeper.Sleep(delay);
        OrbwalkSleeper.Sleep(delay + 0.5f);
        return true;
    }
}