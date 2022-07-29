using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Renderer;

namespace Farmling.Extensions;

public static class UnitExtension
{
    private static Vector2 HealthBarSizeInternal { get; set; }

    public static Vector2 HealthBarSize
    {
        get
        {
            if (HealthBarSizeInternal.IsZero) HealthBarSizeInternal = new Vector2(Info.ScreenRatio * 78, Info.ScreenRatio * 6);

            return HealthBarSizeInternal;
        }
    }

    public static Vector2 HealthBarPositionCorrection
    {
        get
        {
            if (HealthBarPositionCorrectionInternal.IsZero) HealthBarPositionCorrectionInternal = new Vector2(HealthBarSize.X / 2f, Info.ScreenRatio * 11);

            return HealthBarPositionCorrectionInternal;
        }
    }

    public static Vector2 HealthBarPositionCorrectionInternal { get; set; }

    public static float PredictProjectileArrivalTime(this Unit? owner, Unit? target)
    {
        if (target == null || owner == null || !target.IsValid || !owner.IsValid) return 0;

        var tPos = target.Position;
        var ext = owner is Tower ? owner.Position : owner.Position.Extend(tPos, owner.ProjectileCollisionSize);
        var dist = ext.Distance2D(tPos) - target.HullRadius;
        var hitDelay = owner.AttackPoint() + dist / owner.ProjectileSpeed();
        return hitDelay;
    }

    public static Vector2 GetHealthBarPosition(this Unit unit)
    {
        //if ((UnitState & UnitState.NoHealthbar) != 0)
        //{
        //    return Vector2.Zero;
        //}

        var pos = unit.Position;
        pos.Z += unit.HealthBarOffset;
        var position = pos;
        var screenPosition = RendererManager.WorldToScreen(position);

        if (screenPosition.IsZero) return Vector2.Zero;

        return screenPosition - HealthBarPositionCorrection;
    }

    public static class Info
    {
        public static Vector2 GlyphPosition { get; } = RendererManager.ScreenSize * new Vector2(0.16f, 0.965f);

        public static Vector2 ScanPosition { get; } = RendererManager.ScreenSize * new Vector2(0.16f, 0.925f);

        public static float ScreenRatio { get; } = RendererManager.ScreenSize.Y / 1080f;

        public static Vector2 ScreenSize { get; } = RendererManager.ScreenSize;
    }
}
