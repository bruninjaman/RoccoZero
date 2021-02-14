using System;

using Divine.Core.Helpers;
using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Core.Extensions
{
    public static class UnitExtensions
    {
        public static bool IsBlockMagicDamage(this Unit target, TargetModifiers targetModifiers = null)
        {
            if (target.IsMagicImmune() || target.IsInvulnerable())
            {
                return true;
            }

            if (targetModifiers != null)
            {
                return targetModifiers.IsBlockingModifers || targetModifiers.IsDuelAghanimsScepter;
            }

            return false;
        }

        public static bool IsAntimageSpellShield(this Unit unit)
        {
            var spellShield = unit.GetAbilityById(AbilityId.antimage_spell_shield);
            return spellShield != null && spellShield.Level > 0 && spellShield.Cooldown <= 0 && unit.HasAghanimsScepter();
        }

        public static float GetRotationAngle(this Unit unit, Unit target, bool rotationDifference = false)
        {
            return unit.GetRotationAngle(target.Position, rotationDifference);
        }

        public static float GetRotationAngle(this Unit unit, Vector3 targetPosition, bool rotationDifference = false)
        {
            var position = unit.Position;
            var angle = Math.Abs(Math.Atan2(targetPosition.Y - position.Y, targetPosition.X - position.X) - (rotationDifference 
                ? MathUtil.DegreesToRadians(unit.RotationDifference + unit.Rotation) 
                : unit.RotationRad));

            if (angle > Math.PI)
            {
                angle = Math.Abs((Math.PI * 2) - angle);
            }

            return (float)angle;
        }
    }
}