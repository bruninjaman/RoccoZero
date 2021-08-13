namespace O9K.AIO.Heroes.ArcWarden.Units
{
    using Core.Entities.Units;

    using Divine.Helpers;
    using Divine.Numerics;

    public static class PushCommands
    {
        public static bool AttackNextPoint(Unit9 owner, Vector3 attackPoint)
        {
            if (!MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + owner.Handle))
            {
                owner.BaseUnit.Attack(attackPoint);
                MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + owner.Handle, 1000);

                return true;
            }

            return false;
        }

        public static bool AttackTower(Unit9 owner, Unit9 nearestTower)
        {
            if (!owner.IsAttacking && !MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + owner.Handle) && !nearestTower.HasModifier("modifier_fountain_glyph"))
            {
                owner.Attack(nearestTower);
                MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + owner.Handle, 400);

                return true;
            }

            return false;
        }
    }
}